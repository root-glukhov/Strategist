using Newtonsoft.Json;
using Strategist.Common;
using Strategist.Core.Commands;
using Strategist.Core.Models;
using Strategist.Core.Services;
using Strategist.Core.Transports;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Strategist.Core;

public abstract class StrategyBase
{
    #region Static fields 

    internal static Dictionary<string, object> BotConfig;
    internal static ITransport Broker;
    private static readonly Ohlcv[] Candles = new Ohlcv[10];
    #endregion


    #region Virtual methods

    public virtual void OnTick(Ohlcv ohlcv) { }
    public virtual void OnCandle(Ohlcv ohlcv) { }
    public virtual void OnCreateOrder(Order order) { }
    public virtual void OnClosedOrder(Order order) { }

    #endregion
    
    #region Ctor

    public StrategyBase()
    {
        Console.CancelKeyPress += Console_CancelKeyPress!;
        Task.Run(() => Console.ReadLine());

        string json = new StreamReader("Properties/botconfig.json").ReadToEnd();
        BotConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)!;

        Broker = GetBroker();
        OrderService._sb = this;

        RootCommand root = new();
        root.AddCommand(new ExecuteCommand(this));
        root.AddCommand(new TestCommand(this));

        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();
        root.Invoke(args);
    }

    #endregion

    #region Functions

    private ITransport GetBroker()
    {
        string brokerString = BotConfig["Broker"].ToString()!;

        return brokerString.ToLower() switch
        {
            "binance" => new BinanceTransport(this),
            _ => throw new ArgumentOutOfRangeException("'Broker' parameter not specified in botconfig.json"),
        };
    }

    #endregion

    #region Static functions

    public static object GetConfig(string param) => BotConfig[param];
    public static Ohlcv GetCandle(int index) => Candles[index];
    internal static void AddCandle(Ohlcv ohlcv)
    {
        Array.Copy(Candles, 0, Candles, 1, Candles.Length - 1);
        Candles[0] = ohlcv;
    }
    private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;

        OrderService.CloseAll();
        Broker.UnsubscribeAllAsync();

        Stats stats = StatService.GetStats();
        Console.WriteLine(stats);

        Console.ReadLine();
        Environment.Exit(0);
    }

    #endregion
}