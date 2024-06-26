﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Vestaboard.Common;
using Vestaboard.Wordle.Services;

namespace Vestaboard.Wordle.Models;

internal sealed class Game(IWordRepository wordRepository)
{
    private readonly List<Color[]> _colors = new(6);
    private readonly List<string> _guesses = new(6);
    private readonly int _index = RandomNumberGenerator.GetInt32(0, wordRepository.Words.Count);
    private readonly Dictionary<char, Color> _keyColors = [];
    private bool _hardMode;

    public IReadOnlyCollection<Color[]> Colors => this._colors;

    public IReadOnlyCollection<string> Guesses => this._guesses;

    public bool HardMode
    {
        get => this._hardMode;
        set
        {
            if (this._guesses.Count != 0)
            {
                throw new InvalidOperationException("Can not change hard mode after guesses have been made.");
            }
            this._hardMode = value;
        }
    }

    public int Id => 1 + this._index;

    public bool IsOver => this.Guesses.Count == 6 || this.IsSolved;

    public bool IsSolved => this._guesses.Count != 0 && this._guesses[^1] == this.Word;

    public IReadOnlyDictionary<char, Color> KeyColors => this._keyColors;

    public string Uuid { get; } = Guid.NewGuid().ToString();

    public string Word => wordRepository.Words[this._index];

    public Color[] AddGuess(string guess)
    {
        if (this.Guesses.Count == 6)
        {
            throw new InvalidOperationException("Game is over (6 guesses were already made).");
        }
        guess = guess.ToUpperInvariant();
        this.ValidateGuess(guess);
        this._guesses.Add(guess);
        Color[] colors = this.ComputeColors(guess);
        this.UpdateKeyColors(guess, colors);
        this._colors.Add(colors);
        return colors;
    }

    public Color[] ComputeColors(string guess)
    {
        Color[] colors = new Color[guess.Length];
        Dictionary<char, int> letterCounts = new(colors.Length);
        for (int i = 0; i < this.Word.Length; i++)
        {
            char character = this.Word[i];
            if (guess[i] == character)
            {
                colors[i] = Color.Green;
            }
            else
            {
                letterCounts[character] = letterCounts.GetValueOrDefault(character) + 1;
            }
        }
        for (int i = 0; i < guess.Length; i++)
        {
            char character = guess[i];
            if (colors[i] != Color.Green && letterCounts.GetValueOrDefault(character) is { } count and > 0)
            {
                colors[i] = Color.Yellow;
                letterCounts[character] = count - 1;
            }
        }
        return colors;
    }

    public BoardCharacter[][] ToArray(bool onScreenKeyboard = true)
    {
        BoardCharacter[][] rows = new BoardCharacter[6][];
        for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
        {
            rows[rowIndex] = CreateRow(rowIndex);
            if (onScreenKeyboard)
            {
                RenderOnScreenKeyboard(rowIndex);
            }
        }
        if (!onScreenKeyboard)
        {
            if (!this.IsOver)
            {
                RenderSuffix(0, "Good");
                RenderSuffix(1, "Luck");
            }
            else
            {
                RenderSuffix(0, "You");
                RenderSuffix(1, this.IsSolved ? "Win" : "Lose");
            }
            RenderSuffix(4, "Wordle");
        }
        RenderSuffix(5, $"#{this.Id}");
        return rows;

        BoardCharacter[] CreateRow(int rowIndex)
        {
            BoardCharacter[] row = new BoardCharacter[22];
            if (this._guesses.Count == rowIndex && !this.IsSolved)
            {
                row.AsSpan(0, 5).Fill(BoardCharacter.QuestionMark);
            }
            else if (this._guesses.Count > rowIndex)
            {
                string guess = this._guesses[rowIndex];
                for (int i = 0; i < 5; i++)
                {
                    row[i] = BoardCharacters.Map[guess[i]];
                }
                Color[] colors = this._colors[rowIndex];
                for (int i = 0; i < 5; i++)
                {
                    Color color = colors[i];
                    row[6 + i] = color is Color.None
                        ? BoardCharacter.White
                        : Game.GetCharacter(color);
                }
            }
            row[5] = row[11] = BoardCharacter.Blank;
            return row;
        }

        void RenderOnScreenKeyboard(int rowIndex)
        {
            BoardCharacter[] row = rows[rowIndex];
            // Render 5 letters per row (or 1 for last row of just 'Z').
            int offset = rowIndex * 5;
            for (int index = 0; index < 5 && (offset + index) < 26; index++)
            {
                char character = (char)((int)'A' + offset + index);
                row[12 + 2 * index] = BoardCharacters.Map[character];
                row[13 + 2 * index] = this._keyColors.TryGetValue(character, out Color color)
                    ? color is Color.None
                        ? BoardCharacter.White
                        : Game.GetCharacter(color)
                    : BoardCharacter.Hyphen;
            }
        }

        void RenderSuffix(int rowIndex, string text)
        {
            BoardCharacter[] row = rows[rowIndex];
            for (int index = 0; index < text.Length; index++)
            {
                char character = text[index];
                row[row.Length - text.Length + index] = BoardCharacters.Map[character];
            }
        }
    }

