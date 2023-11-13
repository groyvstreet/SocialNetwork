using ChatService.Application;
using ChatService.Domain.Entities;
using ChatService.Infrastructure;
using Hangfire;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ChatServiceIntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _mongoConnection;
        private readonly string _redis;
        private readonly string _bootstrapAddress;

        public CustomWebApplicationFactory(string mongoConnection, string redis, string bootstrapAddress)
        {
            _mongoConnection = mongoConnection;
            _redis = redis;
            _bootstrapAddress = bootstrapAddress;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var serviceDesciptor = services.SingleOrDefault(serviceDesciptor =>
                    serviceDesciptor.ServiceType == typeof(IMongoDatabase));

                if (serviceDesciptor is not null)
                {
                    services.Remove(serviceDesciptor);
                }

                services.AddSingleton(new MongoClient(_mongoConnection).GetDatabase("chat-service"));

                services.AddStackExchangeRedisCache(redisCacheOptions =>
                {
                    redisCacheOptions.Configuration = _redis;
                });

                services.Configure<KafkaConsumerConfig<RequestOperation, User>>(kafkaConsumerConfig =>
                {
                    kafkaConsumerConfig.BootstrapServers = _bootstrapAddress;
                    kafkaConsumerConfig.GroupId = "post_group";
                    kafkaConsumerConfig.Topic = "users";
                });

                var mongoMigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new DropMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy()
                };

                var mongoStorageOptions = new MongoStorageOptions
                {
                    MigrationOptions = mongoMigrationOptions,
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
                    CheckConnection = false
                };

                services.AddHangfire(config =>
                {
                    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                    config.UseSimpleAssemblyNameTypeSerializer();
                    config.UseRecommendedSerializerSettings();
                    config.UseMongoStorage(_mongoConnection, "chat-service", mongoStorageOptions);
                });
            });
        }
    }
}
