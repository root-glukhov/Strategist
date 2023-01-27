using Strategist.Common;
using Strategist.Core.Services;
using Strategist.Core.Transports;
using System.CommandLine;
using System.Reflection.Metadata;

namespace Strategist.Core.Commands;

internal class ExecuteCommand : Command
{
    private readonly StrategyBase _sb;

    public ExecuteCommand(StrategyBase sb)
        : base("execute", "Run a strategy")
    {
        _sb = sb;

        this.SetHandler(Handle);
    }

    private async void Handle()
    {
        ITransport broker = StrategyBase.Broker;
        string ticker = StrategyBase.BotConfig["Ticker"].ToString()!;
        string intervalStr = StrategyBase.BotConfig["Interval"].ToString()!;

        // Get one day history for calculate indicators
        OrderService.OrdersAllowed = false;
        IEnumerable<Ohlcv> ohlcvData = await broker.GetHistoryAsync(ticker, intervalStr);
        foreach (var ohlcv in ohlcvData)
        {
            StrategyBase.AddCandle(ohlcv);
            _sb.OnCandle(ohlcv);
        }

        OrderService.OrdersAllowed = true;
        await broker.SubscribeToTicksAsync(ticker, intervalStr);
    }
}
