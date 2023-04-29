using System.ComponentModel;
using System.Drawing;

namespace StockPlot.Indicators
{
    [Browsable(false)]
    public class XYYSerie : List<(DateTime,double, double)>
    {
        public string SerieName { get; set; }
        public bool IsEnabled { get; set; } = true;
        public Color UpColor { get; set; } = Color.Green;
        public Color DownColor { get; set; } = Color.Crimson;

        public XYYSerie(string serieName)
        {
            SerieName = serieName;
        }

        public void Append((DateTime, double, double) Value)
        {
            Add(Value);
        }
    }
}