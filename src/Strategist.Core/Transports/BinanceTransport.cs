using Binance.Net.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Configuration;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Strategist.Common;
using Spectre.Console;
using Strategist.Core.Extensions;

namespace Strategist.Core.Transports;

internal class BinanceTransport : ITransport
{
    private readonly StrategyBase _sb;
    private BinanceClient Client { get; set; }
    private BinanceSocketClient SocketClient { get; set; }

    public BinanceTransport(StrategyBase sb)
    {
        _sb = sb;

        IConfigurationRoot _config = new ConfigurationBuilder()
            .AddJsonFile($"Properties/secrets.json")
            .Build();

        string apiKey = _config.GetSection("Binance:ApiKey").Value!;
        string apiSecret = _config.GetSection("Binance:ApiSecret").Value!;

        Client = new BinanceClient(new BinanceClientOptions {
            ApiCredentials = new ApiCredentials(apiKey, apiSecret)
        });

        SocketClient = new BinanceSocketClient(new BinanceSocketClientOptions {
            ApiCredentials = new ApiCredentials(apiKey, apiSecret)
        });
    }

    public async void GetTicks()
    {
        await SocketClient.UsdFuturesStreams
            .SubscribeToKlineUpdatesAsync("BTCUSDT", StrategyBase.BotConfig["Interval"].ToString()!.ToKlineInterval(), (dataEvent) =>
            {
                IBinanceStreamKline kline = dataEvent.Data.Data;
                Ohlcv ohlcv = kline.ToOhlcv();

                _sb.OnTick(ohlcv);

                if (kline.Final)
                    _sb.OnCandle(ohlcv);
            });
    }

    public async Task<List<Ohlcv>> GetHistoryAsync(string ticker, int days = 1, int gap = 0)
    {
        string intervalString = StrategyBase.BotConfig["Interval"].ToString()!;
        Interval interval = intervalString.ToInterval();

        DateTime utcNow = DateTime.UtcNow;
        DateTime? startTime = days > 0 ? utcNow.AddDays(-days - gap) : null;
        DateTime? endTime = gap > 0 ? utcNow.AddDays(-gap) :
            utcNow.AddTicks(
                -(utcNow.Ticks % TimeSpan.FromSeconds((double)interval).Ticks));

        List<IBinanceKline> klinesData = new();

        await AnsiConsole.Progress()
            .Columns(new ProgressColumn[] {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn(),
            })
            .StartAsync(async ctx => {
                ProgressTask task = ctx.AddTask("Load history", maxValue: days);

                do
                {
                    var klineChunk = await Client.UsdFuturesApi.ExchangeData.GetKlinesAsync(
                        ticker, (KlineInterval)interval, startTime, endTime);
                    klinesData.AddRange(klineChunk.Data);
                    task.Increment(5.2);
                    startTime = klinesData.Last().CloseTime;
                }
                while (startTime < endTime);
            });

        List<Ohlcv> ohlcvData = new();
        klinesData.ForEach(kline => ohlcvData.Add(kline.ToOhlcv()));
        return ohlcvData;
    }
}