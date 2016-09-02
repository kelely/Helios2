using System.Collections.Generic;
using System.Linq;

namespace Helios.Domain
{
    public static class PagedListExtensions
    {
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return source.AsQueryable().ToPagedList(pageIndex, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            var total = source.Count();
            var items = total == 0 ? new List<T>() : source.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return new PagedList<T>(items, pageIndex, pageSize, total);
        }
    }
}