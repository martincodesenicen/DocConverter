using System.Threading.Channels;
using DocConverter.Application.Interfaces;

namespace DocConverter.Application.Services;

public class ConversionQueue : IConversionQueue
{
    private readonly Channel<Guid> _queue;

    public ConversionQueue()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false // Múltiples controladores HTTP podrían meter archivos a la vez
        };
        
        _queue = Channel.CreateUnbounded<Guid>(options);
    }

    public async ValueTask EnqueueAsync(Guid jobId)
    {
        await _queue.Writer.WriteAsync(jobId);
    }

    public async ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}