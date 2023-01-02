namespace Strategist.Domain;

internal class Stats
{
    public decimal StartBalance;
    public decimal Balance;
    public decimal MaxBalance;
    public decimal MinBalance;
    public decimal Profit;
    public int Long;
    public int LongRight;
    public int Short;
    public int ShortRight;
    //public int CandlesHandled;

    public override string ToString()
    {
        return $"StartBalance:      {StartBalance}\n" +
               $"Balance:           {Balance}\n" +
               $"MaxBalance:        {MaxBalance}\n" +
               $"MinBalance:        {MinBalance}\n" +
               $"Profit:            {Profit}\n" +
               $"Long:              {Long}\n" +
               $"LongRight:         {LongRight}\n" +
               $"Short:             {Short}\n" +
               $"ShortRight:        {ShortRight}\n";
    }
}
