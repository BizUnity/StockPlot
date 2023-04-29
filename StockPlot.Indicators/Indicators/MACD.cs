using System.Drawing;

namespace StockPlot.Indicators.Indicators
{
    public sealed class MACD : IndicatorBase
    {

        public XYSerie Histo { get; } = new XYSerie("Histogram") { PlotType = PlotType.Histogram, DefaultColor = Color.Blue };
        public XYSerie Main { get; } = new XYSerie("Main line") { PlotType = PlotType.Line, DefaultColor = Color.IndianRed, Lenght = 2 };
        public XYSerie Signal_ { get; } = new XYSerie("Signal") { PlotType = PlotType.Line, DefaultColor = Color.Green };

        [IndicatorParameter]
        public int Fast { get; set; } = 12;
        [IndicatorParameter]
        public int Slow { get; set; } = 26;
        [IndicatorParameter]
        public int Signal { get; set; } = 9;

        private XYSerie Minus_ = new XYSerie("");
        private ExponentialMovingAverage Slow_;
        private ExponentialMovingAverage Fast_;

        public MACD()
        {
            this.IsExternal = true;
        }

        public override void Init()
        {
            Slow_ = new ExponentialMovingAverage() { Period = Slow };
            Fast_ = new ExponentialMovingAverage() { Period = Fast };

            this.Name = $"MACD [{Fast}, {Slow}, {Signal}]";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            this.Minus_.Clear();
            this.Fast_.Calculate(total, time, open, high, low, close, volume);
            this.Slow_.Calculate(total, time, open, high, low, close, volume);

            for (int i = 0; i < total; i++)
            {
                var slow = this.Slow_.Ma[i];
                var fast = this.Fast_.Ma[i];

                var minus = fast.Item2 - slow.Item2;

                this.Minus_.Append((time[i], minus));

                var signal = Minus_.GetSMA(i, Signal);

                this.Main.Append((time[i], minus));
                this.Signal_.Append((time[i], signal));
                this.Histo.Append((time[i], minus - signal));
            }
        }
    }
}
