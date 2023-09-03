using Microsoft.EntityFrameworkCore;
using PostService.API.Middlewares;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Services;
using PostService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var assemblyName = builder.Configuration.GetSection("MigrationsAssembly").Get<string>();
builder.Services.AddDbContext<DataContext>(
    opt => opt.UseNpgsql(connectionString,
                         npgsqlOpt => npgsqlOpt.MigrationsAssembly(assemblyName)));

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPostRepository, PostRepository>();
builder.Services.AddTransient<IPostLikeRepository, PostLikeRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<ICommentLikeRepository, CommentLikeRepository>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPostService, PostService.Application.Services.PostService>();
builder.Services.AddTransient<IPostLikeService, PostLikeService>();
builder.Services.AddTransient<ICommentService, CommentService>();
builder.Services.AddTransient<ICommentLikeService, CommentLikeService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();

    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }

    await DbInitializer.SeedData(context);
}

app.Run();
