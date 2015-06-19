using System;

namespace Stone.Framework.Common.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// LogEvent
        /// </summary>
        /// <param name="category"></param>
        /// <param name="eventId"></param>
        /// <param name="parameters"></param>
        void LogEvent(String category, Int32 eventId, params Object[] parameters);
    }
}