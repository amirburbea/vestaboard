using Microsoft.Extensions.Hosting;

namespace Vestaboard.Clock;

internal sealed class DigitalClockService : IHostedService
{
    private readonly IClockRenderer _clockService;
    private readonly Timer _timer;

    public DigitalClockService(IClockRenderer timeRenderer)
    {
        this._timer = new Timer(delegate { this.RenderClockAsync(); });
        this._clockService = timeRenderer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.RenderClockAsync(cancellationToken).ConfigureAwait(false);
        // Start at next minute but with 0 seconds, raise once per minute.
        this._timer.Change((int)Math.Floor(60d - DateTime.Now.Second) * 1000, 60000);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._timer.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }

    private Task RenderClockAsync(CancellationToken cancellationToken = default)
    {
        return this._clockService.RenderClockAsync(DateTime.Now, cancellationToken);
    }
}