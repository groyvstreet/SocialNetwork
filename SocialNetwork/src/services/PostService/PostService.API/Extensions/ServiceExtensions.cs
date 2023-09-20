using FluentValidation;
using FluentValidation.AspNetCore;
using PostService.Application.AutoMapperProfiles;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Services;
using PostService.Application.Validators.PostValidators;
using PostService.Infrastructure.Repositories;

namespace PostService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AddPostValidator>();
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile).Assembly);
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostLikeRepository, PostLikeRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, Application.Services.PostService>();
            services.AddScoped<IPostLikeService, PostLikeService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ICommentLikeService, CommentLikeService>();
        }
    }
}
