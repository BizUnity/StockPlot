using Avalonia.Controls;
using Avalonia.Threading;
using Binance.Net.Clients;
using ScottPlot;
using StockPlot.Charts.Controls;
using StockPlot.Charts.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StockPlot;

public partial class MainWindow : Window
{
    StockChart _chart;
    private string _symbol = "BTCUSDT";

    public MainWindow()
    {
        InitializeComponent();

        _chart = this.Find<StockChart>("StockChart");        

        this.Find<Button>("btn").Click += async (s, e) => 
        {
            await getDataFromBinance();
        };
    }

    private async Task getDataFromBinance()
    {
        var client = new BinanceClient();

        var request = await client.SpotApi.ExchangeData.GetUiKlinesAsync(_symbol, Binance.Net.Enums.KlineInterval.OneHour, limit: 500);

        if (request.Success)
        {
            var model = new StockPricesModel();
            var bars = request.Data.Select(x => new OHLC((double)x.OpenPrice, (double)x.HighPrice, (double)x.LowPrice, (double)x.ClosePrice, x.OpenTime, TimeSpan.FromMinutes(60))).ToArray();

            model.Append(bars);

            _chart.PricesModel = model;

            var socket = new BinanceSocketClient();
            await socket.SpotStreams.SubscribeToKlineUpdatesAsync(_symbol, Binance.Net.Enums.KlineInterval.OneHour, async (data) =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var candle = data.Data.Data;

                    var toUpdate = model.Prices.FirstOrDefault(x => x.DateTime == candle.OpenTime);

                    if (toUpdate != null)
                    {
                        toUpdate.Volume = (double)candle.Volume;
                        toUpdate.High = (double)candle.HighPrice;
                        toUpdate.Close = (double)candle.ClosePrice;
                        toUpdate.Low = (double)candle.LowPrice;

                        model.UpdateBar(toUpdate);
                    }
                    else
                    {
                        var newBar = new OHLC((double)candle.OpenPrice, (double)candle.HighPrice, (double)candle.LowPrice, (double)candle.ClosePrice, candle.OpenTime, TimeSpan.FromMinutes(60));
                        model.Append(newBar);
                    }
                }, DispatcherPriority.Background);                       
            });
        }
    }
}