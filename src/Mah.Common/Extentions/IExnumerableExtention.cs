using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mah.Common.Extentions
{
    public static class IExnumerableExtention
    {
        public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T,int, Task> func)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < list.Count(); i++)
            {
                tasks.Add(func(list.ElementAt(i), i));
            }
            await Task.WhenAll(tasks);
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, Task> func)
        {
            var tasks = new List<Task>();
            foreach (var item in list)
            {
                tasks.Add(func(item));
            }
            await Task.WhenAll(tasks);
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> func)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                func(list.ElementAt(i), i);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> func)
        {
            foreach (var item in list)
            {
                func(item);
            }
        }
    }
}
