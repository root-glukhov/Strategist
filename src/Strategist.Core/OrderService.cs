using Strategist.Core.Models;

namespace Strategist.Core;

internal class OrderService
{
    private readonly StrategyBase _sb;

    internal List<Order> Orders = new();
    private int ordersCount;

    public OrderService(StrategyBase sb)
    {
        _sb = sb;
    }

    public Order CreateOrder(OpenType type)
    {
        

        var order = new Order();
        order.OrderId = ordersCount++;
        order.OpenType = type;
        order.OpenTime = _sb.lastCandle.Timestamp;
        order.OpenPrice = _sb.lastCandle.Close;

        Orders.Add(order);
        return order;
    }

    public Order CloseOrder(Order order)
    {
        order.Type = Models.Type.Both;
        order.CloseTime = _sb.lastCandle.Timestamp;
        order.ClosePrice = _sb.lastCandle.Close;
        order.CloseType = Models.Type.Exit;
        return order;
    }

    internal Chart GetChart()
    {
        if (Orders.Count == 0)
            return null;

        Chart chart = new();
        chart.Name = "Orders";
        chart.Type = "Orders";
        chart.Settings.zIndex = 1;

        var lastOrder = Orders.Last();
        if (lastOrder.CloseTime == 0)
            Orders.Remove(lastOrder);

        Orders.ForEach(o => {
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

        return chart;
    }
}
