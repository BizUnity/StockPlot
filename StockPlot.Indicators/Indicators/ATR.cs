using System.Drawing;

namespace StockPlot.Indicators.Indicators
{
    public sealed class ATR : IndicatorBase
    {
        public XYSerie Atr { get; private set; } = new XYSerie("ATR") { DefaultColor = Color.OrangeRed };

        [IndicatorParameter]
        public int Period { get; set; } = 14;

        public ATR()
        {
            this.IsExternal = true;
        }

        public override void Init()
        {
            Name = $"Average True Range (ATR) [{this.Period}]";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            if (total <= 0)
                return;

            var temp = new double[total];
            temp[0] = 0;

            for (int i = 1; i < total; i++)
            {
                var diff1 = Math.Abs(close[i - 1] - high[i]);
                var diff2 = Math.Abs(close[i - 1] - low[i]);
                var diff3 = high[i] - low[i];

                var max = diff1 > diff2 ? diff1 : diff2;
                temp[i] = max > diff3 ? max : diff3;
            }

            for (int i = 0; i < total; i++)
            {
                var atr = temp.GetSMA(i, Period);
                this.Atr.Append((time[i], atr));
            }
        }
    }
}
