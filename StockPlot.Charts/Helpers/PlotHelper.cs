using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Styles;

namespace StockPlot.Charts.Helpers
{
    public static class PlotHelper
    {
        private static List<AvaPlot> Plots = new List<AvaPlot>();

        private static IStyle _style;

        public static event ResetZoomHandler OnZoomReset;

        public static void SetupBasicPlot(AvaPlot plot, string stockChartID)
        {
            //set style if existing
            if (_style != null)
                plot.Plot.Style(_style);

            // add plot to the list 
            Plots.Add(plot);
            // remove benchmark
            plot.Configuration.DoubleClickBenchmark = false;

            // setup the axises
            plot.Plot.XAxis.DateTimeFormat(true);
            plot.Plot.YAxis.Ticks(false);
            plot.Plot.YAxis2.Ticks(true);

            //plot.Configuration.LockVerticalAxis = true;

            // init the crosshair
            var _crossHair = plot.Plot.AddCrosshair(0, 0);
            _crossHair.IgnoreAxisAuto = true;
            _crossHair.LineStyle = LineStyle.Dash;
            _crossHair.LineWidth = 1;
            _crossHair.Color = System.Drawing.Color.DarkGray;
            _crossHair.VerticalLine.PositionFormatter = pos => DateTime.FromOADate(pos).ToString();
            _crossHair.HorizontalLine.PositionLabel = true;
            _crossHair.HorizontalLine.PositionLabelOppositeAxis = true;

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


            // auto y on zoom
            plot.AxesChanged += (o, e) =>
            {
                plot.Plot.AxisAutoY();
            };
        }

        public static void ResetZoom(string chartId)
        {
            OnZoomReset?.Invoke(chartId);
        }

        public static void SetBlackStyle()
        {
            _style = ScottPlot.Style.GetStyles()[9];
            SetStyle(_style);
        }

        public static void SetWhiteStyle()
        {
            _style = ScottPlot.Style.GetStyles()[6];
            SetStyle(_style);
        }

        private static void SetStyle(IStyle style)
        {
            foreach (var plot in Plots)
            {
                if (plot != null)
                {
                    plot.Plot.Style(style);
                    plot.Refresh();
                }
            }
        }
    }
}
