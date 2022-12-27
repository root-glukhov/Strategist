using Strategist.Core.Models;

namespace Strategist.Core.Interfaces;

internal interface IExchange
{
    public Task<List<Ohlcv>> GetOhlcvData(string ticker, Interval interval, int days = 0, int gap = 0);
}
