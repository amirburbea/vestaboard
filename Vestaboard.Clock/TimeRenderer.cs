﻿using Vestaboard.Clock.Numbers;
using Vestaboard.Common;

namespace Vestaboard.Clock;

public interface ITimeRenderer
{
    Task RenderTimeAsync(DateTime time, CancellationToken cancellationToken = default);
}

internal sealed class TimeRenderer : ITimeRenderer
{
    private static readonly Number[] _numbers = {
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
    };

    private readonly IBoardClient _boardClient;

    public TimeRenderer(IBoardClient boardClient)
    {
        this._boardClient = boardClient;
    }

    public Task RenderTimeAsync(DateTime time, CancellationToken cancellationToken)
    {
        BoardCharacter[][] output = new BoardCharacter[][]
        {
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22]
        };
        RenderValue(time.Hour, 0);
        RenderColon(0);
        RenderColon(1);
        RenderColon(3);
        RenderColon(4);
        RenderValue(time.Minute, 13);
        return this._boardClient.PostMessageAsync(output, cancellationToken);

        void RenderColon(int row) => output[row][10] = output[row][11] = BoardCharacter.White;

        void RenderValue(int value, int startIndex)
        {
            RenderDigit(value / 10, startIndex);
            RenderDigit(value % 10, startIndex + 5);

            void RenderDigit(int digit, int startIndex) => TimeRenderer._numbers[digit].Render(output, startIndex);
        }
    }
}