using Vestaboard.Common;

namespace Vestaboard.Clock.Numbers;

internal sealed class One : Number
{
    public override void Render(BoardCharacter[][] output, int startIndex)
    {
        output[0][startIndex + 2] =
        output[1][startIndex + 1] = output[1][startIndex + 2] =
        output[2][startIndex] = output[2][startIndex + 2] =
        output[3][startIndex + 2] = 
        output[4][startIndex + 2] =
        output[5][startIndex] = output[5][startIndex + 1] = output[5][startIndex + 2] = output[5][startIndex + 3] = BoardCharacter.White;
    }
}