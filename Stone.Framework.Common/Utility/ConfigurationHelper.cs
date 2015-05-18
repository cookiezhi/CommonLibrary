using System;
using System.Configuration;
using System.IO;


namespace Stone.Framework.Common.Utility
{
    public class ConfigurationHelper
    {
        public static String GetConfigurationFile(string appSection)
        {
            String configFile = ConfigurationManager.AppSettings[appSection];

            if (configFile != null)
            {
                return File.Exists(configFile) ? configFile : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile.Replace('/', '\\').TrimStart('\\'));
            }
            return string.Empty;
        }
    }
}
