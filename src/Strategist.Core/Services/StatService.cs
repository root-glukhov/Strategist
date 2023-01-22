using Strategist.Common;

namespace Strategist.Core.Services;

public class Stats
{
    public float StartBalance { get; set; }
    public float MaxBalance { get; set; }
    public float MinBalance { get; set; }
    public float Balance { get; set; }

    public int OrdersCount { get; set; }
    public int Long { get; set; }
    public int LongRight { get; set; }
    public int Short { get; set; }
    public int ShortRight { get; set; }

    public override string ToString() =>
        $"StartBalance: {StartBalance}\n"
        + $"MaxBalance:   {MaxBalance}\n"
        + $"MinBalance:   {MinBalance}\n"
        + $"Balance:      {Balance}\n\n"
        + $"OrdersCount:  {OrdersCount}\n"
        + $"Long:         {Long}\n"
        + $"LongRight:    {LongRight}\n"
        + $"Short:        {Short}\n"
        + $"ShortRight:   {ShortRight}";
}

public static class StatService
{
    public static StrategyBase _sb;

    public static Stats GetStats()
    {
        Stats stats = new();
        List<Order> orders = OrderService.GetOrders();
        IEnumerable<Order> longOrders = orders.Where(x => x.OrderType == OrderType.Buy);
        IEnumerable<Order> shortOrders = orders.Where(x => x.OrderType == OrderType.Sell);

        stats.OrdersCount = orders.Count;
        stats.Long = longOrders.Count();
        stats.LongRight = longOrders.Where(x => x.GetProfitPercent() > 0).Count();
        stats.Short = shortOrders.Count();
        stats.ShortRight = shortOrders.Where(x => x.GetProfitPercent() > 0).Count();

        return stats;
    }
}
