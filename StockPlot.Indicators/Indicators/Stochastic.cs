using System.Drawing;

namespace StockPlot.Indicators.Indicators
{
    public sealed class Stochastic : IndicatorBase
    {
        public XYYSerie Cloud { get; } = new XYYSerie("Stochastic");
        public XYSerie FastSerie { get; } = new XYSerie("Fast") { PlotType = PlotType.Line };
        public XYSerie SlowSerie { get; } = new XYSerie("Slow") { PlotType= PlotType.DashedLine };

        [IndicatorParameter]
        public int K { get; set; } = 5;
        [IndicatorParameter]
        public int Fast { get; set; } = 3;
        [IndicatorParameter]
        public int Slow { get; set; } = 3;

        public Stochastic()
        {
            IsExternal = true;

            CreateLevel(50, Color.White);
            CreateLevel(70, Color.Red);
            CreateLevel(30, Color.LightSeaGreen);
        }

        public override void Init()
        {
            Name = $"Stochastic [{K},{Fast},{Slow}]";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            var sto = new double[total];
            var slow = new double[total];
            var fast = new double[total];

            for (int i = 0; i < total; i++)
            {
                sto[i] = ((close[i] - low.GetLowest(i, K)) / (high.GetHighest(i, K) - low.GetLowest(i, K))) * 100;

                fast[i] = sto.GetSMA(i, Fast);
                slow[i] = fast.GetSMA(i, Slow);

                SlowSerie.Append((time[i],slow[i]));
                FastSerie.Append((time[i], fast[i]));
                Cloud.Append((time[i], slow[i], fast[i]));
            }
        }
    }
}
