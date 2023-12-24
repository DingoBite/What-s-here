using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random Random = new();

        public static T GetRandomElement<T>(this ICollection<T> enumerable)
        {
            var index = enumerable.GetRandomIndex();
            return enumerable.ElementAt(index);
        }

        public static int GetRandomIndex<T>(this ICollection<T> enumerable)
        {
            var index = Random.Next(0, enumerable.Count - 1);
            return index;
        }

        public static void AddOrUpdate<T>(this IList<T> list, int index, T value)
        {
            if (index == list.Count)
                list.Add(value);
            else
                list[index] = value;
        }

        public static void AddIfNotContains<T>(this IList<T> list, T value)
        {
            if (list.Contains(value))
                return;
            list.Add(value);
        }

        public static TValue GetOrAddAndGet<TValue>(this IList<TValue> collection, Func<TValue, bool> predicate) where TValue : new()
        {
            var value = collection.FirstOrDefault(predicate);
            if (value == null)
            {
                value = new TValue();
                collection.Add(value);
            }

            return value;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            return enumerable.ToDictionary(p => p.Key, p => p.Value);
        }
    }
}