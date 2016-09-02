using System.Collections.Generic;
using System.Linq;
using Helios.Domain;

namespace Helios.Data
{
    /// <summary>
    /// 领域对象数据存取仓库
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// 往领域对象仓库中增加一个领域对象
        /// </summary>
        /// <param name="entity">领域对象</param>
        void Insert(T entity);

        /// <summary>
        /// 往领域对象仓库中增加一个领域对象集合
        /// </summary>
        /// <param name="entities">领域对象集合</param>
        void Insert(IEnumerable<T> entities);

        /// <summary>
        /// 在领域对象仓库中更新一个领域对象, 该领域对象必须要有赋值的主键ID
        /// </summary>
        /// <param name="entity">领域对象</param>
        void Update(T entity);

        /// <summary>
        /// 在领域对象仓库中更新一个领域对象集合, 该领域对象必须要有赋值的主键ID
        /// </summary>
        /// <param name="entities">领域对象集合</param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// 在领域对象仓库中删除一个领域对象
        /// </summary>
        /// <param name="entity">领域对象</param>
        void Delete(T entity);

        /// <summary>
        /// 在领域对象仓库中删除一组领域对象
        /// </summary>
        /// <param name="entities">领域对象集合</param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// Gets a queryable table
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a queryable table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<T> TableNoTracking { get; }
    }
}
