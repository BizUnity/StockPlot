using ScottPlot;
using ScottPlot.Avalonia;

namespace StockPlot.Charts.Helpers
{
    public static class PlotHelper
    {
        public static event ResetZoomHandler OnZoomReset;

        public static void SetupBasicPlot(AvaPlot plot, string stockChartID)
        {
            // init the crosshair
            var _crossHair = plot.Plot.AddCrosshair(0, 0);
            _crossHair.IgnoreAxisAuto = true;
            _crossHair.LineStyle = LineStyle.Dash;
            _crossHair.LineWidth = 1;
            _crossHair.Color = System.Drawing.Color.DarkGray;
            _crossHair.VerticalLine.PositionFormatter = pos => DateTime.FromOADate(pos).ToString();

            plot.PointerMoved += (o, e) =>
            {
                (double coordinateX, double coordinateY) = plot.GetMouseCoordinates();
                _crossHair.X = coordinateX;
                _crossHair.Y = coordinateY;

                //set the crosshair X position globaly
                CrossHairHelper.UpdateX(coordinateX, stockChartID);

                plot.Refresh();
            };

            plot.PointerEntered += (o, e) =>
            {
                _crossHair.HorizontalLine.IsVisible = true;
            };

            plot.PointerExited += (o, e) =>
            {
                _crossHair.HorizontalLine.IsVisible = false;
            };

            CrossHairHelper.XUpdated += (x, id) =>
            {
                if (id != stockChartID)
                    return;

                _crossHair.X = x;
                plot.Refresh();
            };

            plot.DoubleTapped += (o, e) =>
            {
                plot.Plot.AxisAuto();
                plot.Refresh();

                ResetZoom(stockChartID);
            };

            OnZoomReset += (id) =>
            {
                if (id != stockChartID)
                    return;

                plot.Plot.AxisAuto();
                plot.Refresh();
            };

            // setup the axises
            plot.Plot.XAxis.DateTimeFormat(true);
            plot.Plot.YAxis.Ticks(false);
            plot.Plot.YAxis2.Ticks(true);

            plot.Configuration.LockVerticalAxis = true;
        }

        public static void ResetZoom(string chartId)
        {
            OnZoomReset?.Invoke(chartId);
        }
    }
}
