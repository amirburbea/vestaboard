using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vestaboard.Wordle.Services;

public interface IWordRepository
{
    IReadOnlyList<string> Words { get; }

    bool IsValidGuess(string word);
}

internal sealed class WordRepository : IWordRepository
{
    private static readonly string[] _validGuesses = WordRepository.ReadWordList("valid-guesses.txt", sorted: true);
    private static readonly string[] _words = WordRepository.ReadWordList("wordle-dictionary.txt");

    public IReadOnlyList<string> Words => WordRepository._words;

    public bool IsValidGuess(string guess) => guess is not null && Array.BinarySearch(WordRepository._validGuesses, guess) is >= 0;

    private static string[] ReadWordList(string name, bool sorted = false)
    {
        using Stream stream = typeof(Program).Assembly.GetManifestResourceStream($"{typeof(Program).Namespace}.Data.{name}")!;
        using StreamReader reader = new(stream);
        List<string> words = new();
        while (reader.ReadLine() is { } word)
        {
            words.Add(word.ToUpperInvariant());
        }
        if (sorted)
        {
            words.Sort();
        }
        return words.ToArray();
    }
}