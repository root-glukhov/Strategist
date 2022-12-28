using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Strategist.Core.Commands;
using Strategist.Core.Interfaces;
using Strategist.Core.Models;
using System.CommandLine;

namespace Strategist.Core;

public abstract class StrategyBase
{
    internal Dictionary<string, object> botOptions;

    public List<Indicator> Indicators = new();

    public virtual void GetIndicators() { }
    public abstract void OnCandle(Ohlcv c);
    public Ohlcv lastCandle { get; set; }

    internal OrderService OrderService { get; set; }

    public StrategyBase()
    {
        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        var rootCommand = new RootCommand("Strategist application");
        rootCommand.AddCommand(TestingCommand());
        rootCommand.AddCommand(ReportCommand());
        rootCommand.Invoke(args);
    }

    #region CLI Commands 

    Command TestingCommand()
    {
        var tickerOption = new Option<string>(new[] { "--ticker", "-t" }, "Tool for work");
        var daysOption = new Option<int>(new[] { "--days", "-d" }, "Number of days to download history, if any");
        var gapOption = new Option<int>(new[] { "--gap", "-g" }, "How many days to deviate from today before starting the history request");

        var testingCmd = new Command("testing", "Strategy testing module")
        {
            tickerOption,
            daysOption,
            gapOption
        };

        testingCmd.SetHandler((ticker, days, gap) => {
            //IConfigurationRoot? config = new ConfigurationBuilder()
            //.AddJsonFile($"Configuration/botoptions.json")
            //.Build();

            using StreamReader r = new StreamReader("Configuration/botoptions.json");
            string json = r.ReadToEnd();
            r.Dispose();

            //string a = config.ToString();
            botOptions = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            IBroker exchange = GetBroker(botOptions["Broker"].ToString());
            Interval interval = GetInterval(botOptions["Interval"].ToString());

            new Testing(this, exchange, ticker, interval, days, gap);
        },
        tickerOption, daysOption, gapOption);

        return testingCmd;
    }

    Command ReportCommand()
    {
        var reportCmd = new Command("report", "Read and display the file.");
        reportCmd.SetHandler(() => new Commands.Report());

        return reportCmd;
    }

    #endregion

    #region Orders 

    public Order CreateOrder(OpenType openType) => OrderService.CreateOrder(openType);
    public void CloseOrder(Order order) => OrderService.CloseOrder(order);

    #endregion

    private IBroker GetBroker(string broker)
    {
        switch (broker.ToLower())
        {
            case "binance":
                return new Exchanges.Binance();
            default:
                throw new NotSupportedException();
        }
    }

    private Interval GetInterval(string interval)
    {
        switch (interval.ToLower()) {
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
}