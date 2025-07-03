using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Contexts
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(a => a.Id);

            modelBuilder.Entity<Transaction>().HasKey(t => t.Id);

            modelBuilder.Entity<Transaction>()
                            .HasOne(t => t.Account)
                            .WithMany(a => a.Transactions)
                            .HasForeignKey(t => t.AccountId);
        }
    }
}