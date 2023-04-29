using System.Drawing;

namespace StockPlot.Indicators.Indicators
{
    public sealed class Ichimoku : IndicatorBase
    {
        private int _InpTenkan = 9;
        private int _InpKijun = 26;
        private int _InpSenkou = 52;
        private XYYSerie _Cloud { get; } = new XYYSerie("");


        public XYSerie Tenkan { get; } = new XYSerie("");
        public XYSerie Kijun { get; } = new XYSerie("") { DefaultColor = Color.Red };
        public XYSerie Chikou { get; } = new XYSerie("") { DefaultColor = Color.Blue };
        public XYYSerie Cloud { get; } = new XYYSerie("");

        public Ichimoku()
        {
            Name = "Ichimoku";
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            _Cloud.Clear();

            for (int i = 0; i < total; i++)
            {
                var highest = high.GetHighest(i, _InpTenkan);
                var lowest = low.GetLowest(i, _InpTenkan);
                Tenkan.Append((time[i], (highest + lowest) / 2));

                highest = high.GetHighest(i, _InpKijun);
                lowest = low.GetLowest(i, _InpKijun);
                Kijun.Append((time[i], (highest + lowest) / 2));

                highest = high.GetHighest(i, _InpSenkou);
                lowest = low.GetLowest(i, _InpSenkou);

               _Cloud.Append((time[i], (Tenkan[i].Value + Kijun[i].Value) / 2, (highest + lowest) / 2));

                Chikou.Append((time[i], i < total - _InpKijun ? close[i + _InpKijun] : double.NaN));
            }


            var span = (time[1] - time[0]).TotalMinutes;

            for (int i = 0; i < total + _InpKijun; i++)
            {
                if (i < _InpKijun)
                {
                    Cloud.Append((time[i], double.NaN, double.NaN));
                }
                else
                {
                    var newTime = time[i - _InpKijun].AddMinutes(_InpKijun * span);

                    Cloud.Append((newTime, _Cloud[i - _InpKijun].Item2, _Cloud[i - _InpKijun].Item3));
                }
            }
        }
    }
}
