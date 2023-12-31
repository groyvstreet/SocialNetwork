using IdentityService.PL.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureLogging();
builder.Host.UseSerilog();

builder.Services.AddDatabaseConnection(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddIdentity();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddFluentValidation();
builder.Services.AddAutoMapper();
builder.Services.AddRedisCache(builder.Configuration);
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

app.ApplyMigrations();
await app.InitializeDatabase();

app.Run();
