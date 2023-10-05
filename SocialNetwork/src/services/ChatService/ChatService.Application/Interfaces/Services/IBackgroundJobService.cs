using System.Linq.Expressions;

namespace ChatService.Application.Interfaces.Services
{
    public interface IBackgroundJobService
    {
        string AddSchedule(Expression<Func<Task>> method, TimeSpan delay);

        void AddRecurringJob(string recurringJobId, Expression<Func<Task>> method, Func<string> cronExpression);
    }
}
