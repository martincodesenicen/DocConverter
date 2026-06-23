using DocConverter.Application.DTOs;

namespace DocConverter.Application.Interfaces;

public interface IConversionService
{
    Task<JobResponse> StartWordToPdfConversionAsync(Stream fileStream, string fileName, long sizeInBytes);
    Task<JobResponse?> GetJobStatusAsync(Guid jobId);
    Task<FileDownloadResponse?> DownloadResultFileAsync(Guid jobId);
    Task<JobResponse> StartPdfMergeAsync(List<(Stream FileStream, string FileName)> files);
    Task<JobResponse> StartPdfSplitAsync(Stream fileStream, string fileName, long sizeInBytes, SplitRequest request);
}