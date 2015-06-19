using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Stone.ConfigurationFiles;
using System.Text;

namespace Stone.Website.Utility.LogTraceListener
{
    public class LogEntryFormatter
    {
        #region field

        private static ILogFormatter _logFormatter;
        private static string _logHeader;
        private static string _logFooter;

        #endregion field

        static LogEntryFormatter()
        {
            _logHeader = ConfigurationManager.LoggingConfigurationManager.LogHeader;
            _logFooter = ConfigurationManager.LoggingConfigurationManager.LogFooter;
        }

        public static void SetLogFormatter(LogEntry logEntry, ILogFormatter logFormatter)
        {
            if (logEntry != null)
            {
                _logFormatter = logFormatter;
            }
        }

        public static ILogFormatter GetLogFormatter(LogEntry logEntry)
        {
            return _logFormatter;
        }

        public static string FormatLogEntry(LogEntry logEntry)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(_logHeader))
            {
                sb.Append(_logHeader);
            }

            var logFormatter = GetLogFormatter(logEntry);
            if (logFormatter != null)
            {
                sb.AppendLine(logFormatter.Format(logEntry));
            }

            if (!string.IsNullOrEmpty(_logFooter))
            {
                sb.AppendLine(_logFooter);
            }

            return sb.ToString();
        }

        public static string FormatterLogEntryBody(LogEntry logEntry)
        {
            return _logFormatter.Format(logEntry);
        }
    }
}