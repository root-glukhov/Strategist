namespace Strategist.Core.Models;

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
}