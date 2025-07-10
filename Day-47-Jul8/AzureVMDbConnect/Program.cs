using AzureVMDbConnect.Context;
using AzureVMDbConnect.Interfaces;
using AzureVMDbConnect.Repositories;
using AzureVMDbConnect.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers();

builder.Services.AddTransient<IUserRepository,UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddDbContext<UserContext>(options=>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
