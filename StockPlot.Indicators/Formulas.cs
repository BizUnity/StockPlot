namespace StockPlot.Indicators
{
    public static class Formulas
    {
        public static double GetSMA(this double[] input, int index, int period)
        {
            var result = 0.0;

            if (index >= period - 1 && period > 0)
            {
                for (int i = 0; i < period; i++)
                {
                    result += input[index - i];
                }

                result /= period;
            }
            else
            {
                result = double.NaN;
            }

            return result;
        }

        public static double GetSMA(this XYSerie input, int index, int period)
        {
            var array = input.Select(x => x.Item2).ToArray();

            return array.GetSMA(index, period);
        }

        public static double GetEMA(this double[] input, int index, int period, double previous)
        {
            var result = 0.0;

            if (period > 0)
            {
                double pr = 2.0 / (period + 1.0);
                result = input[index] * pr + previous * (1 - pr);
            }
            else
            {
                result = double.NaN;
            }

            return result;
        }

        public static double GetEMA(this XYSerie input, int index, int period, double previous)
        {
            var array = input.Select(x => x.Item2).ToArray();

            return array.GetEMA(index, period, previous);
        }

        public static double GetStdDev(this double[] input, int index, int period, double[] inputMa)
        {
            double result = 0.0;

            if (index > period)
            {
                for (int i = 0; i < period; i++)
                {
                    result += Math.Pow(input[index - i] - inputMa[index - 1], 2);
                }

                result = Math.Sqrt(result / period);
            }
            else
            {
                result = double.NaN;
            }

            return (result);
        }

        public static double GetStdDev(this XYSerie input, int index, int period, double[] inputMa)
        {
            var array = input.Select(x=>x.Item2).ToArray();

            return array.GetStdDev(index, period, inputMa);
        }

        public static double GetLowest(this double[] input, int index, int period)
        {
            if (index < period)
            {
                return double.NaN;
            }

            var res = input[index];

            for (var i = index; i > index - period && i >= 0; i--)
            {
                if (res > input[i]) res = input[i];
            }

            return res;
        }

        public static double GetLowest(this XYSerie input, int index, int period)
        {
            var array = input.Select(x => x.Item2).ToArray();

            return array.GetLowest(index, period);
        }

        public static double GetHighest(this double[] input, int index, int period)
        {
            if (index < period)
            {
                return double.NaN;
            }

            var res = input[index];

            for (var i = index; i > index - period && i >= 0; i--)
            {
                res = Math.Max(res, input[i]);
            }

            return res;
        }

        public static double GetHighest(this XYSerie input, int index, int period)
        {
            var array = input.Select(x => x.Item2).ToArray();

            return array.GetHighest(index, period);
        }
    }
}