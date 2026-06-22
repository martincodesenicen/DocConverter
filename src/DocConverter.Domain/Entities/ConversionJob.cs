using DocConverter.Domain.Enums;

namespace DocConverter.Domain.Entities;

public class ConversionJob
{
    public Guid Id { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Relaciones
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid SourceFileId { get; set; }
    public StoredFile SourceFile { get; set; } = null!;

    public Guid? ResultFileId { get; set; } // Nullable porque al inicio no hay PDF resultado
    public StoredFile? ResultFile { get; set; }
}