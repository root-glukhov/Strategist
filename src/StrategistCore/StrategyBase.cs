using CryptoExchange.Net.CommonObjects;
using Microsoft.Extensions.Configuration;
using StrategistCore.Enums;
using StrategistCore.Models;
using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;

namespace StrategistCore;

public abstract class StrategyBase
{

    //ConfigArguments config;

    public abstract void OnCandle(Ohlcv c);

    List<Indicator> indicators = new List<Indicator>();
    public abstract List<Indicator> GetIndicators(List<Indicator> indicators);

    public StrategyBase()
    {
        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        // Root
        var rootCommand = new RootCommand("Sample app for System.CommandLine");
        rootCommand.AddCommand(TestingCommand());
        rootCommand.AddCommand(ReportCommand());
        rootCommand.Invoke(args);
    }

    Command ReportCommand()
    {
        var reportCmd = new Command("report", "Read and display the file.");
        reportCmd.SetHandler(() => {
            try
            {
                Process.Start("Strategist.Plugins.Report.exe");
                Process.Start("cmd", "/C start http://localhost:5000/");
            }
            catch
            {
                Console.WriteLine("Для вызова команды 'report' установите пакет Strategist.Plugins.Report");
            }
        });

        return reportCmd;
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
            // Init Datacube
            Datacube dc = new Datacube();
            dc.Chart.Name = ticker;
            dc.Chart.Type = "Candles";

            dc.OnChart[0].Name = "SMA";

            IExchange exchange;
            Interval interval;
            GetJsonFilesArgs(out exchange, out interval);

            List<Ohlcv> ohlcvData = exchange.GetOhlcvData(ticker, interval, days, gap).Result;
            foreach (var c in ohlcvData)
            {
                OnCandle(c);

                dc.Chart.Data.Add(new decimal[] { c.Timestamp, c.Open, c.High, c.Low, c.Close, c.Volume });
                var indic = GetIndicators(indicators);
                

            }

            dc.Save().Wait();
        },
        tickerOption, daysOption, gapOption);

        return testingCmd;
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
