using Avalonia.Controls;
using ScottPlot.Avalonia;

namespace StockPlot.Charts.Controls
{
    public partial class SubIndicator : UserControl
    {
        private AvaPlot _plot;
        public SubIndicator()
        {
            InitializeComponent();

            _plot = this.Find<AvaPlot>("plotArea");
        }

        public AvaPlot PlotArea { get { return _plot; } }       
    }
}
