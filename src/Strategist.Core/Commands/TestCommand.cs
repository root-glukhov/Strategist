using Newtonsoft.Json;
using Strategist.Core.Services;
using Strategist.Core.Transports;
using Strategist.Domain;
using System.CommandLine;

namespace Strategist.Core.Commands;

internal class TestCommand : Command
{
    private readonly StrategyBase _sb;
    private List<Ohlcv> ohlcvData;

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

    private void Handle(string ticker, int days = 1, int gap = 0)
    {
        string json = new StreamReader("Properties/botconfig.json").ReadToEnd();
        StrategyBase.botConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        IBroker broker = GetBroker(StrategyBase.botConfig["Broker"].ToString());
        Interval interval = GetInterval(StrategyBase.botConfig["Interval"].ToString());
        ohlcvData = broker.GetOhlcvData(ticker, interval, days, gap).Result;

        Datacube dc = CreateDatacube(ticker, broker);

        ohlcvData.ForEach(ohlcv =>
        {
            dc.Chart.Data.Add(ohlcv.ToObject());
            StrategyBase.lastCandle = ohlcv;

            _sb.OnCandle(ohlcv);

            List<Ohlcv> ticks = GenereateTicks(ohlcv);
            ticks.ForEach(tick => {
                _sb.OnTick(tick);
            });

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

        var orders = OrderService.GetOrders();
        if (orders.Count > 0)
        {
            Chart ordersChart = OrderService.GetChart();
            dc.OnChart.Add(ordersChart);

            var stats = BalanceService.GetStats();
            Chart balanceChart = BalanceService.GetChart();
            dc.OffChart.Add(balanceChart);
            Console.WriteLine(stats.ToString());
        }

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

    private List<Ohlcv> GenereateTicks(Ohlcv ohlcv, int numTicks = 4)
    {
        decimal tickHigh = ohlcv.Open;
        decimal tickLow = ohlcv.Open;
        int tickVolume = (int)(ohlcv.Volume / numTicks);

        List<Ohlcv> ticks = new();
        for (int j = 0; j < numTicks - 1; j++)
        {
            decimal rnd = (decimal)new Random().NextDouble();

            Ohlcv tick = ohlcv;
            tick.Close = rnd * (ohlcv.High - ohlcv.Low) + ohlcv.Low;
            tick.High = tickHigh = tick.Close > tickHigh ? tick.Close : tickHigh;
            tick.Low = tickLow = tick.Close < tickLow ? tick.Close : tickLow;
            tick.Volume = tickVolume;
            ticks.Add(tick);
        }

        ticks.Add(ohlcv);

        return ticks;
    }
}

/*
internal class TestCommand : Command
{
    private readonly StrategyBase _sb;
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
        //_sb.Plugins = new();
        _sb.Indicators = new();
        _sb.Registration();
        _sb.BalanceService = new();
        //_sb.OrderService = new();

        string json = new StreamReader("Properties/botconfig.json").ReadToEnd();
        _sb.botConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        IBroker broker = GetBroker(_sb.botConfig["Broker"].ToString());
        Interval interval = GetInterval(_sb.botConfig["Interval"].ToString());

        Datacube dc = CreateDatacube(ticker, broker);
        ohlcvData = broker.GetOhlcvData(ticker, interval, days, gap).Result;

        ohlcvData.ForEach(ohlcv =>
        {
            dc.Chart.Data.Add(ohlcv.ToObject());
            StrategyBase.lastCandle = ohlcv;

            _sb.OnCandle(ohlcv);

            List<Ohlcv> ticks = GenereateTicks(ohlcv);
            ticks.ForEach(tick => {
                _sb.OnTick(tick);
            });

            //_sb.Plugins.ForEach(x => x.OnCandle(ohlcv));

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

        var orders = OrderService.GetOrders();
        if (orders.Count > 0)
        {
            //Chart ordersChart = orderService.GetChart();
            //dc.OnChart.Add(ordersChart);

            var stats = _sb.BalanceService.GetStats(orders);
            Chart balanceChart = _sb.BalanceService.GetChart();
            dc.OffChart.Add(balanceChart);
            Console.WriteLine(stats.ToString());
        }

        dc.Settings = new Settings { RangeFrom = 0, RangeTo = ohlcvData.Count };
        dc.Save().Wait();
    }

    private List<Ohlcv> GenereateTicks(Ohlcv ohlcv, int numTicks = 5)
    {
        decimal tickHigh = ohlcv.Open;
        decimal tickLow = ohlcv.Open;
        int tickVolume = (int)(ohlcv.Volume / numTicks);

        List<Ohlcv> ticks = new ();
        for (int j = 0; j < numTicks - 1; j++)
        {
            decimal rnd = (decimal)new Random().NextDouble();

            Ohlcv tick = ohlcv;
            tick.Close = rnd * (ohlcv.High - ohlcv.Low) + ohlcv.Low;
            tick.High = tickHigh = tick.Close > tickHigh ? tick.Close : tickHigh;
            tick.Low = tickLow = tick.Close < tickLow ? tick.Close : tickLow;
            tick.Volume = tickVolume;
            ticks.Add(tick);
        }

        ticks.Add(ohlcv);

        return ticks;
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
*/