using Stone.Framework.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Stone.Framework.Common.Utility
{
    public class ObjectXmlSerializer
    {
        #region LoadFromXml
        /// <summary>
        /// deserialize an object from a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m_FileNamePattern"></param>
        /// <returns>
        /// Null is returned if any error occurs.
        /// </returns>
        public static T LoadFromXml<T>(String fileName) where T : class
        {
            return LoadFromXml<T>(fileName, true);
        }

        /// <summary>
        /// deserialize an object from a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="needLog"></param>
        /// <returns>
        ///  Null is returned if any error occurs.
        /// </returns>
        public static T LoadFromXml<T>(String fileName, Boolean needLog) where T : class
        {
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return (T)serializer.Deserialize(fs);
            }
            catch (Exception e)
            {
                if (needLog)
                {
                    LogLoadFileException(fileName, e);
                }
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        #endregion

        #region ToXml
        public static String ToStringXmlMessage<T>(T t, Boolean needLog) where T : class
        {
            StringWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                writer = new StringWriter();
                serializer.Serialize(writer, t);
                return writer.ToString();
            }
            catch
            {
                return String.Empty;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Dispose();
                }
            }
        }

        /// <summary>
        /// serialize an object to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns>
        /// Null is returned if any error occurs.
        /// </returns>
        public static String ToXML<T>(T instance)
        {
            UTF8StringWriter sr = null;
            try
            {
                XmlSerializer xr = new XmlSerializer(typeof(T));
                StringBuilder sb = new StringBuilder();

                sr = new UTF8StringWriter(sb);
                xr.Serialize(sr, instance);

                return (sb.ToString());
            }
            catch (Exception ex)
            {
                LogXmlSerializeException(instance.GetType().ToString(), ex);
                return null;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        private class UTF8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }

            public UTF8StringWriter(StringBuilder sb)
                : base(sb)
            {
            }
        }
        #endregion

        #region  FromXML
        public static T LoadFromXmlMessage<T>(String xmlMessage, Boolean needLog) where T : class
        {
            StringReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                reader = new StringReader(xmlMessage);
                return (T)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                if (needLog)
                {
                    LogXmlDeserializeException(xmlMessage, e);
                }
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
        }

        public static T FromXML<T>(String xml) where T : class
        {
            StringReader reader = null;
            try
            {
                XmlSerializer xr = new XmlSerializer(typeof(T));
                reader = new StringReader(xml);

                T result = (T)xr.Deserialize(reader);
                reader.Close();
                return result;
            }
            catch (Exception ex)
            {
                LogXmlDeserializeException(xml, ex);
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        #endregion

        #region Logging
        private const String LogCategory = "Framework.ObjectXmlSerializer";
        private const Int32 LogEventLoadFileException = 1;
        private const Int32 LogEventXmlDeserializeException = 2;
        private const Int32 LogEventXmlSerializeException = 3;

        [Conditional("TRACE")]
        private static void LogLoadFileException(String fileName, Exception ex)
        {
            LoggerFactory.CreateLogger().LogEvent(LogCategory, LogEventLoadFileException, fileName, ex.ToString());
        }

        [Conditional("TRACE")]
        private static void LogXmlDeserializeException(String fileName, Exception ex)
        {
            LoggerFactory.CreateLogger().LogEvent(LogCategory, LogEventXmlDeserializeException, fileName, ex.ToString());
        }

        [Conditional("TRACE")]
        private static void LogXmlSerializeException(String objectTypeName, Exception ex)
        {
            LoggerFactory.CreateLogger().LogEvent(LogCategory, LogEventXmlSerializeException, objectTypeName, ex.ToString());
        }
        #endregion
    }
}
