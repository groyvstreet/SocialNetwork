using ChatService.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConnection(builder.Configuration);

builder.Services.AddAutoMapper();
builder.Services.AddMediatR();
builder.Services.AddServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.InitializeDatabaseAsync();

app.Run();
