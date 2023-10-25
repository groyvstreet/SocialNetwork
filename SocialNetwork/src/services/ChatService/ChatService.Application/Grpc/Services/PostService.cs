using ChatService.Application.Interfaces.Services;
using ChatService.Application.Grpc.Protos;

namespace ChatService.Application.Grpc.Services
{
    public class PostService : IPostService
    {
        private readonly Post.PostClient _postClient;

        public PostService(Post.PostClient postClient)
        {
            _postClient = postClient;
        }

        public async Task<bool> IsPostExistsAsync(Guid postId)
        {
            var request = new Request { PostId = postId.ToString() };
            var reply = await _postClient.IsPostExistsAsync(request);

            return reply.IsPostExists;
        }

        public async Task UpdatePostAsync(Guid postId)
        {
            var request = new Request { PostId = postId.ToString() };
            await _postClient.UpdatePostAsync(request);
        }
    }
}
