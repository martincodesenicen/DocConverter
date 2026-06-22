namespace DocConverter.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Guarda un flujo de datos en el almacenamiento y retorna la ruta relativa donde quedó alojado.
    /// </summary>
    Task<string> SaveFileAsync(Stream fileStream, string fileName);
    
    /// <summary>
    /// Elimina un archivo físico si existiera algún error o purga del sistema.
    /// </summary>
    void DeleteFile(string storagePath);
}