using PostService.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(builder.Configuration);

builder.Configuration.ConfigureLogging();
builder.Host.UseSerilog();

builder.Services.AddDatabaseConnection(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddFluentValidation();
builder.Services.AddAutoMapper();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddGrpc();
builder.Services.AddKafkaServices(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

app.MapGrpc();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.ApplyMigrations();

app.Run();

public partial class Program { }
