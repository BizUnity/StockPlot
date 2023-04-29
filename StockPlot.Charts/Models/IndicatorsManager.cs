using Avalonia.Controls;
using ReactiveUI;
using StockPlot.Charts.Controls;
using StockPlot.Charts.Helpers;
using StockPlot.Indicators;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;

namespace StockPlot.Charts.Models
{
    public class IndicatorsManager : ReactiveObject
    {
        #region private fields
        private StockChart _stockChart;
        private string _selectedIndicator = string.Empty;
        #endregion
              

        public IndicatorsManager(StockChart stockChart)
        {
            _stockChart = stockChart;

            // when we change the price, we need to update the indicators with the new model
            _stockChart.StockPricesModelChanged += _stockChart_StockPricesModelChanged;

            // create the command to add a selected indicator from the available list
            AddSelectedIndicatorCommand = ReactiveCommand.Create(() =>
            {
                if (IndicatorsList.Indicators.Keys.Contains(_selectedIndicator))
                {
                    var newIndicator = Activator.CreateInstance(IndicatorsList.Indicators[_selectedIndicator]) as IndicatorBase;
                    addAnIndicator(newIndicator);
                }
            });
        }

        private void _stockChart_StockPricesModelChanged(StockPricesModel newModel)
        {
            var onPrice = OnPriceIndicators.ToList();
            var sub = SubIndicators.ToList();
            onPrice.AddRange(sub);

            foreach(var manager in onPrice)
            {
                manager._indicator.Calc(newModel);
                manager._plotArea.Plot.AxisAuto();
                manager._plotArea.Refresh();

                _stockChart.PricesModel.OnTick += (bar) =>
                {
                    manager._indicator.Calc(_stockChart.PricesModel);
                    manager._plotArea.Refresh();
                };

                _stockChart.PricesModel.OnBarAdded += (bar) =>
                {
                    manager._indicator.Calc(_stockChart.PricesModel);
                    manager._plotArea.Refresh();
                };
            }
        }

        private void addAnIndicator(IndicatorBase indicator)
        {
            indicator.Init();

            var plotArea = _stockChart.PriceArea;

            if (indicator.IsExternal)
            {
                // create a new sub chart
                var subIndicator = new SubIndicator();
                // update the plot area
                plotArea = subIndicator.PlotArea;
                // link the axises
                subIndicator.PlotArea.Configuration.AddLinkedControl(_stockChart.PriceArea, layout: false);

                foreach(var item in SubIndicators)
                {
                    subIndicator.PlotArea.Configuration.AddLinkedControl(item._plotArea, layout: false);
                    item._plotArea.Configuration.AddLinkedControl(subIndicator.PlotArea, layout: false);

                    //hidding the x axis
                    item._plotArea.Plot.BottomAxis.Ticks(false);
                }

                _stockChart.PriceArea.Configuration.AddLinkedControl(subIndicator.PlotArea, layout: false);
                // setup the plot area
                PlotHelper.SetupBasicPlot(subIndicator.PlotArea, _stockChart.StockChartID);
                // add a new sub chart in the main grid
                _AddSubChart(subIndicator);

                _stockChart.PriceArea.Plot.BottomAxis.Ticks(false);
            }

            var manager = new IndicatorItemManager(indicator, plotArea);          
                       
            _stockChart.PricesModel.OnTick += (bar) =>
            {
                indicator.Calc(_stockChart.PricesModel);
                plotArea.Refresh();
            };

            _stockChart.PricesModel.OnBarAdded += (bar) =>
            {
                indicator.Calc(_stockChart.PricesModel);
                plotArea.Refresh();
            };

            indicator.Calc(_stockChart.PricesModel);

            if(indicator.IsExternal)
            {
                SubIndicators.Add(manager);
            }
            else
            {
                OnPriceIndicators.Add(manager);
            }                     

            plotArea.Refresh();
        }

        private void _AddSubChart(UserControl chart)
        {
            _stockChart.MainArea.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(3, GridUnitType.Auto) });
            _stockChart.MainArea.RowDefinitions.Add(new RowDefinition());

            var splitter = new GridSplitter()
            {
                Height = 3,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
            };

            _stockChart.MainArea.Children.Add(splitter);
            Grid.SetRow(splitter, _stockChart.MainArea.RowDefinitions.Count - 2);

            _stockChart.MainArea.Children.Add(chart);
            Grid.SetRow(chart, _stockChart.MainArea.RowDefinitions.Count - 1);
        }

        #region public fields
        /// <summary>
        /// List of available indicators in an Observable for UI purpose
        /// </summary>
        public ObservableCollection<string> Indicators { get; private set;  } = new ObservableCollection<string>(IndicatorsList.Indicators.Keys);

        public ObservableCollection<IndicatorItemManager> OnPriceIndicators { get; private set; } = new ObservableCollection<IndicatorItemManager>();

        public ObservableCollection<IndicatorItemManager> SubIndicators { get; private set; } = new ObservableCollection<IndicatorItemManager>();

        public string SelectedIndicator
        {
            get => _selectedIndicator;
            set => this.RaiseAndSetIfChanged(ref _selectedIndicator, value);
        }

        public ICommand AddSelectedIndicatorCommand { get; }
        #endregion
    }
}
