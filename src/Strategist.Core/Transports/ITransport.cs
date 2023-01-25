using Strategist.Common;

namespace Strategist.Core.Transports;

internal interface ITransport
{
    void SubscribeToTicksAsync();
    void UnsubscribeAllAsync();
    Task<List<Ohlcv>> GetHistoryAsync(string ticker, int days = 1, int gap = 0);
}
