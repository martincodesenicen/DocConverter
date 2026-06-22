using DocConverter.Domain.Entities;

namespace DocConverter.Application.Interfaces;

public interface IStoredFileRepository
{
    Task AddAsync(StoredFile file);
    Task SaveChangesAsync();
}