
An initiative of [Ouinex Exchange](http://ouinex.com/ "Ouinex Exchange") and [BizUnity](https://www.linkedin.com/company/bizunity/ "BizUnity")

![](/Images/ouinex.png)

![](/Images/BizUnity.jpeg)

# StockPlot
![](/Images/StockPlot6.gif)

A Technical analysis library for [AvaloniaUI](https://avaloniaui.net/ "AvaloniaUI"), based on [ScottPlot](https://scottplot.net/ "ScottPlot") DataVisualization Library (v 4.1.63).

StockPlot *(will)* allow you to have a full Stock Market Analysis module in your application only by using a single UserControl and a single class. 

StockPlot are in a very early stage (started in April 2023). A lot of features need to be created before deploying a proper and working Nuget Package.
Some refactoring may need to be done a couple of time, **do not use it in a production yet. **
Play around, do not hesitate to contribute <3 .



## Current features
+ Display the price using Candlesticks or Bars (OHLC) with a full live datas update support :
	+ Update the last bar (OHLC).
	+ Add a new bar.
	+ Automaticaly update the indicator(s) on price updating.
	+ Working using Background dispatcher.
+ Display studies.
+ Add indicator on price.
+ Add sub indicator.
+ Auto hide or show the X axis. (Only the last Plot X axis need to be shown).
+ Auto link the X axis with sub indicators and main price area.
+ Shared cross hair between price area and sub charts.
+ Display & modify properties of an indicator.


## Features planned

+ ##### Price displaying
	+ Allowing the user to choose the price type (candle, OHLC or line).
	+ Add the line type chart.
	+ Allowing the user to customize the charts and price colors.
+ ##### Indicators :
	+ Ability to change the parametters of an indicator.
	+ Ability to change the visual parametter of an indicator (eg : line style, line color, ...).
		+ For both of those previous point, we need to create and add a PropertyGrid Control.
	+ Display the list of working indicators on price.
+ ##### Drawing tools
	+ Add an horizontal line on a specified price (Y Axis).
	+ Add a vertical line on a specified time (X Axis).
	+ Add a limited line (X1,Y1,X2,Y2).
	+ Add Fibonacci retracement.
	+ Add Andrew Pitchfork.
	+ ...
+ #### Presets
	+ Save and load a preset of indicators.
	+ Save and load a preset of draws.

# How to use
1) From you IDE, add the reference to StockPlot.Charts.
This library contains all the controls and logics. 

2) In your Window or UserControl add the reference to StockPlot.Charts :
`xmlns:stockPlot="using:StockPlot.Charts.Controls"`

3) Add the StockChart control in your axaml code :
```xml
<stockPlot:StockChart CandleDownColor="OrangeRed"
                      CandleWickColor="Black"
		      CandleUpColor="#07BF7D"
		      DisplayPrice="Candlestick"
		      ResetAxisOnDoubleClick="True"
		      Name="StockChart"/>
```
4) In your C# code, find the StockChart control :
```csharp
StockChart _chart = this.Find<StockChart>("StockChart");
```
5) Create a new StockPricesModel and provide the StockChart control with it :
```csharp
var model = new StockPricesModel();
_chart.PricesModel = model;
```
6) FullFill the model with datas. For this exemple we will use Binance API using [Binance.NET](https://github.com/JKorf/Binance.Net "Binance.NET") library.
With this exemple, price is working in live using WebSocket.
```csharp
    var client = new BinanceClient();

    var request = await client.SpotApi.ExchangeData.GetUiKlinesAsync("BTCUSDT", Binance.Net.Enums.KlineInterval.OneMinute, limit: 500);

    if (request.Success)
    {
        var bars = request.Data.Select(x => new OHLC((double)x.OpenPrice,
        (double)x.HighPrice, 
        (double)x.LowPrice, 
        (double)x.ClosePrice, 
        x.OpenTime, 
        TimeSpan.FromMinutes(1))).ToArray();

        // Append the all bars
        model.Append(bars);

        var socket = new BinanceSocketClient();
        await socket.SpotStreams.SubscribeToKlineUpdatesAsync("BTCUSDT", Binance.Net.Enums.KlineInterval.OneMinute, async (data) =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var candle = data.Data.Data;

                var toUpdate = model.Prices.FirstOrDefault(x => x.DateTime == candle.OpenTime);

                // Check if the data time are the same as the last. If not, it means we have to add a new bar
                if (toUpdate != null)
                {
                    toUpdate.Volume = (double)candle.Volume;
                    toUpdate.High = (double)candle.HighPrice;
                    toUpdate.Close = (double)candle.ClosePrice;
                    toUpdate.Low = (double)candle.LowPrice;

                    // Update the last bar
                    model.UpdateBar(toUpdate);
                }
                else
                {
                    var newBar = new OHLC((double)candle.OpenPrice, 
                    (double)candle.HighPrice,
                    (double)candle.LowPrice, 
                    (double)candle.ClosePrice, 
                    candle.OpenTime, TimeSpan.FromMinutes(1));

                    // Append the new bar
                    model.Append(newBar);
                }
            }, DispatcherPriority.Background);                       
        });
    }
```

It is very easy to fullfill the chart with datas just by using Append() and UpdateBar().
**
