namespace DocConverter.Domain.Entities;

public class ConversionJobSourceFile
{
    public Guid ConversionJobId { get; set; }
    public ConversionJob ConversionJob { get; set; } = null!;

    public Guid StoredFileId { get; set; }
    public StoredFile StoredFile { get; set; } = null!;

    // defino qué archivo va primero, segundo, etc.
    public int SequenceOrder { get; set; } 
}