using Microsoft.Extensions.Configuration;
using StrategistCore.Enums;
using System.CommandLine;
using System.Diagnostics;

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

        var testingArgument = new Argument<string?>("testing", () => { return null; }, "Strategy testing module");

        var tickerOption = new Option<string>(new[] { "--ticker", "-t" }, "Tool for work");
        var daysOption = new Option<int>(new[] { "--days", "-d" }, "Number of days to download history, if any");
        var gapOption = new Option<int>(new[] { "--gap", "-g" }, "How many days to deviate from today before starting the history request");

        RootCommand cmd = new() {
            testingArgument,
            tickerOption,
            daysOption,
            gapOption,
        };

        cmd.SetHandler((ticker, days, gap) =>
        {
            Ticker = ticker;
            Days = days;
            Gap = gap;
        },
        tickerOption, daysOption, gapOption);

        cmd.Invoke(args);
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
