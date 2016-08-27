using System.Collections.Generic;
using System.ServiceModel;
using Helios.Domain.Common;

namespace Helios.Services.Common
{
    /// <summary>
    /// 领域实体对象的扩展属性服务接口
    /// </summary>
    [ServiceContract]
    public interface IGenericAttributeService
    {
        /// <summary>
        /// 持久化领域对象的扩展属性
        /// </summary>
        /// <param name="attribute">领域对象的扩展属性对象</param>
        [OperationContract]
        void SaveAttribute(GenericAttribute attribute);

        /// <summary>
        /// 根据扩展属性Id获取领域对象的扩展属性对象
        /// </summary>
        /// <param name="attributeId">扩展属性Id</param>
        [OperationContract]
        GenericAttribute GetAttributeById(int attributeId);

        /// <summary>
        /// 根据领域对象的主键及领域对象类型获取具体领域对象的扩展属性集合
        /// </summary>
        /// <param name="entityId">领域对象Id</param>
        /// <param name="entityGroup">领域对象类型</param>
        [OperationContract]
        IList<GenericAttribute> GetAttributesForEntity(int entityId, string entityGroup);
    }
}