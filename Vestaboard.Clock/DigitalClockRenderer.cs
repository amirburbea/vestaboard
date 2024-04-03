using System;
using System.Threading;
using System.Threading.Tasks;
using Vestaboard.Clock.Numbers;
using Vestaboard.Common;

namespace Vestaboard.Clock;

internal sealed class DigitalClockRenderer(IBoardClient boardClient) : IClockRenderer
{
    private static readonly Number[] _numbers = [
        new Zero(),
        new One(),
        new Two(),
        new Three(),
        new Four(),
        new Five(),
        new Six(),
        new Seven(),
        new Eight(),
        new Nine()
    ];

    public Task RenderTimeAsync(TimeOnly time, CancellationToken cancellationToken)
    {
        BoardCharacter[][] output =
        [
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22]
        ];
        RenderValue(time.Hour, 0);
        RenderColon(0);
        RenderColon(1);
        RenderColon(3);
        RenderColon(4);
        RenderValue(time.Minute, 13);
        return boardClient.PostMessageAsync(output, cancellationToken);

        void RenderColon(int row) => output[row][10] = output[row][11] = BoardCharacter.White;

        void RenderValue(int value, int startIndex)
        {
            RenderDigit(value / 10, startIndex);
            RenderDigit(value % 10, startIndex + 5);

            void RenderDigit(int digit, int startIndex) => DigitalClockRenderer._numbers[digit].Render(output, startIndex);
        }
    }
}
