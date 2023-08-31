using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetSection("MongoConnection").Get<string>();
var database = builder.Configuration.GetSection("MongoDatabase").Get<string>();
builder.Services.AddSingleton(new MongoClient(connection).GetDatabase(database));

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
