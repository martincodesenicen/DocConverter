namespace DocConverter.Domain.Entities;

public class StoredFile
{
    public Guid Id { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty; // Su ubicación en el sv
    public long SizeInBytes { get; set; }
    public string ContentType { get; set; } = string.Empty; // ej: application/pdf
    public DateTime UploadedAt { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}