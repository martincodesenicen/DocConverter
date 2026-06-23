using DocConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocConverter.Infrastructure.Persistence.Configurations;

public class ConversionJobSourceFileConfiguration : IEntityTypeConfiguration<ConversionJobSourceFile>
{
    public void Configure(EntityTypeBuilder<ConversionJobSourceFile> builder)
    {
        builder.ToTable("ConversionJobSourceFiles");

        builder.HasKey(js => new { js.ConversionJobId, js.StoredFileId });

        builder.Property(js => js.SequenceOrder)
            .IsRequired();

        // Relaciones
        builder.HasOne(js => js.ConversionJob)
            .WithMany(j => j.JobSourceFiles)
            .HasForeignKey(js => js.ConversionJobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(js => js.StoredFile)
            .WithMany()
            .HasForeignKey(js => js.StoredFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}