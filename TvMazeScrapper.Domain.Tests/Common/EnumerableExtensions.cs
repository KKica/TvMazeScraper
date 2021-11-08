using FizzWare.NBuilder;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScrapper.Domain.Tests.Common
{
    public static class EnumerableExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                yield return await Task.FromResult(item);
            }
        }

        public static List<T> GetMockedList<T>(int size)
        {
            return Builder<T>.CreateListOfSize(size).Build().ToList();
        }
    }
}
