using System;
using System.Linq;
using Helios.Common.Domain;
using Helios.Common.Services;
using Helios.Domain;
using Helios.Infrastructure;

namespace Helios.Common
{
    public static class GenericAttributeExtensions
    {
        #region BaseEntity
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
        #endregion

        #region IGenericAttributeService
        /// <summary>
        /// 直接获取一个领域实体对象的扩展属性
        /// </summary>
        /// <typeparam name="TPropType">扩展属性的值类型</typeparam>
        /// <param name="service">扩展属性服务接口</param>
        /// <param name="entity">领域实体对象</param>
        /// <param name="key">扩展属性的名称</param>
        /// <returns></returns>
        public static TPropType GetAttribute<TPropType>(this IGenericAttributeService service, BaseEntity entity, string key)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrEmpty(key))
                return default(TPropType);

            // TODO: 此处有可能变成延迟加载生成的代理类
            string entityGroup = entity.GetType().FullName.ToLower(); // entity.GetUnproxiedEntityType().Name;

            var attributes = service.GetAttributesForEntity(entity.Id, entityGroup);

            var attribute = attributes.FirstOrDefault(ga => ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            if (attribute == null || string.IsNullOrEmpty(attribute.Value))
            {
                attribute = attributes.FirstOrDefault(ga => ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            }

            if (attribute == null || string.IsNullOrEmpty(attribute.Value))
                return default(TPropType);

            return CommonHelper.To<TPropType>(attribute.Value);
        }

        public static void SaveAttribute<TPropType>(this IGenericAttributeService service, BaseEntity entity, string key,
            TPropType value)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // TODO: 此处有可能变成延迟加载生成的代理类
            string entityGroup = entity.GetType().FullName.ToLower(); // entity.GetUnproxiedEntityType().Name;

            var valueStr = CommonHelper.To<string>(value);

            var attribute = service.GetAttributesForEntity(entity.Id, entityGroup)
                .FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            if (attribute != null)
            {
                attribute.Value = valueStr;
            }
            else
            {
                attribute = new GenericAttribute
                {
                    EntityId = entity.Id,
                    EntityGroup = entityGroup,
                    Key = key.ToLower(),
                    Value = valueStr,
                };
            }
            service.SaveAttribute(attribute);
        }
        #endregion
    }
}
