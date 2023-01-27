using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Strategist.Plugins.Indicators;

public class SMA
{
    private readonly int _period;
    private readonly decimal[] values;
    private int index = 0;
    private decimal sum = 0;

    public SMA(int period)
    {
        if (period <= 0) 
            throw new ArgumentOutOfRangeException(nameof(period), "Must be greater than 0");

        _period = period;
        values = new decimal[_period];
    }

    public decimal GetValues(int i)
    {
        if (i < 0 || i > _period)
            throw new ArgumentOutOfRangeException(nameof(i));

        return values[^i];
    }

    public decimal Next(decimal next)
    {
        sum = sum - values[index] + next;
        values[index] = next;
        index = (index + 1) % _period;

        if (values[index] == 0)
            Next(next);

        return sum / _period;
    }

    public decimal Moment(decimal value) => (sum - values[index] + value) / _period;
}