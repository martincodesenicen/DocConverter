using DocConverter.Domain.Interfaces;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using SautinSoft.Document;

namespace DocConverter.Infrastructure.Converters;

public class PdfProcessingEngine : IContentConverter
{
    public Task<string> ConvertToPdfAsync(string sourcePath)
    {
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("El archivo Word de origen no se encuentra.", sourcePath);
        }

        var directory = Path.GetDirectoryName(sourcePath) ?? AppContext.BaseDirectory;
        var targetPath = Path.Combine(directory, $"{Guid.NewGuid()}.pdf");

        return Task.Run(() =>
        {
            var document = DocumentCore.Load(sourcePath);
            document.Save(targetPath, SaveOptions.PdfDefault);
            return targetPath;
        });
    }

    // NUEVO: Fusión física de múltiples archivos PDF usando PdfSharp
    public Task<string> MergePdfsAsync(List<string> sourcePaths)
    {
        if (sourcePaths == null || sourcePaths.Count < 2)
        {
            throw new ArgumentException("Se necesitan al menos dos rutas de archivo para fusionar.");
        }

        // Defino dónde se guardará el PDF resultante (en la misma carpeta uploads)
        var directory = Path.GetDirectoryName(sourcePaths[0]) ?? AppContext.BaseDirectory;
        var targetPath = Path.Combine(directory, $"{Guid.NewGuid()}.pdf");

        return Task.Run(() =>
        {
            // Se crea el documento PDF de salida vacío
            using var outputDocument = new PdfDocument();

            foreach (var path in sourcePaths)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"No se pudo encontrar uno de los PDFs a fusionar: {path}");
                }

                // Abro el PDF de origen en modo Importación
                using var inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                
                // Se copia página por página al documento de salida
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfPage page = inputDocument.Pages[idx];
                    outputDocument.AddPage(page);
                }
            }

            // Guardo el archivo final
            outputDocument.Save(targetPath);
            return targetPath;
        });
    }
    public Task<string> SplitPdfAsync(string sourcePath, int startPage, int endPage)
    {
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("El archivo PDF original no se encuentra en el servidor.", sourcePath);
        }

        var directory = Path.GetDirectoryName(sourcePath) ?? AppContext.BaseDirectory;
        var targetPath = Path.Combine(directory, $"{Guid.NewGuid()}.pdf");

        return Task.Run(() =>
        {
            // 1. Abro el documento original en modo Importación
            using var inputDocument = PdfReader.Open(sourcePath, PdfDocumentOpenMode.Import);
            
            int totalPages = inputDocument.PageCount;
            if (startPage > totalPages)
            {
                throw new ArgumentException($"La página de inicio ({startPage}) excede las páginas totales del documento ({totalPages}).");
            }
            
            // Ajusto la página final en caso de que el usuario haya enviado un número mayor al total
            int realEndPage = Math.Min(endPage, totalPages);

            // 2. Creo el documento PDF de salida vacío
            using var outputDocument = new PdfDocument();

            // 3. Extraigo el rango solicitado
            // NOTA: En PdfSharp las páginas son base 0 indexadas, por eso tengo que restar 1
            for (int i = startPage - 1; i < realEndPage; i++)
            {
                PdfPage page = inputDocument.Pages[i];
                outputDocument.AddPage(page);
            }

            // 4. Guardo el archivo binario recortado en disco
            outputDocument.Save(targetPath);
            return targetPath;
        });
    }
}