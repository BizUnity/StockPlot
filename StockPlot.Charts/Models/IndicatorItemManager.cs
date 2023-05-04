using Avalonia.Controls.Documents;
using ReactiveUI;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;
using StockPlot.Indicators;
using System.Drawing;
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

            displayLevels();
            displayFills();
            displayXYSeries();                      
        }

        private void displayXYSeries()
        {
            foreach (var serie in _indicator.Series)
            {
                switch (serie.PlotType)
                {
                    case PlotType.Line:
                        var lineStyle = serie.PlotType == PlotType.DashedLine ? ScottPlot.LineStyle.DashDot : serie.PlotType == PlotType.Dot ? ScottPlot.LineStyle.Dot : ScottPlot.LineStyle.Solid;
                        var line = _plotArea.Plot.AddScatterLines(null, null, serie.DefaultColor, serie.Lenght, lineStyle);
                        line.OnNaN = ScatterPlot.NanBehavior.Gap;
                        line.YAxisIndex = 1;
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

        private void displayFills()
        {
            foreach (var fill in _indicator.XyySeries)
            {
                var color = Color.FromArgb(20, fill.Color);

                var plot = _plotArea.Plot.AddFill(new double[3] { 1,1,1 }, new double[3] { 1,1,1 }, new double[3] { 1, 1, 1 }, color:color);
                plot.YAxisIndex = 1;
                _series.Add(plot);

                _indicator.OnCalculated += () =>
                {
                    var ys1 = fill.Select(x => x.Item2).ToArray();
                    var ys2 = fill.Select(x => x.Item3).ToArray();
                    var xs1 = fill.Select(x => x.Item1.ToOADate()).ToArray();
                    var xs2 = fill.Select(x => x.Item1.ToOADate()).ToArray();

                    // combine xs and ys to make one big curve
                    int pointCount = xs1.Length + xs2.Length;
                    double[] bothX = new double[pointCount];
                    double[] bothY = new double[pointCount];

                    // copy the first dataset as-is
                    Array.Copy(xs1, 0, bothX, 0, xs1.Length);
                    Array.Copy(ys1, 0, bothY, 0, ys1.Length);

                    // copy the second dataset in reverse order
                    for (int i = 0; i < xs2.Length; i++)
                    {
                        bothX[xs1.Length + i] = xs2[xs2.Length - 1 - i];
                        bothY[ys1.Length + i] = ys2[ys2.Length - 1 - i];
                    }

                    
                    plot.Xs = bothX;
                    plot.Ys = bothY;

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
                line.PositionLabel = true;
                line.PositionLabelOppositeAxis = true;
                line.IgnoreAxisAuto = true;
                line.PositionLabelBackground = level.LevelColor;
                line.YAxisIndex = 1;
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

        public ICommand ShowPropertiesCommand { get; internal set; }
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
