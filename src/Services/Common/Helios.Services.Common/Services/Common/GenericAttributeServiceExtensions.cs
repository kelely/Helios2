using System;
using System.Linq;
using Helios.Domain;
using Helios.Domain.Common;
using Nop.Core;

namespace Helios.Services.Common
{
    public static class GenericAttributeServiceExtensions
    {
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
    }
}