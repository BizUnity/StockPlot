
using ScottPlot;
using StockPlot.Charts.Models;

namespace StockPlot.Charts
{
    public delegate void CrossHaireXPositionHandler(double x, string stockChartID);
    public delegate void StockPricesModelChangedHandler(StockPricesModel newModel);
    public delegate void OnBarAddedHandler(params OHLC[] bars);
    public delegate void OnTickHandler(OHLC bar);
    public delegate void ResetZoomHandler(string stcokChartId);
}
