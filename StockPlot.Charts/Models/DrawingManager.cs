using StockPlot.Charts.Controls;
using StockPlot.Charts.Drawings;

namespace StockPlot.Charts.Models
{
    public class DrawingManager
    {
        private StockChart _stockChart;
        private bool _drawingMode = false;
        private DrawType _drawType;

        public DrawingManager(StockChart stockChart)
        {
            _stockChart = stockChart;
            _stockChart.PriceArea.PointerPressed += PriceArea_PointerPressed;

            //test
            ActivateDrawingMode(DrawType.VerticalLine);
        }

        private void PriceArea_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (!_drawingMode)
                return;
        }

        public void ActivateDrawingMode(DrawType type)
        {
            _drawingMode = true;
            _drawType = type;

            switch(type)
            {
                case DrawType.HorizontalLine:
                    var hlnie = new StockPlot.Charts.Drawings.HorizontalLine();
                    hlnie.Create(_stockChart.PriceArea);
                    break;
                case DrawType.VerticalLine:
                    var vline = new StockPlot.Charts.Drawings.VerticalLine();
                    vline.Create(_stockChart.PriceArea);
                    break;
            }
        }
    }
}
