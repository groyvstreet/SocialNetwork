using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces;
using PostService.Domain.Entities;
using PostService.Grpc.Services;
using PostService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var assemblyName = builder.Configuration.GetSection("MigrationsAssembly").Get<string>();
builder.Services.AddDbContext<DataContext>(
    opt => opt.UseNpgsql(connectionString,
                         npgsqlOpt => npgsqlOpt.MigrationsAssembly(assemblyName)));

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddTransient<IRepository<UserProfile>, Repository<UserProfile>>();
builder.Services.AddTransient<IRepository<Post>, Repository<Post>>();

builder.Services.AddTransient<IPostService, PostService.Application.Services.PostService>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<GrpcPostService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();

    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();
