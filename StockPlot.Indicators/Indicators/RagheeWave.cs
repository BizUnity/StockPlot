using System.Linq;

namespace StockPlot.Indicators.Indicators
{
    public sealed class RagheeWave : IndicatorBase
    {
        public XYYSerie MainCloud { get; } = new XYYSerie("Wave");

        public RagheeWave()
        {
            Name = "Raghee Wave";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            var highValues = new double[total];
            var lowValues = new double[total];

            for (int i = 0; i < total; i++)
            {
                highValues[i] = high.GetEMA(i, 34, i == 0 ? high[i] : highValues[i - 1]);
                lowValues[i] = low.GetEMA(i, 34, i == 0 ? low[i] : lowValues[i - 1]);

                MainCloud.Append((time[i], highValues[i], lowValues[i]));
            }
        }
    }
}
