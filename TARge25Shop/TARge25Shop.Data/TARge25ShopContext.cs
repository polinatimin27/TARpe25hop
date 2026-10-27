
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using TARge25Shop.Core.Domain;

namespace TARge25Shop.Data
{
    public class TARge25ShopContext : DbContext
    {
        public TARge25ShopContext(
            DbContextOptions<TARge25ShopContext> options)
            : base(options)
        {
        }

        public DbSet<Spaceship> Spaceships { get; set; }
        public DbSet<FileToApi> FileToApis { get; set; }
        public DbSet<RealEstate> RealEstates { get; set; }
        public DbSet<Kindergarten> Kindergartens { get; set; }

        public DbSet<Book> Books { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Reader)
                .WithMany(r => r.Loans)
                .HasForeignKey(l => l.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
