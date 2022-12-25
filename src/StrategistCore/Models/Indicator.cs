using Newtonsoft.Json;

namespace StrategistCore.Models;

public enum FigureType
{
    Line,
    Hist,
    Dot,
    Bar,
    Text
}

public enum FillType
{
    ToZeroY,
    ToNextY,
    ToSelf
}

public class Indicator {
    public string Name { get; set; }
    public Figure[] Figures { get; set; }
    public bool InChart { get; set; }
}

public class Figure
{
    public string Name { get; set; }
    public FigureType? Type { get; set; } = FigureType.Line;
    public FillType? FillType { get; set; }
    public string? FillColor { get; set; }
    public string? Color { get; set; }
    public List<object> Data { get; set; } = new();

    [JsonIgnore]
    public Func<object> GetValue { get; set; }
    public void AddValue(object value) => Data.Add(value);
}

