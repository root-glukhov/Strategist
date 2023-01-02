using Newtonsoft.Json;

namespace Strategist.Domain;

public class Ohlcv
{
    [JsonIgnore]
    public DateTime Date {get; set;}
    public long Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    public static long DateTimeToTimestamp(DateTime dateTime)
    {
        long timestamp = (long)(TimeZoneInfo.ConvertTimeToUtc(dateTime)
            - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        return timestamp * 1000;
    }

    public override string ToString() => JsonConvert.SerializeObject(this);
}
