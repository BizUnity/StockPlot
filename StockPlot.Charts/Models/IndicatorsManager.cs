using Avalonia.Controls;
using ReactiveUI;
using StockPlot.Charts.Controls;
using StockPlot.Charts.Helpers;
using StockPlot.Indicators;
using StockPlot.Indicators.Indicators;
using System.Collections.ObjectModel;

namespace StockPlot.Charts.Models
{
    public class IndicatorsManager : ReactiveObject
    {
        private StockChart _stockChart;

        public ObservableCollection<IndicatorItemManager> OnPriceIndicators { get; private set; } = new ObservableCollection<IndicatorItemManager>();

        public IndicatorsManager(StockChart stockChart)
        {
            _stockChart = stockChart;

            // when we change the price, we need to update the indicators with the new model
            _stockChart.StockPricesModelChanged += _stockChart_StockPricesModelChanged;

            addAnIndicator(new Donchian() { Period = 50 });
            addAnIndicator(new Ichimoku());
            addAnIndicator(new MACD());
            addAnIndicator(new ATR());
        }

        private void _stockChart_StockPricesModelChanged(StockPricesModel newModel)
        {
            foreach(var manager in OnPriceIndicators)
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
                _stockChart.PriceArea.Configuration.AddLinkedControl(subIndicator.PlotArea, layout: false);
                // setup the plot area
                PlotHelper.SetupBasicPlot(subIndicator.PlotArea, _stockChart.StockChartID);
                // add a new sub chart in the main grid
                _AddSubChart(subIndicator);
            }

            var manager = new IndicatorItemManager(indicator, plotArea);          
                       
            _stockChart.PricesModel.OnTick += (bar) =>
            {
                indicator.Calc(_stockChart.PricesModel);
                plotArea.Refresh();
            };          

            indicator.Calc(_stockChart.PricesModel);
            OnPriceIndicators.Add(manager);
            _stockChart.PriceArea.Plot.BottomAxis.Ticks(false);
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
    }
}
