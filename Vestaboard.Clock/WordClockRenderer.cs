using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vestaboard.Common;

namespace Vestaboard.Clock;

internal sealed class WordClockRenderer(IBoardClient boardClient) : IClockRenderer
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
        [16] = "sixteen",
        [17] = "seventeen",
        [18] = "eighteen",
        [19] = "nineteen",
        [20] = "twenty",
    };

    public Task RenderTimeAsync(TimeOnly time, CancellationToken cancellationToken = default)
    {
        BoardCharacter[][] output =
        [
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22],
            new BoardCharacter[22]
        ];
        string text = WordClockRenderer.ComposeSentence(time);
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
        return boardClient.PostMessageAsync(output, cancellationToken);

        void Fill(Span<BoardCharacter> span) => span.Fill(row % 2 is 0 ? BoardCharacter.White : BoardCharacter.ParisBlue);
    }

    private static string ComposeSentence(TimeOnly time)
    {
        int hour = time.AddHours(time.Minute > 30 ? 1d : 0d).Hour;
        int displayedHour = hour % 12;
        return $"it is {time.Minute switch
        {
            0 => $"{WordClockRenderer._text[displayedHour]} o'clock",
            1 => $"one minute past {WordClockRenderer._text[displayedHour]}",
            15 => $"a quarter past {WordClockRenderer._text[displayedHour]}",
            < 21 => $"{WordClockRenderer._text[time.Minute]} minutes past {WordClockRenderer._text[displayedHour]}",
            < 30 => $"twenty {WordClockRenderer._text[time.Minute % 10]} minutes past {WordClockRenderer._text[displayedHour]}",
            30 => $"half past {WordClockRenderer._text[displayedHour]}",
            < 40 => $"twenty {WordClockRenderer._text[(60 - time.Minute) % 10]} minutes to {WordClockRenderer._text[displayedHour]}",
            45 => $"a quarter to {WordClockRenderer._text[displayedHour]}",
            59 => $"one minute to {WordClockRenderer._text[displayedHour]}",
            _ => $"{WordClockRenderer._text[60 - time.Minute]} minutes to {WordClockRenderer._text[displayedHour]}",
        }} {hour switch
        {
            < 12 => "in the morning",
            < 18 => "in the afternoon",
            < 21 => "in the evening",
            _ => "at night"
        }}";
    }
}
