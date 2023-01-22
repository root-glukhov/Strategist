namespace Strategist.Core.Extensions;

public static class DataTimeExtensions
{
    public static long ToTimestamp(this DateTime dateTime)
    {
        long timestamp = (long)(TimeZoneInfo.ConvertTimeToUtc(dateTime)
            - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        return timestamp * 1000;
    }
}
