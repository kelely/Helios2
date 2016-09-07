using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ServiceModel;
using Helios.Common.Domain;

namespace Helios.Common.Services
{
    /// <summary>
    /// 租户隔离的配置项服务接口
    /// </summary>
    [ServiceContract]
    public interface ISettingService
    {
        /// <summary>
        /// 获取所有配置项列表
        /// </summary>
        /// <returns>Settings</returns>
        [OperationContract]
        IList<Setting> GetAllSettings();

        /// <summary>
        /// 根据指定Id获取一个配置项
        /// </summary>
        /// <param name="settingId">配置项Id</param>
        /// <returns>配置项</returns>
        [OperationContract]
        Setting GetSettingById(int settingId);

        /// <summary>
        /// 根据配置项的 Key 获取到配置的值
        /// </summary>
        /// <param name="key">配置的Key</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="loadSharedValueIfNotFound">该值指示如果某个特定的值没有找到时，是否读取共用的配置项</param>
        /// <returns>配置项</returns>
        [OperationContract]
        Setting GetSetting(string key, int tenantId, bool loadSharedValueIfNotFound = false);

        /// <summary>
        /// 删除一个配置项
        /// </summary>
        /// <param name="key">配置的Key</param>
        /// <param name="tenantId">租户Id</param>
        [OperationContract]
        void DeleteSetting(string key, int tenantId);

        /// <summary>
        /// 保存一批配置信息对象
        /// </summary>
        /// <param name="tenantId">租户Id</param>
        /// <param name="settings">配置的键值对集合</param>
        [OperationContract]
        void SaveSettings(NameValueCollection settings, int tenantId);
    }
}
