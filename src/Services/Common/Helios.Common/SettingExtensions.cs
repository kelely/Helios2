using System;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using Helios.Common.Services;
using Helios.Configuration;

namespace Helios.Common
{
    public static class SettingExtensions
    {
        #region ISettings
        /// <summary>
        /// Get setting key (stored into database)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Key</returns>
        public static string GetSettingKey<T, TPropType>(this T entity,
            Expression<Func<T, TPropType>> keySelector)
            where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            return typeof(T).Name + "." + propInfo.Name;
        }

        public static NameValueCollection AsNameValueCollection<T>(this T entity)
            where T : ISettings, new()
        {
            var collection = new NameValueCollection();

            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;

                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = CommonHelper.To<string>(prop.GetValue(entity, null));
                collection.Add(key, value);
            }

            return collection;
        }
        #endregion


        #region ISettingService

        /// <summary>
        /// 根据配置项的 Key 获取到配置的值
        /// </summary>
        /// <typeparam name="T">配置值的数据类型</typeparam>
        /// <param name="settingService"></param>
        /// <param name="key">配置的Key</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="loadSharedValueIfNotFound">该值指示如果某个特定的值没有找到时，是否读取共用的配置项</param>
        /// <returns>配置值</returns>
        public static T GetSettingByKey<T>(this ISettingService settingService, string key, T defaultValue = default(T),
            int tenantId = 0, bool loadSharedValueIfNotFound = false)
        {
            // 当 key 没有提供时，强制返回默认值
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var setting = settingService.GetSetting(key, tenantId, loadSharedValueIfNotFound);

            if(setting != null)
                return CommonHelper.To<T>(setting.Value);

            return defaultValue;
        }

        /// <summary>
        /// 获取租户指定类型的配置参数
        /// </summary>
        /// <typeparam name="T">配置类型，该类型必须实现 ISettings 接口</typeparam>
        /// <param name="settingService"></param>
        /// <param name="tenantId">租户Id</param>
        public static T LoadSetting<T>(this ISettingService settingService, int tenantId = 0) where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // 确保属性可读可写
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;

                // 获取特定租户的配置项的值，该方法从缓存中读取数据，所以性能还行，只是需要注意缓存同步的问题
                var setting = settingService.GetSettingByKey<string>(key, tenantId: tenantId, loadSharedValueIfNotFound: true);
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
        /// <param name="settingService"></param>
        /// <param name="settings">Setting instance</param>
        public static void SaveSettings<T>(this ISettingService settingService, T settings, int tenantId = 0) where T : ISettings, new()
        {
            settingService.SaveSettings(settings.AsNameValueCollection(), tenantId);
        }


        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settingService"></param>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="tenantId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public static void SaveSetting<T, TPropType>(this ISettingService settingService, T settings,
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
                settingService.SaveSettings(new NameValueCollection { { key, value } }, tenantId);
            else
                settingService.SaveSettings(new NameValueCollection { { key, "" } }, tenantId);
        }
        #endregion
    }
}
