using Strategist.Core;
using Strategist.Core.Services;
using Strategist.Domain;
using Strategist.Plugins.Indicators;
using Strategist.Plugins.TakeStop;

public class MyStrategy : StrategyBase
{
    TrailingStop trailingStop = new TrailingStop();

    SMA fastSma = new SMA(20);
    SMA slowSma = new SMA(60);
    List<decimal> fastSmaData = new() { 0, 0, 0 };
    List<decimal> slowSmaData = new() { 0, 0, 0 };
    bool allowOrders = false;
    Order? order;
    int orderCandleCounter;

    public MyStrategy()
    {
        Indicators.Add(new Indicator()
        {
            Name = "SMA",
            Figures = new[]
            {
                new Figure()
                {
                    Name = "fast",
                    GetValue = () => fastSmaData[0]
                },
                new Figure()
                {
                    Name = "slow",
                    GetValue = () => slowSmaData[0]
                }
            },
            InChart = true
        });

        // Start strategy
        Start();
    }

    public override void OnCandle(Ohlcv c)
    {
    }

    public override void OnTick(Ohlcv c)
    {
        decimal fastSmaNext = fastSma.Next(c.Close);
        decimal slowSmaNext = slowSma.Next(c.Close);

        fastSmaData.Insert(0, fastSmaNext);
        if (fastSmaData.Count > 3)
            fastSmaData.Remove(fastSmaData.Last());

        slowSmaData.Insert(0, slowSmaNext);
        if (slowSmaData.Count > 3)
            slowSmaData.Remove(slowSmaData.Last());

        if (fastSmaData[0] == 0 || slowSmaData[0] == 0)
            return;

        decimal percent = Math.Abs(((fastSmaData[0] - slowSmaData[0]) / slowSmaData[0]) * 100);

        if (percent < 1)
        {
            allowOrders = true;
        }

        if (
            fastSmaData[0] > fastSmaData[1] &&
            slowSmaData[0] > slowSmaData[1] &&
            fastSmaData[0] > slowSmaData[0] &&
            percent >= 1.2m /*this.opts.openPercent*/ &&
            allowOrders &&
            order == null
        )
        {
            order = OrderService.CreateOrder(OpenType.Sell);
            allowOrders = false;
        }

        if (
            fastSmaData[0] < fastSmaData[1] &&
            slowSmaData[0] < slowSmaData[1] &&
            fastSmaData[0] < slowSmaData[0] &&
            percent >= 1.2m /*this.opts.openPercent*/ &&
            order == null &&
            allowOrders
        )
        {
            order = OrderService.CreateOrder(OpenType.Buy);
            allowOrders = false;
        }

        if (order != null)
        {
            orderCandleCounter++;
            if (orderCandleCounter >= 4 * 9)
            {
                decimal stop = order.OpenType == OpenType.Buy ? c.Close - c.Close * 0.10m : c.Close * 1.10m;

                var closedOrder = trailingStop.SetForOrder(order, stop);
                if (closedOrder != null)
                {
                    Console.WriteLine($"{closedOrder} {orderCandleCounter}");
                    order = null;
                    allowOrders = true;
                    orderCandleCounter = 0;
                }
            }
        }
    }
}