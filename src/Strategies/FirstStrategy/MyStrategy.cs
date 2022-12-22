using StrategistCore.Models;
using StrategistCore;

public class MyStrategy : StrategyBase
{
    decimal fast = 0;
    decimal slow = 0;

    public override List<Indicator> GetIndicators(List<Indicator> indicators)
    {
        indicators.Add(new Indicator()
        {
            Name = "SMA",
            Figures = new[]
            {
                new Figure()
                {
                    Name = "fast",
                    Value = fast
                },
                new Figure() {
                    Name = "slow",
                    Value = slow
                }
            },
            InChart = true
        });

        return indicators;
    }

    public override void OnCandle(Ohlcv c)
    {
        fast = c.High + 5;
        slow = c.Low - 5;
    }
}