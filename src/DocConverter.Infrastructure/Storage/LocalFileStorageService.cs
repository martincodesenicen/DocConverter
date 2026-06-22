using DocConverter.Application.Interfaces;

namespace DocConverter.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private const string UploadsFolder = "uploads";

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
    {
        // Asegurar que la carpeta base exista en el directorio de ejecución del servidor
        var basePath = Path.Combine(AppContext.BaseDirectory, UploadsFolder);
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        // Generar un nombre único usando un Guid conservando la extensión original (.docx, .png)
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(basePath, uniqueFileName);

        // Escribir el Stream de datos
        using var targetStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(targetStream);

        // Retorno la ruta para guardarla en la base de datos
        return fullPath;
    }

    public void DeleteFile(string storagePath)
    {
        if (File.Exists(storagePath))
        {
            File.Delete(storagePath);
        }
    }
}