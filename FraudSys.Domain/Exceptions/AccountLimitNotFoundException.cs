namespace FraudSys.Domain.Exceptions
{
    public class AccountLimitNotFoundException : DomainException
    {
        public AccountLimitNotFoundException(string document)
           : base($"Conta com documento {document} não encontrada.")
        {
        }
    }
}
