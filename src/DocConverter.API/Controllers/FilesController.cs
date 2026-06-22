using DocConverter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocConverter.API.Controllers;

[Authorize] // Protege todo el controlador
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
    [Consumes("multipart/form-data")] // Indica a Swagger que este endpoint recibe archivos binarios
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file was uploaded." });
        }

        try
        {
            // Abrimos el stream del archivo directamente desde la petición HTTP
            using var stream = file.OpenReadStream();
            
            var response = await _fileService.UploadFileAsync(
                stream, 
                file.FileName, 
                file.ContentType, 
                file.Length
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}