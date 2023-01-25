using Strategist.Common;
using Strategist.Core;
using Strategist.Core.Services;
using Strategist.Plugins.TakeStop.Models;

namespace Strategist.Plugins.TakeStop;

public class TrailingStop
{
    private readonly float stopPercent;
    private readonly List<StopOrder> stopOrders = new();

    public delegate void OnStop(Order order);

    public TrailingStop()
    {
        stopPercent = Convert.ToSingle(StrategyBase.GetConfig("Stop"));
        if (stopPercent == 0)
            throw new ArgumentNullException(nameof(stopPercent));
    }

    public void SetForPercent(Order order, OnStop? onStop = null)
    {
        int dir = order.OrderType == OrderType.Buy ? 1 : -1;
        decimal curPrice = StrategyBase.GetCandle(0).Close;
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
            onStop?.Invoke(stopOrder.Order);
        }
    }
}