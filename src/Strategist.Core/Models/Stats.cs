namespace Strategist.Core.Models;

internal class Stats
{
    public float StartBalance { get; set; }
    public float MaxBalance { get; set; }
    public float MinBalance { get; set; }
    public float Balance { get; set; }
    public float PercentProfit { get; set; }

    public int OrdersCount { get; set; }
    public int Long { get; set; }
    public int LongRight { get; set; }
    public int Short { get; set; }
    public int ShortRight { get; set; }

    public Stats(float balance)
    {
        StartBalance = balance;
        MaxBalance = balance;
        MinBalance = balance;
        Balance = balance;
    }

    public override string ToString() =>
        $"StartBalance: {StartBalance}\n"
        + $"MaxBalance:   {MaxBalance}\n"
        + $"MinBalance:   {MinBalance}\n"
        + $"Balance:      {Balance} ({PercentProfit:0.00}%)\n\n"
        + $"OrdersCount:  {OrdersCount}\n"
        + $"Long:         {Long}\n"
        + $"LongRight:    {LongRight}\n"
        + $"Short:        {Short}\n"
        + $"ShortRight:   {ShortRight}";
}
