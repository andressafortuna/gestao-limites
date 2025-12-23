namespace FraudSys.Infrastructure.Configuration
{
    public class DynamoDbSettings
    {
        public string ServiceURL { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Region { get; set; } = "us-east-1";
        public string AccountLimitsTableName { get; set; } = "AccountLimits";
        public string PixTransactionsTableName { get; set; } = "PixTransactions";
    }
}
