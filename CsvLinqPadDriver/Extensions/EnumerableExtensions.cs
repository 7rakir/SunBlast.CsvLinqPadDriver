using System;
using System.Collections.Generic;
using System.Linq;

namespace CsvLinqPadDriver.Extensions
{
    // ReSharper disable UnusedType.Global
    // ReSharper disable UnusedMember.Global
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable MemberCanBePrivate.Local
    
    public static class EnumerableExtensions
    {
        public static RatioStatistic GetStatistics<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            var list = collection.ToList();
            var count = list.Count;
            var portion = list.Where(predicate).Count();
            return new RatioStatistic(portion, count, GetPercent(portion, count));
        }

        public static IOrderedEnumerable<KeyStatistic<TKey>> GetStatistics<T, TKey>(this IEnumerable<T> collection,
            Func<T, TKey> selector)
        {
            var list = collection.ToList();
            var total = list.Count;
            return list
                .GroupBy(selector)
                .Select(c =>
                {
                    var count = c.Count();
                    return new KeyStatistic<TKey>(c.Key, c.Count(), GetPercent(count, total));
                })
                .OrderByDescending(c => c.Count);
        }

        public static IEnumerable<IGrouping<TKey, T>> GetDuplicates<T, TKey>(this IEnumerable<T> collection,
            Func<T, TKey> selector)
        {
            return collection.GroupBy(selector).Where(c => c.Count() > 1);
        }

        private static double GetPercent(int count, int total)
        {
            return Math.Round((double) count / total * 100, 1);
        }

        public static IEnumerable<dynamic> GetDelayedPolledNodes(this IEnumerable<dynamic> collection,
            DateTime timeOfGatheringDiagnostics)
        {
            return collection.Where(n =>
                n.UnManaged == "False" && n.External == "False" &&
                DateTime.Parse(n.NextPoll).AddSeconds(Int32.Parse(n.PollInterval) * 2) < timeOfGatheringDiagnostics);
        }

        public record KeyStatistic<T>(T? Key, int Count, double Percent);
        
        public record RatioStatistic(int Count, int Total, double Percent);
    }
}