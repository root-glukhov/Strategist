using Strategist.Common;

namespace Strategist.Core.Transports;

internal interface ITransport
{
    Task<IEnumerable<Ohlcv>> GetHistoryAsync(string ticker, string intervalStr, int days = 1, int gap = 0);
    Task SubscribeToTicksAsync(string ticker, string intervalStr);
    Task UnsubscribeAllAsync();
}
