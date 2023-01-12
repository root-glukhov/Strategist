namespace Strategist.Domain;

public enum Type
{
    Entry, 
    Open, 
    Close, 
    Both, 
    Reduce,
    Exit,
    Stop
} 

public enum OpenType
{
    Buy, 
    Sell
}

public class Order
{
    public long CloseTime;
    public int OrderId;
    public Type Type;
    public decimal OpenPrice;
    public OpenType OpenType;
    public decimal StopPrice;
    public Type CloseType;
    public decimal ClosePrice;
    public long OpenTime;

    public override string ToString()
    {
        int dir = OpenType == OpenType.Buy ? 1 : -1;
        decimal profit = (ClosePrice - OpenPrice) * dir;
        decimal profitPercent = (ClosePrice / (OpenPrice * 0.01m) - 100) * dir;

        return $"{OrderId} {OpenType}\t{profit.ToString("#.00")}|{profitPercent.ToString("0.00")}%";
    }
}