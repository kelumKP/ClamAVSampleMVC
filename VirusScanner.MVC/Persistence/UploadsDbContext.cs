using Microsoft.EntityFrameworkCore;
using VirusScanner.MVC.Domain.Entities;

namespace VirusScanner.MVC.Persistence
{
    public class UploadsDbContext : DbContext
    {
        public UploadsDbContext(DbContextOptions<UploadsDbContext> options)
            : base(options)
        {
        }

        public DbSet<File> Files { get; set; }
        public DbSet<Virus> Viruses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UploadsDbContext).Assembly);
        }
    }
}
