namespace DocConverter.Application.DTOs;

public record FileResponse(Guid Id, string OriginalName, long SizeInBytes, string ContentType);