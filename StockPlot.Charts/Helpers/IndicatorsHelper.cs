using StockPlot.Charts.Controls;
using StockPlot.Charts.Models;
using StockPlot.Indicators;
using System.Runtime.CompilerServices;

namespace StockPlot.Charts
{
    public static class IndicatorsHelper
    {
        public static bool Calc(this IndicatorBase indicator, StockPricesModel model)
        {
            if (indicator == null) 
                return false;

            return indicator.Calculate(model.Prices.Count,
                model.Prices.Select(x => x.DateTime).ToArray(),
                model.Prices.Select(x => x.Open).ToArray(),
                model.Prices.Select(x => x.High).ToArray(),
                model.Prices.Select(x => x.Low).ToArray(),
                model.Prices.Select(x => x.Close).ToArray(),
                model.Prices.Select(x => x.Volume).ToArray());
        }

        public static void ShowProperties(this IndicatorBase indicator, StockChart chart)
        {
            chart.PropertyGrid.ZIndex = chart.MainArea.Children.Count;
            chart.PropertyGrid.Item = indicator;
            chart.PropertyGrid.IsVisible = true;
        }
    }    
}
