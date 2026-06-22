using DocConverter.Application.Interfaces;
using DocConverter.Domain.Entities;
using DocConverter.Infrastructure.Persistence;

namespace DocConverter.Infrastructure.Repositories;

public class StoredFileRepository : IStoredFileRepository
{
    private readonly ApplicationDbContext _context;

    public StoredFileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StoredFile file)
    {
        await _context.StoredFiles.AddAsync(file);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}