using DocConverter.Application.DTOs;
using DocConverter.Application.Interfaces;
using DocConverter.Domain.Entities;
using DocConverter.Domain.Enums;
using System.IO;
using DocConverter.Domain.Exceptions;

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

    public async Task<FileDownloadResponse?> DownloadResultFileAsync(Guid jobId)
    {
        var userId = _currentUser.UserId 
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        // Buscamos el Job incluyendo la metadata del archivo resultado
        var job = await _jobs.GetJobWithResultFileAsync(jobId, userId);

        // Validaciones de negocio orientadas a producción
        if (job == null)
        {
            throw new NotFoundException("Conversion job not found.");
        }
        if (job.Status == JobStatus.Failed)
        {
            throw new BadRequestException($"El trabajo de conversión falló: {job.ErrorMessage}");
        }
        if (job.Status != JobStatus.Completed || job.ResultFile == null)
        {
            throw new BadRequestException("El archivo aún no está listo para descargar.");
        }
        if (!File.Exists(job.ResultFile.StoragePath))
        {
            throw new FileNotFoundException("El archivo físico no se encuentra en el servidor.");
        }

        // Abrimos el archivo en modo lectura compartida (eficiente para múltiples descargas)
        var stream = new FileStream(job.ResultFile.StoragePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return new FileDownloadResponse(stream, job.ResultFile.ContentType, job.ResultFile.OriginalName);
    }

    public async Task<JobResponse> StartPdfMergeAsync(List<(Stream FileStream, string FileName)> files)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (files == null || files.Count < 2)
            throw new BadRequestException("Se necesitan al menos 2 archivos PDF.");

        var job = new ConversionJob
        {
            Id = Guid.NewGuid(),
            Status = JobStatus.Pending,
            UserId = userId
        };

        var fileEntities = new List<(StoredFile file, int order)>();

        for (int i = 0; i < files.Count; i++)
        {
            var fileData = files[i];

            var storagePath = await _storage.SaveFileAsync(fileData.FileStream, fileData.FileName);

            var storedFile = new StoredFile
            {
                Id = Guid.NewGuid(),
                OriginalName = fileData.FileName,
                StoragePath = storagePath,
                SizeInBytes = fileData.FileStream.Length,
                ContentType = "application/pdf",
                UserId = userId
            };

            fileEntities.Add((storedFile, i));
        }

        await _jobs.AddMergeJobAsync(job, fileEntities);

        await _queue.EnqueueAsync(job.Id);

        return new JobResponse(job.Id, job.Status.ToString(), job.CreatedAt);
    }
}