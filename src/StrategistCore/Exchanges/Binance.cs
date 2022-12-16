using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Configuration;
using StrategistCore.Enums;
using StrategistCore.Models;

namespace StrategistCore.Exchanges;

internal class Binance : IExchange
{
    private IConfigurationRoot _config { get; set; }
    private BinanceClient _client { get; set; }

	public Binance()
	{
        _config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
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

        var klinesData = new List<IBinanceKline>();

        do
        {
            var klineChunk = await _client.UsdFuturesApi.ExchangeData.GetKlinesAsync(ticker, (KlineInterval)interval, startTime, endTime);
            klinesData.AddRange(klineChunk.Data);
            startTime = klinesData.Last().CloseTime;
        } 
        while (startTime < endTime);

        return Ohlcv.KlineToOhlcv(klinesData);
    }
}
