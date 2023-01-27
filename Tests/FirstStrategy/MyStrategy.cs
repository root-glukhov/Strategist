using Strategist.Common;
using Strategist.Core;
using Strategist.Core.Services;
using Strategist.Plugins.Indicators;
using Strategist.Plugins.TakeStop;

namespace FirstStrategy;

internal class MyStrategy : StrategyBase
{
    Order? order;
    readonly SMA smaFast;
    readonly SMA smaSlow;
    readonly TrailingStop trailing;

    public MyStrategy()
    {
        smaFast = new SMA(9);
        smaSlow = new SMA(32);
        trailing = new TrailingStop();
    }

    public override void OnTick(Ohlcv ohlcv)
    {
        decimal smaFastMoment = smaFast.Moment(ohlcv.Close);
        decimal smaSlowMoment = smaSlow.Moment(ohlcv.Close);

        if (order == null)
        {
            if (smaFastMoment > smaSlowMoment 
                && smaFast.GetValues(1) <= smaSlow.GetValues(1))
            {
                order = OrderService.CreateOrder(OrderType.Sell);
            }
            else if (smaFastMoment < smaSlowMoment 
                && smaFast.GetValues(1) >= smaSlow.GetValues(1))
            {
                order = OrderService.CreateOrder(OrderType.Buy);
            }
        }
        else
        {
            trailing.SetForPercent(order);
        }
    }

    public override void OnCandle(Ohlcv ohlcv)
    { 
        smaFast.Next(ohlcv.Close);
        smaSlow.Next(ohlcv.Close);
    }

    //public override void OnCreateOrder(Order createdOrder) => 
    //    Console.WriteLine($"OnCreateOrder:\n{createdOrder}");

    public override void OnClosedOrder(Order closedOrder)
    {
        Console.WriteLine($"OnClosedOrder:\n{closedOrder}");
        order = null;
    }
}
