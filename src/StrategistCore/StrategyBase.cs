using Microsoft.Extensions.Configuration;
using StrategistCore.Enums;
using StrategistCore.Models;

namespace StrategistCore;

public abstract class StrategyBase
{
    public abstract void OnCandle(Ohlcv c);

    List<Indicator> indicators = new List<Indicator>();
    public abstract List<Indicator> GetIndicators(List<Indicator> indicators);

    public StrategyBase()
    {
        IConfigurationRoot? config = new ConfigurationBuilder()
            .AddJsonFile($"botoptions.json")
            .Build();

        string exchaneName = config.GetSection("Exchange").Value!;
        IExchange exchange = CreateExchange(exchaneName);

        string ticker = config.GetSection("Ticker").Value!;
        Interval interval; Enum.TryParse(config.GetSection("Interval").Value!, true, out interval);
        int days; int.TryParse(config.GetSection("Days").Value, out days);
        int gap; int.TryParse(config.GetSection("Gap").Value, out gap);

        List<Ohlcv> ohlcvData = exchange.GetOhlcvData(ticker, interval, days, gap).Result;
        foreach (var c in ohlcvData)
        {
            OnCandle(c);
            var indic = GetIndicators(indicators);
            Console.WriteLine("{0}", indic[0].Name);
        }
    }

    private IExchange CreateExchange(string exchangeName)
    {
        switch (exchangeName.ToLower())
        {
            case "binance":
                return new Exchanges.Binance();
            default:
                throw new ArgumentNullException();
        }
    }
}
