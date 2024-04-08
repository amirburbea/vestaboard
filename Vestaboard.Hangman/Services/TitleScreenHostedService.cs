using Vestaboard.Common;

namespace Vestaboard.Hangman.Services;

internal sealed class TitleScreenHostedService(IBoardClient boardClient) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return boardClient.PostMessageAsync(
            [
                FilledRow(0),
                MiddleRow(1),
                MiddleRow(2, "Vestaboard"),
                MiddleRow(3, "Hangman!"),
                MiddleRow(4),
                FilledRow(5),
            ],
            cancellationToken
        );

        static BoardCharacter[] FilledRow(int rowIndex)
        {
            BoardCharacter[] row = new BoardCharacter[22];
            for (int i = 0; i < 22; i++)
            {
                row[i] = (i % 2) == (rowIndex / 5) ? BoardCharacter.ParisBlue : BoardCharacter.White;
            }
            return row;
        }

        static BoardCharacter[] MiddleRow(int rowIndex, string word = "")
        {
            BoardCharacter[] row = new BoardCharacter[22];
            row[0] = (rowIndex % 2) == 0 ? BoardCharacter.White : BoardCharacter.ParisBlue;
            row[21] = (rowIndex % 2) == 0 ? BoardCharacter.ParisBlue : BoardCharacter.White;
            int start = 11 - word.Length / 2;
            for (int i = 0; i < word.Length; i++)
            {
                row[start + i] = BoardCharacters.Map[word[i]];
            }
            return row;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
