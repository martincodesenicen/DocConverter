using DocConverter.Domain.Entities;

public interface IConversionJobRepository
{
    Task AddAsync(ConversionJob job);

    Task<ConversionJob?> GetByIdAndUserAsync(Guid jobId, Guid userId);
    Task<ConversionJob?> GetJobWithResultFileAsync(Guid jobId, Guid userId);
}