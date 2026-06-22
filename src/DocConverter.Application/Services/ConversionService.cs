using DocConverter.Application.DTOs;
using DocConverter.Application.Interfaces;
using DocConverter.Domain.Entities;
using DocConverter.Domain.Enums;

namespace DocConverter.Application.Services;

public class ConversionService : IConversionService
{
    private readonly IStoredFileRepository _files;
    private readonly IConversionJobRepository _jobs;
    private readonly IFileStorageService _storage;
    private readonly ICurrentUserService _currentUser;
    private readonly IConversionQueue _queue;

    public ConversionService(
        IStoredFileRepository files,
        IConversionJobRepository jobs,
        IFileStorageService storage,
        ICurrentUserService currentUser,
        IConversionQueue queue)
    {
        _files = files;
        _jobs = jobs;
        _storage = storage;
        _currentUser = currentUser;
        _queue = queue;
    }

    public async Task<JobResponse> StartWordToPdfConversionAsync(
        Stream fileStream,
        string fileName,
        long sizeInBytes)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException();

        var storagePath = await _storage.SaveFileAsync(fileStream, fileName);

        var sourceFile = new StoredFile
        {
            Id = Guid.NewGuid(),
            OriginalName = fileName,
            StoragePath = storagePath,
            SizeInBytes = sizeInBytes,
            ContentType = "application/docx",
            UserId = userId
        };

        await _files.AddAsync(sourceFile);

        var job = new ConversionJob
        {
            Id = Guid.NewGuid(),
            Status = JobStatus.Pending,
            UserId = userId,
            SourceFileId = sourceFile.Id
        };

        await _jobs.AddAsync(job);

        await _files.SaveChangesAsync(); // o UnitOfWork

        await _queue.EnqueueAsync(job.Id);

        return new JobResponse(job.Id, job.Status.ToString(), job.CreatedAt);
    }

    public async Task<JobResponse?> GetJobStatusAsync(Guid jobId)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException();

        var job = await _jobs.GetByIdAndUserAsync(jobId, userId);

        if (job == null) return null;

        return new JobResponse(job.Id, job.Status.ToString(), job.CreatedAt);
    }
}