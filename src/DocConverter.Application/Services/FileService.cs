using DocConverter.Application.DTOs;
using DocConverter.Application.Interfaces;
using DocConverter.Domain.Entities;

namespace DocConverter.Application.Services;

public class FileService : IFileService
{
    private readonly IStoredFileRepository _storedFiles;
    private readonly IFileStorageService _storageService;
    private readonly ICurrentUserService _currentUserService;

    public FileService(
        IStoredFileRepository storedFiles,
        IFileStorageService storageService,
        ICurrentUserService currentUserService)
    {
        _storedFiles = storedFiles;
        _storageService = storageService;
        _currentUserService = currentUserService;
    }

    public async Task<FileResponse> UploadFileAsync(Stream fileStream, string fileName, string contentType, long sizeInBytes)
    {
        // 1. Obtener el ID del usuario del contexto seguro
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

        // 2. Guardar el archivo físico en disco
        var storagePath = await _storageService.SaveFileAsync(fileStream, fileName);

        // 3. Registrar la metadata en la Base de Datos
        var storedFile = new StoredFile
        {
            Id = Guid.NewGuid(),
            OriginalName = fileName,
            StoragePath = storagePath,
            SizeInBytes = sizeInBytes,
            ContentType = contentType,
            UserId = userId
        };

        await _storedFiles.AddAsync(storedFile);
        await _storedFiles.SaveChangesAsync();

        return new FileResponse(storedFile.Id, storedFile.OriginalName, storedFile.SizeInBytes, storedFile.ContentType);
    }
}