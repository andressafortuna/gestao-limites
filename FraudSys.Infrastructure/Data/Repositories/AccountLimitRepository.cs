using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FraudSys.Domain.Entities;
using FraudSys.Domain.Interfaces;
using FraudSys.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace FraudSys.Infrastructure.Data.Repositories
{
    public class AccountLimitRepository : IAccountLimitRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName;

        public AccountLimitRepository(IAmazonDynamoDB dynamoDb, IOptions<DynamoDbSettings> settings)
        {
            _dynamoDb = dynamoDb;
            _tableName = settings.Value.AccountLimitsTableName;
        }

        public async Task<AccountLimit?> GetByDocumentAsync(string document)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Document", new AttributeValue { S = document } }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);

            if (!response.IsItemSet)
                return null;

            return MapToEntity(response.Item);
        }

        public async Task CreateAsync(AccountLimit accountLimit)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "Document", new AttributeValue { S = accountLimit.Document.Value } },
                { "AgencyNumber", new AttributeValue { S = accountLimit.AgencyNumber } },
                { "AccountNumber", new AttributeValue { S = accountLimit.AccountNumber } },
                { "PixLimit", new AttributeValue { N = accountLimit.PixLimit.Value.ToString(CultureInfo.InvariantCulture) } },
                { "CreatedAt", new AttributeValue { S = accountLimit.CreatedAt.ToString("o") } }
            };

            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = item
            };

            await _dynamoDb.PutItemAsync(request);
        }

        public async Task UpdateAsync(AccountLimit accountLimit)
        {
            var request = new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Document", new AttributeValue { S = accountLimit.Document.Value } }
                },
                UpdateExpression = "SET PixLimit = :limit, UpdatedAt = :updatedAt",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":limit", new AttributeValue { N = accountLimit.PixLimit.Value.ToString(CultureInfo.InvariantCulture) } },
                    { ":updatedAt", new AttributeValue { S = DateTime.UtcNow.ToString("o") } }
                }
            };

            await _dynamoDb.UpdateItemAsync(request);
        }

        public async Task DeleteAsync(string document)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Document", new AttributeValue { S = document } }
                }
            };

            await _dynamoDb.DeleteItemAsync(request);
        }

        public async Task<bool> ExistsAsync(string document)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Document", new AttributeValue { S = document } }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);
            return response.IsItemSet;
        }

        private AccountLimit MapToEntity(Dictionary<string, AttributeValue> item)
        {
            DateTime? updatedAt = null;
            if (item.ContainsKey("UpdatedAt"))
            {
                updatedAt = DateTime.Parse(item["UpdatedAt"].S);
            }

            return AccountLimit.Reconstruct(
                document: item["Document"].S,
                agencyNumber: item["AgencyNumber"].S,
                accountNumber: item["AccountNumber"].S,
                pixLimit: decimal.Parse(item["PixLimit"].N, CultureInfo.InvariantCulture),
                createdAt: DateTime.Parse(item["CreatedAt"].S),
                updatedAt: updatedAt
            );
        }
    }
}
