using Microsoft.Extensions.Configuration;
using Strategist.Core.Commands;
using Strategist.Core.Interfaces;
using Strategist.Core.Models;
using System.CommandLine;

namespace Strategist.Core;

public abstract class StrategyBase
{
    public List<Indicator> Indicators = new();

    public virtual void GetIndicators() { }
    public abstract void OnCandle(Ohlcv c);

    public StrategyBase()
    {
        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        // Root
        var rootCommand = new RootCommand("Strategist application");
        rootCommand.AddCommand(TestingCommand());
        rootCommand.AddCommand(ReportCommand());
        rootCommand.Invoke(args);
    }

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
            IExchange exchange;
            Interval interval;
            GetJsonFilesArgs(out exchange, out interval);

            new Testing(this, exchange, ticker, interval, days, gap);
        },
        tickerOption, daysOption, gapOption);

        return testingCmd;
    }

    Command ReportCommand()
    {
        var reportCmd = new Command("report", "Read and display the file.");
        reportCmd.SetHandler(() => new Report());

        return reportCmd;
    }

    void GetJsonFilesArgs(out IExchange exchange, out Interval interval)
    {
        IConfigurationRoot? config = new ConfigurationBuilder()
            .AddJsonFile($"Configuration/botoptions.json")
            .Build();

        string exchaneName = config.GetSection("Exchange").Value!;
        exchange = CreateExchange(exchaneName);
        Enum.TryParse(config.GetSection("Interval").Value!, true, out interval);
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