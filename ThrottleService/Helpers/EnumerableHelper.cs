using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThrottleService.Helpers
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T newValue, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (var item in source)
            {
                yield return
                    predicate(item)
                        ? newValue
                        : item;
            }
        }

        public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (var item in source)
            {
                if (predicate(item))
                    continue;
                yield return item;
            }
        }
    }
}
