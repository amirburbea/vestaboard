using Vestaboard.Common;

namespace Vestaboard.Clock.Numbers;

internal sealed class Four : Number
{
    public override void Render(BoardCharacter[][] output, int startIndex)
    {
        output[0][startIndex] =
        output[1][startIndex] =
        output[2][startIndex] = output[2][startIndex + 2] =
        output[3][startIndex] = output[3][startIndex + 1] = output[3][startIndex + 2] = output[3][startIndex + 3] =
        output[4][startIndex + 2] =
        output[5][startIndex + 2] = BoardCharacter.White;
    }
}