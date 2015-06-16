using Microsoft.Practices.EnterpriseLibrary.Logging;
using Stone.Framework.Common.Collection;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Stone.ConfigurationFiles.Utility.LogTraceListener
{
    using EventHashtable = Dictionary<int, bool>;

    using LogHashtable = Dictionary<string, Dictionary<int, bool>>;

    [XmlRoot("Logging")]
    public class LoggingConfiguration
    {
        #region fields

        private object _syncEmailList = new object();
        private LogHashtable _logHashtable;

        #endregion fields

        #region properties

        [XmlElement("logFolder")]
        public string LogFolder
        {
            get;
            set;
        }

        [XmlElement("fileNamePattern")]
        public string FileNamePattern
        {
            get;
            set;
        }

        [XmlElement("logHeader")]
        public string LogHeader
        {
            get;
            set;
        }

        [XmlElement("logFooter")]
        public string LogFooter
        {
            get;
            set;
        }

        [XmlElement("logsPerFile")]
        public int LogsPerFile
        {
            get;
            set;
        }

        [XmlElement("bufferSize")]
        public int BufferSize
        {
            get;
            set;
        }

        [XmlElement("databaseLogSwitch")]
        public bool DatabaseLogSwitch
        {
            get;
            set;
        }

        [XmlElement("fileLogSwitch")]
        public bool FileLogSwitch
        {
            get;
            set;
        }

        [XmlElement("emailSetting")]
        public EmailSetting EmailSetting
        {
            get;
            set;
        }

        #endregion properties

        public bool ShouldSendEmail(LogEntry logEntry)
        {
            PrepareEmailLogEntryList();

            EventHashtable eventHashtable;
            var category = string.Empty;
            foreach (var item in logEntry.Categories)
            {
                category = item;
                break;
            }

            var res = _logHashtable.TryGetValue(category, out eventHashtable);
            return res && eventHashtable.ContainsKey(logEntry.EventId);
        }

        private void PrepareEmailLogEntryList()
        {
            if (_logHashtable == null)
            {
                lock (_syncEmailList)
                {
                    if (_logHashtable != null) return;
                    _logHashtable = new Dictionary<string, Dictionary<int, bool>>();

                    if (EmailSetting.LogsForEmail.LogEntryList == null)
                    {
                        return;
                    }

                    foreach (var logEntry in this.EmailSetting.LogsForEmail.LogEntryList)
                    {
                        EventHashtable eventHashtable;
                        var needNew = !_logHashtable.TryGetValue(logEntry.Category, out eventHashtable);

                        if (needNew)
                        {
                            eventHashtable = new EventHashtable();
                        }

                        eventHashtable[logEntry.EventId] = true;
                        if (needNew)
                        {
                            _logHashtable[logEntry.Category] = eventHashtable;
                        }
                    }
                }
            }
        }
    }

    #region LogsForEmail

    public class LogsForEmail
    {
        private List<LoggingEntryInfo> m_LogEntryList;
        private KeyedObjectCollection<string, LogCategoryForEmail> m_LogCategoryForEmailList;

        [XmlElement("logCategory")]
        public KeyedObjectCollection<string, LogCategoryForEmail> LogCategoryForEmailList
        {
            get { return m_LogCategoryForEmailList; }
            set { m_LogCategoryForEmailList = value; }
        }

        [XmlIgnore()]
        public List<LoggingEntryInfo> LogEntryList
        {
            get
            {
                if (m_LogEntryList != null) return m_LogEntryList;
                m_LogEntryList = new List<LoggingEntryInfo>();
                foreach (var category in m_LogCategoryForEmailList)
                {
                    foreach (int eventId in category.EventIdList)
                    {
                        var entry = new LoggingEntryInfo
                        {
                            Category = category.Name,
                            EventId = eventId
                        };
                        m_LogEntryList.Add(entry);
                    }
                }
                return m_LogEntryList;
            }
        }
    }

    #endregion LogsForEmail

    #region EmailSetting

    public class EmailSetting
    {
        [XmlElement("from")]
        public string From
        {
            get;
            set;
        }

        [XmlElement("replyTo")]
        public string ReplyTo
        {
            get;
            set;
        }

        [XmlElement("recipient")]
        public string Recipient
        {
            get;
            set;
        }

        [XmlElement("ccList")]
        public string CcList
        {
            get;
            set;
        }

        [XmlElement("bccList")]
        public string BccList
        {
            get;
            set;
        }

        [XmlElement("logList")]
        public LogsForEmail LogsForEmail
        {
            get;
            set;
        }
    }

    #endregion EmailSetting

    #region LoggingEntryInfo

    public class LoggingEntryInfo
    {
        [XmlAttribute("logCategory")]
        public string Category
        {
            get;
            set;
        }

        [XmlAttribute("eventId")]
        public int EventId
        {
            get;
            set;
        }

        private string m_Message;

        public string Message
        {
            get;
            set;
        }
    }

    #endregion LoggingEntryInfo

    #region LogCategoryForEmail

    public class LogCategoryForEmail : IKeyedObject<string>
    {
        private string m_Name;
        private List<int> m_EventIdList;

        [XmlAttribute("name")]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [XmlElement("eventId")]
        public List<int> EventIdList
        {
            get { return m_EventIdList; }
            set { m_EventIdList = value; }
        }

        #region IKeyedObject<string> Members

        public string Key
        {
            get
            {
                return this.m_Name;
            }
        }

        #endregion IKeyedObject<string> Members
    }

    #endregion LogCategoryForEmail
}