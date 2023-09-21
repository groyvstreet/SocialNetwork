using ChatService.API.Extensions;
using ChatService.Application.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConnection(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy",
        builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        //.AllowAnyOrigin()
        .WithOrigins("http://127.0.0.1:3000")
        .AllowCredentials()
        .SetIsOriginAllowed((hosts) => true));
});

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddAutoMapper();
builder.Services.AddMediatR();
builder.Services.AddServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseGlobalExceptionHandler();

app.UseCors("CORSPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<ChatHub>("/dialogs");

await app.InitializeDatabaseAsync();

app.Run();
