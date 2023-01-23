using Strategist.Common;
using Strategist.Core.Models;

namespace Strategist.Core.Services;

internal static class StatService
{
    internal static StrategyBase _sb;

    internal static Stats GetStats()
    {
        float balance = Convert.ToSingle(StrategyBase.BotConfig["Amount"]);
        Stats stats = new(balance);
        List<Order> orders = OrderService.GetOrders();
        stats.OrdersCount = orders.Count;

        orders.ForEach(order =>
        {
            float profit = order.GetProfit().Item1;

            stats.Balance += profit;
            stats.MaxBalance = stats.Balance > stats.MaxBalance ? stats.Balance : stats.MaxBalance;
            stats.MinBalance = stats.Balance < stats.MinBalance ? stats.Balance : stats.MinBalance;

            if (order.OrderType == OrderType.Buy)
            {
                stats.Long++;
                if (profit > 0)
                    stats.LongRight++;
            } 
            else
            { 
                stats.Short++;
                if (profit > 0)
                    stats.ShortRight++;
            }
        });

        stats.PercentProfit = (stats.Balance / balance - 1) * 100;
        return stats;
    }
}
