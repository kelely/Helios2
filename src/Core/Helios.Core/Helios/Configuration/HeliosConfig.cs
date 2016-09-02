using System;
using System.Configuration;
using System.Xml;

namespace Helios.Configuration
{
    /// <summary>
    /// 处理运行 Helios 平台必要的配置信息的访问
    /// </summary>
    public class HeliosConfig : IConfigurationSectionHandler
    {
        #region Ctor
        public HeliosConfig()
        {
            this.IgnoreStartupTasks = false;
            this.RedisCachingEnabled = false;
            this.RedisCachingConnectionString = string.Empty;
        }
        #endregion

        #region Methods
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new HeliosConfig();

            return config;
        }
        #endregion

        #region Properties

        /// <summary>
        /// 是否忽略启动任务，默认不忽略
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }

        /// <summary>
        /// 是否启用 Redis 缓存，如果不启用将使用内存缓存(in-memory caching)，默认是不启用的
        /// </summary>
        public bool RedisCachingEnabled { get; private set; }

        /// <summary>
        /// Redis 服务的连接字符串，如果启用Redis缓存，此配置项时必须的
        /// </summary>
        public string RedisCachingConnectionString { get; private set; }
        #endregion
    }
}