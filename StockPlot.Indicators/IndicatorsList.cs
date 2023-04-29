using StockPlot.Indicators.Indicators;

namespace StockPlot.Indicators
{
    public static class IndicatorsList
    {
        public static Dictionary<string, Type> Indicators
        {
            get
            {
                return new Dictionary<string, Type>()
                {
                    {"ATR", typeof(ATR) },
                    {"ATR Stop", typeof(ATRStop) },
                    {"Bollinger Bands", typeof(BollingerBands) },
                    {"Ichimoku", typeof(Ichimoku) },
                    {"Raghee Horner Wave", typeof(RagheeWave) },
                    {"MACD", typeof(MACD) },
                    {"EMA", typeof(ExponentialMovingAverage)},
                    {"Donchian", typeof(Donchian)] }
                };
            }
        }
    }
}
