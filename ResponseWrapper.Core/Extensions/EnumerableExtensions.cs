using System.Collections.Generic;
using System.Linq;

namespace ResponseWrapper.Core.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Check to see if an IEnumerable of TSource is either null or empty
        /// </summary>
        /// <typeparam name="TSource">The type of the list</typeparam>
        /// <param name="source">The source object which needs to be interrogated</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null ||
                   source.Any() == false;
        }
    }
}
