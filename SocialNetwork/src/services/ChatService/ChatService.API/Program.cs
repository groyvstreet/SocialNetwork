using ChatService.API.Extensions;
using Serilog;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureLogging();
builder.Host.UseSerilog();

builder.Services.AddDatabaseConnection(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddAutoMapper();
builder.Services.AddMediatR();
builder.Services.AddHangfire(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddKafkaServices(builder.Configuration);
builder.Services.AddGrpcServices(builder.Configuration);
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

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapSignalR();

app.UseHangfireDashboardUI();

app.Run();
