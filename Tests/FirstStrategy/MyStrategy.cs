using Strategist.Common;
using Strategist.Core;
using Strategist.Core.Services;
using Strategist.Plugins.TakeStop;

namespace FirstStrategy;

internal class MyStrategy : StrategyBase
{
    Order? order;
    TrailingStop trailing;

    int counter;
    Random random;

    public MyStrategy()
    {
        random = new Random();
        trailing = new TrailingStop();
    }

    public override void OnTick(Ohlcv ohlcv)
    {
        if(order == null)
        {
            OrderType randomOrderType = random.Next(2) == 0 ? OrderType.Buy : OrderType.Sell;
            order = OrderService.CreateOrder(randomOrderType);
        }
        else
        {
            counter++;
            if(counter >= 5)
            {
                trailing.SetForPercent(order);
            }
        } 
    }

    public override void OnCreateOrder(Order createdOrder) => 
        Console.WriteLine($"OnCreateOrder:\n{createdOrder}");

    public override void OnClosedOrder(Order closedOrder)
    {
        order = null;
        counter = 0;
        Console.WriteLine($"OnClosedOrder:\n{closedOrder}");
    }
}
