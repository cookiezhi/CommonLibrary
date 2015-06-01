using System;
using System.Text;

namespace Stone.Framework.Common.Utility
{
    public static class AntiXssEncoder
    {
        private const string EmptyStringJavaScripts = "''";
        private const string EmptyStringVbs = "\"\"";

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private static string EncodeHtml(string strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder("", strInput.Length * 2);

            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch == ' ') || ((ch > '/') && (ch < ':'))) || (((ch == '.') || (ch == ',')) || ((ch == '-') || (ch == '_')))))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append("&#" + ((Int32)ch) + ";");
                }
            }
            return builder.ToString();
        }

        private static string EncodeHtmlAttribute(string strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder("", strInput.Length * 2);
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch > '/') && (ch < ':')) || (((ch == '.') || (ch == ',')) || ((ch == '-') || (ch == '_')))))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append("&#" + ((Int32)ch).ToString() + ";");
                }
            }
            return builder.ToString();
        }
        private static String EncodeJs(String strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return "\"\"";
            }
            StringBuilder builder = new StringBuilder("\"", strInput.Length * 2);
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch == ' ') || ((ch > '/') && (ch < ':'))) || (((ch == '.') || (ch == ',')) || ((ch == '-') || (ch == '_')))))
                {
                    builder.Append(ch);
                }
                else if (ch > '\x007f')
                {
                    builder.Append(@"\u" + TwoByteHex(ch));
                }
                else
                {
                    builder.Append(@"\x" + SingleByteHex(ch));
                }
            }
            builder.Append("\"");
            return builder.ToString();
        }

        private static String EncodeUrl(String strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return String.Empty;
            }
            StringBuilder builder = new StringBuilder("", strInput.Length * 2);
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch > '/') && (ch < ':')) || (((ch == '.') || (ch == '-')) || (ch == '_'))))
                {
                    builder.Append(ch);
                }
                else if (ch > '\x007f')
                {
                    builder.Append("%u" + TwoByteHex(ch));
                }
                else
                {
                    builder.Append("%" + SingleByteHex(ch));
                }
            }
            return builder.ToString();
        }

        private static String EncodeVbs(String strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            if (strInput.Length == 0)
            {
                return "\"\"";
            }
            StringBuilder builder = new StringBuilder("", strInput.Length * 2);
            Boolean flag = false;
            foreach (char ch in strInput)
            {
                if ((((ch > '`') && (ch < '{')) || ((ch > '@') && (ch < '['))) || (((ch == ' ') || ((ch > '/') && (ch < ':'))) || (((ch == '.') || (ch == ',')) || ((ch == '-') || (ch == '_')))))
                {
                    if (!flag)
                    {
                        builder.Append("&\"");
                        flag = true;
                    }
                    builder.Append(ch);
                }
                else
                {
                    if (flag)
                    {
                        builder.Append("\"");
                        flag = false;
                    }
                    builder.Append("&chrw(" + ((uint)ch).ToString() + ")");
                }
            }
            if ((builder.Length > 0) && (builder[0] == '&'))
            {
                builder.Remove(0, 1);
            }
            if (builder.Length == 0)
            {
                builder.Insert(0, "\"\"");
            }
            if (flag)
            {
                builder.Append("\"");
            }
            return builder.ToString();
        }

        private static String EncodeXml(String strInput)
        {
            return EncodeHtml(strInput);
        }

        private static String EncodeXmlAttribute(String strInput)
        {
            return EncodeHtmlAttribute(strInput);
        }

        public static String HtmlAttributeEncode(String s)
        {
            return EncodeHtmlAttribute(s);
        }

        public static String HtmlEncode(String s)
        {
            return EncodeHtml(s);
        }

        public static String HtmlEncodeUseLineBreak(String s)
        {
            return EncodeHtml(s).Replace("&#13;&#10;", "<br>").Replace("&#10;", "<br>").Replace("&#60;br&#62;", "<br>").Replace("&#60;br&#47;&#62;", "<br>");
        }

        public static String JavaScriptEncode(String s)
        {
            return EncodeJs(s);
        }

        private static String SingleByteHex(char c)
        {
            uint num = c;
            return num.ToString("x").PadLeft(2, '0');
        }

        private static String TwoByteHex(char c)
        {
            uint num = c;
            return num.ToString("x").PadLeft(4, '0');
        }

        public static String UrlEncode(String s)
        {
            return EncodeUrl(s);
        }

        public static String VisualBasicScriptEncode(String s)
        {
            return EncodeVbs(s);
        }

        public static String XmlAttributeEncode(String s)
        {
            return EncodeXmlAttribute(s);
        }

        public static String XmlEncode(String s)
        {
            return EncodeXml(s);
        }
    }
}

