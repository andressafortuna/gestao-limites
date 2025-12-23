namespace FraudSys.Domain.Exceptions
{
    public class InvalidDocumentException : DomainException
    {
        public InvalidDocumentException(string document)
            : base($"CPF inválido: {document}")
        {
        }
    }
}
