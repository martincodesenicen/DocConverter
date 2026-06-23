using DocConverter.Application.Interfaces;
using DocConverter.Domain.Exceptions;
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
            throw new BadRequestException("No file was uploaded.");
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".docx" && extension != ".doc")
        {
            throw new BadRequestException("Only Word files (.doc, .docx) are allowed.");
        }

        using var stream = file.OpenReadStream();
        var response = await _conversionService.StartWordToPdfConversionAsync(stream, file.FileName, file.Length);
        
        return AcceptedAtAction(nameof(GetJobStatus), new { jobId = response.JobId }, response);
    }

    [HttpGet("status/{jobId}")]
    public async Task<IActionResult> GetJobStatus(Guid jobId)
    {
        var response = await _conversionService.GetJobStatusAsync(jobId);
        if (response == null)
        {
            throw new NotFoundException("Conversion job not found.");
        }

        return Ok(response);
    }

    [HttpGet("download/{jobId}")]
    public async Task<IActionResult> DownloadResult(Guid jobId)
    {
        var response = await _conversionService.DownloadResultFileAsync(jobId);
        
        // Si el servicio no devolvió nulo, el stream está listo para ser enviado
        return File(response!.FileStream, response.ContentType, response.FileName);
    }
}