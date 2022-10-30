using Vestaboard.Common;

namespace Vestaboard.WordClock;

public interface ITimeRenderer
{
    Task RenderTimeAsync(DateTime time, CancellationToken cancellationToken = default);
}

internal sealed class TimeRenderer : ITimeRenderer
{
    private static readonly Dictionary<int, string> _text = new()
    {
        [0] = "twelve", // Special edge case for 0 hour
        [1] = "one",
        [2] = "two",
        [3] = "three",
        [4] = "four",
        [5] = "five",
        [6] = "six",
        [7] = "seven",
        [8] = "eight",
        [9] = "nine",
        [10] = "ten",
        [11] = "eleven",
        [12] = "twelve",
        [13] = "thirteen",
        [14] = "fourteen",
        [15] = "a quarter",
        [16] = "sixteen",
        [17] = "seventeen",
        [18] = "eighteen",
        [19] = "nineteen",
        [20] = "twenty",
        [30] = "half",
    };

    private readonly IBoardClient _boardClient;

    public TimeRenderer(IBoardClient boardClient)
    {
        this._boardClient = boardClient;
    }

    public Task RenderTimeAsync(DateTime time, CancellationToken cancellationToken = default)
    {
        BoardCharacter[][] output = new[]
        {
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22]
        };
        string text = TimeRenderer.ComposeSentence(time);
        int row = 0; // output row index
        int column = 0; // output column index
        int position = 0; // text position
        while (position < text.Length)
        {
            int endPosition = text.IndexOf(' ', position);
            string word = text[position..(endPosition is not -1 ? endPosition : ^0)];
            if (word.Length + column > 22)
            {
                row++;
                column = 0;
            }
            for (int index = 0; index < word.Length; index++)
            {
                output[row][column + index] = BoardCharacters.Map[word[index]];
            }
            column += word.Length + 1;
            position += word.Length + 1;
        }
        if (column <= 21)
        {
            Fill(output[row].AsSpan(column));
        }
        while (++row < 6)
        {
            Fill(output[row].AsSpan());
        }
        return this._boardClient.PostMessageAsync(output, cancellationToken);

        void Fill(Span<BoardCharacter> span) => span.Fill(row % 2 is 0 ? BoardCharacter.White : BoardCharacter.ParisBlue);
    }

    private static string ComposeSentence(DateTime time)
    {
        int hour = time.Hour % 12;
        string timeText = time.Minute switch
        {
            0 => $"{TimeRenderer._text[hour]} o'clock",
            1 => $"{TimeRenderer._text[1]} minute past {TimeRenderer._text[hour]}",
            15 or 30 => $"{TimeRenderer._text[time.Minute]} past {TimeRenderer._text[hour]}",
            45 => $"{TimeRenderer._text[15]} to {TimeRenderer._text[(hour + 1) % 12]}",
            59 => $"{TimeRenderer._text[1]} minute to {TimeRenderer._text[(hour + 1) % 12]}",
            < 21 => $"{TimeRenderer._text[time.Minute]} minutes past {TimeRenderer._text[hour]}",
            < 30 => $"{TimeRenderer._text[20]} {TimeRenderer._text[time.Minute % 10]} minutes past {TimeRenderer._text[hour]}",
            < 40 => $"{TimeRenderer._text[20]} {TimeRenderer._text[(60 - time.Minute) % 10]} minutes to {TimeRenderer._text[(hour + 1) % 12]}",
            _ => $"{TimeRenderer._text[60 - time.Minute]} minutes to {TimeRenderer._text[(hour + 1) % 12]}",
        };
        string timeOfDay = (time.Hour, time.Minute) switch
        {
            ( < 11, _) or (11, <= 30) or (23, > 30) => "in the morning",
            ( < 17, _) or (17, <= 30) => "in the afternoon",
            ( < 20, _) or (20, <= 30) => "in the evening",
            _ => "at night",
        };
        return $"it is {timeText} {timeOfDay}";
    }
}