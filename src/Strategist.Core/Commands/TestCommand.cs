using Binance.Net.Enums;
using Strategist.Common;
using Strategist.Core.Extensions;
using Strategist.Core.Models;
using Strategist.Core.Services;
using Strategist.Core.Transports;
using System.CommandLine;

namespace Strategist.Core.Commands;

internal class TestCommand : Command
{
    private readonly StrategyBase _sb;

    public TestCommand(StrategyBase sb)
           : base("test", "Run strategy backtest")
    {
        _sb = sb;

        #region Parameters initialization

        var daysOption = new Option<int>("--days")
        {
            Name = "days",
            Description = "Number of days for the test"
        };
        AddOption(daysOption);

        var gapOption = new Option<int>("--gap")
        {
            Name = "gap",
            Description = "Skip number of days"
        };
        AddOption(gapOption);

        #endregion

        this.SetHandler(Handle, daysOption, gapOption);
    }

    private async void Handle(int days = 1, int gap = 0)
    {
        ITransport broker = StrategyBase.Broker;
        string ticker = StrategyBase.BotConfig["Ticker"].ToString()!;
        string intervalString = StrategyBase.BotConfig["Interval"].ToString()!;

        List<Ohlcv> historyData = await StrategyBase.Broker.GetHistoryAsync(ticker, intervalString, days, gap);

        historyData.ForEach(ohlcv => {
            StrategyBase.AddCandle(ohlcv);

            List<Ohlcv> ticks = GenerateTicks(ohlcv);
            ticks.ForEach(tick => {
                _sb.OnTick(tick);

                if (ticks.Last() == tick)
                    _sb.OnCandle(ohlcv);
            });
        });

        OrderService.CloseAll();
        Stats stats = StatService.GetStats();
        Console.WriteLine("\n" + stats.ToString());
    }

    #region Private functions

    private static List<Ohlcv> GenerateTicks(Ohlcv ohlcv, int numTicks = 4)
    {
        List<Ohlcv> ticks = new();
        Random random = new();

        #region Generate ticks

        decimal tickHigh = ohlcv.Open;
        decimal tickLow = ohlcv.Open;

        for (int i = 0; i < numTicks - 1; i++)
        {
            decimal priceRnd = (decimal)random.NextDouble();

            Ohlcv tick = (Ohlcv)ohlcv.Clone();
            tick.Close = priceRnd * (ohlcv.High - ohlcv.Low) + ohlcv.Low;

            tickHigh = tick.Close > tickHigh ? tick.Close : tickHigh;
            tickLow = tick.Close < tickLow ? tick.Close : tickLow;

            tick.High = tickHigh;
            tick.Low = tickLow;

            tick.Volume = 0;
            ticks.Add(tick);
        }

        Ohlcv lastTick = (Ohlcv)ohlcv.Clone();
        lastTick.Volume = 0;
        ticks.Add(lastTick);

        #endregion

        #region Generate volume

        decimal totalVolume = ohlcv.Volume;
        decimal[] parts = new decimal[ticks.Count];
        decimal sum = 0;
        decimal curVal = (decimal)random.NextDouble() * totalVolume / ticks.Count;

        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = curVal;
            sum += curVal;
            curVal *= 1.5m;
        }

        for (int i = 0; i < ticks.Count; i++)
        {
            parts[i] = parts[i] / sum * totalVolume;
            ticks[i].Volume = parts[i];
        }

        #endregion

        return ticks;
    }

    #endregion
}