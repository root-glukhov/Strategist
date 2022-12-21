using StrategistCore.Models;
using System.Text.Json;

namespace StrategistCore;

public abstract class StrategyBase
{

    ConfigArguments config;

    public abstract void OnCandle(Ohlcv c);

    List<Indicator> indicators = new List<Indicator>();
    public abstract List<Indicator> GetIndicators(List<Indicator> indicators);

    public StrategyBase()
    {
        config = new ConfigArguments();

        // Init Datacube
        Datacube dc = new Datacube();
        dc.Chart.Name = config.Ticker;
        dc.Chart.Type = "Candles";

        List<Ohlcv> ohlcvData = config.Exchange.GetOhlcvData(config.Ticker, config.Interval, config.Days, config.Gap).Result;
        foreach (var c in ohlcvData)
        {
            OnCandle(c);
            
            dc.Chart.Data.Add(new decimal[] { c.Timestamp, c.Open, c.High, c.Low, c.Close, c.Volume });
            //var indic = GetIndicators(indicators);
            //Console.WriteLine("{0}", indic[0].Name);
        }

        dc.Save().Wait();
        //string json = JsonSerializer.Serialize(dc);
        //Console.WriteLine(json);
    }
}
