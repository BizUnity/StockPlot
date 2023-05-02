using System.Drawing;

namespace StockPlot.Indicators.Indicators
{
    public sealed class Ichimoku : IndicatorBase
    {
        [IndicatorParameter]
        public int InpTenkan { get; set; } = 9;
        [IndicatorParameter]
        public int InpKijun { get; set; } = 26;
        [IndicatorParameter]
        public int InpSenkou { get; set; } = 52;

        public XYSerie Tenkan { get; } = new XYSerie("Tenkan");
        public XYSerie Kijun { get; } = new XYSerie("Kijun") { DefaultColor = Color.Red };
        public XYSerie Chikou { get; } = new XYSerie("Chikou") { DefaultColor = Color.Blue };
        public XYSerie CloudA { get; } = new XYSerie("CouldA") { DefaultColor = Color.FromArgb(50, Color.Green) };
        public XYSerie CloudB { get; } = new XYSerie("CloudB") { DefaultColor = Color.FromArgb(50, Color.Green) };

        public XYYSerie Cloud { get; } = new XYYSerie("Cloud");

        public override void Init()
        {
            Name = $"Ichimoku [Tenkan ({InpTenkan}), Kijun ({InpKijun}), Senkou ({InpSenkou})]";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            var span = (time[1] - time[0]).TotalMinutes;

            for (int i = 0; i < total; i++)
            {
                var highest = high.GetHighest(i, InpTenkan);
                var lowest = low.GetLowest(i, InpTenkan);
                Tenkan.Append((time[i], (highest + lowest) / 2));

                highest = high.GetHighest(i, InpKijun);
                lowest = low.GetLowest(i, InpKijun);
                Kijun.Append((time[i],(highest + lowest) / 2));

                highest = high.GetHighest(i, InpSenkou);
                lowest = low.GetLowest(i, InpSenkou);

                Chikou.Append((time[i], i < total - InpKijun ? close[i + InpKijun] : double.NaN));

                var newTime = time[i].AddMinutes(InpKijun * span);
                var a = (Tenkan[i].Value + Kijun[i].Value) / 2;
                var b = (highest + lowest) / 2;
                CloudA.Append((newTime, a));
                CloudB.Append((newTime, b));
                Cloud.Append((newTime, a, b));
            }
        }
    }
}

