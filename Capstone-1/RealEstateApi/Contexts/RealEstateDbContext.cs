using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RealEstateApi.Models;

namespace RealEstateApi.Contexts
{
    public class RealEstateDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RealEstateDbContext(DbContextOptions options,IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<PropertyListing> PropertyListings { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var entries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetTableName(),
                    UserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                };
                entries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.ActionType = "Create";
                            break;
                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.ActionType = "Delete";
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                auditEntry.ActionType = "Update";
                            }
                            break;
                    }
                }
            }

            return entries;
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                var auditLog = auditEntry.ToAuditLog();
                AuditLogs.Add(auditLog);
            }

            return SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            // user <-> agent
            modelBuilder.Entity<Agent>()
                        .HasOne(a => a.User)
                        .WithOne(u => u.AgentProfile)
                        .HasForeignKey<Agent>(a => a.Id)
                        .OnDelete(DeleteBehavior.Restrict);

            // user <-> buyer
            modelBuilder.Entity<Buyer>()
                        .HasOne(b => b.User)
                        .WithOne(u => u.BuyerProfile)
                        .HasForeignKey<Buyer>(b => b.Id)
                        .OnDelete(DeleteBehavior.Restrict);

            //  Agent <-> PropertyListing
            modelBuilder.Entity<PropertyListing>()
                        .HasOne(pl => pl.Agent)
                        .WithMany(a => a.Listings)
                        .HasForeignKey(pl => pl.AgentId)
                        .OnDelete(DeleteBehavior.Restrict);

            //  Buyer <-> Inquires
            modelBuilder.Entity<Inquiry>()
                        .HasOne(i => i.Buyer)
                        .WithMany(b => b.Inquiries)
                        .HasForeignKey(i => i.BuyerId)
                        .OnDelete(DeleteBehavior.Restrict);

            // PropertyListing <-> Inquiry
            modelBuilder.Entity<Inquiry>()
                        .HasOne(i => i.Listing)
                        .WithMany(l => l.Inquiries)
                        .HasForeignKey(i => i.ListingId)
                        .OnDelete(DeleteBehavior.Restrict);

            // PropertyListing <-> PropertyImages
            modelBuilder.Entity<PropertyImage>()
                        .HasOne(pi => pi.Listing)
                        .WithMany(pl => pl.Images)
                        .HasForeignKey(pi => pi.PropertyListingId)
                        .OnDelete(DeleteBehavior.Restrict);

            // Optional: Global query filters for soft delete
            // modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            // modelBuilder.Entity<PropertyListing>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}