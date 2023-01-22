using Binance.Net.Enums;
using Strategist.Common;

namespace Strategist.Core.Extensions;

public static class StringExtensions
{
    public static Interval ToInterval(this string str)
    {
        return str.ToLower() switch
        {
            "1m" => Interval.OneMinute,
            "3m" => Interval.ThreeMinutes,
            "5m" => Interval.FiveMinutes,
            "15m" => Interval.FifteenMinutes,
            "30m" => Interval.ThirtyMinutes,
            "1h" => Interval.OneHour,
            "4h" => Interval.FourHour,
            _ => throw new ArgumentOutOfRangeException(nameof(str)),
        };
    }

    public static KlineInterval ToKlineInterval(this string str)
    {
        return str.ToLower() switch
        {
            "1m" => KlineInterval.OneMinute,
            "3m" => KlineInterval.ThreeMinutes,
            "5m" => KlineInterval.FiveMinutes,
            "15m" => KlineInterval.FifteenMinutes,
            "30m" => KlineInterval.ThirtyMinutes,
            "1h" => KlineInterval.OneHour,
            "4h" => KlineInterval.FourHour,
            _ => throw new ArgumentOutOfRangeException(nameof(str)),
        };
    }
}
