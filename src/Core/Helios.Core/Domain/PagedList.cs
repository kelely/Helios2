using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Helios.Domain
{
    /// <summary>
    /// 支持分页的集合类
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    [DataContract]
    public class PagedList<T>
    {
        internal PagedList(IEnumerable<T> items, int pageIndex, int pageSize, int totalCount)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex), "页码必须大于0");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "分页大小必须大于0");

            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;

            this.TotalPages = TotalCount / this.PageSize;

            // 修正页码
            if (this.TotalCount % this.PageSize > 0)
                this.TotalPages++;

            this.Items = items.ToArray();
        }

        /// <summary>
        /// 当前数据集合在总数据中的页码,从0开始
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// 分页大小,每页包含多少条数据
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// 总数据条数
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage => (PageIndex > 0);

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => (PageIndex + 1 < TotalPages);

        /// <summary>
        /// 包含当前页数据的数组集合
        /// </summary>
        public T[] Items { get; private set; }
    }
}
