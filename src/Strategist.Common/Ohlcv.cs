using System.Text.Json.Serialization;

namespace Strategist.Common;

public class Ohlcv : ICloneable
{
    [JsonIgnore]
    public DateTime Date { get; set; }
    public long Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    #region Methods

    public object Clone() => MemberwiseClone();
    public override string ToString() => $"{Date} O:{Open:0.00} H:{High:0.00} L:{Low:0.00} C:{Close:0.00} V:{Volume:0.00}";

    #endregion
}