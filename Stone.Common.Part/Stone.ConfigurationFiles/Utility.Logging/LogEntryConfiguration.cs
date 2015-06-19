using Stone.Framework.Common.Collection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Stone.ConfigurationFiles.Utility.Logging
{
    [XmlRoot("logEntryConfiguratioin", Namespace = "http://www.centaline.com/Website/Logging")]
    public class LogEntryConfiguration
    {
        [XmlElement("logCategory")]
        public List<LogCategoryInfo> CategoryList
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Summary of this class.
    /// </summary>
    public class LogCategoryInfo
    {
        [XmlAttribute("name")]
        public string CategoryName
        {
            get;
            set;
        }

        [XmlElement("log")]
        public KeyedObjectCollection<int, LogEntryInfo> LogEntryList
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Summary of this class.
    /// </summary>
    public class LogEntryInfo : IKeyedObject<int>
    {
        [XmlAttribute("eventId")]
        public int EventId
        {
            get;
            set;
        }

        [XmlAttribute("severity")]
        public TraceEventType Severity
        {
            get;
            set;
        }

        [XmlElement("message")]
        public string Message
        {
            get;
            set;
        }

        #region IKeyedObject<int> Members

        public int Key
        {
            get { return this.EventId; }
        }

        #endregion IKeyedObject<int> Members
    }
}