using System.Drawing;

namespace StockPlot.Indicators.Indicators
{
    public sealed class BollingerBands : IndicatorBase
    {
        public XYSerie Middle { get; } = new XYSerie("Middle") { DefaultColor = Color.Blue };

        public XYSerie Up { get; private set; } = new XYSerie("Up");

        public XYSerie Down { get; private set; } = new XYSerie("Down");

        [IndicatorParameter]
        public int Period { get; set; } = 20;

        [IndicatorParameter]
        public double Deviation { get; set; } = 2.0;

        public override void Init()
        {
            this.Name = $"Bollinger Bands [{Period}]";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            for (int i = 0; i < total; i++)
            {
                var array = this.Middle.Select(x => x.Value).ToArray();
                var std = close.GetStdDev(i, Period, array);
                var ma = close.GetSMA(i, Period);

                this.Middle.Append((time[i], ma));
                this.Down.Append((time[i], ma - (Deviation * std)));
                this.Up.Append((time[i], ma + (Deviation * std)));
            }
        }
    }
}
