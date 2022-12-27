using Binance.Net.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using CryptoExchange.Net.CommonObjects;
using String = System.String;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Newtonsoft.Json;

namespace Strategist.Core.Models;

public class Ohlcv
{
    public long Timestamp { get; private set; }
    public decimal Open { get; private set; }
    public decimal High { get; private set; }
    public decimal Low { get; private set; }
    public decimal Close { get; private set; }
    public decimal Volume { get; private set; }

    public static List<Ohlcv> KlineToOhlcv(IEnumerable<IBinanceKline> klines)
    {
        List<Ohlcv> ohlcvList = new();

        foreach (var kline in klines)
        {
            ohlcvList.Add(new Ohlcv()
            {
                Timestamp = DateTimeToTimestamp(kline.OpenTime),
                Open = kline.OpenPrice,
                High = kline.HighPrice,
                Low = kline.LowPrice,
                Close = kline.ClosePrice,
                Volume = kline.Volume,
            });
        }

        return ohlcvList;
    }

    public static long DateTimeToTimestamp(DateTime dateTime)
    {
        long timestamp = (long)(TimeZoneInfo.ConvertTimeToUtc(dateTime)
            - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        return timestamp * 1000;
    }

    public override string ToString() => JsonConvert.SerializeObject(this);
}
