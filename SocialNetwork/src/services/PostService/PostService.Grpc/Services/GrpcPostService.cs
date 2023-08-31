using AutoMapper;
using Grpc.Core;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Interfaces;

namespace PostService.Grpc.Services
{
    public class GrpcPostService : GrpcPost.GrpcPostBase
    {
        private readonly ILogger<GrpcPostService> logger;
        private readonly IMapper mapper;
        private readonly IPostService postService;

        public GrpcPostService(ILogger<GrpcPostService> logger,
                               IMapper mapper,
                               IPostService postService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.postService = postService;
        }

        public override async Task<PostList> GetAll(Empty empty, ServerCallContext context)
        {
            var posts = await postService.GetPostsAsync();
            var postList = new PostList();
            var mappedPosts = posts.Select(mapper.Map<Post>).ToList();
            postList.Posts.AddRange(mappedPosts);

            return postList;
        }

        public override async Task<Post> Get(PostId postId, ServerCallContext context)
        {
            var id = Guid.Parse(postId.Id);
            var post = await postService.GetPostByIdAsync(id);
            var mappedPost = mapper.Map<Post>(post);

            return mappedPost;
        }

        public override async Task<Empty> Add(Post post, ServerCallContext context)
        {
            var mappedPost = mapper.Map<AddPostDTO>(post);
            await postService.AddPostAsync(mappedPost);

            return new Empty();
        }
    }
}
