using Avalonia.Media;
using Avalonia.Metadata;

namespace StockPlot.Charts
{
    public static class ColorHelper
    {      
        public static System.Drawing.Color ConvertToColor(this IBrush brush)
        {
            var solidColorBrush = brush as ISolidColorBrush;
            if (solidColorBrush != null)
            {
                var color = solidColorBrush.Color;
                return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            }

            return System.Drawing.Color.Transparent;
        }
    }
}
