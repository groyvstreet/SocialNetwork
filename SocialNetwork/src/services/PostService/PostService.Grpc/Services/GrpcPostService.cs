using AutoMapper;
using Grpc.Core;
using PostService.Application.Interfaces;

namespace PostService.Grpc.Services
{
    public class GrpcPostService : GrpcPost.GrpcPostBase
    {
        private readonly ILogger<GrpcPostService> logger;
        private readonly IPostService postService;

        public GrpcPostService(ILogger<GrpcPostService> logger,
                               IPostService postService)
        {
            this.logger = logger;
            this.postService = postService;
        }

        public override async Task<PostList> GetAll(Empty empty, ServerCallContext context)
        {
            var posts = await postService.GetPostsAsync();
            var postList = new PostList();
            var mappedPosts = posts.Select().ToList();
            postList.Posts.AddRange(mappedPosts);

            return Task.FromResult(posts);
        }
    }
}
