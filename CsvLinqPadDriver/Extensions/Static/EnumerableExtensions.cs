using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CsvLinqPadDriver.Extensions.Static
{
    public static class EnumerableExtensions
    {
        public static RatioStatistic<T> GetRatio<T>(this IEnumerable<T> collection,
            Expression<Func<T, bool>> expression)
        {
            var list = collection.ToList();
            var count = list.Count;
            Func<T, bool> predicate = expression.Compile().Invoke;
            var portion = list.Where(predicate).Count();
            return new RatioStatistic<T>(expression, portion, count, GetPercent(portion, count));
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
            return Math.Round((double)count / total * 100, 1);
        }
        
        public record KeyStatistic<T>(T? Key, int Count, double Percent)
        {
            private object ToDump() => new
            {
                Key,
                Count,
                Percent
            };
        }

        public record RatioStatistic<T>(Expression<Func<T, bool>> Expression, int Count, int Total, double Percent)
        {
            private object ToDump() => new
            {
                Expression = Expression.ToString(), 
                Ratio = $"{Count}/{Total}", 
                Percent
            };
        }
    }
}