using Microsoft.EntityFrameworkCore;

public class NotifyDbContext : DbContext
{
    public NotifyDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasOne(u => u.User)
                                        .WithOne(e => e.Employee)
                                        .HasForeignKey<Employee>(e => e.Email)
                                        .HasConstraintName("FK_User_Employee")
                                        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Document>().HasOne(d => d.UploadedBy)
                                        .WithMany()
                                        .HasForeignKey(u => u.UploadedById);
    }
}