    private static BoardCharacter GetCharacter(Color color) => color switch
    {
        Color.Green => BoardCharacter.Green,
        Color.Yellow => BoardCharacter.Yellow,
        _ => BoardCharacter.Blank,
    };

    private void UpdateKeyColors(string guess, Color[] colors)
    {
        for (int index = 0; index < colors.Length; index++)
        {
            Color color = colors[index];
            char character = guess[index];
            if (this._keyColors.TryGetValue(character, out Color existing))
            {
                this._keyColors[character] = (Color)Math.Max((int)color, (int)existing);
            }
            else
            {
                this._keyColors.Add(character, color);
            }
        }
    }

    private void ValidateGuess(string guess)
    {
        if (this._guesses.Count != 0 && this._guesses[^1] == this.Word)
        {
            throw new InvalidOperationException($"Game is over (it was solved on guess #{this._guesses.Count}).");
        }
        if (this._guesses.Contains(guess))
        {
            throw new ArgumentException($"Guess {guess} was already made.", nameof(guess));
        }
        if (!wordRepository.IsValidGuess(guess))
        {
            throw new ArgumentException("Guess must be a valid 5 character word.", nameof(guess));
        }
        if (this._guesses.Count == 0 || !this.HardMode)
        {
            return;
        }
        Color[] previousColors = this._colors[^1];
        string previousGuess = this._guesses[^1];
        Dictionary<char, int> yellowCounts = [];
        for (int i = 0; i < guess.Length; i++)
        {
            char character = previousGuess[i];
            Color color = previousColors[i];
            if (color == Color.Green && character != guess[i])
            {
                throw new ArgumentException($"Hard mode but did not reuse GREEN character at position {i + 1}.", nameof(guess));
            }
            else if (color == Color.Yellow)
            {
                yellowCounts[character] = yellowCounts.TryGetValue(character, out int count) ? count + 1 : 1;
            }
        }
        for (int index = 0; index < guess.Length; index++)
        {
            if (previousColors[index] == Color.Green)
            {
                continue; // We coudn't be here if they don't match.
            }
            char character = guess[index];
            if (!yellowCounts.TryGetValue(character, out int count))
            {
                continue;
            }
            if (count == 1)
            {
                yellowCounts.Remove(character);
            }
            else
            {
                yellowCounts[character] = count - 1;
            }
        }
        if (yellowCounts.Count != 0)
        {
            throw new ArgumentException($"Hard mode but did not reuse YELLOW character{(yellowCounts.Count > 1 ? "s" : " ")} {string.Join(',', yellowCounts.Keys)}.", nameof(guess));
        }
    }
}

public readonly struct GameData
{
    private readonly Game _game;

    internal GameData(Game game) => this._game = game;

    public IReadOnlyCollection<Color[]> Colors => this._game.Colors;

    public IReadOnlyCollection<string> Guesses => this._game.Guesses;

    public bool HardMode => this._game.HardMode;

    public int Id => this._game.Id;

    [JsonIgnore]
    public bool IsOver => this._game.IsOver;

    public IReadOnlyDictionary<char, Color> KeyColors => this._game.KeyColors;

    public string Uuid => this._game.Uuid;

    [JsonIgnore]
    public string Word => this._game.Word;
}
