using ScottPlot.Avalonia;
using ScottPlot.Plottable;
using StockPlot.Indicators;

namespace StockPlot.Charts.Models
{
    public class IndicatorItemManager
    {
        internal IndicatorBase _indicator;
        internal AvaPlot _plotArea;

        public IndicatorItemManager(IndicatorBase indicator, AvaPlot priceArea)
        {
            _indicator = indicator;
            _plotArea = priceArea;

            _indicator.Init();

            displayXYSeries();
            // displayXYYSeries();
            displayLevels();
        }

        private void displayXYSeries()
        {
            foreach (var serie in _indicator.Series)
            {
                switch (serie.PlotType)
                {
                    case PlotType.Line:
                        var line = _plotArea.Plot.AddScatterLines(null, null, serie.DefaultColor);
                        line.OnNaN = ScatterPlot.NanBehavior.Gap;
                        line.YAxisIndex = 1;
                        line.LineWidth = serie.Lenght;

                        _indicator.OnCalculated += () =>
                        {
                            line.Update(serie.Select(x=> x.Item1.ToOADate()).ToArray(), serie.Select(x=>x.Item2).ToArray());
                        };
                        break;
                    case PlotType.Histogram:
                        var bar = _plotArea.Plot.AddBar(new double[1] { 1 }, new double[1] { 1 });
                        //TODO: re work the barwidth with the correct time span
                        bar.BarWidth = (1.0 / 2460) * .8;
                        bar.YAxisIndex = 1;

                        _indicator.OnCalculated += () =>
                        {
                            bar.Replace(serie.Select(x => x.Item1.ToOADate()).ToArray(), serie.Select(x => x.Item2).ToArray());
                        };
                        break;
                }
            }
        }

        private void displayXYYSeries()
        {
            foreach (var xyy in _indicator.XyySeries)
            { 
                _indicator.OnCalculated += () =>
                {
                    var xs = xyy.Select(x => x.Item1.ToOADate()).ToArray();
                    var y1 = xyy.Select(x => x.Item2).ToArray();
                    var y2 = xyy.Select(x => x.Item3).ToArray();

                    var fill = _plotArea.Plot.AddFill(xs, y2, y1);
                };
            }
        }

        private void displayLevels()
        {
            if (_indicator.Levels.Count <= 0)
                return;

            foreach (var level in _indicator.Levels)
            {
                _plotArea.Plot.AddHorizontalLine(level.Y, level.LevelColor, label: level.Y.ToString());
            }
        }
    }
}
