using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Helios.Caching;
using Helios.Common.Domain;
using Helios.Configuration;
using Helios.Data;
using Nop.Core;

namespace Helios.Common.Services
{
    /// <summary>
    /// 租户隔离的配置项服务接口
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string SETTINGS_ALL_KEY = "Helios.setting.all";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SETTINGS_PATTERN_KEY = "Helios.setting.";

        #endregion

        #region Fields

        private readonly IRepository<Setting> _settingRepository;
//        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="settingRepository">Setting repository</param>
        public SettingService(ICacheManager cacheManager, /*IEventPublisher eventPublisher,*/
            IRepository<Setting> settingRepository)
        {
            this._cacheManager = cacheManager;
//            this._eventPublisher = eventPublisher;
            this._settingRepository = settingRepository;
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public int TenantId { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// 从缓存中加载所有的配置项，如果缓存不存在，就会从数据持久层加载出所有的数据并保存到缓存中
        /// </summary>
        /// <returns>配置集合</returns>
        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            string key = string.Format(SETTINGS_ALL_KEY);

            return _cacheManager.Get(key, () =>
            {
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();

                //这里使用NoTracking以优化读取性能
                var query = from s in _settingRepository.TableNoTracking
                            orderby s.Name, s.TenantId
                            select s;
                var settings = query.ToList();

                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        TenantId = s.TenantId
                    };

                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //第一个配置项
                        dictionary.Add(resourceName, new List<SettingForCaching>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //新增配置项
                        //当某些租户使用了自定义的配置值覆盖了系统默认值时，会有这种情况出现
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Insert(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);

            //event notification
//            _eventPublisher.EntityInserted(setting);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Update(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);

            //event notification
//            _eventPublisher.EntityUpdated(setting);
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);

            //event notification
//            _eventPublisher.EntityDeleted(setting);
        }

        /// <summary>
        /// 根据配置项的Id获取配置项
        /// </summary>
        /// <param name="settingId">配置项Id</param>
        /// <returns>配置项</returns>
        public virtual Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            return _settingRepository.GetById(settingId);
        }

        public Setting GetSingleSetting(string name, string value)
        {
            var query = from s in _settingRepository.Table
                        where s.Name == name && s.Value == value
                        orderby s.Name, s.TenantId
                        select s;
            
            return query.SingleOrDefault();
        }


        /// <summary>
        /// 根据配置名称获取一个配置项
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="loadSharedValueIfNotFound">该值指示如果某个特定的值没有找到时，是否读取共用的配置项</param>
        /// <returns>配置项</returns>
        public virtual Setting GetSetting(string key, int tenantId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            // 从缓存中加载全部配置项
            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();

            if (!settings.ContainsKey(key))
                return null;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.TenantId == tenantId);

            //读取共用配置值?
            if (setting == null && tenantId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.TenantId == 0);

            if (setting != null)
                return GetSettingById(setting.Id);

            return null;
        }


        /// <summary>
        /// 根据配置项的 Key 获取到配置的值
        /// </summary>
        /// <typeparam name="T">配置值的数据类型</typeparam>
        /// <param name="key">配置的Key</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="loadSharedValueIfNotFound">该值指示如果某个特定的值没有找到时，是否读取共用的配置项</param>
        /// <returns>配置值</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T), int tenantId = 0, bool loadSharedValueIfNotFound = false)
        {
            // 当 key 没有提供时，强制返回默认值
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            // 从缓存中加载全部配置项
            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();

            if (!settings.ContainsKey(key))
                return defaultValue;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.TenantId == tenantId);

            //读取共用配置值?
            if (setting == null && tenantId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.TenantId == 0);

            // 进行类型转换
            if (setting != null)
                return CommonHelper.To<T>(setting.Value);

            return defaultValue;
        }

        /// <summary>
        /// 设置配置项的值
        /// </summary>
        /// <typeparam name="T">配置值的数据类型</typeparam>
        /// <param name="key">配置的Key</param>
        /// <param name="value">配置项的值</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="clearCache">该值指示是否在更新了数据后清除缓存</param>
        public virtual void SetSetting<T>(string key, T value, int tenantId = 0, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            key = key.Trim().ToLowerInvariant();
            string valueStr = CommonHelper.GetNopCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ? 
                allSettings[key].FirstOrDefault(x => x.TenantId == tenantId) : null;

            if (settingForCaching != null)
            {
                // 更新
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                // 新增
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    TenantId = tenantId
                };
                InsertSetting(setting, clearCache);
            }
        }

        /// <summary>
        /// 获取所有配置项列表
        /// </summary>
        /// <returns>Settings</returns>
        public virtual IList<Setting> GetAllSettings()
        {
            var query = from s in _settingRepository.Table
                        orderby s.Name, s.TenantId
                        select s;
            return query.ToList();
        }

        /// <summary>
        /// 确定是否存在配置项
        /// </summary>
        /// <typeparam name="T">配置类型，该类型必须实现 ISettings 接口</typeparam>
        /// <typeparam name="TPropType">属性值的类型</typeparam>
        /// <param name="settings">配置对象</param>
        /// <param name="keySelector">属性选择器</param>
        /// <param name="tenantId">租户Id</param>
        /// <returns>true - 配置存在; false - 配置不存在</returns>
        public virtual bool SettingExists<T, TPropType>(T settings, 
            Expression<Func<T, TPropType>> keySelector, int tenantId = 0) 
            where T : ISettings, new()
        {
            string key = settings.GetSettingKey(keySelector);

            var setting = GetSettingByKey<string>(key, tenantId: tenantId);
            return setting != null;
        }

        /// <summary>
        /// 获取租户指定类型的配置参数
        /// </summary>
        /// <typeparam name="T">配置类型，该类型必须实现 ISettings 接口</typeparam>
        /// <param name="tenantId">租户Id</param>
        public virtual T LoadSetting<T>(int tenantId = 0) where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // 确保属性可读可写
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                
                // 获取特定租户的配置项的值，该方法从缓存中读取数据，所以性能还行，只是需要注意缓存同步的问题
                var setting = GetSettingByKey<string>(key, tenantId: tenantId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                // 属性赋值
                prop.SetValue(settings, value, null);
            }

            return settings;
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="tenantId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        public virtual void SaveSetting<T>(T settings, int tenantId = 0) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;
                
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(key, value, tenantId, false);
                else
                    SetSetting(key, "", tenantId, false);
            }

            // 清除缓存
            ClearCache();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="tenantId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int tenantId = 0, bool clearCache = true) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = settings.GetSettingKey(keySelector);
            //Duck typing is not supported in C#. That's why we're using dynamic type
            dynamic value = propInfo.GetValue(settings, null);
            if (value != null)
                SetSetting(key, value, tenantId, clearCache);
            else
                SetSetting(key, "", tenantId, clearCache);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual void DeleteSetting<T>() where T : ISettings, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = GetAllSettings();
            foreach (var prop in typeof(T).GetProperties())
            {
                string key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            foreach (var setting in settingsToDelete)
                DeleteSetting(setting);
        }

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="tenantId">Store ID</param>
        public virtual void DeleteSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int tenantId = 0) where T : ISettings, new()
        {
            string key = settings.GetSettingKey(keySelector);
            key = key.Trim().ToLowerInvariant();

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault(x => x.TenantId == tenantId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                DeleteSetting(setting);
            }
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);
        }

        #endregion
    }
}