using Newtonsoft.Json;
using Strategist.Core.Interfaces;
using Strategist.Core.Models;

namespace Strategist.Core.Commands;

internal class Testing
{
    private Datacube dc = new();

	public Testing(StrategyBase strategyBase, IExchange exchange, string ticker, Interval interval, int days, int gap)
	{
        dc.Title = $"{ticker} - {exchange}";
        dc.Chart.IndexBased = true;
        dc.Settings = new Settings { RangeFrom = 0, RangeTo = 100 };

        strategyBase.GetIndicators();

        List<Ohlcv> ohlcvData = exchange.GetOhlcvData(ticker, interval, days, gap).Result;

        foreach (var indicator in strategyBase.Indicators)
        {
            var chart = new Chart()
            {
                Name = indicator.Name,
                Type = "Indicators",
            };

            chart.Settings.Schema = new();
            chart.Settings.Colors = new();

            chart.Settings.Schema.Add("time");

            foreach (var figure in indicator.Figures)
            {
                string schemaValue = $"{figure.Name}.{figure.Type}.value".ToLower();
                chart.Settings.Schema.Add(schemaValue);
                chart.Settings.Colors.Add(null);
            }

            if (indicator.InChart)
            {
                dc.OnChart.Add(chart);
            }
            else
            {
                dc.OffChart.Add(chart);
            }
        }

        foreach (var c in ohlcvData)
        {
            dc.Chart.Data.Add(new() { c.Timestamp, c.Open, c.High, c.Low, c.Close, c.Volume });

            strategyBase.OnCandle(c);

            strategyBase.Indicators.ForEach(x => {
                var index = strategyBase.Indicators.IndexOf(x);
                var obj = new List<object> { c.Timestamp };

                x.Figures.ToList().ForEach(f =>
                {
                    var value = f.AddValue(f.GetValue());
                    obj.Add(value);
                });

                if (x.InChart)
                    dc.OnChart[index].Data.Add(obj);
                else
                    dc.OffChart[index].Data.Add(obj);
            });
        }

        dc.Save().Wait();

        Console.WriteLine(JsonConvert.SerializeObject(dc));
    }
}
