using FraudSys.Domain.Exceptions;
using FraudSys.Domain.ValueObjects;

namespace FraudSys.Domain.Entities
{
    public class AccountLimit
    {
        public Document Document { get; private set; }
        public string AgencyNumber { get; private set; }
        public string AccountNumber { get; private set; }
        public Money PixLimit { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public AccountLimit(string document, string agencyNumber, string accountNumber, decimal pixLimit)
        {
            Document = new Document(document);
            SetAgencyNumber(agencyNumber);
            SetAccountNumber(accountNumber);
            PixLimit = new Money(pixLimit);
            CreatedAt = DateTime.UtcNow;
        }

        private AccountLimit() { }

        public static AccountLimit Reconstruct(string document, string agencyNumber, string accountNumber,
            decimal pixLimit, DateTime createdAt, DateTime? updatedAt)
        {
            return new AccountLimit
            {
                Document = new Document(document),
                AgencyNumber = agencyNumber,
                AccountNumber = accountNumber,
                PixLimit = new Money(pixLimit),
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
        }

        public void UpdateLimit(decimal newLimit)
        {
            PixLimit = new Money(newLimit);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ConsumeLimit(decimal amount)
        {
            var transactionAmount = new Money(amount);

            if (!PixLimit.IsGreaterThanOrEqual(transactionAmount))
            {
                throw new InsufficientLimitException(PixLimit.Value, amount);
            }

            PixLimit = PixLimit.Subtract(transactionAmount);
            UpdatedAt = DateTime.UtcNow;
        }

        private void SetAgencyNumber(string agencyNumber)
        {
            if (string.IsNullOrWhiteSpace(agencyNumber))
                throw new DomainException("Número da agência é obrigatório");

            AgencyNumber = agencyNumber;
        }

        private void SetAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new DomainException("Número da conta é obrigatório");

            AccountNumber = accountNumber;
        }
    }
}
