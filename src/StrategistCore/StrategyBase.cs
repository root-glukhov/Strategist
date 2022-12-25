using CryptoExchange.Net.CommonObjects;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StrategistCore.Enums;
using StrategistCore.Models;
using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;

namespace StrategistCore;

public abstract class StrategyBase
{

    //ConfigArguments config;

    public virtual void GetIndicators() { }
    public abstract void OnCandle(Ohlcv c);

    public List<Indicator> Indicators = new();
    

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
            IExchange exchange;
            Interval interval;
            GetJsonFilesArgs(out exchange, out interval);

            // Init Datacube
            Datacube dc = new Datacube();
            dc.Chart.Name = $"{ticker}";
            dc.Chart.Type = "Candles";

            List<Ohlcv> ohlcvData = exchange.GetOhlcvData(ticker, interval, days, gap).Result;

            GetIndicators();

            foreach (var c in ohlcvData)
            {
                OnCandle(c);

                dc.Chart.Data.Add(new object[] { c.Timestamp, c.Open, c.High, c.Low, c.Close, c.Volume });

                foreach (var indicator in Indicators)
                {
                    foreach (var figure in indicator.Figures)
                    {
                        figure.AddValue(figure.GetValue());
                    }
                }
            }

            // Console.WriteLine(JsonConvert.SerializeObject(Indicators));

            // Indicators to Datacube
            foreach (var idctr in Indicators)
            {
                if(idctr.InChart)
                {
                    var chart = new Chart();
                    chart.Name = idctr.Name;
                    chart.Type = "Indicators";

                    List<object[]> data = new();

                    foreach (var dcD in dc.Chart.Data)
                    {
                        var obj = new object[idctr.Figures.Length + 1];
                        obj[0] = dcD[0];
                        for(int i = 1; i < idctr.Figures.Length; i++)
                        {
                            obj[i] = idctr.Figures[i - 1];
                        }
                        data.Add(obj);
                    }

                    chart.Data = data;
                    dc.OnChart.Add(chart);
                }
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
