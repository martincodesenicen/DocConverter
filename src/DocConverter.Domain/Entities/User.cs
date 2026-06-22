namespace DocConverter.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Un usuario tiene muchos archivos
    public ICollection<StoredFile> Files { get; set; } = new List<StoredFile>();
}