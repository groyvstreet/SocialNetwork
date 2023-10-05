using ChatService.Application.Interfaces.Services;
using Hangfire;
using System.Linq.Expressions;

namespace ChatService.Infrastructure.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public BackgroundJobService(IBackgroundJobClient backgroundJobClient,
                                    IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        public string AddSchedule(Expression<Func<Task>> method, TimeSpan delay)
        {
            return _backgroundJobClient.Schedule(method, delay);
        }

        public void AddRecurringJob(string recurringJobId, Expression<Func<Task>> method, Func<string> cronExpression)
        {
            _recurringJobManager.AddOrUpdate(recurringJobId, method, cronExpression);
        }
    }
}
