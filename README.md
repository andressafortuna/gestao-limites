# FraudSys - Sistema de Gestão de Limites PIX

Sistema completo para gestão de limites de transações PIX, desenvolvido com arquitetura limpa e boas práticas de engenharia de software.

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Arquitetura](#-arquitetura)
- [Instalação e Execução](#-instalação-e-execução)
- [Endpoints da API](#-endpoints-da-api)
  - [Account Limits](#1-account-limits---gestão-de-limites)
  - [PIX Transactions](#2-pix-transactions---processamento-de-transações)
- [Conceitos Importantes](#-conceitos-importantes)
- [Estrutura do Projeto](#-estrutura-do-projeto)

---

## 🎯 Sobre o Projeto

O **FraudSys** é um sistema de gestão de limites para transações PIX do Banco KRT. O sistema permite que analistas de fraude:

1. **Cadastrem limites** para contas bancárias
2. **Consultem, atualizem e removam** limites existentes
3. **Processem transações PIX** com validação automática de limite disponível
4. **Controlem o consumo de limite** em tempo real

### Funcionalidades Principais

✅ CRUD completo de limites de conta  
✅ Validação de CPF com algoritmo oficial  
✅ Processamento de transações PIX com controle de limite  
✅ Aprovação/negação automática baseada em saldo disponível  
✅ Histórico de transações com rastreabilidade  
✅ **Idempotência** para evitar processamento duplicado  
✅ Tratamento robusto de erros  

---

## 🚀 Tecnologias Utilizadas

### Stack Principal

- **.NET 8.0** - Framework base
- **ASP.NET Core Web API** - Camada de apresentação com controllers
- **AWS DynamoDB** - Banco de dados NoSQL
- **FluentValidation** - Validações de entrada
- **AutoMapper** - Mapeamento de objetos

### Princípios e Padrões

- **Clean Architecture** - Separação clara de responsabilidades
- **Domain-Driven Design (DDD)** - Modelagem rica de domínio
- **SOLID** - Princípios de design orientado a objetos
- **Repository Pattern** - Abstração de acesso a dados
- **Value Objects** - Encapsulamento de lógica de validação

---

## 🏗️ Arquitetura

O projeto segue **Clean Architecture** com 4 camadas:

```
┌─────────────────────────────────────────────────┐
│              FraudSys.API                       │
│         (Controllers, Middlewares)              │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│          FraudSys.Application                   │
│     (Services, DTOs, Validators, Mappings)      │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│            FraudSys.Domain                      │
│  (Entities, Value Objects, Interfaces, Enums)   │
└─────────────────────────────────────────────────┘
                  ▲
┌─────────────────┴───────────────────────────────┐
│         FraudSys.Infrastructure                 │
│      (Repositories, DynamoDB Configuration)     │
└─────────────────────────────────────────────────┘
```

### Camadas

- **Domain**: Lógica de negócio pura (sem dependências externas)
- **Application**: Casos de uso e orquestração
- **Infrastructure**: Implementações de infraestrutura (BD, APIs)
- **API**: Camada de apresentação (HTTP)

---

## 🔧 Instalação e Execução

### 1. Clone o Repositório

```bash
git clone https://github.com/andressafortuna/gestao-limites.git
cd fraudsys
```

### 2. Inicie o DynamoDB Local

Abra o terminal e execute:

```bash
docker run -d -p 8000:8000 amazon/dynamodb-local
```

Verifique se está rodando:

```bash
docker ps
```

### 3. Restaure as Dependências

```bash
dotnet restore
```

### 4. Execute a Aplicação

```bash
cd FraudSys.API
dotnet run
```

Ou use o Visual Studio (F5).

### 5. Acesse o Swagger

Abra o navegador em:

```
https://localhost:7055/swagger
```

ou

```
http://localhost:5011/swagger
```

---

## 📡 Endpoints da API

Base URL: `https://localhost:7055`

### 1. Account Limits - Gestão de Limites

#### 1.1. Criar Limite

**POST** `/api/accountlimits`

Cadastra um novo limite para uma conta bancária.

**Request Body:**
```json
{
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "pixLimit": 5000.00
}
```

**Validações:**
- ✅ CPF deve ser válido (validação com dígitos verificadores)
- ✅ Todos os campos são obrigatórios
- ✅ Limite PIX deve ser ≥ 0
- ❌ Não pode cadastrar CPF duplicado

**Response 201 Created:**
```json
{
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "pixLimit": 5000.00,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

**Response 400 Bad Request:**
```json
{
  "message": "CPF inválido: 12345678900"
}
```

---

#### 1.2. Buscar Limite por CPF

**GET** `/api/accountlimits/{document}`

Retorna as informações de limite de uma conta específica.

**Exemplo:**
```
GET /api/accountlimits/12345678909
```

**Response 200 OK:**
```json
{
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "pixLimit": 4500.00,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T14:20:00Z"
}
```

**Response 404 Not Found:**
```json
{
  "message": "Conta com documento 12345678909 não encontrada."
}
```

---

#### 1.3. Atualizar Limite

**PUT** `/api/accountlimits/{document}`

Atualiza o limite PIX de uma conta existente.

**Exemplo:**
```
PUT /api/accountlimits/12345678909
```

**Request Body:**
```json
{
  "newPixLimit": 10000.00
}
```

**Validações:**
- ✅ Novo limite deve ser ≥ 0
- ✅ CPF deve existir no banco

**Response 200 OK:**
```json
{
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "pixLimit": 10000.00,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T15:45:00Z"
}
```

---

#### 1.4. Remover Limite

**DELETE** `/api/accountlimits/{document}`

Remove um limite cadastrado do banco de dados.

**Exemplo:**
```
DELETE /api/accountlimits/12345678909
```

**Response 204 No Content**

(Sem corpo de resposta - sucesso)

**Response 404 Not Found:**
```json
{
  "message": "Conta com documento 12345678909 não encontrada."
}
```

---

### 2. PIX Transactions - Processamento de Transações

#### 2.1. Processar Transação PIX

**POST** `/api/pix/transactions`

Processa uma transação PIX validando o limite disponível.

**Request Body:**
```json
{
  "transactionId": "opcional-uuid-aqui",
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "amount": 150.00
}
```

**Campos:**

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `transactionId` | string | ❌ Não | ID único da transação (veja explicação abaixo) |
| `document` | string | ✅ Sim | CPF do titular |
| `agencyNumber` | string | ✅ Sim | Número da agência |
| `accountNumber` | string | ✅ Sim | Número da conta |
| `amount` | decimal | ✅ Sim | Valor da transação (> 0) |

**⚠️ Sobre o `transactionId` (Campo Opcional):**

Este campo implementa o conceito de **IDEMPOTÊNCIA**, crucial em sistemas financeiros:

**Quando NÃO enviar (deixar vazio/null):**
```json
{
  "transactionId": null,
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "amount": 150.00
}
```
- ✅ **Comportamento**: O sistema gera um novo UUID automaticamente
- ✅ **Use quando**: Cada requisição representa uma transação diferente

**Quando ENVIAR um ID específico:**
```json
{
  "transactionId": "minha-transacao-123",
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "amount": 150.00
}
```
- ✅ **Comportamento**: Se esse ID já foi processado, retorna o resultado anterior SEM processar novamente
- ✅ **Use quando**: Quer garantir que uma transação não seja duplicada (ex: retry de rede, falha temporária)

**Exemplo de Idempotência:**

**1ª Requisição:**
```json
POST /api/pix/transactions
{
  "transactionId": "txn-001",
  "document": "12345678909",
  "amount": 100.00,
  ...
}
```
**Resposta:** Transação processada, limite consumido

**2ª Requisição (idêntica - retry acidental):**
```json
POST /api/pix/transactions
{
  "transactionId": "txn-001",  // MESMO ID
  "document": "12345678909",
  "amount": 100.00,
  ...
}
```
**Resposta:** Retorna o resultado da 1ª requisição, **NÃO consome limite novamente**

---

**Validações:**
- ✅ Valor deve ser > 0
- ✅ CPF deve existir no cadastro
- ✅ Agência e conta devem corresponder ao CPF
- ✅ Se `transactionId` fornecido e já existe, retorna resultado anterior

**Lógica de Processamento:**

```
1. Verifica se transactionId já foi processado (idempotência)
   └─ Se SIM: retorna resultado anterior
   └─ Se NÃO: continua

2. Busca o limite da conta pelo CPF
   └─ Se não existe: erro 404

3. Valida agência e conta
   └─ Se não correspondem: erro 400

4. Verifica se há limite disponível
   ├─ Se SIM: 
   │   ├─ Aprova transação
   │   ├─ Desconta do limite
   │   └─ Salva no histórico
   └─ Se NÃO:
       ├─ Nega transação
       ├─ NÃO desconta limite
       └─ Salva no histórico
```

**Response 200 OK (Aprovada):**
```json
{
  "transactionId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "amount": 150.00,
  "status": "Approved",
  "statusMessage": "Transação aprovada com sucesso",
  "remainingLimit": 4850.00,
  "processedAt": "2024-01-15T16:30:00Z"
}
```

**Response 200 OK (Negada por limite insuficiente):**
```json
{
  "transactionId": "a23bc10b-58cc-4372-a567-0e02b2c3d123",
  "document": "12345678909",
  "agencyNumber": "0001",
  "accountNumber": "123456",
  "amount": 6000.00,
  "status": "Denied",
  "statusMessage": "Limite insuficiente. Disponível: R$ 4.850,00, Solicitado: R$ 6.000,00",
  "remainingLimit": 4850.00,
  "processedAt": "2024-01-15T16:35:00Z"
}
```

**⚠️ Importante:** Mesmo transações NEGADAS retornam status **200 OK**, pois foram processadas com sucesso. O campo `status` indica se foi aprovada ou negada.

**Response 400 Bad Request:**
```json
{
  "message": "Agência ou conta não correspondem ao CPF informado"
}
```

**Response 404 Not Found:**
```json
{
  "message": "Conta com documento 12345678909 não encontrada."
}
```

---

## 💡 Conceitos Importantes

### 1. Validação de CPF

O sistema implementa validação completa de CPF usando o algoritmo oficial:

```
CPFs Válidos (para testes):
✅ 12345678909
✅ 11144477735
✅ 52998224725

CPFs Inválidos:
❌ 12345678900 (dígito verificador errado)
❌ 11111111111 (todos dígitos iguais)
❌ 123456789 (menos de 11 dígitos)
```

### 2. Idempotência

**Problema que resolve:**
```
Cliente → [Requisição PIX R$ 100]
            ↓ (timeout de rede)
Cliente → [Requisição PIX R$ 100] (retry)

Sem idempotência: Cobrado R$ 200 ❌
Com idempotência: Cobrado R$ 100 ✅
```

**Como usar:**
```javascript
// Cliente gera ID único
const myTransactionId = `txn-${Date.now()}-${Math.random()}`;

// Envia requisição
fetch('/api/pix/transactions', {
  method: 'POST',
  body: JSON.stringify({
    transactionId: myTransactionId, // Sempre o mesmo ID para retries
    document: "12345678909",
    amount: 100.00,
    // ...
  })
});

// Se der timeout, pode fazer retry com MESMO transactionId
// Sistema não processará duas vezes
```

### 3. Controle de Limite

```
Limite Inicial: R$ 5.000,00

┌─────────────────────────────────────┐
│ Transação #1: R$ 150,00 ✅ Aprovada │
├─────────────────────────────────────┤
│ Limite Atual: R$ 4.850,00           │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Transação #2: R$ 6.000,00 ❌ Negada │
├─────────────────────────────────────┤
│ Limite Atual: R$ 4.850,00 (inalterado) │
└─────────────────────────────────────┘
```

**Regras:**
- ✅ Limite suficiente = Aprova e desconta
- ❌ Limite insuficiente = Nega e NÃO desconta
- 📊 Todas as transações são registradas (aprovadas e negadas)

### 4. Value Objects

O projeto usa **Value Objects** do DDD para encapsular validações:

```csharp
// Document (CPF) - Valida automaticamente
var document = new Document("12345678909");

// Money (Valor monetário) - Impede valores negativos
var amount = new Money(100.00m);

// Se tentar criar com dados inválidos, lança exceção
var invalidDoc = new Document("111111111"); // ❌ Exceção!
```

---

## 📁 Estrutura do Projeto

```
FraudSys/
│
├── FraudSys.API/                           # Camada de Apresentação
│   ├── Controllers/
│   │   ├── AccountLimitsController.cs      # Endpoints de limite
│   │   └── TransactionsController.cs       # Endpoints de PIX
│   ├── Middlewares/
│   │   └── ExceptionHandlingMiddleware.cs  # Tratamento global de erros
│   ├── Program.cs                          # Configuração da aplicação
│   └── appsettings.json                    # Configurações (DynamoDB)
│
├── FraudSys.Application/                   # Casos de Uso
│   ├── DTOs/
│   │   ├── CreateAccountLimitDto.cs
│   │   ├── UpdateAccountLimitDto.cs
│   │   ├── AccountLimitResponseDto.cs
│   │   ├── ProcessPixTransactionDto.cs
│   │   └── PixTransactionResponseDto.cs
│   ├── Interfaces/
│   │   ├── IAccountLimitService.cs
│   │   └── IPixTransactionService.cs
│   ├── Services/
│   │   ├── AccountLimitService.cs          # Lógica de gestão de limites
│   │   └── PixTransactionService.cs        # Lógica de transações
│   ├── Validators/
│   │   ├── CreateAccountLimitValidator.cs
│   │   ├── UpdateAccountLimitValidator.cs
│   │   └── ProcessPixTransactionValidator.cs
│   └── Mappings/
│       └── MappingProfile.cs               # AutoMapper
│
├── FraudSys.Domain/                        # Lógica de Negócio Pura
│   ├── Entities/
│   │   ├── AccountLimit.cs                 # Entidade de limite
│   │   └── PixTransaction.cs               # Entidade de transação
│   ├── ValueObjects/
│   │   ├── Document.cs                     # CPF com validação
│   │   └── Money.cs                        # Valor monetário
│   ├── Enums/
│   │   └── TransactionStatus.cs            # Approved/Denied
│   ├── Exceptions/
│   │   ├── DomainException.cs
│   │   ├── AccountLimitNotFoundException.cs
│   │   ├── InsufficientLimitException.cs
│   │   └── InvalidDocumentException.cs
│   └── Interfaces/
│       ├── IAccountLimitRepository.cs
│       └── IPixTransactionRepository.cs
│
└── FraudSys.Infrastructure/                # Infraestrutura
    ├── Data/
    │   └── Repositories/
    │       ├── AccountLimitRepository.cs   # Implementação DynamoDB
    │       └── PixTransactionRepository.cs
    ├── Configuration/
    │   └── DynamoDbSettings.cs             # Settings AWS
    └── Extensions/
        └── DependencyInjection.cs          # Injeção de dependências
```

---

## 🛠️ Troubleshooting

### Erro: "Cannot connect to DynamoDB"

**Solução:**
```bash
# Verifique se o container está rodando
docker ps

# Se não estiver, inicie novamente
docker run -d -p 8000:8000 amazon/dynamodb-local
```

### Erro: "Table not found"

**Solução:** As tabelas são criadas automaticamente ao iniciar a aplicação. Certifique-se de que a aplicação foi iniciada pelo menos uma vez.

---

## 📝 Notas do Desenvolvedor

### Decisões Técnicas

1. **TransactionId Opcional**: Implementado para suportar idempotência, permitindo que clientes evitem processamento duplicado em caso de retries
2. **DynamoDB Local**: Escolhido para facilitar desenvolvimento sem custos de AWS
3. **Value Objects**: Usados para encapsular validações e garantir consistência de dados
4. **Clean Architecture**: Garante testabilidade e manutenibilidade do código
