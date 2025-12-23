using Amazon.DynamoDBv2;
using FraudSys.Application.Interfaces;
using FraudSys.Application.Mappings;
using FraudSys.Application.Services;
using FraudSys.Domain.Interfaces;
using FraudSys.Infrastructure.Configuration;
using FraudSys.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FraudSys.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DynamoDbSettings>(
                configuration.GetSection("DynamoDB"));

            var dynamoDbSettings = configuration.GetSection("DynamoDB").Get<DynamoDbSettings>();

            var clientConfig = new AmazonDynamoDBConfig();

            if (!string.IsNullOrWhiteSpace(dynamoDbSettings?.ServiceURL))
            {
                clientConfig.ServiceURL = dynamoDbSettings.ServiceURL;
            }
            else
            {
                clientConfig.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(
                    dynamoDbSettings?.Region ?? "us-east-1");
            }

            services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                if (!string.IsNullOrWhiteSpace(dynamoDbSettings?.ServiceURL))
                {
                    // Ambiente local
                    return new AmazonDynamoDBClient(
                           new Amazon.Runtime.BasicAWSCredentials("fake", "fake"),
                           clientConfig
                    );

                }
                else
                {
                    // AWS Cloud
                    return new AmazonDynamoDBClient(
                        dynamoDbSettings?.AccessKey,
                        dynamoDbSettings?.SecretKey,
                        clientConfig);
                }
            });

            // Repositórios
            services.AddScoped<IAccountLimitRepository, AccountLimitRepository>();
            services.AddScoped<IPixTransactionRepository, PixTransactionRepository>();

            // Serviços
            services.AddScoped<IAccountLimitService, AccountLimitService>();
            services.AddScoped<IPixTransactionService, PixTransactionService>();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
