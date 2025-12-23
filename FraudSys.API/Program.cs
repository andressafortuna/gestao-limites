using FluentValidation;
using FraudSys.API.Middlewares;
using FraudSys.Application.Validators;
using FraudSys.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FraudSys API - Banco KRT",
        Version = "v1",
        Description = "Sistema de Gestão de Limites PIX"
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountLimitValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

await InitializeDynamoDbTables(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task InitializeDynamoDbTables(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dynamoDb = scope.ServiceProvider.GetRequiredService<Amazon.DynamoDBv2.IAmazonDynamoDB>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var accountLimitsTableName = configuration["DynamoDB:AccountLimitsTableName"] ?? "AccountLimits";
    var pixTransactionsTableName = configuration["DynamoDB:PixTransactionsTableName"] ?? "PixTransactions";

    try
    {
        var tables = await dynamoDb.ListTablesAsync();

        if (!tables.TableNames.Contains(accountLimitsTableName))
        {
            var createTableRequest = new Amazon.DynamoDBv2.Model.CreateTableRequest
            {
                TableName = accountLimitsTableName,
                KeySchema = new List<Amazon.DynamoDBv2.Model.KeySchemaElement>
                {
                    new Amazon.DynamoDBv2.Model.KeySchemaElement
                    {
                        AttributeName = "Document",
                        KeyType = Amazon.DynamoDBv2.KeyType.HASH
                    }
                },
                AttributeDefinitions = new List<Amazon.DynamoDBv2.Model.AttributeDefinition>
                {
                    new Amazon.DynamoDBv2.Model.AttributeDefinition
                    {
                        AttributeName = "Document",
                        AttributeType = Amazon.DynamoDBv2.ScalarAttributeType.S
                    }
                },
                BillingMode = Amazon.DynamoDBv2.BillingMode.PAY_PER_REQUEST
            };

            await dynamoDb.CreateTableAsync(createTableRequest);
            Console.WriteLine($"Tabela {accountLimitsTableName} criada com sucesso!");
        }

        if (!tables.TableNames.Contains(pixTransactionsTableName))
        {
            var createTableRequest = new Amazon.DynamoDBv2.Model.CreateTableRequest
            {
                TableName = pixTransactionsTableName,
                KeySchema = new List<Amazon.DynamoDBv2.Model.KeySchemaElement>
                {
                    new Amazon.DynamoDBv2.Model.KeySchemaElement
                    {
                        AttributeName = "TransactionId",
                        KeyType = Amazon.DynamoDBv2.KeyType.HASH
                    }
                },
                AttributeDefinitions = new List<Amazon.DynamoDBv2.Model.AttributeDefinition>
                {
                    new Amazon.DynamoDBv2.Model.AttributeDefinition
                    {
                        AttributeName = "TransactionId",
                        AttributeType = Amazon.DynamoDBv2.ScalarAttributeType.S
                    },
                    new Amazon.DynamoDBv2.Model.AttributeDefinition
                    {
                        AttributeName = "Document",
                        AttributeType = Amazon.DynamoDBv2.ScalarAttributeType.S
                    }
                },
                GlobalSecondaryIndexes = new List<Amazon.DynamoDBv2.Model.GlobalSecondaryIndex>
                {
                    new Amazon.DynamoDBv2.Model.GlobalSecondaryIndex
                    {
                        IndexName = "Document-Index",
                        KeySchema = new List<Amazon.DynamoDBv2.Model.KeySchemaElement>
                        {
                            new Amazon.DynamoDBv2.Model.KeySchemaElement
                            {
                                AttributeName = "Document",
                                KeyType = Amazon.DynamoDBv2.KeyType.HASH
                            }
                        },
                        Projection = new Amazon.DynamoDBv2.Model.Projection
                        {
                            ProjectionType = Amazon.DynamoDBv2.ProjectionType.ALL
                        }
                    }
                },
                BillingMode = Amazon.DynamoDBv2.BillingMode.PAY_PER_REQUEST
            };

            await dynamoDb.CreateTableAsync(createTableRequest);
            Console.WriteLine($"Tabela {pixTransactionsTableName} criada com sucesso!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar tabelas: {ex.Message}");
    }
}