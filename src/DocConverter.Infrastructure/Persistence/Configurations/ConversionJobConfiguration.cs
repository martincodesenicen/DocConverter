using DocConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocConverter.Infrastructure.Persistence.Configurations;

public class ConversionJobConfiguration : IEntityTypeConfiguration<ConversionJob>
{
    public void Configure(EntityTypeBuilder<ConversionJob> builder)
    {
        builder.ToTable("ConversionJobs");

        builder.HasKey(j => j.Id);

        // Elegí almacenar el Enum como un String en la BD (ej: "Pending") en vez de un número (0)
        builder.Property(j => j.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(j => j.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(j => j.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.Property(j => j.SourceFileId)
            .IsRequired(false);

        // Relaciones
        builder.HasOne(j => j.User)
            .WithMany()
            .HasForeignKey(j => j.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.SourceFile)
            .WithMany()
            .HasForeignKey(j => j.SourceFileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.ResultFile)
            .WithMany()
            .HasForeignKey(j => j.ResultFileId)
            .OnDelete(DeleteBehavior.SetNull); // Si se borra el archivo resultado, el Job se mantiene pero en nulo
    }
}