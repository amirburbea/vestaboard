using Vestaboard.Common;

namespace Vestaboard.Clock.Numbers;

internal sealed class Seven : Number
{
    public override void Render(BoardCharacter[][] output, int startIndex)
    {
        output[0][startIndex] = output[0][startIndex + 1] = output[0][startIndex + 2] = output[0][startIndex + 3] =
        output[1][startIndex + 3] =
        output[2][startIndex + 3] =
        output[3][startIndex + 2] =
        output[4][startIndex + 1] =
        output[5][startIndex] = BoardCharacter.White;
    }
}