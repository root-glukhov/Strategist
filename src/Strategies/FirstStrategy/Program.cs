using StrategistCore;
using StrategistCore.Models;

Strategy _strategy = new Strategy();

Console.WriteLine("Done.");
Console.Read();

public class Strategy : StrategyBase
{
    int counter = 0;

    public override List<Indicator> GetIndicators(List<Indicator> indicators) {
        indicators.Add(new Indicator()
        {
            Name = "SMA",
            Figures = new[]
            {
                new Figure()
                {
                    Name = "fast",
                    Value = 12
                },
                new Figure() {
                    Name = "slow",
                    Value = counter
                }
            },
            InChart = true
        });

        return indicators;
    }

    public override void OnCandle(Ohlcv c)
    {
        Console.WriteLine("{0} {1}", counter, c.ToString());
        counter++;
    }
}
