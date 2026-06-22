using DocConverter.Application.DTOs;

namespace DocConverter.Application.Interfaces;

public interface IConversionService
{
    Task<JobResponse> StartWordToPdfConversionAsync(Stream fileStream, string fileName, long sizeInBytes);
    Task<JobResponse?> GetJobStatusAsync(Guid jobId);
}