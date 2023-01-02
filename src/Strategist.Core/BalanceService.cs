using Strategist.Domain;

namespace Strategist.Core;

internal class BalanceService
{
    private readonly StrategyBase _sb;
    private decimal fee;

    public BalanceService(StrategyBase sb)
    {
        _sb = sb;
    }

    //internal Stats GetStats(List<Order> orders)
    //{
    //    fee = Decimal.Parse(_sb.botOptions["Fee"].ToString());
    //    decimal amount = Decimal.Parse(_sb.botOptions["Amount"].ToString());

    //    Stats stats = new() {
    //        StartBalance = amount,
    //        Balance = amount,
    //        MinBalance = amount
    //    };

    //    orders.ForEach(o => {
    //        int dir = o.OpenType == OpenType.Buy ?  1 : -1;
    //        decimal profit = (o.ClosePrice - o.OpenPrice) * 1 /* lots */ * dir - fee;

    //        stats.Balance += profit;
    //        stats.Profit += profit;

    //        if (stats.Balance > stats.MaxBalance)
    //            stats.MaxBalance = stats.Balance;

    //        if (stats.Profit < 0) {
    //            decimal currentEquity = stats.StartBalance + stats.Profit;

    //            if (currentEquity < stats.MinBalance)
    //                stats.MinBalance = currentEquity;
    //        }


    //        if (o.OpenType == OpenType.Buy)
    //        {
    //            stats.Long++;
    //            stats.LongRight += profit > 0 ? 1 : 0;
    //        } 
    //        else
    //        {
    //            stats.Short++;
    //            stats.ShortRight += profit > 0 ? 1 : 0;
    //        }
                

            
    //    });

    //    return stats;
    //}

    //internal Chart GetChart()
    //{
    //    Chart chart = new();
    //    chart.Name = "Balance & Equity";
    //    chart.Type = "Balance";

    //    _sb.OrderService.Orders.ForEach(o =>
    //    {
    //        int dir = o.OpenType == OpenType.Buy ? 1 : -1;
    //        decimal profit = (o.ClosePrice - o.OpenPrice) * 1 /* lots */ * dir - fee;
    //        chart.Data.Add(new() { o.CloseTime, profit });
    //    });

    //    return chart;
    //}
}
