using Strategist.Domain;

namespace Strategist.Core.Services;

internal class Stats
{
    public decimal StartBalance;
    public decimal Balance;
    public decimal MaxBalance;
    public decimal MinBalance;
    public decimal Profit;
    public int Long;
    public int LongRight;
    public int Short;
    public int ShortRight;
    //public int CandlesHandled;

    public override string ToString()
    {
        return $"StartBalance:      {StartBalance.ToString("0.00")}\n" +
               $"Balance:           {Balance.ToString("0.00")}\n" +
               $"MaxBalance:        {MaxBalance.ToString("0.00")}\n" +
               $"MinBalance:        {MinBalance.ToString("0.00")}\n" +
               $"Profit:            {Profit.ToString("0.00")}\n" +
               $"Long:              {Long}\n" +
               $"LongRight:         {LongRight}\n" +
               $"Short:             {Short}\n" +
               $"ShortRight:        {ShortRight}\n";
    }
}

internal class BalanceService : StrategyBase
{
    //private static decimal fee;
    private static decimal amount;


    internal static Stats GetStats()
    {
        //fee = Decimal.Parse(botConfig["Fee"].ToString());
        amount = decimal.Parse(botConfig["Amount"].ToString());
        decimal orderSum = 100; // amount * 0.02m;

        Stats stats = new()
        {
            StartBalance = amount,
            Balance = amount,
            MinBalance = amount,
            MaxBalance = amount
        };

        OrderService.GetOrders().ForEach(o =>
        {
            int dir = o.OpenType == OpenType.Buy ? 1 : -1;
            decimal lot = orderSum / (o.OpenPrice * 0.01m) / 100;
            decimal profit = (o.ClosePrice - o.OpenPrice) * lot * dir; // - fee;

            stats.Balance += profit;
            stats.Profit += profit;

            if (stats.Balance > stats.MaxBalance)
                stats.MaxBalance = stats.Balance;

            if (stats.Profit < 0)
            {
                decimal currentEquity = stats.StartBalance + stats.Profit;

                if (currentEquity < stats.MinBalance)
                    stats.MinBalance = currentEquity;
            }


            if (o.OpenType == OpenType.Buy)
            {
                stats.Long++;
                stats.LongRight += profit > 0 ? 1 : 0;
            }
            else
            {
                stats.Short++;
                stats.ShortRight += profit > 0 ? 1 : 0;
            }



        });

        return stats;
    }

    internal static Chart GetChart()
    {
        Chart chart = new("Balance", "Balance & Equity");

        OrderService.GetOrders().ForEach(o =>
        {
            int dir = o.OpenType == OpenType.Buy ? 1 : -1;
            decimal orderSum = 100;
            decimal lot = orderSum / (o.OpenPrice * 0.01m) / 100;
            decimal profit = (o.ClosePrice - o.OpenPrice) * lot * dir;// - fee;
            chart.Data.Add(new() { o.CloseTime, profit });
        });

        return chart;
    }
}
