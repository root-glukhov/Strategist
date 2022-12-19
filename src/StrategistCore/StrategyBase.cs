using Microsoft.Extensions.Configuration;
using StrategistCore.Enums;
using StrategistCore.Models;
using System.Collections;
using System.CommandLine;

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

        List<Ohlcv> ohlcvData = config.Exchange.GetOhlcvData(config.Ticker, config.Interval, config.Days, config.Gap).Result;
        foreach (var c in ohlcvData)
        {
            OnCandle(c);
            var indic = GetIndicators(indicators);
            Console.WriteLine("{0}", indic[0].Name);
        }
    }
}
