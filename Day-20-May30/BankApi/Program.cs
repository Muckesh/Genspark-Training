using BankApi.Contexts;
using BankApi.Interfaces;
using BankApi.Models;
using BankApi.Repositories;
using BankApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

builder.Services.AddTransient<IRepository<Guid, Account>, AccountRepository>();
builder.Services.AddTransient<IRepository<Guid,Transaction>,TransactionRepository>();

// builder.Services.AddTransient<IBankService,BankService>();
builder.Services.AddTransient<IAccountService,AccountService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<ITransferServiceWithTransaction, TransferServiceWithTransaction>();
builder.Services.AddTransient<IFaqService,FaqService>();

builder.Services.AddHttpClient<FaqService>();


builder.Services.AddDbContext<BankDbContext>(options =>
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

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();


