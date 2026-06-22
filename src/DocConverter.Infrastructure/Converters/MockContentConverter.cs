using DocConverter.Domain.Interfaces;

namespace DocConverter.Infrastructure.Converters;

public class MockContentConverter : IContentConverter
{
    public async Task<string> ConvertToPdfAsync(string sourcePath)
    {
        // Simulo que la conversión toma 3 segundos de procesamiento pesado
        await Task.Delay(3000);

        // Devuelvo una ruta ficticia
        var directory = Path.GetDirectoryName(sourcePath) ?? AppContext.BaseDirectory;
        var newFileName = $"{Guid.NewGuid()}.pdf";
        var fullPath = Path.Combine(directory, newFileName);

        await File.WriteAllTextAsync(fullPath, "%PDF-1.4 Mock Content Data"); // Escribe un archivo de texto plano simulando un PDF

        return fullPath;
    }
}