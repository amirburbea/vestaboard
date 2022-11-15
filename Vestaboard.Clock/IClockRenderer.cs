using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vestaboard.Clock;

public interface IClockRenderer
{
    Task RenderTimeAsync(TimeOnly time, CancellationToken cancellationToken = default);
}