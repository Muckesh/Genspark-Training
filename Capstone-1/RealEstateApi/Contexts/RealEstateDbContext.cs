using Microsoft.EntityFrameworkCore;
using RealEstateApi.Models;

namespace RealEstateApi.Contexts
{
    public class RealEstateDbContext : DbContext
    {
        public RealEstateDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<PropertyListing> PropertyListings { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }

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