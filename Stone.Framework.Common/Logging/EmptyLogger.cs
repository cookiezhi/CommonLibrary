using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stone.Framework.Common.Logging
{
    public class EmptyLogger : ILogger
    {
        /// <summary>
        /// Do Nothing
        /// </summary>
        /// <param name="category"></param>
        /// <param name="eventId"></param>
        /// <param name="parameters"></param>
        public void LogEvent(String category, Int32 eventId, params Object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
