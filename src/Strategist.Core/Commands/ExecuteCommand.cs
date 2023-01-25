using Strategist.Core.Transports;
using System.CommandLine;
using System.Reflection.Metadata;

namespace Strategist.Core.Commands;

internal class ExecuteCommand : Command
{
    //private readonly StrategyBase _sb;

    public ExecuteCommand(StrategyBase sb)
        : base("execute", "Run a strategy")
    {
        //_sb = sb;

        this.SetHandler(Handle);
    }

    private void Handle()
    {
        StrategyBase.Broker.SubscribeToTicksAsync();
    }
}
