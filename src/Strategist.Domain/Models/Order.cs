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
        return $"{this.OrderId} {this.OpenType} {this.OpenTime} {this.OpenPrice} {this.CloseTime} {this.ClosePrice}";
    }
}