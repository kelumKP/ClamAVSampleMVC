using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirusScanner.MVC.Domain.Entities;

namespace VirusScanner.MVC.Persistence.Configurations
{
    public class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasIndex(f => f.Name)
                .IsUnique();

            builder.Property(f => f.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(f => f.Alias)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(f => f.Region)
                .IsRequired()
                .HasMaxLength(24);

            builder.Property(f => f.Bucket)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(f => f.ContentType)
                .HasMaxLength(100);

            builder.Property(f => f.Size)
                .IsRequired();

            builder.Property(f => f.Uploaded)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(f => f.ScanResult)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(f => f.Infected)
                .IsRequired();

            builder.Property(f => f.Scanned)
                .HasColumnType("datetime")
                .IsRequired();
        }
    }
}
