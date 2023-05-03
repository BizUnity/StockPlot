using ReactiveUI;
using ScottPlot.Plottable;
using System.Drawing;

namespace StockPlot.Charts.Drawings
{
    public class HorizontalLine : ReactiveObject
    {
        private HLine _line = new HLine();

        public HorizontalLine()
        {
            _line.DragEnabled = true;
            _line.Dragged += _line_Dragged;
        }

        private void _line_Dragged(object? sender, EventArgs e)
        {
            this.RaisePropertyChanged(nameof(Y));
        }

        public double Y
        {
            get => _line.Y;
            set
            {
                _line.Y = value;
                this.RaisePropertyChanged(nameof(Y));
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
    }
}
