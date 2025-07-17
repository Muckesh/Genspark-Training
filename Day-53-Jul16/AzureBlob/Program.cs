using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureBlob.Services;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

var vaultUrl = builder.Configuration["AzureBlob:VaultUrl"];
// var secretName = builder.Configuration["AzureBlob:ConnectionString"];

string? connectionString = null;


var client = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());
KeyVaultSecret secret = client.GetSecret("ConnectionString");
connectionString = secret.Value;


Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.AzureBlobStorage(
                connectionString:connectionString,
                storageContainerName: "app-logs",
                storageFileName: "log-{yyyyMMdd}.txt",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
            )
            .Enrich.FromLogContext()
            .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<BlobStorageService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();

