using DocConverter.Application.Interfaces;
using DocConverter.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocConverter.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("No file was uploaded.");
        }

        using var stream = file.OpenReadStream();
        
        var response = await _fileService.UploadFileAsync(
            stream, 
            file.FileName, 
            file.ContentType, 
            file.Length
        );

        return Ok(response);
    }
}