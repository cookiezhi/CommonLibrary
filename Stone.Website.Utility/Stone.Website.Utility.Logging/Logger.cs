using Microsoft.Practices.EnterpriseLibrary.Logging;
using Stone.ConfigurationFiles;
using Stone.ConfigurationFiles.Utility.Logging;
using Stone.Framework.Common.Logging;
using System.Diagnostics;
using System.Threading;

namespace Stone.Website.Utility.Logging
{
    using InternalLogger = Microsoft.Practices.EnterpriseLibrary.Logging.Logger;

    public class Logger : ILogger
    {
        public void LogEvent(string category, int eventId, params object[] parameters)
        {
            var entry = GetLogEntry(category, eventId);
            var message = "No Message";
            var severity = TraceEventType.Information;
            if (entry != null)
            {
                message = entry.Message;
                severity = entry.Severity;
            }

            if (parameters != null && parameters.Length > 0)
            {
                message = string.Format(message, parameters);
            }

            LogEvent(category, eventId, message, severity);
        }

        private static void LogEvent(string category, int eventId, string message, TraceEventType severity)
        {
            var entry = new LogEntry();
            entry.Categories.Add(category);
            entry.EventId = eventId;
            entry.Message = message;
            entry.Severity = severity;
            entry.ManagedThreadName = Thread.CurrentThread.ManagedThreadId.ToString();
            InternalLogger.Write(entry);
        }

        private static LogEntryInfo GetLogEntry(string category, int eventId)
        {
            var categoryInfo = GetLogCategory(category);

            if (categoryInfo != null)
            {
                foreach (var entry in categoryInfo.LogEntryList)
                {
                    if (entry.EventId == eventId)
                    {
                        return entry;
                    }
                }
            }
            return null;
        }

        private static LogCategoryInfo GetLogCategory(string categoryName)
        {
            foreach (var category in ConfigurationManager.LogEntryConfigurationManager.CategoryList)
            {
                if (category.CategoryName.Trim().ToLower() == categoryName.Trim().ToLower())
                {
                    return category;
                }
            }
            return null;
        }
    }
}