namespace DocConverter.Application.DTOs;

public record JobResponse(Guid JobId, string Status, DateTime CreatedAt);