using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Stone.Framework.Common.Utility;
using System.Collections.Generic;
using System.Text;

namespace Stone.Website.Utility.LogTraceListener
{
    [ConfigurationElementType(typeof(CustomFormatterData))]
    internal class XmlFormatter : LogFormatter
    {
        public override string Format(LogEntry log)
        {
            var sb = new StringBuilder();
            sb.Append("<log>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<timestamp>");
            sb.Append(SecurityHelper.XmlEncode(log.TimeStampString));
            sb.Append("</timestamp>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<message>");
            sb.Append(SecurityHelper.XmlEncode(log.Message));
            sb.Append("</message>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<category>");
            sb.Append(SecurityHelper.XmlEncode(FormatCategoriesCollection(log.Categories)));
            sb.Append("</category>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<priority>");
            sb.Append(log.Priority.ToString());
            sb.Append("</priority>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<eventId>");
            sb.Append(log.EventId.ToString());
            sb.Append("</eventId>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<severity>");
            sb.Append(SecurityHelper.XmlEncode(log.Severity.ToString()));
            sb.Append("</severity>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<title>");
            sb.Append(SecurityHelper.XmlEncode(log.Title));
            sb.Append("</title>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<machine>");
            sb.Append(SecurityHelper.XmlEncode(log.MachineName));
            sb.Append("</machine>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<applicationDomain>");
            sb.Append(SecurityHelper.XmlEncode(log.AppDomainName));
            sb.Append("</applicationDomain>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<processId>");
            sb.Append(SecurityHelper.XmlEncode(log.ProcessId));
            sb.Append("</processId>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<processName>");
            sb.Append(SecurityHelper.XmlEncode(log.ProcessName));
            sb.Append("</processName>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<threadName>");
            sb.Append(SecurityHelper.XmlEncode(log.ManagedThreadName));
            sb.Append("</threadName>");
            sb.Append(System.Environment.NewLine);
            sb.Append("</log>");
            return sb.ToString();
        }

        public static string FormatCategoriesCollection(ICollection<string> categories)
        {
            if (categories == null || categories.Count == 0)
            {
                return string.Empty;
            }

            var categoriesListBuilder = new StringBuilder();
            var i = 0;
            foreach (var category in categories)
            {
                categoriesListBuilder.Append(category);
                if (++i < categories.Count)
                {
                    categoriesListBuilder.Append(",");
                }
            }

            return categoriesListBuilder.ToString();
        }
    }
}