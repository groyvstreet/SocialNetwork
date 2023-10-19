namespace ChatService.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<bool> IsPostExistsAsync(Guid postId);

        Task UpdatePostAsync(Guid postId);
    }
}
