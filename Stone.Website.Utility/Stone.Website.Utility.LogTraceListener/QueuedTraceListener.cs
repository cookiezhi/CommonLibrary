using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Stone.ConfigurationFiles;
using System;
using System.IO;
using System.Web;

namespace Stone.Website.Utility.LogTraceListener
{
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    public class QueuedTraceListener : CustomTraceListener
    {
        #region fields

        private int _bufferSize;
        private int _logCount;
        private static string _logFolder;

        #endregion fields

        #region constructor

        static QueuedTraceListener()
        {
            _logFolder = ConfigurationManager.LoggingConfigurationManager.LogFolder;
            if (HttpContext.Current != null)
            {
                LogEntryDispatcher.Start(HttpContext.Current.Server.MapPath(_logFolder));
            }
            else
            {
                LogEntryDispatcher.Start(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                    _logFolder.Replace("/", "\\").TrimStart('~').TrimStart('\\')));
            }
        }

        public QueuedTraceListener()
        {
            _bufferSize = ConfigurationManager.LoggingConfigurationManager.BufferSize;
            _logCount = 0;
        }

        #endregion constructor

        public override void Write(string message)
        {
            return;
        }

        public override void WriteLine(string message)
        {
            return;
        }

        public override void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, object data)
        {
            var log = data as LogEntry;

            LogEntryFormatter.SetLogFormatter(log, this.Formatter);
            try
            {
                if (log != null && Formatter != null)
                {
                    log.TimeStamp = DateTime.Now;
                    if (ConfigurationManager.LoggingConfigurationManager.DatabaseLogSwitch)
                    {
                        //写入到数据库中
                        LogEntryQueueManager.EnQueueForDbWriter(log);
                    }

                    if (ConfigurationManager.LoggingConfigurationManager.FileLogSwitch)
                    {
                        LogEntryQueueManager.EnQueueForDbWriter(log);
                    }

                    //发送邮件
                    LogEntryQueueManager.EnQueueForEmailSender(log);
                }
                else
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                }

                if (!ConfigurationManager.LoggingConfigurationManager.FileLogSwitch) return;
                _logCount = (_logCount + 1) % _bufferSize;
                if (_logCount == 0)
                {
                    LogEventManager.NotifyWriterLog();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}