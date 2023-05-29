using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;
using StockPlot.Charts.Helpers;
using StockPlot.Charts.Models;
using System.Text;

namespace StockPlot.Charts.Controls
{
    public partial class StockChart : UserControl
    {
        #region private and internal fields
        private Grid _mainArea;
        private AvaPlot _pricePlot;
        private DisplayPrice _chartType = DisplayPrice.OHLC;
        
        private FinancePlot _candlesPlot, _ohlcsPlot;
        private TextBlock _mouseTxtBlock;
        private HLine _lastPriceLine;
        private IndicatorsManager _indicatorsManager;
        private DrawingManager _drawingManager;
        internal PropertyGrid _propertyGrid;

        #endregion

        public event StockPricesModelChangedHandler StockPricesModelChanged;

        public StockChart()
        {
            InitializeComponent();

            _mainArea = this.Find<Grid>("MainArea");
            _pricePlot = this.Find<AvaPlot>("PriceArea");
            _mouseTxtBlock = this.Find<TextBlock>("MouseTextBlock");
            _propertyGrid = this.Find<PropertyGrid>("PropertyGrid");
            // add the indicator manager
            // indicator manager helps to manage the indicators logic in order to keep this class clean as it's dedicated only to the price changement flow and price area
            //TODO : Make it a s a property to share it in external apps
            _indicatorsManager = new IndicatorsManager(this);

            _drawingManager = new DrawingManager(this);

            StockChartID = GenId();
            initBases();
        }

        #region private methods
        
        private string GenId()
        {
            int length = 7;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }

            return str_build.ToString();
        }

        private void initBases()
        {
            PlotHelper.SetupBasicPlot(_pricePlot, StockChartID);

            // create the price line
            _lastPriceLine = _pricePlot.Plot.AddHorizontalLine(0, System.Drawing.Color.FromArgb(23, 62, 113), 1, LineStyle.Dot);
            _lastPriceLine.PositionLabel = true;
            _lastPriceLine.PositionLabelOppositeAxis = true;
            _lastPriceLine.IgnoreAxisAuto = true;
            _lastPriceLine.PositionLabelBackground = System.Drawing.Color.FromArgb(23, 62, 113);
            _lastPriceLine.YAxisIndex = 1;

            // init the price series
            _candlesPlot = _pricePlot.Plot.AddCandlesticks(PricesModel.Prices.ToArray());            
            _ohlcsPlot = _pricePlot.Plot.AddOHLCs(PricesModel.Prices.ToArray());            

            // setup the  axis
            _candlesPlot.YAxisIndex = 1;
            _ohlcsPlot.YAxisIndex = 1;

            _pricePlot.Refresh();            
        }

        private void updatePriceModel(StockPricesModel model)
        {
            // set the nes price model and create the event
            this.StockPricesModelChanged?.Invoke(model);

            // clear the old values in charts
            _candlesPlot.Clear();
            _ohlcsPlot.Clear();

            // update the chart with the new data (if existing)
            _candlesPlot.AddRange(model.Prices.ToArray());
            _ohlcsPlot.AddRange(model.Prices.ToArray());
            _lastPriceLine.Y = model.Prices.Last().Close;

            _pricePlot.Plot.AxisAuto();
            _pricePlot.Refresh();

            model.OnBarAdded += (bars) =>
            {
                _candlesPlot.AddRange(bars);
                _ohlcsPlot.AddRange(bars);
                _lastPriceLine.Y = bars.Last().Close;
                _pricePlot.Refresh();
            };

            model.OnTick += (bar) =>
            {
                // refresh the price with the last update                    
                _lastPriceLine.Y = bar.Close;
                _pricePlot.Refresh();
            };

        }
        #endregion

        #region Properties
        public static StyledProperty<IBrush> CandleUpFillProperty = AvaloniaProperty.Register<StockChart, IBrush>(nameof(CandleUpColor));
        
