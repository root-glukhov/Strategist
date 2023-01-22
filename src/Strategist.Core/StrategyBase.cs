using Newtonsoft.Json;
using Strategist.Common;
using Strategist.Core.Commands;
using Strategist.Core.Transports;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Strategist.Core;

public abstract class StrategyBase
{
    #region Static fields 

    public static Dictionary<string, object> BotConfig;
    public static Ohlcv[] Candles = new Ohlcv[10];

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
        string json = new StreamReader("Properties/botconfig.json").ReadToEnd();
        BotConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)!;

        RootCommand root = new();
        root.AddCommand(new ExecuteCommand(this));
        root.AddCommand(new TestCommand(this));

        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();
        root.Invoke(args);
    }

    #endregion

    #region Internal functions

    internal ITransport GetBroker()
    {
        string brokerString = BotConfig["Broker"].ToString()!;

        return brokerString.ToLower() switch {
            "binance" => new BinanceTransport(this),
            _ => throw new ArgumentOutOfRangeException("'Broker' parameter not specified in botconfig.json"),
        };
    }

    internal void AddCandle(Ohlcv ohlcv)
    {
        Array.Copy(Candles, 0, Candles, 1, Candles.Length - 1);
        Candles[0] = ohlcv;
    }

    #endregion
}