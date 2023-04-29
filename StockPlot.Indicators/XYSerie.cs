using System.ComponentModel;
using System.Drawing;

namespace StockPlot.Indicators
{
    [Browsable(false)]
    public class XYSerie : List<(DateTime Time,double Value)>
    {
        public PlotType PlotType { get; set; } = PlotType.Line;
        public Color DefaultColor { get; set; } = Color.Red;
        public int Lenght { get; set; } = 1;
        public bool IsEnabled { get; set; } = true;
        public string SerieName { get; }

        public XYSerie(string name)
        {
            SerieName = name;
        }

        public void Append((DateTime, double) value)
        {
            Add(value);
        }
    }
}