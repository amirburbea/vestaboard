namespace Vestaboard.Clock;

public interface IClockRenderer
{
    Task RenderTimeAsync(DateTime time, CancellationToken cancellationToken = default);
}
