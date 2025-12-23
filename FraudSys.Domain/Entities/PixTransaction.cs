using FraudSys.Domain.Enums;
using FraudSys.Domain.ValueObjects;

namespace FraudSys.Domain.Entities
{
    public class PixTransaction
    {
        public string TransactionId { get; private set; }
        public Document Document { get; private set; }
        public string AgencyNumber { get; private set; }
        public string AccountNumber { get; private set; }
        public Money Amount { get; private set; }
        public TransactionStatus Status { get; private set; }
        public string StatusMessage { get; private set; }
        public DateTime ProcessedAt { get; private set; }

        public PixTransaction(string transactionId, string document, string agencyNumber,
            string accountNumber, decimal amount)
        {
            TransactionId = string.IsNullOrWhiteSpace(transactionId)
                ? Guid.NewGuid().ToString()
                : transactionId;
            Document = new Document(document);
            AgencyNumber = agencyNumber;
            AccountNumber = accountNumber;
            Amount = new Money(amount);
            ProcessedAt = DateTime.UtcNow;
        }

        public void Approve()
        {
            Status = TransactionStatus.Approved;
            StatusMessage = "Transação aprovada com sucesso";
        }

        public void Deny(string reason)
        {
            Status = TransactionStatus.Denied;
            StatusMessage = reason;
        }
    }
}
