using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FraudSys.Domain.Entities;
using FraudSys.Domain.Enums;
using FraudSys.Domain.Interfaces;
using FraudSys.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace FraudSys.Infrastructure.Data.Repositories
{
    public class PixTransactionRepository : IPixTransactionRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName;

        public PixTransactionRepository(IAmazonDynamoDB dynamoDb, IOptions<DynamoDbSettings> settings)
        {
            _dynamoDb = dynamoDb;
            _tableName = settings.Value.PixTransactionsTableName;
        }

        public async Task SaveAsync(PixTransaction transaction)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "TransactionId", new AttributeValue { S = transaction.TransactionId } },
                { "Document", new AttributeValue { S = transaction.Document.Value } },
                { "AgencyNumber", new AttributeValue { S = transaction.AgencyNumber } },
                { "AccountNumber", new AttributeValue { S = transaction.AccountNumber } },
                { "Amount", new AttributeValue { N = transaction.Amount.Value.ToString(CultureInfo.InvariantCulture) } },
                { "Status", new AttributeValue { S = transaction.Status.ToString() } },
                { "StatusMessage", new AttributeValue { S = transaction.StatusMessage } },
                { "ProcessedAt", new AttributeValue { S = transaction.ProcessedAt.ToString("o") } }
            };

            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = item
            };

            await _dynamoDb.PutItemAsync(request);
        }

        public async Task<PixTransaction?> GetByIdAsync(string transactionId)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "TransactionId", new AttributeValue { S = transactionId } }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);

            if (!response.IsItemSet)
                return null;

            return MapToEntity(response.Item);
        }

        public async Task<IEnumerable<PixTransaction>> GetByDocumentAsync(string document)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                IndexName = "Document-Index",
                KeyConditionExpression = "Document = :document",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":document", new AttributeValue { S = document } }
                }
            };

            var response = await _dynamoDb.QueryAsync(request);

            return response.Items.Select(MapToEntity).ToList();
        }

        private PixTransaction MapToEntity(Dictionary<string, AttributeValue> item)
        {
            var transaction = new PixTransaction(
                transactionId: item["TransactionId"].S,
                document: item["Document"].S,
                agencyNumber: item["AgencyNumber"].S,
                accountNumber: item["AccountNumber"].S,
                amount: decimal.Parse(item["Amount"].N, CultureInfo.InvariantCulture)
            );

            var status = Enum.Parse<TransactionStatus>(item["Status"].S);
            if (status == TransactionStatus.Approved)
            {
                transaction.Approve();
            }
            else
            {
                transaction.Deny(item["StatusMessage"].S);
            }

            return transaction;
        }
    }
}
