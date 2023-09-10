using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Services;
using PostService.Infrastructure.Repositories;

namespace PostService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IPostLikeRepository, PostLikeRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<ICommentLikeRepository, CommentLikeRepository>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPostService, Application.Services.PostService>();
            services.AddTransient<IPostLikeService, PostLikeService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<ICommentLikeService, CommentLikeService>();
        }
    }
}
