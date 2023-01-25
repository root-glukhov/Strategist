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
        Ohlcv curCandle = StrategyBase.Candles[0];

        float amount = Convert.ToSingle(StrategyBase.BotConfig["Amount"]);
        float amountOrderPct = Convert.ToSingle(StrategyBase.BotConfig["AmountOrderPct"]);
        float lots = (amount * amountOrderPct) / (float)curCandle.Close;

        Order order = new()
        {
            Id = ordersCounter++,
            Status = OrderStatus.Local,
            Lots = lots,
            OrderType = orderType,
            OpenTime = curCandle.Date,
            OpenTimestamp = curCandle.Timestamp,
            OpenPrice = curCandle.Close
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
