using Strategist.Common;
using Strategist.Core;
using Strategist.Core.Services;

namespace Strategist.Plugins.TakeStop;

class StopOrder
{
    public Order Order { get; set; }
    public decimal StopPrice { get; set; }

    public StopOrder(Order order, decimal stopPrice)
    {
        Order = order;
        StopPrice = stopPrice;
    }

    //public decimal GetStopPrice(decimal curPrice)
    //{
    //    int dir = Order.OrderType == OrderType.Buy ? 1 : -1;
    //    return curPrice - (curPrice * (decimal)StopPercent * dir);
    //}
}


public class TrailingStop
{
    private float stopPercent;
    private List<StopOrder> stopOrders = new();

    public delegate void OnStop(Order order);

    public TrailingStop()
    {
        var a = StrategyBase.BotConfig["Stop"];
        stopPercent = Convert.ToSingle(a);
        if (stopPercent == 0)
            throw new ArgumentNullException(nameof(stopPercent));
    }

    public void SetForPercent(Order order, OnStop? onStop = null)
    {
        int dir = order.OrderType == OrderType.Buy ? 1 : -1;
        decimal curPrice = StrategyBase.Candles.First().Close;
        decimal curStopPrice = curPrice - (curPrice * (decimal)stopPercent * dir);

        StopOrder? stopOrder = stopOrders.Find(x => x.Order.Id == order.Id);
        if (stopOrder == null)
        {
            stopOrder = new(order, curStopPrice);
            stopOrders.Add(stopOrder);
        }

        if (order.OrderType == OrderType.Buy && curStopPrice > stopOrder.StopPrice
            || order.OrderType == OrderType.Sell && curStopPrice < stopOrder.StopPrice)
        {
            stopOrder.StopPrice = curStopPrice;
        }
        else if (order.OrderType == OrderType.Buy && curPrice <= stopOrder.StopPrice
            || order.OrderType == OrderType.Sell && curPrice >= stopOrder.StopPrice)
        {
            OrderService.CloseOrder(stopOrder.Order);

            if (onStop != null)
                onStop(stopOrder.Order);
        }
    }
}