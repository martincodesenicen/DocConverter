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
    public async Task AddMergeJobAsync(ConversionJob job, List<(StoredFile file, int order)> files)
    {
        _context.ConversionJobs.Add(job);

        foreach (var (file, order) in files)
        {
            _context.StoredFiles.Add(file);

            var relation = new ConversionJobSourceFile
            {
                ConversionJobId = job.Id,
                StoredFileId = file.Id,
                SequenceOrder = order
            };

            _context.ConversionJobSourceFiles.Add(relation);
        }

        await _context.SaveChangesAsync();
    }
}