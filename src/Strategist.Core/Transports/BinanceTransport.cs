using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Strategist.Domain;

namespace Strategist.Core.Transports;

internal class BinanceTransport : IBroker
{
    private IConfigurationRoot _config { get; set; }
    private BinanceClient _client { get; set; }

    public BinanceTransport()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile($"Properties/secrets.json")
            .Build();

        _client = new BinanceClient(new BinanceClientOptions
        {
            ApiCredentials = new ApiCredentials(
                _config.GetSection("Binance:ApiKey").Value!,
                _config.GetSection("Binance:ApiSecret").Value!),
        });
    }

    public async Task<List<Ohlcv>> GetOhlcvData(string ticker, Interval interval, int days = 0, int gap = 0)
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime? startTime = days > 0 ? utcNow.AddDays(-days - gap) : null;
        DateTime? endTime = gap > 0 ? utcNow.AddDays(-gap) :
            utcNow.AddTicks(
                -(utcNow.Ticks % TimeSpan.FromSeconds((double)interval).Ticks)
            );

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
            ProgressTask task = ctx.AddTask("Load klines", maxValue: days);

            do
            {
                var klineChunk = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync(ticker, (KlineInterval)interval, startTime, endTime);
                klinesData.AddRange(klineChunk.Data);
                task.Increment(5.2);
                startTime = klinesData.Last().CloseTime;
            }
            while (startTime < endTime);
        });

        return KlineToOhlcv(klinesData);
    }

    public static List<Ohlcv> KlineToOhlcv(IEnumerable<IBinanceKline> klines)
    {
        List<Ohlcv> ohlcvList = new();

        foreach (var kline in klines)
        {
            ohlcvList.Add(new Ohlcv()
            {
                Date = kline.OpenTime,
                Timestamp = Ohlcv.DateTimeToTimestamp(kline.OpenTime),
                Open = kline.OpenPrice,
                High = kline.HighPrice,
                Low = kline.LowPrice,
                Close = kline.ClosePrice,
                Volume = kline.Volume,
            });
        }

        return ohlcvList;
    }
}

