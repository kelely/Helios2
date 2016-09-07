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
        /// ����������� Key ��ȡ�����õ�ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����������</typeparam>
        /// <param name="settingService"></param>
        /// <param name="key">���õ�Key</param>
        /// <param name="tenantId">�⻧Id</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <param name="loadSharedValueIfNotFound">��ֵָʾ���ĳ���ض���ֵû���ҵ�ʱ���Ƿ��ȡ���õ�������</param>
        /// <returns>����ֵ</returns>
        public static T GetSettingByKey<T>(this ISettingService settingService, string key, T defaultValue = default(T),
            int tenantId = 0, bool loadSharedValueIfNotFound = false)
        {
            // �� key û���ṩʱ��ǿ�Ʒ���Ĭ��ֵ
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var setting = settingService.GetSetting(key, tenantId, loadSharedValueIfNotFound);

            if(setting != null)
                return CommonHelper.To<T>(setting.Value);

            return defaultValue;
        }

        /// <summary>
        /// ��ȡ�⻧ָ�����͵����ò���
        /// </summary>
        /// <typeparam name="T">�������ͣ������ͱ���ʵ�� ISettings �ӿ�</typeparam>
        /// <param name="settingService"></param>
        /// <param name="tenantId">�⻧Id</param>
        public static T LoadSetting<T>(this ISettingService settingService, int tenantId = 0) where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // ȷ�����Կɶ���д
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;

                // ��ȡ�ض��⻧���������ֵ���÷����ӻ����ж�ȡ���ݣ��������ܻ��У�ֻ����Ҫע�⻺��ͬ��������
                var setting = settingService.GetSettingByKey<string>(key, tenantId: tenantId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                // ���Ը�ֵ
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
