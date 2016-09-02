using System.Collections.Generic;
using System.Linq;
using Helios.Domain;

namespace Helios.Data
{
    /// <summary>
    /// 领域对象仓库扩展方法
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// 根据主键Id从仓库中获取一个领域对象
        /// </summary>
        public static T GetById<T>(this IRepository<T> repository, int id) where T : BaseEntity
        {
            return repository.Table.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 根据主键Id数组从仓库中获取一个领域对象集合
        /// </summary>
        public static IList<T> GetByIds<T>(this IRepository<T> repository, int[] ids) where T : BaseEntity
        {
            if (ids == null || ids.Length == 0)
                return new List<T>();

            var query = from c in repository.Table
                where ids.Contains(c.Id)
                select c;

            var entities = query.ToList();

            // 根据传入的Id进行排序
            var sortedEntities = new List<T>();
            foreach (int id in ids)
            {
                var entity = entities.Find(x => x.Id == id);
                if (entity != null)
                    sortedEntities.Add(entity);
            }
            return sortedEntities;
        }
    }
}