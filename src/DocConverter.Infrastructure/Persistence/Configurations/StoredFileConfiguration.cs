using DocConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocConverter.Infrastructure.Persistence.Configurations;

public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("StoredFiles");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.OriginalName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.StoragePath)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.UploadedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        //  Un Usuario tiene muchos Archivos
        builder.HasOne(f => f.User)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Si se borra el usuario, se borran sus registros de archivos
    }
}