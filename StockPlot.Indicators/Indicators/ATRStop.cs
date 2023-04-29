using System.Drawing;

namespace StockPlot.Indicators.Indicators
{
    public sealed class ATRStop : IndicatorBase
    {
        private ATR ATR_;

        public XYSerie UpTrend { get; private set; } = new XYSerie("Up trend") { DefaultColor = Color.LightGreen, Lenght = 2 };
        public XYSerie DownTrend { get; private set; } = new XYSerie("Down trend") { DefaultColor = Color.OrangeRed, Lenght = 2 };

        [IndicatorParameter]
        public int Period { get; set; } = 14;

        public ATRStop()
        {
            Name = "ATR Stop";
        }

        public override void Init()
        {
            ATR_ = new ATR() { Period = Period };
        }

        protected override void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            this.ATR_.Calculate(total, time, open, high, low, close, volume);

            var _trend = new int[1];
            var _downBuffer = new double[total];
            var _upBuffer = new double[total];

            for (int i = 0; i < total; i++)
            {
                double median = (high[i] + low[i]) / 2;
                double atr = this.ATR_.Atr[i].Value;

                _upBuffer[i] = median + 2 * atr;
                _downBuffer[i] = median - 2 * atr;

                if (i < 1)
                {
                    _trend[i] = 1;
                    this.DownTrend.Append((time[i], double.NaN));
                    this.UpTrend.Append((time[i],double.NaN));
                    continue;
                }

                Array.Resize(ref _trend, _trend.Length + 1);

                // Main Logic
                if (close[i] > _upBuffer[i - 1])
                {
                    _trend[i] = 1;
                }
                else if (close[i] < _downBuffer[i - 1])
                {
                    _trend[i] = -1;
                }
                else if (_trend[i - 1] == 1)
                {
                    _trend[i] = 1;
                }
                else if (_trend[i - 1] == -1)
                {
                    _trend[i] = -1;
                }

                if (_trend[i] < 0 && _trend[i - 1] > 0)
                    _upBuffer[i] = median + (2 * atr);
                else if (_trend[i] < 0 && _upBuffer[i] > _upBuffer[i - 1])
                    _upBuffer[i] = _upBuffer[i - 1];

                if (_trend[i] > 0 && _trend[i - 1] < 0)
                    _downBuffer[i] = median - (2 * atr);
                else if (_trend[i] > 0 && _downBuffer[i] < _downBuffer[i - 1])
                    _downBuffer[i] = _downBuffer[i - 1];

                // Draw Indicator
                if (_trend[i] == 1)
                {
                    this.UpTrend.Append((time[i], _downBuffer[i]));
                    this.DownTrend.Append((time[i], double.NaN));
                }
                else if (_trend[i] == -1)
                {
                    this.DownTrend.Append((time[i], _upBuffer[i]));
                    this.UpTrend.Append((time[i], double.NaN));
                }
            }
        }
    }
}
