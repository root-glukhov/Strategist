using System.Text.Json.Serialization;

namespace Strategist.Common;

public enum OrderStatus
{
    Local,
    Placed
}

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
    public long Id { get; set; }
    public OrderStatus Status { get; set; }
    public OrderType OrderType { get; set; }
    public float Lots { get; set; }

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

    public (float, float) GetProfit()
    {
        int dir = OrderType == OrderType.Buy ? 1 : -1;
        float percent = (float)((ClosePrice / OpenPrice) - 1) * dir;
        float profit = (float)OpenPrice * Lots * percent;

        return (profit, percent * 100);
    }

    public override string ToString()
    {
        (float, float) getProfit = GetProfit();
        return $"{Id} {OrderType}\n" 
            + $"Open\t{OpenTime}\t{OpenPrice:0.00}\n" 
            + $"Close\t{CloseTime}\t{ClosePrice:0.00}\n" 
            + $"Profit\t{getProfit.Item1} ({getProfit.Item2:0.00}%)";
    }
}
