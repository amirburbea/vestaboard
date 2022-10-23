using Vestaboard.Common;

namespace Vestaboard.Clock.Numbers;

internal sealed class Nine : Number
{
    public override void Render(BoardCharacter[][] output, int startIndex)
    {
        output[0][startIndex + 1] = output[0][startIndex + 2] =
        output[1][startIndex] = output[1][startIndex + 3] =
        output[2][startIndex] = output[2][startIndex + 3] =
        output[3][startIndex + 1] = output[3][startIndex + 2] = output[3][startIndex + 3] =
        output[4][startIndex + 3] =
        output[5][startIndex + 1] = output[5][startIndex + 2] = BoardCharacter.White;
    }
}