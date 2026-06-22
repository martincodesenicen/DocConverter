using DocConverter.Domain.Interfaces;
using SautinSoft.Document;

namespace DocConverter.Infrastructure.Converters;

public class WordToPdfConverter : IContentConverter
{
    public Task<string> ConvertToPdfAsync(string sourcePath)
    {
        // Valido que el archivo de origen realmente exista en el disco
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("El archivo Word de origen no se encuentra en el servidor.", sourcePath);
        }

        // Defino la ruta de salida reemplazando la extensión .docx por .pdf
        var directory = Path.GetDirectoryName(sourcePath) ?? AppContext.BaseDirectory;
        var uniqueFileName = $"{Guid.NewGuid()}.pdf";
        var targetPath = Path.Combine(directory, uniqueFileName);

        // Cargo el documento Word y lo guardo como PDF con SautinSoft
        return Task.Run(() =>
        {
            var document = DocumentCore.Load(sourcePath);
            document.Save(targetPath, SaveOptions.PdfDefault);
            
            return targetPath;
        });
    }
}