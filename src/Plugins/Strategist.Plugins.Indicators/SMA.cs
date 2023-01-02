namespace Strategist.Plugins.Indicators
{
    public class SMA
    {
        private readonly int _period;
        private readonly decimal[] _values;

        private int _index = 0;
        private decimal _sum = 0;
        //private bool visual = false;

        public SMA(int period)
        {
            if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period), "Must be greater than 0");

            _period = period;
            _values = new decimal[period];
        }

        public decimal Next(decimal next)
        {
            // calculate the new sum
            _sum = _sum - _values[_index] + next;

            // overwrite the old value with the new one
            _values[_index] = next;

            // increment the index (wrapping back to 0)
            _index = (_index + 1) % _period;
            //if (_index == 0)
            //    visual = true;

            // calculate the average
            // return visual ? _sum / _period : 0;
            return _sum / _period;
        }
    }
}