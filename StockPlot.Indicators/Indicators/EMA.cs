using System.Linq;

namespace StockPlot.Indicators.Indicators
{
    public sealed class ExponentialMovingAverage : IndicatorBase
    {
        [IndicatorParameter]
        public int Period { get; set; } = 14;
         
        public XYSerie Ma { get; } = new XYSerie("EMA");

        public override void Init()
        {
            Name = $"Exponential Moving Average [{Period}]";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            for (int i = 0; i < total; i++)
            {
                Ma.Append((time[i],close.GetEMA(i, Period, i == 0 ? close[i] : Ma[i - 1].Item2)));
            }
        }
    }
}
