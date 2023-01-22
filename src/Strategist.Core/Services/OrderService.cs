using Strategist.Common;

namespace Strategist.Core.Services;

public static class OrderService
{
    public static StrategyBase _sb;

    private static int ordersCounter;
    private static List<Order> Orders = new();

    #region Static methods

    internal static List<Order> GetOrders() => Orders;

    public static Order CreateOrder(OrderType orderType)
    {
        Order order = new()
        {
            Id = ordersCounter++,
            OrderType = orderType,
            OpenTime = StrategyBase.Candles[0].Date,
            OpenTimestamp = StrategyBase.Candles[0].Timestamp,
            OpenPrice = StrategyBase.Candles[0].Close
        };

        Orders.Add(order);
        _sb.OnCreateOrder(order);
        return order;
    }

    public static Order CloseOrder(Order order)
    {
        order.CloseTime = StrategyBase.Candles[0].Date;
        order.CloseTimestamp = StrategyBase.Candles[0].Timestamp;
        order.ClosePrice = StrategyBase.Candles[0].Close;
        order.isClosed = true;

        _sb.OnClosedOrder(order);
        return order;
    }

    public static void CloseAll()
    {
        foreach (var order in Orders.Where(x => !x.isClosed))
        {
            CloseOrder(order);
        }
    }

    #endregion
}
