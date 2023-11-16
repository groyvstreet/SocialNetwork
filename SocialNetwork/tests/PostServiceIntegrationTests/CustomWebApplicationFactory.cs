using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PostService.Application;
using PostService.Domain.Entities;
using PostService.Infrastructure;
using PostService.Infrastructure.Data;

namespace PostServiceIntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _postgreSqlConnection;
        private readonly string _redis;
        private readonly string _bootstrapAddress;

        public CustomWebApplicationFactory(string postgreSqlConnection, string redis, string bootstrapAddress)
        {
            _postgreSqlConnection = postgreSqlConnection;
            _redis = redis;
            _bootstrapAddress = bootstrapAddress;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var serviceDesciptor = services.SingleOrDefault(serviceDesciptor =>
                    serviceDesciptor.ServiceType == typeof(DbContextOptions<DataContext>));

                if (serviceDesciptor is not null)
                {
                    services.Remove(serviceDesciptor);
                }

                services.AddDbContextPool<DataContext>(dbContextOptions => dbContextOptions.UseNpgsql(_postgreSqlConnection));
                
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
            });
        }
    }
}
