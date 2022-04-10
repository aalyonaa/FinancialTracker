using FinTracker.Assets;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;

namespace FinTracker
{
    public class Analisys
    {
        private static readonly Storage _storage = Storage.GetStorage();

        private static DateTime GetStartDateRange(Storage.DateRange range)
        {
            DateTime startRange = DateTime.Now;
            switch (range)
            {
                case Storage.DateRange.Месяц:
                    startRange = DateTime.Now.AddMonths(-1);
                    break;
                case Storage.DateRange.Полгода:
                    startRange = DateTime.Now.AddMonths(-6);
                    break;
                case Storage.DateRange.Год:
                    startRange = DateTime.Now.AddYears(-1);
                    break;
            }

            return startRange;
        }

        public static SeriesCollection GetCategoriesSeriesCollectionByAsset(string name, string assetName, List<string> categories, Storage.DateRange range)
        {

            DateTime startDate = GetStartDateRange(range);

            SeriesCollection seriesCollection = new SeriesCollection();

            User user = _storage.GetUserByName(name);
            AbstractAsset asset = user.GetAssetByName(assetName);

            foreach (string catName in categories)
            {
                double tmpSum = 0;
                foreach (Transaction transaction in asset.Transactions)
                {
                    if (transaction.Category == catName && transaction.Date >= startDate)
                    {
                        tmpSum += transaction.Amount;
                    }
                }

                if (tmpSum != 0)
                {
                    PieSeries tmpSeries = new PieSeries
                    {
                        Title = catName,
                        Values = new ChartValues<ObservableValue> { new ObservableValue(tmpSum) },
                        DataLabels = true
                    };
                    seriesCollection.Add(tmpSeries);
                }
            }

            return seriesCollection;
        }

        public static SeriesCollection GetAverageAmountByCategory(List<string> categories, Storage.DateRange range, string assetName)
        {
            SeriesCollection seriesCollection = new SeriesCollection();

            AbstractAsset asset = _storage.actualUser.GetAssetByName(assetName);

            DateTime startDate = GetStartDateRange(range);

            foreach (String category in categories)
            {
                int countOfTransactions = 0;
                double total = 0;
                foreach (Transaction transaction in asset.Transactions)
                {
                    if (transaction.Category == category && transaction.Date >= startDate)
                    {
                        countOfTransactions++;
                        total += transaction.Amount;
                    }
                }
                if (countOfTransactions > 0)
                {
                    ColumnSeries tmpSeries = new ColumnSeries
                    {
                        Title = category,
                        Values = new ChartValues<double> { total / countOfTransactions }
                    };

                    seriesCollection.Add(tmpSeries);
                }
            }
            return seriesCollection;
        }
    }
}