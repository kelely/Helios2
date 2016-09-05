using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ServiceModel;
using Helios.Common.Domain;
using Helios.Configuration;

namespace Helios.Common.Services
{
    /// <summary>
    /// 租户隔离的配置项服务接口
    /// </summary>
    [ServiceContract]
    public interface ISettingService
    {
        /// <summary>
        /// 根据指定Id获取一个配置项
        /// </summary>
        /// <param name="settingId">配置项Id</param>
        /// <returns>配置项</returns>
        [OperationContract]
        Setting GetSettingById(int settingId);

        /// <summary>
        /// 根据指定Id获取一个配置项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <param name="value">配置项的值</param>
        /// <returns>配置项</returns>
        [OperationContract]
        Setting GetSingleSetting(string name, string value);

        /// <summary>
        /// 删除一个配置项
        /// </summary>
        /// <param name="setting">配置项</param>
        [OperationContract]
        void DeleteSetting(Setting setting);

        /// <summary>
        /// 根据配置名称获取一个配置项
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="loadSharedValueIfNotFound">该值指示如果某个特定的值没有找到时，是否读取共用的配置项</param>
        /// <returns>配置项</returns>
        [OperationContract]
        Setting GetSetting(string key, int tenantId = 0, bool loadSharedValueIfNotFound = false);

        /// <summary>
        /// 根据配置项的 Key 获取到配置的值
        /// </summary>
        /// <typeparam name="T">配置值的数据类型</typeparam>
        /// <param name="key">配置的Key</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="loadSharedValueIfNotFound">该值指示如果某个特定的值没有找到时，是否读取共用的配置项</param>
        /// <returns>配置值</returns>
        [OperationContract]
        T GetSettingByKey<T>(string key, T defaultValue = default(T), int tenantId = 0, bool loadSharedValueIfNotFound = false);

        /// <summary>
        /// 设置配置项的值
        /// </summary>
        /// <typeparam name="T">配置值的数据类型</typeparam>
        /// <param name="key">配置的Key</param>
        /// <param name="value">配置项的值</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="clearCache">该值指示是否在更新了数据后清除缓存</param>
        [OperationContract]
        void SetSetting<T>(string key, T value, int tenantId = 0, bool clearCache = true);

        /// <summary>
        /// 获取所有配置项列表
        /// </summary>
        /// <returns>Settings</returns>
        [OperationContract]
        IList<Setting> GetAllSettings();


        /// <summary>
        /// 确定是否存在配置项
        /// </summary>
        /// <typeparam name="T">配置类型，该类型必须实现 ISettings 接口</typeparam>
        /// <typeparam name="TPropType">属性值的类型</typeparam>
        /// <param name="settings">配置对象</param>
        /// <param name="keySelector">属性选择器</param>
        /// <param name="tenantId">租户Id</param>
        /// <returns>true - 配置存在; false - 配置不存在</returns>
        [OperationContract]
        bool SettingExists<T, TPropType>(T settings, 
            Expression<Func<T, TPropType>> keySelector, int tenantId = 0)
            where T : ISettings, new();

        /// <summary>
        /// 获取租户指定类型的配置参数
        /// </summary>
        /// <typeparam name="T">配置类型，该类型必须实现 ISettings 接口</typeparam>
        /// <param name="tenantId">租户Id</param>
        [OperationContract]
        T LoadSetting<T>(int tenantId = 0) where T : ISettings, new();

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="tenantId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        [OperationContract]
        void SaveSetting<T>(T settings, int tenantId = 0) where T : ISettings, new();

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="tenantId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        [OperationContract]
        void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int tenantId = 0, bool clearCache = true) where T : ISettings, new();

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        [OperationContract]
        void DeleteSetting<T>() where T : ISettings, new();

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="tenantId">Store ID</param>
        [OperationContract]
        void DeleteSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int tenantId = 0) where T : ISettings, new();

        /// <summary>
        /// Clear cache
        /// </summary>
        [OperationContract]
        void ClearCache();
    }
}
