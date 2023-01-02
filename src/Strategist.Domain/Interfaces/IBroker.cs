namespace Strategist.Domain;

public interface IBroker
{
    public Task<List<Ohlcv>> GetOhlcvData(string ticker, Interval interval, int days = 0, int gap = 0);
}
