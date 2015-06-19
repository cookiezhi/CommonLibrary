using System.Threading;

namespace Stone.Website.Utility.LogTraceListener
{
    internal static class LogEventManager
    {
        private static AutoResetEvent _writeLogEvent = new AutoResetEvent(false);
        private static AutoResetEvent _sendEmailEvent = new AutoResetEvent(false);
        private static AutoResetEvent _dbLogEvent = new AutoResetEvent(false);
        private static AutoResetEvent _exitEvent = new AutoResetEvent(false);
        private static AutoResetEvent[] _eventList = new AutoResetEvent[] { _writeLogEvent, _sendEmailEvent, _dbLogEvent, _exitEvent };

        public const int INDEX_FILE_WRITER = 0;
        public const int INDEX_EMAIL_SENDER = 1;
        public const int INDEX_DB_WRITER = 2;
        public const int INDEX_EXIT_EVENT = 3;

        public static void NotifyWriterLog()
        {
            _writeLogEvent.Set();
        }

        public static void NotifySendEmail()
        {
            _sendEmailEvent.Set();
        }

        public static void NotifyExit()
        {
            _exitEvent.Set();
        }

        public static void NotifyDbLog()
        {
            _dbLogEvent.Set();
        }

        public static WaitHandle[] EventList
        {
            get { return _eventList; }
        }
    }
}