using ReactiveUI;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;
using System.Drawing;

namespace StockPlot.Charts.Drawings
{
    public class VerticalLine : ReactiveObject
    {
        internal VLine _line = new VLine();
        private bool _inCreationMode = false;

        public VerticalLine()
        {
            _line.DragEnabled = true;
            _line.Dragged += _line_Dragged;
        }

        private void _line_Dragged(object? sender, EventArgs e)
        {
            this.RaisePropertyChanged(nameof(X));
        }

        public double X
        {
            get => _line.X;
            set
            {
                _line.X = value;
                this.RaisePropertyChanged(nameof(X));
            }
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
            plot.PointerPressed += Plot_PointerPressed;
        }

        private void Plot_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_inCreationMode)
            {
                (sender as AvaPlot).Plot.Add(_line);

                (double coordinateX, double coordinateY) = (sender as AvaPlot).GetMouseCoordinates();

                X = coordinateX;

                _inCreationMode = false;

                (sender as AvaPlot).PointerPressed -= Plot_PointerPressed;
            }
        }
    }
}
