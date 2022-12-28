using CryptoExchange.Net.CommonObjects;
using Newtonsoft.Json;
using Strategist.Core.Interfaces;
using Strategist.Core.Models;

namespace Strategist.Core.Commands;

internal class Testing
{
    private readonly StrategyBase _sb;
    private List<Ohlcv> ohlcvData;


    public Testing(StrategyBase strategyBase, IBroker exchange, string ticker, Interval interval, int days, int gap)
	{
        _sb = strategyBase;
        _sb.GetIndicators();
        _sb.OrderService = new OrderService(_sb);


        Datacube dc = CreateDatacube(ticker, exchange);
        ohlcvData = exchange.GetOhlcvData(ticker, interval, days, gap).Result;

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

        

        if (_sb.OrderService.Orders.Count > 0)
        {
            Chart ordersChart = _sb.OrderService.GetChart();
            dc.OnChart.Add(ordersChart);

            var bService = new BalanceService(_sb);
            var stats = bService.GetStats(_sb.OrderService.Orders);
            Chart balanceChart = bService.GetChart();
            dc.OffChart.Add(balanceChart);
            Console.WriteLine(stats.ToString());
        }
        

        dc.Settings = new Settings { RangeFrom = ohlcvData.Count - 500, RangeTo = ohlcvData.Count };
        dc.Save().Wait();

        
        
    }

    private Datacube CreateDatacube(string ticker, IBroker exchange)
    {
        Datacube dc = new();
        dc.Title = $"{ticker} - {exchange.GetType().Name}";
        dc.Chart.IndexBased = true;

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
