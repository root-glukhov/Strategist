using Microsoft.Extensions.DependencyInjection;
using Strategist.Core.Commands;
using Strategist.Domain;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Strategist.Core;

public abstract class StrategyBase
{
    #region Static Objects

    internal static Dictionary<string, object> botConfig;
    public static Ohlcv lastCandle;

    #endregion

    #region Virtual Methods

    public virtual void Registration() { }
    public virtual void OnTick(Ohlcv tick) { }
    public virtual void OnCandle(Ohlcv c) { }

    #endregion

    public List<Indicator> Indicators = new();

    public void Start()
    {
        RootCommand root = new RootCommand();
        root.AddCommand(new TestCommand(this));
        root.AddCommand(new ReportCommand());

        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();
        root.Invoke(args);
    }
}