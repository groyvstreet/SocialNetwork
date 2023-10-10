using FluentValidation;
using FluentValidation.AspNetCore;
using PostService.Application;
using PostService.Application.AutoMapperProfiles;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Services;
using PostService.Application.Validators.PostValidators;
using PostService.Domain.Entities;
using PostService.Infrastructure;
using PostService.Infrastructure.Interfaces;
using PostService.Infrastructure.Repositories;
using PostService.Infrastructure.Services;

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

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
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

            services.Configure<KafkaConsumerConfig<RequestOperation, User>>(ko =>
            {
                var section = configuration.GetSection("KafkaOptions");
                ko.BootstrapServers = section.GetSection("BootstrapServers").Get<string>();
                ko.GroupId = section.GetSection("GroupId").Get<string>();
                ko.Topic = "users";
            });
            services.AddHostedService<KafkaConsumerService<RequestOperation, User>>();
            services.AddTransient<IKafkaConsumerHandler<RequestOperation, User>, UserKafkaConsumerHandler>();
        }

        public static void MapGrpc(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapGrpcService<Infrastructure.Services.GrpcServices.PostService>();
        }
    }
}
