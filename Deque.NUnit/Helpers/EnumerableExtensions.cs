using System;
using System.Collections.Generic;
using System.Linq;

namespace Deque.NUnit.Helpers
{
    internal static class EnumerableExtensions
    {
        public static Int64 LongSum<T>(this IEnumerable<T> collection, Func<T, Int32> selector)
        {
            Int64 sum = 0;
            foreach (var i in collection)
                sum += selector(i);

            return sum;
        }

        public static Int64 LongSum(this IEnumerable<Int32> collection)
        {
            Int64 sum = 0;
            foreach (var i in collection)
                sum += i;

            return sum;
        }
        
        /// <summary>
        /// Determines whether all elements of a sequence are set to the default value of <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Boolean AllDefault<T>(this IEnumerable<T> array)
        {
            var comparer = EqualityComparer<T>.Default;
            var defaultValue = default(T);

            return array.All(item => comparer.Equals(item, defaultValue));
        }
    }
}
