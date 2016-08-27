using System;
using System.Linq;
using Helios.Domain.Common;
using Helios.Infrastructure;
using Helios.Services.Common;

namespace Helios.Domain
{
    public static class GenericAttributeExtensions
    {
        #region GetAttribute
        /// <summary>
        /// 直接获取一个领域实体对象的扩展属性
        /// </summary>
        /// <typeparam name="TPropType">扩展属性的值类型</typeparam>
        /// <param name="entity">领域实体对象</param>
        /// <param name="key">扩展属性的名称</param>
        /// <returns>领域实体的扩展属性值</returns>
        public static TPropType GetAttribute<TPropType>(this BaseEntity entity, string key)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            return GetAttribute<TPropType>(entity, key, genericAttributeService);
        }

        /// <summary>
        /// 直接获取一个领域实体对象的扩展属性
        /// </summary>
        /// <remarks>
        /// 此方法为单元测试专用的方法
        /// </remarks>
        /// <typeparam name="TPropType">扩展属性的值类型</typeparam>
        /// <param name="entity">领域实体对象</param>
        /// <param name="key">扩展属性的名称</param>
        /// <param name="service">通用属性服务,默认为空，保留此参数是为了方便测试时进行注入，正常使用时不需要设置</param>
        /// <returns>领域实体的扩展属性值</returns>
        internal static TPropType GetAttribute<TPropType>(this BaseEntity entity, string key, IGenericAttributeService service)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            //little hack here (only for unit testing). we should write ecpect-return rules in unit tests for such cases
            if (service == null)
                service = EngineContext.Current.Resolve<IGenericAttributeService>();

            return service.GetAttribute<TPropType>(entity, key);
        }
        #endregion

        #region SaveAttribute
        public static void SaveAttribute<TPropType>(this BaseEntity entity, string key, TPropType value)
        {
            var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            SaveAttribute(entity, key, value, genericAttributeService);
        }

        internal static void SaveAttribute<TPropType>(this BaseEntity entity, string key, TPropType value, IGenericAttributeService service)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            //little hack here (only for unit testing). we should write ecpect-return rules in unit tests for such cases
            if (service == null)
                service = EngineContext.Current.Resolve<IGenericAttributeService>();

            service.SaveAttribute(entity, key, value);
        }
        #endregion
    }
}