        public IBrush CandleUpColor
        {
            get => GetValue(CandleUpFillProperty);
            set 
            {
                SetValue(CandleUpFillProperty, value);
                _candlesPlot.ColorUp = value.ConvertToColor();
                _pricePlot.Refresh();
            } 
        }

        public static StyledProperty<IBrush> CandleDownFillProperty = AvaloniaProperty.Register<StockChart, IBrush>(nameof(CandleDownColor));

        public IBrush CandleDownColor
        {
            get => GetValue(CandleDownFillProperty);
            set
            {
                SetValue(CandleDownFillProperty, value);
                _candlesPlot.ColorDown = value.ConvertToColor();
                _pricePlot.Refresh();
            }
        }

        public static StyledProperty<IBrush> CandleWickProperty = AvaloniaProperty.Register<StockChart, IBrush>(nameof(CandleWickColor));

        public IBrush CandleWickColor
        {
            get => GetValue(CandleWickProperty);
            set
            {
                SetValue(CandleWickProperty, value);
                _candlesPlot.WickColor = value.ConvertToColor();
                _pricePlot.Refresh();
            }
        }

        public static StyledProperty<DisplayPrice> DisplayPriceProperty = AvaloniaProperty.Register<StockChart, DisplayPrice>(nameof(DisplayPrice), DisplayPrice.Candlestick);

        public DisplayPrice DisplayPrice
        {
            get => GetValue(DisplayPriceProperty);
            set 
            {
                SetValue(DisplayPriceProperty, value);

                switch (value)
                {
                    case DisplayPrice.Candlestick:
                        _candlesPlot.IsVisible = true;
                        _ohlcsPlot.IsVisible = false;
                        break;
                    case DisplayPrice.OHLC:
                        _ohlcsPlot.IsVisible = true;
                        _candlesPlot.IsVisible = false;
                        break;
                }

                _pricePlot.Refresh();
            }
        }

        public static StyledProperty<bool> ResetAxisOnDoubleClickProperty = AvaloniaProperty.Register<StockChart, bool>(nameof(ResetAxisOnDoubleClick), false);

        /// <summary>
        /// Set X axis and Y axis to default on DoubleTapped
        /// </summary>
        public bool ResetAxisOnDoubleClick
        {
            get=> GetValue(ResetAxisOnDoubleClickProperty);
            set => SetValue(ResetAxisOnDoubleClickProperty, value);
        }

        public static StyledProperty<StockPricesModel> PricesModelProperty = AvaloniaProperty.Register<StockChart, StockPricesModel>(nameof(ResetAxisOnDoubleClick), new StockPricesModel(true));

        /// <summary>
        /// PricesModel is the StockPricesModel used i the current StockChart Control instance.
        /// </summary>
        public StockPricesModel PricesModel
        {
            get=> GetValue(PricesModelProperty);
            set
            {
                SetValue(PricesModelProperty, value);
                updatePriceModel(value);
            }
        }

        public static StyledProperty<string> StockChartIDProperty = AvaloniaProperty.Register<StockChart, string>(nameof(ResetAxisOnDoubleClick), "42");

        /// <summary>
        /// Should be Unique. ID is used in static events as zoom reset and CrossHair X update.
        /// It help to ensure we set the crosshaire or reset a zoom is using the right StockChart in case of multiple instances in an application
        /// </summary>
        public string StockChartID
        {
            get => GetValue(StockChartIDProperty);
            set
            {
                SetValue(StockChartIDProperty, value);
            }
        }

        /// <summary>
        /// Indicators Manager allows you to display the available indicator, on price indicators and sub indicators on you own UI.
        /// </summary>
        public IndicatorsManager IndicatorsManager
        {
            get => _indicatorsManager;
        }

        /// <summary>
        /// Drawing manager contains de logics to create and manage draw such as trendline and analysis tools.
        /// </summary>
        public DrawingManager DrawingManager
        { 
            get => _drawingManager; 
        }
        #endregion
    }
}
