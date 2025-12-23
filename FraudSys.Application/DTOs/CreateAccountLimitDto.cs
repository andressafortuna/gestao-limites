namespace FraudSys.Application.DTOs
{
    public class CreateAccountLimitDto
    {
        public string Document { get; set; } = string.Empty;
        public string AgencyNumber { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal PixLimit { get; set; }
    }
}
