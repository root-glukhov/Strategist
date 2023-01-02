using Strategist.Core.Commands;
using Strategist.Domain;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Strategist.Core;

public abstract class StrategyBase
{
    public List<Indicator> Indicators = new();

    public virtual void GetIndicators() { }
    public abstract void OnCandle(Ohlcv c);

    public StrategyBase()
    {
        RootCommand root = new RootCommand();
        root.AddCommand(new TestCommand(this));
        root.AddCommand(new ReportCommand());

        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();
        root.Invoke(args);
    }
}