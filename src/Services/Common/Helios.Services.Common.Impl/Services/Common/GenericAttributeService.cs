using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Helios.Caching;
using Helios.Data;
using Helios.Domain.Common;

namespace Helios.Services.Common
{
    /// <summary>
    /// 领域实体对象的扩展属性服务接口的实现类
    /// </summary>
    public partial class GenericAttributeService : IGenericAttributeService
    {
        private static readonly ILog Logger = LogManager.GetLogger<GenericAttributeService>();

        #region Constants

        /// <summary>
        /// 扩展属性缓存的Key
        /// </summary>
        /// <remarks>
        /// {0} : 领域实体对象的主键Id
        /// {1} : 领域实体对象的类型
        /// </remarks>
        private const string GenericattributeKey = "Helios.genericattribute.{0}-{1}";

        /// <summary>
        /// 当清除缓存时使用此模式清除缓存
        /// </summary>
        private const string GenericattributePatternKey = "Helios.genericattribute.";
        #endregion

        #region Fields

        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly ICacheManager _cacheManager;
        // TODO:private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor
        public GenericAttributeService(ICacheManager cacheManager,
            IRepository<GenericAttribute> genericAttributeRepository/*,
                    IEventPublisher eventPublisher*/)
        {
            this._cacheManager = cacheManager;
            this._genericAttributeRepository = genericAttributeRepository;
            //            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Deletes an attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        protected void DeleteAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            _genericAttributeRepository.Delete(attribute);

            Logger.Debug($"领域对象[{attribute.EntityGroup}(#{attribute.EntityId})]的扩展属性[{attribute.Key}]被删除");

            //cache
            _cacheManager.RemoveByPattern(GenericattributePatternKey);

            //event notification
            //_eventPublisher.EntityDeleted(attribute);
        }

        /// <summary>
        /// Inserts an attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        protected void InsertAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            _genericAttributeRepository.Insert(attribute);
            Logger.Debug($"领域对象[{attribute.EntityGroup}(#{attribute.EntityId})]增加扩展属性[{attribute.Key}]，属性值[{attribute.Value}]");
            
            //cache
            _cacheManager.RemoveByPattern(GenericattributePatternKey);

            //event notification
            //            _eventPublisher.EntityInserted(attribute);
        }

        /// <summary>
        /// Updates the attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        protected void UpdateAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            var oldAttribute = GetAttributeById(attribute.Id);

            if(oldAttribute.Value != attribute.Value)
            {
                oldAttribute.Value = attribute.Value;
                _genericAttributeRepository.Update(oldAttribute);

                Logger.Debug($"领域对象[{oldAttribute.EntityGroup}(#{oldAttribute.EntityId})]的扩展属性[{oldAttribute.Key}]被修改，新属性值[{oldAttribute.Value}]");

                //cache
                _cacheManager.RemoveByPattern(GenericattributePatternKey);

                //event notification
                //            _eventPublisher.EntityUpdated(attribute);
            }
        }
        #endregion

        #region Methods

        public void SaveAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                return;

            if (attribute.Id > 0)
            {
                if (string.IsNullOrWhiteSpace(attribute.Value))
                    DeleteAttribute(attribute);
                else
                    UpdateAttribute(attribute);
            }
            else
            {
                InsertAttribute(attribute);
            }
        }

        public virtual GenericAttribute GetAttributeById(int attributeId)
        {
            if (attributeId == 0)
                return null;

            return _genericAttributeRepository.Table.FirstOrDefault(ga => ga.Id == attributeId);
        }

        public virtual IList<GenericAttribute> GetAttributesForEntity(int entityId, string entityGroup)
        {
            string key = string.Format(GenericattributeKey, entityId, entityGroup);

            return _cacheManager.Get(key, () => {
                var query = from ga in _genericAttributeRepository.Table
                            where ga.EntityId == entityId && ga.EntityGroup.Equals(entityGroup, StringComparison.InvariantCultureIgnoreCase)
                            select ga;
                return query.ToList();
            });
        }

        #endregion
    }
}