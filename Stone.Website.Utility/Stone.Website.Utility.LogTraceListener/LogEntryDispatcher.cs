using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Threading;

namespace Stone.Website.Utility.LogTraceListener
{
    public class LogEntryDispatcher
    {
        private static string _logFolder;

        public static void Start(string logFolder)
        {
            _logFolder = logFolder;
            LogEntryFileWriter.LogFolder = _logFolder;
            Thread monitorThread = new Thread(new ThreadStart(Monitor));
            monitorThread.Start();
        }

        private static void Monitor()
        {
            while (true)
            {
                var eventIndex = WaitHandle.WaitAny(LogEventManager.EventList);
                ProcessLogEntry(eventIndex);

                if (eventIndex == LogEventManager.INDEX_EXIT_EVENT)
                {
                    return;
                }
            }
        }

        private static void ProcessLogEntry(int index)
        {
            try
            {
                if (index == LogEventManager.INDEX_EXIT_EVENT || index == LogEventManager.INDEX_FILE_WRITER)
                {
                    ProcessLogEntryViaWriter();
                    LogEntryFileWriter.Flush();
                }

                if (index == LogEventManager.INDEX_EXIT_EVENT || index == LogEventManager.INDEX_EMAIL_SENDER)
                {
                    ProcessLogEntryViaEmail();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        private static void ProcessLogEntryViaEmail()
        {
        }

        private static void ProcessLogEntryViaWriter()
        {
            LogEntry logEntry;
            while ((logEntry = LogEntryQueueManager.DeQueueForFileWriter()) != null)
            {
                LogEntryFileWriter.Write(logEntry);
            }
        }
    }
}