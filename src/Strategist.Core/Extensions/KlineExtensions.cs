using Binance.Net.Interfaces;
using Strategist.Common;

namespace Strategist.Core.Extensions;

public static class KlineExtensions
{
    public static Ohlcv ToOhlcv(this IBinanceKline kline)
    {
        return new Ohlcv()
        {
            Date = kline.OpenTime,
            Timestamp = kline.OpenTime.ToTimestamp(),
            Open = kline.OpenPrice,
            High = kline.HighPrice,
            Low = kline.LowPrice,
            Close = kline.ClosePrice,
            Volume = kline.Volume
        };
    }

    public static Ohlcv ToOhlcv(this IBinanceStreamKline kline)
    {
        return new Ohlcv()
        {
            Date = kline.OpenTime,
            Timestamp = kline.OpenTime.ToTimestamp(),
            Open = kline.OpenPrice,
            High = kline.HighPrice,
            Low = kline.LowPrice,
            Close = kline.ClosePrice,
            Volume = kline.Volume
        };
    }
}
