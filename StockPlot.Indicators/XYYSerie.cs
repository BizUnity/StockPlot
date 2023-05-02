using System.ComponentModel;
using System.Drawing;

namespace StockPlot.Indicators
{
    [Browsable(false)]
    public class XYYSerie : List<(DateTime,double, double)>
    {
        public string SerieName { get; set; }
        public bool IsEnabled { get; set; } = true;
        public Color Color { get; set; } = Color.Green;

        public XYYSerie(string serieName)
        {
            SerieName = serieName;
        }

        public void Append((DateTime, double, double) Value)
        {
            if (double.IsNaN(Value.Item2) ||  double.IsNaN(Value.Item3))
                return;

            Add(Value);
        }
    }
}