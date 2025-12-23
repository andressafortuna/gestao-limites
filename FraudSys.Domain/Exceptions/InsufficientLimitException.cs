namespace FraudSys.Domain.Exceptions
{
    public class InsufficientLimitException : DomainException
    {
        public InsufficientLimitException(decimal availableLimit, decimal requestedAmount)
            : base($"Limite insuficiente. Disponível: {availableLimit:C}, Solicitado: {requestedAmount:C}")
        {
        }
    }
}
