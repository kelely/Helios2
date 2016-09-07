using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Helios.Caching;
using Helios.Common.Domain;
using Helios.Configuration;
using Helios.Data;

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

        /// <summary>
        /// 增加一个配置项
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        protected virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _settingRepository.Insert(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);

            //event notification
            //            _eventPublisher.EntityInserted(setting);
        }

        /// <summary>
        /// 更新一个配置项
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        protected virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _settingRepository.Update(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);

            //event notification
            //            _eventPublisher.EntityUpdated(setting);
        }

        /// <summary>
        /// 删除一个配置项
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _settingRepository.Delete(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);

            //event notification
            //            _eventPublisher.EntityDeleted(setting);
        }

        /// <summary>
        /// 设置配置项的值
        /// </summary>
        /// <typeparam name="T">配置值的数据类型</typeparam>
        /// <param name="key">配置的Key</param>
        /// <param name="value">配置项的值</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="clearCache">该值指示是否在更新了数据后清除缓存</param>
        public void SetSetting(string key, object value, int tenantId, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            key = key.Trim().ToLowerInvariant();
            string valueStr = CommonHelper.GetNopCustomTypeConverter(value.GetType()).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault(x => x.TenantId == tenantId) : null;

            if (settingForCaching != null)
            {
                // 更新
                var setting = GetSettingById(settingForCaching.Id);

                if (string.IsNullOrEmpty(valueStr))
                {
                    this.DeleteSetting(setting);
                }
                else
                { 
                    setting.Value = valueStr;
                    UpdateSetting(setting, clearCache);
                }
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
        /// 清除缓存
        /// </summary>
        protected virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);
        }

        #endregion

        #region Methods

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


        /// <summary>
        /// 根据配置名称获取一个配置项
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="loadSharedValueIfNotFound">该值指示如果某个特定的值没有找到时，是否读取共用的配置项</param>
        /// <returns>配置项</returns>
        public virtual Setting GetSetting(string key, int tenantId, bool loadSharedValueIfNotFound = false)
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
        /// 删除一个配置项
        /// </summary>
        /// <param name="key">配置的Key</param>
        /// <param name="tenantId">租户Id</param>
        public void DeleteSetting(string key, int tenantId)
        {
            var setting = this.GetSetting(key, tenantId);
            if(setting != null)
                this.DeleteSetting(setting);
        }

        /// <summary>
        /// 保存一批配置信息对象
        /// </summary>
        /// <param name="tenantId">租户Id</param>
        /// <param name="settings">配置的键值对集合</param>
        public void SaveSettings(NameValueCollection settings, int tenantId)
        {
            /* 每次设置更新后，我们不清除缓存。
             * 此行为可以提高性能，因为不会每一次更新都会更新缓存 */
            foreach (var key in settings.AllKeys)
            {
                string value = settings[key];
                SetSetting(key, !string.IsNullOrEmpty(value) ? value : "", tenantId, false);
            }

            // 统一清除缓存
            // TODO: 更新一个租户的配置会把全部缓存清除，这里有优化的空间
            ClearCache();
        }

        #endregion
    }
}
 