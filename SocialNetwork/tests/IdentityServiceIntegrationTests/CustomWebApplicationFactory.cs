using IdentityService.BLL;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.DAL.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServiceIntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly ushort _msSqlServerPort;
        private readonly string _redis;
        private readonly string _bootstrapAddress;

        public CustomWebApplicationFactory(ushort msSqlServerPort, string redis, string bootstrapAddress)
        {
            _msSqlServerPort = msSqlServerPort;
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

                services.AddDbContextPool<DataContext>(dbContextOptions =>
                    dbContextOptions.UseSqlServer($"Server=127.0.0.1,{_msSqlServerPort}; Database=identity; User Id=sa; Password=S3cur3P@ssW0rd!; TrustServerCertificate=true;",
                        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

                services.AddStackExchangeRedisCache(redisCacheOptions =>
                {
                    redisCacheOptions.Configuration = _redis;
                });

                services.Configure<KafkaProducerConfig<RequestOperation, GetUserDTO>>(kafkaOptions =>
                {
                    kafkaOptions.BootstrapServers = _bootstrapAddress;
                    kafkaOptions.Topic = "users";
                });
            });
        }
    }
}
