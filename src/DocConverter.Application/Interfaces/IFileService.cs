using DocConverter.Application.DTOs;

namespace DocConverter.Application.Interfaces;

public interface IFileService
{
    Task<FileResponse> UploadFileAsync(Stream fileStream, string fileName, string contentType, long sizeInBytes);
}