using Strategist.Core.Models;
using Strategist.Core;

public class MyStrategy : StrategyBase
{
    int counter = 0;

    decimal? fast;
    decimal? slow;

    SMA sma = new SMA(20);

    public MyStrategy()
    {

    }

    public override void GetIndicators()
    {
        Indicators.Add(new Indicator()
        {
            Name = "SMA",
            Figures = new[]
            {
                new Figure()
                {
                    Name = "fast",
                    GetValue = () => fast
                }
            },
            InChart = true
        });
    }

    public override void OnCandle(Ohlcv c)
    {
        fast = sma.Update(c.Close);

        Console.WriteLine(c.ToString());
    }
}

public class SMA
{
    private readonly int _period;
    private readonly decimal[] _values;

    private int _index = 0;
    private decimal _sum = 0;
    private bool visual = false;

    public SMA(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period), "Must be greater than 0");

        _period = period;
        _values = new decimal[period];
    }

    public decimal? Update(decimal nextInput)
    {
        // calculate the new sum
        _sum = _sum - _values[_index] + nextInput;

        // overwrite the old value with the new one
        _values[_index] = nextInput;

        // increment the index (wrapping back to 0)
        _index = (_index + 1) % _period;
        if (_index == 0)
            visual = true;

        // calculate the average
        return visual ? _sum / _period : null;
    }
}