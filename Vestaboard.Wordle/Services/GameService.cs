using System.Threading;
using System.Threading.Tasks;
using Vestaboard.Common;
using Vestaboard.Wordle.Models;

namespace Vestaboard.Wordle.Services;

public interface IGameService
{
    GameData Data { get; }

    bool OnScreenKeyboard { get; set; }

    Task<Color[]> AddGuessAsync(string guess, CancellationToken cancellationToken = default);

    Task RenderAsync(CancellationToken cancellationToken = default);

    Task StartNewAsync(bool hardMode = false, CancellationToken cancellationToken = default);
}

internal sealed class GameService(IBoardClient client, IWordRepository wordRepository) : IGameService
{
    private Game _game = new(wordRepository);

    public GameData Data => new(this._game);

    public bool OnScreenKeyboard { get; set; } = true;

    public async Task<Color[]> AddGuessAsync(string guess, CancellationToken cancellationToken)
    {
        Color[] colors = this._game.AddGuess(guess);
        await this.RenderAsync(cancellationToken).ConfigureAwait(false);
        return colors;
    }

    public Task RenderAsync(CancellationToken cancellationToken)
    {
        return client.PostMessageAsync(this._game.ToArray(this.OnScreenKeyboard), cancellationToken);
    }

    public Task StartNewAsync(bool hardMode, CancellationToken cancellationToken)
    {
        this._game = this.CreateNew(hardMode);
        return this.RenderAsync(cancellationToken);
    }

    private Game CreateNew(bool hardMode = false) => new(wordRepository) { HardMode = hardMode };
}
