using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PostService.Application.Grpc.Protos;
using PostService.Application.Interfaces.PostInterfaces;

namespace PostService.Application.Grpc.Services
{
    public class PostService : Post.PostBase
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public override async Task<Reply> IsPostExists(Request request, ServerCallContext context)
        {
            var post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == Guid.Parse(request.PostId));
            var isPostExists = post is not null;
            var reply = new Reply { IsPostExists = isPostExists };

            return reply;
        }

        public override async Task<Empty> UpdatePost(Request request, ServerCallContext context)
        {
            var post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == Guid.Parse(request.PostId));
            
            if (post is not null)
            {
                post.RepostCount++;
                await _postRepository.SaveChangesAsync();
            }

            return new Empty();
        }
    }
}
