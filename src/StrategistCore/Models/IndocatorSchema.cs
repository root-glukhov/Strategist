namespace StrategistCore.Models;

public class Indicator {
    public string Name { get; set; }
    public Figure[] Figures { get; set; }
    public bool InChart { get; set; }
}

public class Figure
{
    public string Name { get; set; }
    public decimal Value { get; set; }
}

