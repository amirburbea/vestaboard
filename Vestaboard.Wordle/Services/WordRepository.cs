using System;
using System.Collections.Generic;
using System.IO;

namespace Vestaboard.Wordle.Services;

public interface IWordRepository
{
    IReadOnlyList<string> Words { get; }

    bool IsValidGuess(string word);
}

internal sealed class WordRepository : IWordRepository
{
    private static readonly HashSet<string> _validGuesses = [.. WordRepository.ReadWordList("valid-guesses.txt")];
    private static readonly List<string> _words = WordRepository.ReadWordList("wordle-dictionary.txt");

    public IReadOnlyList<string> Words => WordRepository._words;

    public bool IsValidGuess(string guess) => WordRepository._validGuesses.Contains(guess);

    private static List<string> ReadWordList(string name)
    {
        using Stream stream = typeof(Program).Assembly.GetManifestResourceStream($"{typeof(Program).Namespace}.Data.{name}")!;
        using StreamReader reader = new(stream);
        List<string> words = [];
        while (reader.ReadLine() is { } word)
        {
            words.Add(word.ToUpperInvariant());
        }
        return words;
    }
}
