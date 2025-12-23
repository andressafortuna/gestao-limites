namespace FraudSys.Application.DTOs
{
    public class PixTransactionResponseDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string AgencyNumber { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
        public decimal RemainingLimit { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
