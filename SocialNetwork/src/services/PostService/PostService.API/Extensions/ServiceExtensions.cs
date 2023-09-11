using FluentValidation;
using FluentValidation.AspNetCore;
using PostService.Application.AutoMapperProfiles;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Services;
using PostService.Application.Validators.CommentValidators;
using PostService.Application.Validators.PostValidators;
using PostService.Domain.Entities;
using PostService.Infrastructure.Repositories;

namespace PostService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidator<AddPostDTO>, AddPostValidator>();
            services.AddScoped<IValidator<UpdatePostDTO>, UpdatePostValidator>();
            services.AddScoped<IValidator<AddCommentDTO>, AddCommentValidator>();
            services.AddScoped<IValidator<UpdateCommentDTO>, UpdateCommentValidator>();

            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(PostProfile));
            services.AddAutoMapper(typeof(CommentProfile));
            services.AddAutoMapper(typeof(PostLikeProfile));
            services.AddAutoMapper(typeof(CommentLikeProfile));
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
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
