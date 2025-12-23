namespace FraudSys.Application.DTOs
{
    public class AccountLimitResponseDto
    {
        public string Document { get; set; } = string.Empty;
        public string AgencyNumber { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal PixLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
