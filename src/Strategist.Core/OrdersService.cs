using Strategist.Core.Models;

namespace Strategist.Core;

public class OrdersService
{
    private readonly StrategyBase _sb;

    private List<Order> orders = new();
    private int ordersCount;

    public OrdersService(StrategyBase sb)
    {
        _sb = sb;
    }

    public Order CreateOrder(OpenType type)
    {
        Console.WriteLine($"Ордер {type} создан!");

        var order = new Order();
        order.OrderId = ordersCount++;
        order.OpenType = type;
        order.OpenTime = _sb.lastCandle.Timestamp;
        order.OpenPrice = _sb.lastCandle.Close;

        orders.Add(order);

        return order;
    }

    public void CloseOrder(Order order)
    {
        order.Type = Models.Type.Both;
        order.CloseTime = _sb.lastCandle.Timestamp;
        order.ClosePrice = _sb.lastCandle.Close;
        order.CloseType = Models.Type.Exit;
    }

    internal void AddToDatacube(ref Datacube dc)
    {
        Chart chart = new();
        chart.Name = "Orders";
        chart.Type = "Orders";
        chart.Settings.zIndex = 1;

        var lastOrder = orders.Last();
        if (lastOrder.CloseTime == 0)
            orders.Remove(lastOrder);

        orders.ForEach(o => {
            List<object> item = new() { 
                o.CloseTime, 
                o.OrderId, 
                o.Type.ToString(), 
                o.OpenPrice, 
                o.OpenType.ToString(), 
                o.StopPrice, 
                o.CloseType.ToString(), 
                o.ClosePrice, 
                o.OpenTime 
            };
            chart.Data.Add(item);
        });

        dc.OnChart.Add(chart);
    }
}
