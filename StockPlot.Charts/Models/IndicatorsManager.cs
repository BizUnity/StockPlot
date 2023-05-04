using Avalonia.Controls;
using Avalonia.VisualTree;
using ReactiveUI;
using StockPlot.Charts.Controls;
using StockPlot.Charts.Helpers;
using StockPlot.Indicators;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
            var allIndicators = OnPriceIndicators.ToList();
            allIndicators.AddRange(SubIndicators);

            foreach(var manager in allIndicators)
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

            // need to create a null instance to manager the deletion command
            SubIndicator subIndicator = null;

            if (indicator.IsExternal)
            {
                // create a new sub chart
                subIndicator = new SubIndicator();
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
                addSubChart(subIndicator);

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

            //  propertygrid
            indicator.ShowProperties(_stockChart);

            _stockChart.PropertyGrid.OkButton.Click += (o, e) =>
            {
                indicator.Init();

                indicator.Calc(_stockChart.PricesModel);
                _stockChart.PropertyGrid.IsVisible = false;
            };

            manager.ShowPropertiesCommand = ReactiveCommand.Create(() =>
            {
                indicator.ShowProperties(_stockChart);
            });

            // handle te remove process
            addDeleteCommand(manager);
            // if the indicator is a sub one, we need to provide the Button with the delete command
            if (subIndicator != null)
            {
                subIndicator!.removeBTN.Command = manager.RemoveIndicatorCommand;
                subIndicator!.nameTxtBlock.Click += (o, e) => manager.ShowPropertiesCommand.Execute(null);
                subIndicator.nameTxtBlock.Content = indicator.Name;

                //manage the new name
                _stockChart.PropertyGrid.OkButton.Click += (o, e) =>
                {
                    if (subIndicator != null && indicator != null)
                    {
                        subIndicator.nameTxtBlock.Content = indicator.Name;
                        //reset the zoom to fit the area
                        plotArea.Plot.AxisAutoY();
                    }                       
                };
            }
               

            plotArea.Refresh();

            //reset the zomm
            if(indicator.IsExternal)
                plotArea.Plot.AxisAuto();          
        }

        private void addSubChart(UserControl chart)
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

        private void removeSubChart(UserControl chart)
        {
            var row = Grid.GetRow(chart);
            var grid = _stockChart.MainArea;

            var num = grid.Children.IndexOf(chart);

            grid.Children.RemoveAt(num);
            grid.Children.RemoveAt(num - 1);
            grid.RowDefinitions.RemoveAt(row);
            grid.RowDefinitions.RemoveAt(row - 1);

            for (var i = 0; i < grid.Children.Count; i++)
            {
                var row2 = Grid.GetRow(grid.Children[i]);
                if (row2 > row)
                {
                    Grid.SetRow(grid.Children[i], row2 - 2);
                }
            }
        }

        private void addDeleteCommand(IndicatorItemManager manager)
        {
            manager.RemoveIndicatorCommand = ReactiveCommand.Create(() =>
            {               
                if (manager.Indicator.IsExternal)
                {
                    // delete the visual
                    var subIndicator = manager._plotArea.FindAncestorOfType<SubIndicator>();
                    if (subIndicator != null)
                        removeSubChart(subIndicator);

                    // remove from list
                    SubIndicators.Remove(manager);

                    // re show the hidded bottom axis
                    if(SubIndicators.Count > 0)
                    {
                        SubIndicators.Last()._plotArea.Plot.BottomAxis.Ticks(true);
                    }
                    else
                    {
                        _stockChart.PriceArea.Plot.BottomAxis.Ticks(true);
                    }
                }
                else
                {
                    OnPriceIndicators.Remove(manager);
                }

                manager.Dispose();
                // set manager as null to sensure it wont be used on tick update
                manager = null;
            });
        }

        #region public fields
        /// <summary>
        /// List of available indicators in an Observable for UI purpose
        /// </summary>
        public ObservableCollection<string> Indicators { get; private set;  } = new ObservableCollection<string>(IndicatorsList.Indicators.Keys.Order());

        /// <summary>
        /// The list of working indicators displayed on the main price area
        /// </summary>
        public ObservableCollection<IndicatorItemManager> OnPriceIndicators { get; private set; } = new ObservableCollection<IndicatorItemManager>();

        /// <summary>
        /// The list of working sub indicators 
        /// </summary>
        public ObservableCollection<IndicatorItemManager> SubIndicators { get; private set; } = new ObservableCollection<IndicatorItemManager>();

        /// <summary>
        /// The selected indicator from available indcators list. Used to add a new IndicatorBase instance
        /// </summary>
        public string SelectedIndicator
        {
            get => _selectedIndicator;
            set => this.RaiseAndSetIfChanged(ref _selectedIndicator, value);
        }

        /// <summary>
        /// Command to add a new instance of indicator. The added indicator is the selected one from the Indicators list
        /// </summary>
        public ICommand AddSelectedIndicatorCommand { get; }
        #endregion
    }
}
