using System.Text.Json.Serialization;
using Vestaboard.Common;

namespace Vestaboard.Wordle.Models;

[JsonConverter(typeof(NumericEnumJsonConverter<Color>))]
public enum Color
{
    None = 0,
    Yellow = 1,
    Green = 2,
}