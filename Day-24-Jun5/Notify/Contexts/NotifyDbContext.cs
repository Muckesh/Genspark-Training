using Microsoft.EntityFrameworkCore;

public class NotifyDbContext : DbContext
{
    public NotifyDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasOne(e => e.User)
                                        .WithOne(u => u.Employee)
                                        .HasForeignKey<Employee>(e => e.Email)
                                        .HasConstraintName("FK_User_Employee")
                                        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Document>().HasOne(d => d.UploadedBy)
                                        .WithMany()
                                        .HasForeignKey(u => u.UploadedById);
    }
}