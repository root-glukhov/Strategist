﻿using Binance.Net.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Configuration;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Strategist.Common;
using Spectre.Console;
using Strategist.Core.Extensions;
using System.Threading.Tasks;

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

    public async Task<IEnumerable<Ohlcv>> GetHistoryAsync(string ticker, string intervalStr, int days = 1, int gap = 0)
    {
        KlineInterval klineInterval = intervalStr.ToKlineInterval();

        DateTime utcNow = DateTime.UtcNow;
        DateTime? startTime = days > 0 ? utcNow.AddDays(-days - gap) : null;
        DateTime? endTime = gap > 0 ? utcNow.AddDays(-gap) :
            utcNow.AddTicks(
                -(utcNow.Ticks % TimeSpan.FromSeconds((double)klineInterval).Ticks));

        List<IBinanceKline> klinesData = new();

        await AnsiConsole.Progress()
            .Columns(new ProgressColumn[] {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(),
            })
            .StartAsync(async ctx => {
                int klineCount = 0;
                int klineIntervalInt = (int)klineInterval;

                for (int i = 0; i < days; i++)
                    klineCount += 86400 / klineIntervalInt; // 86400 seconds in one day

                ProgressTask task = ctx.AddTask($"Load {klineCount}({intervalStr}) klines", maxValue: klineCount);

                do
                {
                    var webCallResult = await Client.UsdFuturesApi.ExchangeData.GetKlinesAsync(
                            ticker, klineInterval, startTime, endTime);
                    IEnumerable<IBinanceKline> klineChunk = webCallResult.Data;

                    klinesData.AddRange(klineChunk);
                    task.Increment((double)klineChunk.Count());
                    startTime = klinesData.Last().CloseTime;
                }
                while (startTime < endTime);
            });

        List<Ohlcv> ohlcvData = new();
        klinesData.ForEach(kline => ohlcvData.Add(kline.ToOhlcv()));
        return ohlcvData;
    }

    public async Task SubscribeToTicksAsync(string ticker, string intervalStr)
    {
        KlineInterval klineInterval = intervalStr.ToKlineInterval();

        await SocketClient.UsdFuturesStreams
            .SubscribeToKlineUpdatesAsync(ticker, klineInterval, (dataEvent) =>
            {
                IBinanceStreamKline kline = dataEvent.Data.Data;
                Ohlcv ohlcv = kline.ToOhlcv();
                StrategyBase.AddCandle(ohlcv);

                _sb.OnTick(ohlcv);

                if (kline.Final)
                    _sb.OnCandle(ohlcv);
            });
    }

    public async Task UnsubscribeAllAsync() => await SocketClient.UnsubscribeAllAsync();

    
    //public async Task<Order> PlaceOrderAsync(Order order)
    //{
    //    //var placedOrderCall = await Client.UsdFuturesApi.Trading.PlaceOrderAsync("BTCUSDT", OrderSide.Buy, FuturesOrderType.Market, 0.001m);
    //    //BinanceFuturesPlacedOrder bfpOrder = placedOrderCall.Data;

    //    //return new Order()
    //    //{
    //    //    Id = bfpOrder.Id,
    //    //    Status = Common.OrderStatus.Placed,
    //    //    OrderType = order.OrderType,
    //    //    OpenTime = bfpOrder.UpdateTime,
    //    //    OpenTimestamp = bfpOrder.UpdateTime.ToTimestamp(),
    //    //    OpenPrice = bfpOrder.AveragePrice,
    //    //};

    //    throw new NotImplementedException();
    //}
}