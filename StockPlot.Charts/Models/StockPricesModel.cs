using ReactiveUI;
using ScottPlot;

namespace StockPlot.Charts.Models
{  

    public class StockPricesModel : ReactiveObject
    {
        public event OnBarAddedHandler OnBarAdded;
        public event OnTickHandler OnTick;

        public List<OHLC> Prices { get; private set; } = new List<OHLC>();

        public StockPricesModel(bool generateRandom = false, int barsCount=100)
        {
            if (generateRandom)
            {
                var random = DataGen.RandomStockPrices(null, barsCount, TimeSpan.FromMinutes(5));

                Append(random);
            }
        }

        public void Append(params OHLC[] bars)
        {
            Prices.AddRange(bars);

            OnBarAdded?.Invoke(bars);
        }

        public void UpdateBar(OHLC bar)
        {
            OnTick?.Invoke(bar);
        }

        public void Clear()
        {
            Prices.Clear();
        }
    }
}
