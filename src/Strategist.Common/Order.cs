using System.Text.Json.Serialization;

namespace Strategist.Common;

public enum OrderType
{
    Buy,
    Sell
}

public enum CloseType
{
    Take,
    Stop,
    Exit
}

public class Order
{
    public int Id { get; set; }
    public OrderType OrderType { get; set; }

    [JsonIgnore]
    public DateTime OpenTime { get; set; }
    public long OpenTimestamp { get; set; }
    public decimal OpenPrice { get; set; }

    [JsonIgnore]
    public DateTime CloseTime { get; set; }
    public long CloseTimestamp { get; set; }
    public decimal ClosePrice { get; set; }

    public CloseType CloseType { get; set; }
    public bool isClosed;

    public float GetProfitPercent()
    {
        int dir = OrderType == OrderType.Buy ? 1 : -1;
        return (float)(ClosePrice / OpenPrice * 100 - 100) * dir;
    } 

    public override string ToString()
    {
        return $"{Id} {OrderType}\nOpen\t{OpenTime}\t{OpenPrice}\nClose\t{CloseTime}\t{ClosePrice}";
    }
}
