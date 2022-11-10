using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Vestaboard.Common;

/// <summary>
/// Names and values taken from <see href="https://docs.vestaboard.com/characters"/>.
/// </summary>
[JsonConverter(typeof(NumericEnumJsonConverter<BoardCharacter>))]
public enum BoardCharacter
{
    [Character(' ')]
    Blank = 0,

    [Character('A')]
    A = 1,

    [Character('B')]
    B = 2,

    [Character('C')]
    C = 3,

    [Character('D')]
    D = 4,

    [Character('E')]
    E = 5,

    [Character('F')]
    F = 6,

    [Character('G')]
    G = 7,

    [Character('H')]
    H = 8,

    [Character('I')]
    I = 9,

    [Character('J')]
    J = 10,

    [Character('K')]
    K = 11,

    [Character('L')]
    L = 12,

    [Character('M')]
    M = 13,

    [Character('N')]
    N = 14,

    [Character('O')]
    O = 15,

    [Character('P')]
    P = 16,

    [Character('Q')]
    Q = 17,

    [Character('R')]
    R = 18,

    [Character('S')]
    S = 19,

    [Character('T')]
    T = 20,

    [Character('U')]
    U = 21,

    [Character('V')]
    V = 22,

    [Character('W')]
    W = 23,

    [Character('X')]
    X = 24,

    [Character('Y')]
    Y = 25,

    [Character('Z')]
    Z = 26,

    [Character('1')]
    One = 27,

    [Character('2')]
    Two = 28,

    [Character('3')]
    Three = 29,

    [Character('4')]
    Four = 30,

    [Character('5')]
    Five = 31,

    [Character('6')]
    Six = 32,

    [Character('7')]
    Seven = 33,

    [Character('8')]
    Eight = 34,

    [Character('9')]
    Nine = 35,

    [Character('0')]
    Zero = 36,

    [Character('!')]
    ExclamationMark = 37,

    [Character('@')]
    AtSign = 38,

    [Character('#')]
    PoundSign = 39,

    [Character('$')]
    DollarSign = 40,

    [Character('(')]
    LeftParen = 41,

    [Character(')')]
    RightParen = 42,

    [Character('-')]
    Hyphen = 44,

    [Character('+')]
    PlusSign = 46,

    [Character('&')]
    Ampersand = 47,

    [Character('=')]
    EqualsSign = 48,

    [Character(';')]
    Semicolon = 49,

    [Character(':')]
    Colon = 50,

    [Character('\'')]
    SingleQuote = 52,

    [Character('"')]
    DoubleQuote = 53,

    [Character('%')]
    PercentSign = 54,

    [Character(',')]
    Comma = 55,

    [Character('.')]
    Period = 56,

    [Character('/')]
    Slash = 59,

    [Character('?')]
    QuestionMark = 60,

    [Character('°')]
    DegreeSign = 62,

    PoppyRed = 63,
    Orange = 64,
    Yellow = 65,
    Green = 66,
    ParisBlue = 67,
    Violet = 68,
    White = 69,
    Black = 70,
}

public static class BoardCharacters
{
    public static readonly IReadOnlyDictionary<char, BoardCharacter> Map = ComputeCharacterMap();

    private static Dictionary<char, BoardCharacter> ComputeCharacterMap() => new(
        from field in typeof(BoardCharacter).GetFields(BindingFlags.Public | BindingFlags.Static)
        let attribute = field.GetCustomAttribute<CharacterAttribute>()
        where attribute is not null
        select KeyValuePair.Create(attribute.AssociatedCharacter, (BoardCharacter)field.GetValue(null)!),
        new CaseInsensitiveCharacterComparer()
    );
}

internal sealed class CaseInsensitiveCharacterComparer : EqualityComparer<char>
{
    public override bool Equals(char x, char y) => char.ToUpperInvariant(x) == char.ToUpperInvariant(y);

    public override int GetHashCode(char obj) => char.ToUpperInvariant(obj);
}

[AttributeUsage(AttributeTargets.Field)]
internal sealed class CharacterAttribute : Attribute
{
    public CharacterAttribute(char associatedCharacter) => this.AssociatedCharacter = associatedCharacter;

    public char AssociatedCharacter { get; }
}