using ChatService.Application.Interfaces.Services;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatService.Infrastructure.Services.Hangfire
{
    public class RecurringJobExecutorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RecurringJobExecutorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

            RecurringJob.AddOrUpdate(Guid.NewGuid().ToString(), () => chatService.RemoveEmptyChatsAsync(), Cron.Daily);

            return Task.CompletedTask;
        }
    }
}
