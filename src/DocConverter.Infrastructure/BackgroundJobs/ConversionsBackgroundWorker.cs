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

                // 3. Busco el Job en la base de datos
                var job = await context.ConversionJobs
                    .Include(j => j.SourceFile)
                    .Include(j => j.JobSourceFiles)
                        .ThenInclude(js => js.StoredFile) // metadata de los múltiples archivos si existieran
                    .FirstOrDefaultAsync(j => j.Id == jobId, stoppingToken);

                if (job == null) continue;

                // 4. Actualizar estado a Processing
                job.Status = JobStatus.Processing;
                await context.SaveChangesAsync(stoppingToken);

                // 5. Determinar la estrategia de procesamiento (Word vs Merge)
                string pdfStoragePath = string.Empty;

                if (job.SourceFileId.HasValue)
                {
                    // Estrategia A: Conversión unitaria (Word a PDF)
                    pdfStoragePath = await converter.ConvertToPdfAsync(job.SourceFile.StoragePath);
                }
                else if (job.JobSourceFiles.Any())
                {
                    // Estrategia B: Fusión de múltiples PDFs (Merge)
                    // rutas físicas ordenadas estrictamente por la secuencia
                    var orderedPaths = job.JobSourceFiles
                        .OrderBy(js => js.SequenceOrder)
                        .Select(js => js.StoredFile.StoragePath)
                        .ToList();

                    pdfStoragePath = await converter.MergePdfsAsync(orderedPaths);
                }
                else
                {
                    throw new Exception("El trabajo no posee archivos de origen válidos definidos.");
                }

                // 6. Registrar el nuevo archivo PDF resultante en la metadata
                var resultFileInfo = new FileInfo(pdfStoragePath);
                var pdfFile = new Domain.Entities.StoredFile
                {
                    Id = Guid.NewGuid(),
                    OriginalName = job.SourceFileId.HasValue 
                        ? Path.GetFileNameWithoutExtension(job.SourceFile.OriginalName) + ".pdf"
                        : "merged_document.pdf", // Nombre por defecto para fusiones
                    StoragePath = pdfStoragePath,
                    SizeInBytes = resultFileInfo.Length,
                    ContentType = "application/pdf",
                    UserId = job.UserId
                };

                context.StoredFiles.Add(pdfFile);

                // 7. Finalizar el Job con éxito
                job.Status = JobStatus.Completed;
                job.ResultFileId = pdfFile.Id;
                job.CompletedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Job {JobId} procesado y finalizado con éxito por el Worker.", jobId);
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