using ChatService.API.Extensions;
using ChatService.Application.Commands.MessageCommands.MessageCommandHandlers;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConnection(builder.Configuration);

builder.Services.AddMediatR(typeof(AddDialogMessageCommandHandler));
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
