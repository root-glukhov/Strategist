using CryptoExchange.Net.CommonObjects;
using Newtonsoft.Json;
using Strategist.Core.Transports;
using Strategist.Domain;
using System.CommandLine;

namespace Strategist.Core.Commands;

internal class TestCommand : Command
{
    private readonly StrategyBase _sb;
    private Dictionary<string, object> botConfig;
    private List<Ohlcv> ohlcvData;

    #region Constructor

    public TestCommand(StrategyBase sb)
           : base("test", "Запустить бэктест стратегии.")
    {
        _sb = sb;

        #region Init Options

        var tickerOption = new Option<string>("--ticker")
        {
            Name = "ticker",
            Description = "Тикер",
            IsRequired = true
        };
        AddOption(tickerOption);

        var daysOption = new Option<int>("--days")
        {
            Name = "days",
            Description = "Кол-во дней для теста"
        };
        AddOption(daysOption);

        var gapOption = new Option<int>("--gap")
        {
            Name = "gap",
            Description = "Пропустить кол-во дней"
        };
        AddOption(gapOption);

        #endregion

        this.SetHandler(Handle, tickerOption, daysOption, gapOption);
    }

    #endregion

    private void Handle(string ticker, int days = 1, int gap = 0)
    {
        _sb.GetIndicators();

        string json = new StreamReader("Properties/botconfig.json").ReadToEnd();
        botConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        IBroker broker = GetBroker(botConfig["Broker"].ToString());
        Interval interval = GetInterval(botConfig["Interval"].ToString());

        Datacube dc = CreateDatacube(ticker, broker);
        ohlcvData = broker.GetOhlcvData(ticker, interval, days, gap).Result;

        ohlcvData.ForEach(ohlcv =>
        {
            dc.Chart.Data.Add(new() { ohlcv.Timestamp, ohlcv.Open, ohlcv.High, ohlcv.Low, ohlcv.Close, ohlcv.Volume });
            // TODO: Get last candles
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

        //if (_sb.OrderService.Orders.Count > 0)
        //{
        //    Chart ordersChart = _sb.OrderService.GetChart();
        //    dc.OnChart.Add(ordersChart);

        //    var bService = new BalanceService(_sb);
        //    var stats = bService.GetStats(_sb.OrderService.Orders);
        //    Chart balanceChart = bService.GetChart();
        //    dc.OffChart.Add(balanceChart);
        //    Console.WriteLine(stats.ToString());
        //}

        dc.Settings = new Settings { RangeFrom = 0, RangeTo = ohlcvData.Count };
        dc.Save().Wait();
    }

    private IBroker GetBroker(string broker)
    {
        switch (broker.ToLower())
        {
            case "binance":
                return new BinanceTransport();
            default:
                throw new NotSupportedException();
        }
    }

    private Interval GetInterval(string interval)
    {
        switch (interval.ToLower())
        {
            case "1m":
                return Interval.OneMinute;
            case "3m":
                return Interval.ThreeMinutes;
            case "5m":
                return Interval.FiveMinutes;
            case "15m":
                return Interval.FifteenMinutes;
            case "30m":
                return Interval.ThirtyMinutes;
            case "1h":
                return Interval.OneHour;
            case "4h":
                return Interval.FourHour;
            default:
                throw new NotSupportedException();
        }
    }

    private Datacube CreateDatacube(string ticker, IBroker broker)
    {
        Datacube dc = new();
        dc.Title = $"{ticker} - {broker.GetType().Name}";
        dc.Chart.IndexBased = true;

        _sb.Indicators.ForEach(indicator =>
        {
            Chart chart = new("Indicators", indicator.Name);
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