using ReactiveUI;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;
using StockPlot.Indicators;
using System.Windows.Input;

namespace StockPlot.Charts.Models
{
    public class IndicatorItemManager :ReactiveObject, IDisposable
    {
        internal IndicatorBase _indicator;
        internal AvaPlot _plotArea;
        // this list is used when the indicator will be deleted. We have to clear the series from the main area
        private List<IPlottable> _series = new List<IPlottable>();

        public IndicatorItemManager(IndicatorBase indicator, AvaPlot priceArea)
        {
            _indicator = indicator;
            _plotArea = priceArea;

            _indicator.Init();

            displayXYSeries();
            displayXYYSeries();
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
                        // add the serie to the whoe list to clear in on removeing indictors
                        _series.Add(line);

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

                        _series.Add(bar);

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
                var fill = _plotArea.Plot.AddFillError(new double[1] { 1 }, new double[1] { 1 }, new double[1] { 1 });
                fill.YAxisIndex = 1;
                _series.Add(fill);

                _indicator.OnCalculated += () =>
                {                    
                    var xs = xyy.Select(x => x.Item1.ToOADate()).ToArray();
                    var ys1 = xyy.Select(x => x.Item2).ToArray();
                    var ys2 = xyy.Select(x => x.Item3).ToArray();

                    double[] polyXs = new double[] { xs[0] }.Concat(xs.Concat(xs.Reverse())).ToArray();
                    double[] polyYs = new double[] { ys1[0] }.Concat(ys1.Concat(ys2.Reverse())).ToArray();

                    fill.Xs = polyXs;
                    fill.Ys = polyYs;
                };
            }
        }

        private void displayLevels()
        {
            if (_indicator.Levels.Count <= 0)
                return;

            foreach (var level in _indicator.Levels)
            {
                var line = _plotArea.Plot.AddHorizontalLine(level.Y, level.LevelColor, label: level.Y.ToString());
                _series.Add(line);
            }
        }
        #region public fields
        public IndicatorBase Indicator
        {
            get => _indicator;
            internal set => this.RaiseAndSetIfChanged(ref _indicator, value);
        }

        public ICommand RemoveIndicatorCommand { get; internal set; }
        #endregion

        public void Dispose()
        {
            // remove the visual series from the price or indicator area
            foreach(var item in _series)
            {
                _plotArea.Plot.Remove(item);
            }

            _plotArea.Refresh();
        }
    }
}
