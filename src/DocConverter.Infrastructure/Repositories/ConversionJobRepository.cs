using DocConverter.Application.Interfaces;
using DocConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DocConverter.Infrastructure.Persistence;

public class ConversionJobRepository : IConversionJobRepository
{
    private readonly ApplicationDbContext _context;

    public ConversionJobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ConversionJob job)
    {
        await _context.ConversionJobs.AddAsync(job);
    }

    public async Task<ConversionJob?> GetByIdAndUserAsync(Guid jobId, Guid userId)
    {
        return await _context.ConversionJobs
            .FirstOrDefaultAsync(x => x.Id == jobId && x.UserId == userId);
    }

    public async Task<ConversionJob?> GetJobWithResultFileAsync(Guid jobId, Guid userId)
    {
        return await _context.ConversionJobs
            .Include(j => j.ResultFile)
            .FirstOrDefaultAsync(j => j.Id == jobId && j.UserId == userId);
    }
}