using AzureVMDbConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureVMDbConnect.Context
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

    }
}

// dotnet ef database update --context UserContext --connection "User ID=postgres;Password=password;Host=172.203.225.12;Port=5432;Database=AzureVMDB;"

// muckesh@C02G7BQGML7H-muckesh AzureVMDbConnect % dotnet ef dbcontext info

// Build started...
// Build succeeded.
// Type: AzureVMDbConnect.Context.UserContext
// Provider name: Npgsql.EntityFrameworkCore.PostgreSQL
// Database name: AzureVMDB
// Data source: tcp://172.203.225.12:5432
// Options: None