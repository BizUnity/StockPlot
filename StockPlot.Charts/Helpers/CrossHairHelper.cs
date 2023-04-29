namespace StockPlot.Charts
{
    public static class CrossHairHelper
    {
        public static CrossHaireXPositionHandler XUpdated;

        public static void UpdateX(double x, string stockChartId)
        {
            XUpdated?.Invoke(x, stockChartId);
        }
    }
}
