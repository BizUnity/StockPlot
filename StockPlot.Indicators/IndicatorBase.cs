using System.ComponentModel;
using System.Drawing;

namespace StockPlot.Indicators
{
    public abstract class IndicatorBase
    {
        private string _name;
        private List<XYSerie> _xySeries = new List<XYSerie>();
        private List<XYYSerie> _xyySeries = new List<XYYSerie>();
        private List<IndicatorLevel> _levels = new List<IndicatorLevel>();

        [Browsable(false)]
        public string Name
        {
            get => _name;
            protected set
            {
                _name = value;
            }
        }

        [Browsable(false)]
        public bool IsExternal { get; protected set; } = false;

        [Browsable(false)]
        public IReadOnlyList<XYSerie> Series
        {
            get
            {
                return _xySeries;
            }
        }

        [Browsable(false)]
        public IReadOnlyList<XYYSerie> XyySeries
        {
            get
            {
                return _xyySeries;
            }
        }

        [Browsable(false)]
        public IReadOnlyList<IndicatorLevel> Levels
        {
            get
            {
                return _levels;
            }
        }

        public event CalculatedHandler OnCalculated;

        protected IndicatorBase()
        {
            _xySeries = GetType()
                .GetProperties()
                .Where((p) => (p.CanRead && (p.PropertyType.IsAssignableFrom(typeof(XYSerie)))))
                .Select((p) => p.GetValue(this))
                .Cast<XYSerie>()
                .ToList();

            _xyySeries = GetType()
                .GetProperties()
                .Where((p) => (p.CanRead && (p.PropertyType.IsAssignableFrom(typeof(XYYSerie)))))
                .Select((p) => p.GetValue(this))
                .Cast<XYYSerie>()
                .ToList();

            _levels = GetType()
                .GetProperties()
                .Where((p) => (p.CanRead && (p.PropertyType.IsAssignableFrom(typeof(IndicatorLevel)))))
                .Select((p) => p.GetValue(this))
                .Cast<IndicatorLevel>()
                .ToList();
        }

        protected abstract void Calculate_(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume);

        public virtual void Init() { }

        protected void CreateLevel(double y, Color color)
        {
            _levels.Add(new IndicatorLevel(y, color));
        }

        public bool Calculate(int total, DateTime[] time, double[] open, double[] high, double[] low, double[] close, double[] volume)
        {
            try
            {
                _xyySeries.ForEach(s => s.Clear());
                _xySeries.ForEach(s => s.Clear());

                this.Calculate_(total, time, open, high, low, close, volume);

                this.OnCalculated?.Invoke();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public delegate void CalculatedHandler();
    }
}