using Strategist.Common;

namespace Strategist.Plugins.TakeStop.Models;

internal class StopOrder
{
    public Order Order { get; set; }
    public decimal StopPrice { get; set; }

    public StopOrder(Order order, decimal stopPrice)
    {
        Order = order;
        StopPrice = stopPrice;
    }
}
