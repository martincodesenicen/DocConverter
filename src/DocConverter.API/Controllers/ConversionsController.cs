using DocConverter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocConverter.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConversionsController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConversionsController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    [HttpPost("word-to-pdf")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ConvertWordToPdf(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file was uploaded." });
        }

        // Validación básica de extensión para simular entorno real
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".docx" && extension != ".doc")
        {
            return BadRequest(new { message = "Only Word files (.doc, .docx) are allowed." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var response = await _conversionService.StartWordToPdfConversionAsync(stream, file.FileName, file.Length);
            
            // Retornamos un 202 Accepted, el estándar REST para indicar que el proceso comenzó pero no terminó.
            return AcceptedAtAction(nameof(GetJobStatus), new { jobId = response.JobId }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("status/{jobId}")]
    public async Task<IActionResult> GetJobStatus(Guid jobId)
    {
        try
        {
            var response = await _conversionService.GetJobStatusAsync(jobId);
            if (response == null)
            {
                return NotFound(new { message = "Conversion job not found." });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("download/{jobId}")]
    public async Task<IActionResult> DownloadResult(Guid jobId)
    {
        try
        {
            var response = await _conversionService.DownloadResultFileAsync(jobId);
            if (response == null)
            {
                return BadRequest(new { message = "El archivo no está listo para descargar o el trabajo falló." });
            }

            return File(response.FileStream, response.ContentType, response.FileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}