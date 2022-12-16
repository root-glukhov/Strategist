using Binance.Net.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using CryptoExchange.Net.CommonObjects;
using String = System.String;

namespace StrategistCore.Models;

public class Ohlcv
{
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    public static List<Ohlcv> KlineToOhlcv(IEnumerable<IBinanceKline> klines)
    {
        List<Ohlcv> ohlcvList = new List<Ohlcv>();

        foreach (var kline in klines)
        {
            ohlcvList.Add(new Ohlcv()
            {
                Timestamp = kline.OpenTime, //DateTimeToTimestamp(kline.OpenTime),
                Open = kline.OpenPrice,
                High = kline.HighPrice,
                Low = kline.LowPrice,
                Close = kline.ClosePrice,
                Volume = kline.Volume,
            });
        }

        return ohlcvList;
    }

    public static double DateTimeToTimestamp(DateTime dateTime)
    {
        return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
               new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
    }

    public override string ToString()
    {
        return String.Format("{0}\t{1}; {2}; {3}; {4};", Timestamp, Open, High, Low, Close);
    }
}
