using System.Linq;

namespace StockPlot.Indicators.Indicators
{
    public sealed class RagheeWave : IndicatorBase
    {
        public XYSerie Upper { get; } = new XYSerie("Upper");
        public XYSerie Lower { get; } = new XYSerie("Lower");

        public RagheeWave()
        {
            Name = "Raghee Wave";
            AddFill("Lower", "Upper");
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            var highValues = new double[total];
            var lowValues = new double[total];

            for (int i = 0; i < total; i++)
            {
                highValues[i] = high.GetEMA(i, 34, i == 0 ? high[i] : highValues[i - 1]);
                lowValues[i] = low.GetEMA(i, 34, i == 0 ? low[i] : lowValues[i - 1]);

                Upper.Append((time[i], highValues[i]));
                Lower.Append((time[i], lowValues[i]));
            }
        }
    }
}
