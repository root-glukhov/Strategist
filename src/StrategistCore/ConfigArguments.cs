using Microsoft.Extensions.Configuration;
using StrategistCore.Enums;
using StrategistCore.Models;
using System.CommandLine;
using System.Diagnostics;
using System.IO;

namespace StrategistCore;

internal class ConfigArguments
{
    public IExchange Exchange { get; private set; }
    public string Ticker { get; private set; }
    public Interval Interval;
    public int Days { get; private set; }
    public int Gap { get; private set; }

	public ConfigArguments()
	{
        GetCommandLineArgs();
        GetJsonFilesArgs();
    }

    void GetCommandLineArgs()
    {
        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        // Report
        var reportCmd = new Command("report", "Read and display the file.");
        reportCmd.SetHandler(() => {
            try {
                Process.Start("Strategist.Plugins.Report.exe");
            } catch {
                Console.WriteLine("Для вызова команды 'report' установите пакет Strategist.Plugins.Report");
            }
        });

        // Testing
        var tickerOption = new Option<string>(new[] { "--ticker", "-t" }, "Tool for work");
        var daysOption = new Option<int>(new[] { "--days", "-d" }, "Number of days to download history, if any");
        var gapOption = new Option<int>(new[] { "--gap", "-g" }, "How many days to deviate from today before starting the history request");

        var testingCmd = new Command("testing", "Strategy testing module")
        {
            tickerOption,
            daysOption,
            gapOption
        };

        testingCmd.SetHandler((ticker, days, gap) =>
        {
            Ticker = ticker;
            Days = days;
            Gap = gap;
        },
        tickerOption, daysOption, gapOption);

        // Root
        var rootCommand = new RootCommand("Sample app for System.CommandLine");
        rootCommand.AddCommand(reportCmd);
        rootCommand.AddCommand(testingCmd);
        rootCommand.Invoke(args);
    }

    void GetJsonFilesArgs()
    {
        IConfigurationRoot? config = new ConfigurationBuilder()
            .AddJsonFile($"Configuration/botoptions.json")
            .Build();

        string exchaneName = config.GetSection("Exchange").Value!;
        Exchange = CreateExchange(exchaneName);

        //Ticker = config.GetSection("Ticker").Value!;
        Enum.TryParse(config.GetSection("Interval").Value!, true, out Interval);
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
