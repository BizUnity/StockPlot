using StockPlot.Charts.Controls;

namespace StockPlot.Charts.Models
{
    public class DrawingManager
    {
        private StockChart _stockChart;

        public DrawingManager(StockChart stockChart)
        {
            _stockChart = stockChart;
            _stockChart.PriceArea.PointerPressed += PriceArea_PointerPressed;
        }

        private void PriceArea_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
