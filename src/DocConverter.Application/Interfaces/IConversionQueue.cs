namespace DocConverter.Application.Interfaces;

public interface IConversionQueue
{
    // Agrega un Job ID a la cola de procesamiento
    ValueTask EnqueueAsync(Guid jobId);

    // Lee (y remueve) un Job ID de la cola. Si está vacía, espera.
    ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken);
}