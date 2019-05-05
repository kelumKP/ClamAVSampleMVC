using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using VirusScanner.MVC.Domain.Entities;

namespace VirusScanner.MVC.Persistence.Configurations
{
    public class VirusConfiguration : IEntityTypeConfiguration<Virus>
    {
        public void Configure(EntityTypeBuilder<Virus> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(v => v.FileId)
                .IsRequired();

            builder.Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasOne(v => v.File)
                .WithMany(f => f.Viruses)
                .HasForeignKey(v => v.FileId);
        }
    }
}
