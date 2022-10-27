using Microsoft.Extensions.Hosting;

namespace Vestaboard.Clock;

internal sealed class ClockService : IHostedService
{
    private readonly ITimeRenderer _timeRenderer;
    private readonly Timer _timer;

    public ClockService(ITimeRenderer timeRenderer)
    {
        this._timer = new Timer(delegate { this.RenderTimeAsync(); });
        this._timeRenderer = timeRenderer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.RenderTimeAsync(cancellationToken).ConfigureAwait(false);
        // Start at next minute but with 0 seconds, raise once per minute.
        this._timer.Change((int)Math.Floor(60d - DateTime.Now.Second) * 1000, 60000);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._timer.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }

    private Task RenderTimeAsync(CancellationToken cancellationToken = default)
    {
        return this._timeRenderer.RenderTimeAsync(DateTime.Now, cancellationToken);
    }
}