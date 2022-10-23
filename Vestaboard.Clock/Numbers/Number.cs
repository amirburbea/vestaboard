using Vestaboard.Common;

namespace Vestaboard.Clock.Numbers;

internal abstract class Number
{
    public abstract void Render(BoardCharacter[][] output, int startIndex);
}