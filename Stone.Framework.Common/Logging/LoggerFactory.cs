using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stone.Framework.Common.Logging
{
    public class LoggerFactory
    {
        private const String LOGGER_TYPE_NAME = "LoggerTypeName";
        private static ILogger _mSingleLogger = null;
        private static Object _mSynObj = new Object();
        public static ILogger CreateLogger()
        {
            if (_mSingleLogger == null)
            {
                lock (_mSynObj)
                {
                    try
                    {
                        String[] loggerTypeName = ConfigurationManager.AppSettings[LOGGER_TYPE_NAME].Split(',');
                        String fullLoggerClassName = loggerTypeName[0];
                        String loggerAssembleName = loggerTypeName[1];
                        _mSingleLogger = (ILogger)Assembly.Load(loggerAssembleName).CreateInstance(fullLoggerClassName);

                        if (_mSingleLogger == null)
                        {
                            _mSingleLogger = new EmptyLogger();
                        }
                    }
                    catch (Exception)
                    {
                        _mSingleLogger = new EmptyLogger();
                    }
                }
            }
            return _mSingleLogger;
        }
    }
}
