using DocConverter.Application.Interfaces;
using DocConverter.Domain.Enums;
using DocConverter.Domain.Interfaces;
using DocConverter.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace DocConverter.Infrastructure.BackgroundJobs;

public class ConversionBackgroundWorker : BackgroundService
{
    private readonly IConversionQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConversionBackgroundWorker> _logger;

    public ConversionBackgroundWorker(
        IConversionQueue queue,
        IServiceProvider serviceProvider,
        ILogger<ConversionBackgroundWorker> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Conversion Background Worker iniciado correctamente.");

        // Bucle infinito que corre mientras la aplicación esté encendida
        while (!stoppingToken.IsCancellationRequested)
        {
            Guid jobId = Guid.Empty;
            try
            {
                // Espero hasta que entre un Job ID a la cola
                jobId = await _queue.DequeueAsync(stoppingToken);
                _logger.LogInformation("Procesando Job {JobId} extraído de la cola.", jobId);

                // Como los BackgroundServices son Singletons, para usar el DbContext (que es Scoped) 
                // creo manualmente un Scope por cada vuelta de procesamiento.
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var converter = scope.ServiceProvider.GetRequiredService<IContentConverter>();
                var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();

                // Busco el Job en la base de datos
                var job = await context.ConversionJobs
                    .Include(j => j.SourceFile)
                    .FirstOrDefaultAsync(j => j.Id == jobId, stoppingToken);

                if (job == null) continue;

                // Actualizo estado a Processing
                job.Status = JobStatus.Processing;
                await context.SaveChangesAsync(stoppingToken);

                // Ejecuto la conversión real
                string pdfStoragePath = await converter.ConvertToPdfAsync(job.SourceFile.StoragePath);

                // Registro el nuevo archivo PDF en la metadata
                var resultFileInfo = new FileInfo(pdfStoragePath);
                var pdfFile = new Domain.Entities.StoredFile
                {
                    Id = Guid.NewGuid(),
                    OriginalName = Path.GetFileNameWithoutExtension(job.SourceFile.OriginalName) + ".pdf",
                    StoragePath = pdfStoragePath,
                    SizeInBytes = resultFileInfo.Length,
                    ContentType = "application/pdf",
                    UserId = job.UserId
                };

                context.StoredFiles.Add(pdfFile);

                // Finalizo el Job con éxito
                job.Status = JobStatus.Completed;
                job.ResultFileId = pdfFile.Id;
                job.CompletedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Job {JobId} completado con éxito.", jobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico procesando el Job {JobId}.", jobId);

                if (jobId != Guid.Empty)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var job = await context.ConversionJobs.FindAsync(new object[] { jobId }, stoppingToken);
                    if (job != null)
                    {
                        job.Status = JobStatus.Failed;
                        job.ErrorMessage = ex.Message;
                        job.CompletedAt = DateTime.UtcNow;
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
            }
        }
    }
}