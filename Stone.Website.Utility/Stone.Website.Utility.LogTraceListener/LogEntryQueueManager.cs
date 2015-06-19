using Microsoft.Practices.EnterpriseLibrary.Logging;
using Stone.ConfigurationFiles;
using System.Collections.Generic;

namespace Stone.Website.Utility.LogTraceListener
{
    using LogEntryQueue = Queue<LogEntry>;

    public static class LogEntryQueueManager
    {
        private static LogEntryQueue _logEntryQueueForFileWriter = new LogEntryQueue();
        private static LogEntryQueue _logEntryQueueForEmailSender = new LogEntryQueue();
        private static LogEntryQueue _logEntryQueueForDbWriter = new LogEntryQueue();
        private static object _fileLogSyncObject = new object();
        private static object _emailLogSyncObject = new object();
        private static object _syncObject = new object();

        private static LogEntry EnQueueForFileWriter(LogEntry logEntry)
        {
            lock (_fileLogSyncObject)
            {
                _logEntryQueueForFileWriter.Enqueue(logEntry);
            }
            return logEntry;
        }

        public static LogEntry DeQueueForFileWriter()
        {
            LogEntry result = null;
            if (_logEntryQueueForFileWriter.Count <= 0) return null;
            lock (_fileLogSyncObject)
            {
                if (_logEntryQueueForFileWriter.Count > 0)
                {
                    result = _logEntryQueueForFileWriter.Dequeue();
                }
            }
            return result;
        }

        public static LogEntry EnQueueForEmailSender(LogEntry logEntry)
        {
            if (ConfigurationManager.LoggingConfigurationManager.ShouldSendEmail(logEntry))
            {
                lock (_emailLogSyncObject)
                {
                    _logEntryQueueForEmailSender.Enqueue(logEntry);
                }
            }
            return logEntry;
        }

        public static LogEntry DeQueueForEmailSender()
        {
            LogEntry result = null;

            if (_logEntryQueueForEmailSender.Count <= 0) return null;
            lock (_emailLogSyncObject)
            {
                if (_logEntryQueueForEmailSender.Count > 0)
                {
                    result = _logEntryQueueForEmailSender.Dequeue();
                }
            }
            return result;
        }

        public static LogEntry EnQueueForDbWriter(LogEntry logEntry)
        {
            lock (_syncObject)
            {
                _logEntryQueueForDbWriter.Enqueue(logEntry);
                LogEventManager.NotifyDbLog();
            }
            return logEntry;
        }

        public static LogEntry DeQueueForDbWriter()
        {
            lock (_syncObject)
            {
                if (_logEntryQueueForDbWriter.Count == 0)
                {
                    return null;
                }
                return _logEntryQueueForDbWriter.Dequeue();
            }
        }
    }
}