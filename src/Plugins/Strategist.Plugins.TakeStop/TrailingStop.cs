using Strategist.Core;
using Strategist.Core.Services;
using Strategist.Domain;

namespace Strategist.Plugins.TakeStop;

public class TrailingStop : StrategyBase
{
    public Order? SetForOrder(Order order, decimal stop)
    {
        if(order.OpenType == OpenType.Buy)
        {
            order.StopPrice = stop > order.StopPrice ? stop : order.StopPrice;
            if(order.ClosePrice <= order.StopPrice)
            {
                Console.WriteLine("Закрываем ордер по стопу");
                return OrderService.CloseOrder(order);
            }
        }
        else
        {
            order.StopPrice = stop < order.StopPrice ? stop : order.StopPrice;
            if (order.ClosePrice >= order.StopPrice)
            {
                Console.WriteLine("Закрываем ордер по стопу");
                return OrderService.CloseOrder(order);
            }
        }

        return null;
    }
}