using Microsoft.Extensions.Hosting;

namespace Vestaboard.Clock;

internal sealed class ClockService : IHostedService
{
    private readonly IClockRenderer _renderer;
    private readonly Timer _timer;

    public ClockService(IClockRenderer renderer)
    {
        this._timer = new Timer(delegate { this.RenderTimeAsync(default); });
        this._renderer = renderer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.RenderTimeAsync(cancellationToken).ConfigureAwait(false);
        // Start at next interval but with 0 seconds.
        this.ChangeTimer(Minutes.One);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._timer.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }

    private void ChangeTimer(Minutes interval)
    {
        int secondsToNextMinute = (int)Math.Floor(60d - DateTime.Now.Second);
        int minutes = (int)interval;
        for (int i = 0; i < minutes; i++)
        {
            DateTime temp = DateTime.Now.AddSeconds(secondsToNextMinute).AddMinutes(i);
            if (temp.Minute % minutes == 0)
            {
                this._timer.Change(temp - DateTime.Now, TimeSpan.FromMinutes(minutes));
                return;
            }
        }
    }

    private Task RenderTimeAsync(CancellationToken cancellationToken) => this._renderer.RenderTimeAsync(DateTime.Now, cancellationToken);

    enum Minutes
    {
        One = 1,
        Five = 5
    }
}