using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces;
using PostService.Domain.Entities;
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
}

app.Run();
