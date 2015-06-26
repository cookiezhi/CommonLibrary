using Stone.Framework.Common.Utility;
using System;
using System.Web;
using System.Web.Caching;

namespace Stone.Framework.Common.Configuration
{
    /// <summary>
    /// 反序列化配置文件到运行时的对象，并监视配置文件,反射任何改变运行时的对象也是如此。
    /// </summary>
    /// <remarks>
    /// 需要注意的扩展：
    /// 未来的工作：目前，配置管理器依靠的System.Web.Caching.Cache管理配置对象。
    /// MS企业库缓存组件不使用，因为filedependecy检查文件的日期每一个缓存项被访问时间，从而施加太多的IO操作。
    /// System.Web.Caching.Cache支持很多功能，因此在这里使用。然而，这限制了该组件造成消费者将不得不依赖于System.Web.dll的将来scalablity和使用方案。
    /// </remarks>
    public abstract class ConfigurationManagerBase
    {
        private Cache m_CacheManager;

        #region 异常处理

        private class LoadFileException : ApplicationException
        {
            private String m_FileName;
            private String m_TypeName;

            public LoadFileException(String typeName, String fileName)
            {
                m_FileName = fileName;
                m_TypeName = typeName;
            }

            public override string Message
            {
                get { return string.Format("Unable to load file {0} for type {1}", m_FileName, m_TypeName); }
            }
        }

        #endregion 异常处理

        private Object m_SyncObject;

        protected ConfigurationManagerBase()
        {
            m_SyncObject = new object();
            m_CacheManager = CreateCache();
        }

        private Cache CreateCache()
        {
            return HttpRuntime.Cache;
        }

        #region cache manipulation

        private T LoadConfiguration<T>(string cacheKey, string configFile, bool needLog) where T : class
        {
            var config = ObjectXmlSerializer.LoadFromXml<T>(configFile, needLog);
            if (config != null)
            {
                AddToCache(cacheKey, config, configFile, needLog);
            }
            else
            {
                throw new LoadFileException(typeof(T).Name, configFile);
            }

            return config;
        }

        /// <summary>
        /// 将配置文件添加到缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="depedencyFile"></param>
        /// <param name="needLog"></param>
        private void AddToCache(string key, object value, string depedencyFile, bool needLog)
        {
            var callBack = needLog ? new CacheItemRemovedCallback(LogCacheItemRemoved) : null;
            m_CacheManager.Add(key, value, new CacheDependency(depedencyFile), Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, callBack);
        }

        /// <summary>
        /// 从缓存中获取的配置对象。如果底层文件的更改，对象将重新加载。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T GetFromCache<T>(String cacheKey, String key) where T : class
        {
            var realKey = cacheKey ?? key;
            var response = m_CacheManager[realKey] as T;
            if (response != null) return response;
            lock (m_SyncObject)
            {
                var configFile = ConfigurationHelper.GetConfigurationFile(key);

                if (!configFile.ExtensionIsNullOrEmpty())
                {
                    response = LoadConfiguration<T>(realKey, configFile, true);
                }
            }
            return response;
        }

        protected T GetFromCache<T>(String cacheKey, String key, bool needLog) where T : class
        {
            var realKey = cacheKey ?? key;
            var res = m_CacheManager[realKey] as T;
            if (res != null) return res;
            lock (m_SyncObject)
            {
                var configFile = ConfigurationHelper.GetConfigurationFile(key);

                if (!configFile.ExtensionIsNullOrEmpty())
                {
                    res = LoadConfiguration<T>(realKey, configFile, needLog);
                }
            }
            return res;
        }

        #endregion cache manipulation

        #region Logging

        private static void LogCacheItemRemoved(string key, object obj, CacheItemRemovedReason reason)
        {
            //LoggerFactory.CreateLogger().LogEvent("Framework.Configuration", 1, key, obj, reason);
        }

        #endregion Logging
    }
}