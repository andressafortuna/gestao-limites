namespace FraudSys.Application.DTOs
{
    public class ProcessPixTransactionDto
    {
        public string? TransactionId { get; set; }
        public string Document { get; set; } = string.Empty;
        public string AgencyNumber { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
