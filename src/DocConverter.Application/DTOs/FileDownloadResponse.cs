namespace DocConverter.Application.DTOs;

public record FileDownloadResponse(Stream FileStream, string ContentType, string FileName);