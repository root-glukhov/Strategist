using Strategist.Core;
using Strategist.Domain;
using Strategist.Plugins.Indicators;

public class MyStrategy : StrategyBase
{
    SMA fastSma = new SMA(20);
    SMA slowSma = new SMA(60);
    List<decimal> fastSmaData = new() { 0, 0, 0 };
    List<decimal> slowSmaData = new() { 0, 0, 0 };
    //bool allowOrders = false;
    //Order order;

    public override void GetIndicators()
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
    }

    public override void OnCandle(Ohlcv c)
    {
        decimal fastSmaNext = fastSma.Next(c.Close);
        decimal slowSmaNext = slowSma.Next(c.Close);

        fastSmaData.Insert(0, fastSmaNext);
        if (fastSmaData.Count > 3)
            fastSmaData.Remove(fastSmaData.Last());

        slowSmaData.Insert(0, slowSmaNext);
        if (slowSmaData.Count > 3)
            slowSmaData.Remove(slowSmaData.Last());

        //if (fastSmaData[0] == 0 || slowSmaData[0] == 0)
        //    return;

        //decimal percent = Math.Abs(((fastSmaData[0] - slowSmaData[0]) / slowSmaData[0]) * 100);

        //if (percent < 1)
        //{
        //    allowOrders = true;
        //}

        //if (
        //    fastSmaData[0] > fastSmaData[1] &&
        //    slowSmaData[0] > slowSmaData[1] &&
        //    fastSmaData[0] > slowSmaData[0] &&
        //    percent >= 1.2m /*this.opts.openPercent*/ &&
        //    allowOrders &&
        //    order == null
        //)
        //{
        //    order = CreateOrder(OpenType.Sell);
        //    allowOrders = false;
        //}

        //if (
        //    fastSmaData[0] < fastSmaData[1] &&
        //    slowSmaData[0] < slowSmaData[1] &&
        //    fastSmaData[0] < slowSmaData[0] &&
        //    percent >= 1.2m /*this.opts.openPercent*/ &&
        //    order == null &&
        //    allowOrders
        //)
        //{
        //    order = CreateOrder(OpenType.Buy);
        //    allowOrders = false;
        //}

        //if (order != null)
        //{
        //    if (order.OpenType == OpenType.Buy)
        //    {
        //        decimal profit = c.Close - order.OpenPrice;
        //        if (profit > 800 || profit < -300)
        //        {
        //            Order closedOrder = CloseOrder(order);
        //            Console.WriteLine(closedOrder.ToString());
        //            order = null;
        //            allowOrders = true;
        //        }
        //    } 
        //    else
        //    {
        //        decimal profit = -(c.Close - order.OpenPrice);
        //        if (profit > 800 || profit < -300)
        //        {
        //            Order closedOrder = CloseOrder(order);
        //            Console.WriteLine(closedOrder.ToString());
        //            order = null;
        //            allowOrders = true;
        //        }
        //    }
        //}
    }
}