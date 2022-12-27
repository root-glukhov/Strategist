using Newtonsoft.Json;

namespace Strategist.Core.Models;

public enum FigureType
{
    Line,
    Hist,
    Dot,
    Bar,
    Text
}

public class Figure
{
    public string Name { get; set; }
    public FigureType? Type { get; set; } = FigureType.Line;
    // public FillType? FillType { get; set; }
    // public string? FillColor { get; set; }
    // public string? Color { get; set; }
    // public List<object> Data { get; set; } = new();

    [JsonIgnore]
    public Func<object> GetValue { get; set; }
    public object AddValue(object value) => value; // Data.Add(value);
}

public class Indicator
{
    public string Name { get; set; }
    public Figure[] Figures { get; set; }
    public bool InChart { get; set; }
}
