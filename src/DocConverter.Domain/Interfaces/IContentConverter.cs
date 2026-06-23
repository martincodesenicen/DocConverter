namespace DocConverter.Domain.Interfaces;

public interface IContentConverter
{
    // Toma la ruta de un archivo origen, lo procesa y devuelve la ruta del nuevo archivo PDF generado
    Task<string> ConvertToPdfAsync(string sourcePath);
    Task<string> MergePdfsAsync(List<string> sourcePaths);
}