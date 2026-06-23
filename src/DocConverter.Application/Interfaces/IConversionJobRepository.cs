using DocConverter.Domain.Entities;

public interface IConversionJobRepository
{
    Task AddAsync(ConversionJob job);

    Task<ConversionJob?> GetByIdAndUserAsync(Guid jobId, Guid userId);
    Task<ConversionJob?> GetJobWithResultFileAsync(Guid jobId, Guid userId);
    Task AddMergeJobAsync(ConversionJob job, List<(StoredFile file, int order)> files);
}