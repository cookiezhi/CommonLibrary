using Microsoft.Practices.EnterpriseLibrary.Logging;
using Stone.ConfigurationFiles;
using Stone.ConfigurationFiles.Utility.Logging;
using Stone.Framework.Common.Logging;
using System;
using System.Diagnostics;
using System.Threading;

namespace Stone.Website.Utility.Logging
{
    using InternalLogger = Microsoft.Practices.EnterpriseLibrary.Logging.Logger;

    /// <summary>
    /// Summary of this class.
    /// </summary>
    public class Logger : ILogger
    {
        #region ILogger Members

        public void LogEvent(String category, Int32 eventId, params Object[] parameters)
        {
            LogEntryInfo entry = GetLogEntry(category, eventId);
            String message = "No Message";
            TraceEventType severity = TraceEventType.Information;
            if (entry != null)
            {
                message = entry.Message;
                severity = entry.Severity;
            }
            if (parameters != null && parameters.Length > 0)
            {
                message = String.Format(message, parameters);
            }
            LogEvent(category, eventId, message, severity);
        }

        #endregion ILogger Members

        private static void LogEvent(String category, Int32 eventId, String message, TraceEventType serverity)
        {
            LogEntry entry = new LogEntry();
            entry.Categories.Add(category);
            entry.EventId = eventId;
            entry.Message = message;
            entry.Severity = serverity;
            entry.ManagedThreadName = Thread.CurrentThread.ManagedThreadId.ToString();
            InternalLogger.Write(entry);
        }

        internal static LogEntryInfo GetLogEntry(String category, Int32 eventId)
        {
            LogCategoryInfo categoryInfo = GetLogCategory(category);
            if (categoryInfo != null)
            {
                foreach (LogEntryInfo entry in categoryInfo.LogEntryList)
                {
                    if (entry.EventId == eventId)
                    {
                        return entry;
                    }
                }
            }
            return null;
        }

        internal static LogCategoryInfo GetLogCategory(String categoryName)
        {
            foreach (LogCategoryInfo category in ConfigurationManager.LogEntryConfigurationManager.CategoryList)
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