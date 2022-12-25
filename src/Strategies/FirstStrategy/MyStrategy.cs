using StrategistCore.Models;
using StrategistCore;

public class MyStrategy : StrategyBase
{
    decimal fast = 0;
    decimal slow = 0;

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
                },
                new Figure() {
                    Name = "slow",
                    GetValue = () => slow
                }
            },
            InChart = true
        });
    }

    public override void OnCandle(Ohlcv c)
    {
        fast = c.High + 5;
        slow = c.Low - 5;
    }
}