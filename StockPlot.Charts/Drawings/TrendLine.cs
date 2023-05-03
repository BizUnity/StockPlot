using ReactiveUI;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;
using System.Drawing;

namespace StockPlot.Charts.Drawings
{
    public class TrendLine : ReactiveObject
    {
        private bool _inCreationMode = false;
        private OScatterPlotDraggable _line;
        private int _steps = 0;
        private double[] xs = new double[2] { 0, 0 };
        private double[] ys = new double[2] { 0, 0 };
        private AvaPlot _plot;

        public TrendLine()
        {
            _line = new OScatterPlotDraggable(xs, ys);
            _line.DragEnabled = true;
            _line.DragEnabledX = true;
            _line.DragEnabledY = true;
            Color = Color.Red;
        }

        public Color Color
        {
            get => _line.Color;
            set
            {
                _line.Color = value;
                this.RaisePropertyChanged(nameof(Color));
            }
        }

        internal void Create(AvaPlot plot)
        {
            _inCreationMode = true;
            _plot = plot;           
            
            plot.PointerPressed += Plot_PointerPressed;
            plot.PointerMoved += Plot_PointerMoved;
        }

        private void Plot_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if(_steps == 1)
            {
                (double coordinateX, double coordinateY) = _plot.GetMouseCoordinates();
                xs[1] = coordinateX;
                ys[1] = coordinateY;

                _line.Update(xs, ys);
            }          
        }

        private void Plot_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_inCreationMode)
            {
                (double coordinateX, double coordinateY) = _plot.GetMouseCoordinates();

               
                if(_steps == 0)
                {
                    xs[0] = coordinateX;
                    ys[0] = coordinateY;
                    xs[1] = coordinateX;
                    ys[1] = coordinateY;

                    _plot.Plot.Add(_line);

                    _steps++;
                }
                else
                {
                    xs[1] = coordinateX;
                    ys[1] = coordinateY;

                    _inCreationMode = false;

                    _line.Update(xs, ys);
                    _plot.PointerPressed -= Plot_PointerPressed;
                    _plot.PointerMoved -= Plot_PointerMoved;
                }
            }
        }
    }
}
