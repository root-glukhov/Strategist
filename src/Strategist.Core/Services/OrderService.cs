using Strategist.Domain;

namespace Strategist.Core.Services;

public class OrderService : StrategyBase
{
    private static List<Order> Orders = new List<Order>();
    private static int ordersCount;

    public static Order GetOrder(int OrderId) => Orders.Find(o => o.OrderId == OrderId);
    public static List<Order> GetOrders() => Orders;

    public static Order CreateOrder(OpenType openType)
    {
        var order = new Order();
        order.OrderId = ordersCount++;
        order.OpenType = openType;
        order.OpenTime = lastCandle.Timestamp;
        order.OpenPrice = lastCandle.Close;

        Orders.Add(order);
        return order;
    }

    public static Order CloseOrder(Order order)
    {
        order.Type = Domain.Type.Both;
        order.CloseTime = lastCandle.Timestamp;
        order.ClosePrice = lastCandle.Close;
        order.CloseType = Domain.Type.Close;

        return order;
    }

    public static Chart GetChart()
    {
        if (Orders.Count == 0)
            return null;

        Chart chart = new("Orders", "Orders");
        chart.Settings.zIndex = 1;

        var lastOrder = Orders.Last();
        if (lastOrder.CloseTime == 0)
            CloseOrder(lastOrder);

        Orders.ForEach(o =>
        {
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