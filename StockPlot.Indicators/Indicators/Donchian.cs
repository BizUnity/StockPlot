using System.Drawing;
using System.Linq;

namespace StockPlot.Indicators.Indicators
{
    public sealed class Donchian : IndicatorBase
    {
        public XYSerie Up { get; private set; } = new XYSerie("Up") { DefaultColor = Color.Green };

        public XYSerie Down { get; private set; } = new XYSerie("Down") { DefaultColor = Color.OrangeRed };

        [IndicatorParameter]
        public int Period { get; set; } = 14;

        public Donchian()
        {
            AddFill("Up", "Down");
        }

        public override void Init()
        {
            Name = $"Donchian Channel [{this.Period}]";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            for (int i = 0; i < total; i++)
            {
                this.Up.Append((time[i], high.GetHighest(i, Period)));
                this.Down.Append((time[i], low.GetLowest(i, Period)));
            }
        }
    }
}
