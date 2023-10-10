using ChatService.Application.Interfaces.Services;
using ChatService.Infrastructure.Protos;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace ChatService.Infrastructure.Services.GrpcServices
{
    public class PostService : IPostService
    {
        private readonly Post.PostClient _postClient;

        public PostService(IOptions<GrpcOptions> grpcOptions)
        {
            var channel = GrpcChannel.ForAddress(grpcOptions.Value.Address);
            _postClient = new Post.PostClient(channel);
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
