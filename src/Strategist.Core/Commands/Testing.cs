using CryptoExchange.Net.CommonObjects;
using Newtonsoft.Json;
using Strategist.Core.Interfaces;
using Strategist.Core.Models;

namespace Strategist.Core.Commands;

internal class Testing
{
    private readonly StrategyBase _sb;

	public Testing(StrategyBase strategyBase, IExchange exchange, string ticker, Interval interval, int days, int gap)
	{
        _sb = strategyBase;
        _sb.GetIndicators();
        _sb.Orders = new OrdersService(_sb);


        Datacube dc = CreateDatacube(ticker, exchange);
        List<Ohlcv> ohlcvData = exchange.GetOhlcvData(ticker, interval, days, gap).Result;

        ohlcvData.ForEach(ohlcv => {
            dc.Chart.Data.Add(new() { ohlcv.Timestamp, ohlcv.Open, ohlcv.High, ohlcv.Low, ohlcv.Close, ohlcv.Volume });
            _sb.lastCandle = ohlcv;
            _sb.OnCandle(ohlcv);
 
            _sb.Indicators.ForEach(indicator => {
                int index = _sb.Indicators.IndexOf(indicator);
                var item = new List<object> { ohlcv.Timestamp };

                indicator.Figures.ToList().ForEach(figure => {
                    var value = figure.AddValue(figure.GetValue());
                    item.Add(value);
                });

                if (indicator.InChart)
                    dc.OnChart[index].Data.Add(item);
                else
                    dc.OffChart[index].Data.Add(item);
            });
        });

        // Туть добавить в dc все созданные ордера
        _sb.Orders.AddToDatacube(ref dc);

        dc.Save().Wait();
    }

    private Datacube CreateDatacube(string ticker, IExchange exchange)
    {
        Datacube dc = new();
        dc.Title = $"{ticker} - {exchange.GetType().Name}";
        dc.Chart.IndexBased = true;
        dc.Settings = new Settings { RangeFrom = 0, RangeTo = 100 };

        _sb.Indicators.ForEach(indicator => {
            Chart chart = new();
            chart.Name = indicator.Name;
            chart.Type = "Indicators";
            chart.Settings.Schema = new();
            chart.Settings.Colors = new();

            chart.Settings.Schema.Add("time");

            indicator.Figures.ToList().ForEach(figure => {
                string schemaValue = $"{figure.Name}.{figure.Type}.value".ToLower();
                chart.Settings.Schema.Add(schemaValue);
                chart.Settings.Colors.Add(null);
            });

            if (indicator.InChart)
                dc.OnChart.Add(chart);
            else
                dc.OffChart.Add(chart);
        });

        return dc;
    }
}
