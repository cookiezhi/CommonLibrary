using Stone.ConfigurationFiles.Utility.Logging;
using Stone.ConfigurationFiles.Utility.LogTraceListener;
using Stone.Framework.Common.Configuration;

namespace Stone.ConfigurationFiles
{
    public static class ConfigurationManager
    {
        private class InternalConfiguration : ConfigurationManagerBase
        {
            private const string SECTION_NAME_LOGENTRY_CONFIG = "LogEntryConfigurationFile";
            private const string SECTION_NAME_LOGGING_CONFIG = "LoggingConfigurationFile";
            private const string SECTION_NAME_SERVER_CONFIG = "ServerConfigurationFile";
            private const string SECTION_NAME_WEBSERVICE_CONFIG = "WebServiceConfigurationFile";

            private const string CACHEKEY_PREFIX = "Stone.ConfigurationFiles";
            private const string CACHEKEY_SECTION_NAME_LOGGING_CONFIG = CACHEKEY_PREFIX + SECTION_NAME_LOGGING_CONFIG;
            private const string CACHEKEY_SECTION_NAME_LOGENTRY_CONFIG = CACHEKEY_PREFIX + SECTION_NAME_LOGENTRY_CONFIG;
            private const string CACHEKEY_SECTION_NAME_SERVER_CONFIG = CACHEKEY_PREFIX + SECTION_NAME_SERVER_CONFIG;
            private const string CACHEKEY_SECTION_NAME_WEBSERVICE_CONFIG = CACHEKEY_PREFIX + SECTION_NAME_WEBSERVICE_CONFIG;

            private static InternalConfiguration _config;

            public static InternalConfiguration GetInstance()
            {
                return _config ?? new InternalConfiguration();
            }

            public LogEntryConfiguration LogEntryConfiguration
            {
                get
                {
                    return GetFromCache<LogEntryConfiguration>(CACHEKEY_SECTION_NAME_LOGENTRY_CONFIG, SECTION_NAME_LOGENTRY_CONFIG, false);
                }
            }

            public LoggingConfiguration LoggingConfiguration
            {
                get
                {
                    return GetFromCache<LoggingConfiguration>(CACHEKEY_SECTION_NAME_LOGGING_CONFIG, SECTION_NAME_LOGGING_CONFIG);
                }
            }
        }

        private static readonly InternalConfiguration Config = InternalConfiguration.GetInstance();

        public static LogEntryConfiguration LogEntryConfigurationManager
        {
            get { return Config.LogEntryConfiguration; }
        }

        public static LoggingConfiguration LoggingConfigurationManager
        {
            get { return Config.LoggingConfiguration; }
        }
    }
}