using Microsoft.Practices.EnterpriseLibrary.Logging;
using Stone.ConfigurationFiles;
using System;
using System.IO;

namespace Stone.Website.Utility.LogTraceListener
{
    public class LogEntryFileWriter
    {
        #region fields

        private static StreamWriter _streamWriter;
        private static string _logFolder;
        private static string _fileNamePatten;
        private static int _logsPerFile;
        private static int _logCountWriten;

        private static StreamWriter gomz_StreamWriter;
        private static int gomz_LogCountWriten;

        #endregion fields

        static LogEntryFileWriter()
        {
            _fileNamePatten = ConfigurationManager.LoggingConfigurationManager.FileNamePattern;
            _logsPerFile = ConfigurationManager.LoggingConfigurationManager.LogsPerFile;
            _logCountWriten = 0;
            gomz_LogCountWriten = 0;
        }

        public static void Write(LogEntry logEntry)
        {
            var formattedText = LogEntryFormatter.FormatLogEntry(logEntry);
            _logCountWriten = (_logCountWriten + 1) % _logsPerFile;
            PrepareWriter(logEntry);
            _streamWriter.Write(formattedText);
        }

        public static void Flush()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Flush();
            }

            if (gomz_StreamWriter != null)
            {
                gomz_StreamWriter.Flush();
            }
        }

        public static string LogFolder
        {
            set { _logFolder = value; }
        }

        public static void PrepareWriter(LogEntry logEntry)
        {
            if (gomz_LogCountWriten == 0)
            {
                if (gomz_StreamWriter != null)
                {
                    gomz_StreamWriter.Close();
                    gomz_StreamWriter = null;
                }
            }

            if (gomz_StreamWriter == null)
            {
                var logFile = GetLogFileName(logEntry);
                var folder = Path.GetDirectoryName(logFile);
                if (folder != null && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                gomz_StreamWriter = new StreamWriter(logFile);
            }
        }

        public static string GetLogFileName(LogEntry logEntry)
        {
            var fileTitle = Path.GetFileNameWithoutExtension(_fileNamePatten);
            var fileExtension = Path.GetExtension(_fileNamePatten);

            var actualFileName = string.Format("{0}_{1}_{2}_{3}{4}", fileTitle, DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                DateTime.Now.Millisecond.ToString("0000"), logEntry.ProcessId, fileExtension);

            return Path.Combine(_logFolder, actualFileName);
        }
    }
}