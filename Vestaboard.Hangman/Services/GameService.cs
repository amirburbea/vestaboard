using Vestaboard.Common;

namespace Vestaboard.Hangman.Services;

public interface IGameService
{
}

internal class GameService(IBoardClient client) : IGameService
{
}
