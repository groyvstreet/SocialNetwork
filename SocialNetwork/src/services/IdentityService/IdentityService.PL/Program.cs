using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;
using IdentityService.DAL;
using IdentityService.DAL.Interfaces;
using IdentityService.DAL.Repositories;
using IdentityService.PL.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var assemblyName = builder.Configuration.GetSection("MigrationsAssembly").Get<string>();
builder.Services.AddDbContext<DataContext>(
    opt => opt.UseSqlServer(connectionString,
                            sqlServerOpt => sqlServerOpt.MigrationsAssembly(assemblyName)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IRoleService, RoleService>();

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
}

app.Run();
