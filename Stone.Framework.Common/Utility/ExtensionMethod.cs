using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Stone.Framework.Common.Utility
{
    public static class ExtensionMethod
    {
        public static Boolean ExtensionIsNullOrEmpty(this String expression)
        {
            return String.IsNullOrEmpty(expression) || expression.Trim().Length == 0;
        }

        /// <summary>
        /// Trims the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>return trim value,if value can convert to String;otherwise,return null.</returns>
        public static String ExtensionTrim(this String value)
        {
            String convertible = Convert.ToString(value);
            return String.IsNullOrEmpty(convertible) ? String.Empty : convertible.Trim();
        }

        /// <summary>
        /// 繁体中文 -> 简体中文
        /// </summary>
        /// <returns></returns>
        public static String ExtensionSimplifiedChinese(this String expression)
        {
            return Strings.StrConv(expression, VbStrConv.SimplifiedChinese);
        }

        /// <summary>
        /// 简体中文 -> 繁体中文
        /// </summary>
        /// <returns></returns>
        public static String ExtensionTraditionalChinese(this String expression)
        {
            return Strings.StrConv(expression, VbStrConv.TraditionalChinese);
        }

        /// <summary>
        ///  判断日期是1900或者0001
        /// </summary>
        public static Boolean ExtensionIsDate1900(this DateTime expression)
        {
            string str = expression.ToString("yyyy");
            return expression == default(DateTime) || str.Equals("1900") || str.Equals("0001");
        }

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <returns></returns>
        public static Boolean ExtensionIsSafeSqlString(this String expression)
        {
            return !Regex.IsMatch(expression, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 判断对象是否是合法的数字.例如:33.2、30、-1均为合法.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Boolean ExtensionIsNumeric(this Object expression)
        {
            return Information.IsNumeric(expression);
        }

        /// <summary>
        /// 判断是否是合法时间
        /// </summary>
        /// <returns></returns>
        public static Boolean ExtensionIsTime(this String timeval)
        {
            return !String.IsNullOrEmpty(timeval) && Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// 判断是否为base64字符串
        /// </summary>
        /// <returns></returns>
        public static Boolean ExtensionIsBase64(this String expression)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(expression, @"[A-Za-z0-9\+\/\=]");
        }

        /// <summary>
        /// 判断是否符合email格式
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static Boolean ExtensionIsValidEmail(this String email)
        {
            return !String.IsNullOrEmpty(email) && Regex.IsMatch(email, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }

        public static Boolean ExtensionIsValidDoEmail(String email)
        {
            return !String.IsNullOrEmpty(email) && Regex.IsMatch(email, @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 判断是否是正确的Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Boolean ExtensionIsUrl(this String url)
        {
            return !String.IsNullOrEmpty(url) && Regex.IsMatch(url, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|Int32|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        /// <summary>
        /// 删除最后一个字符
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String ExtensionClearLastChar(this String expression)
        {
            return (String.IsNullOrEmpty(expression)) ? String.Empty : expression.Substring(0, expression.Length - 1);
        }

        /// <summary>
        ///  过滤字符串.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String ExtensionFilterBlankString(this String expression)
        {
            return String.IsNullOrEmpty(expression) ? String.Empty : expression.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }

        /// <summary>
        /// 将13923012322格式成139-2301-2322
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String ExtensionFormatMobile(this String expression)
        {
            return String.IsNullOrEmpty(expression) ? String.Empty : Regex.Replace(expression, "([\\d]{3})([\\d]{4})([\\d]{4})", "$1-$2-$3", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 生成指定长度的空格字串
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static String ExtensionBuildSpace(this String expression, Int32 number)
        {
            return Strings.Space(number);
        }

        /// <summary>
        /// 重复指定次数的字符.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static String ExtensionRepeat(this Char expression, Int32 number)
        {
            return Strings.StrDup(number, expression);
        }

        /// <summary>
        /// 重复指定次数的字符串.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String ExtensionRepeat(this String expression, Int32 number)
        {
            return Strings.StrDup(number, expression);
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String ExtensionReverse(this String expression)
        {
            return Strings.StrReverse(expression);
        }

        /// <summary>
        /// Lowercase first letter
        /// </summary>
        /// <returns></returns>
        public static String ExtensionLCase(this String expression)
        {
            return Strings.LCase(expression);
        }

        /// <summary>
        /// 右截指定长度的字符串.例如:("Microsoft",2)则为ft
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String ExtensionRight(this String expression, Int32 len)
        {
            return Strings.Right(expression, len);
        }

        /// <summary>
        /// 左截指定长度的字符串
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String ExtensionLeft(this String expression, Int32 len)
        {
            return Strings.Left(expression, len);
        }

        /// <summary>
        /// 左截指定长度的字符串
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String ExtensionLeftAccurate(this String expression, Int32 len)
        {
            if (len <= 0 || expression.ExtensionIsNullOrEmpty())
            {
                return String.Empty;
            }
            expression = HttpUtility.HtmlEncode(expression);
            if (expression.ExtensionByteLen() <= len)
            {
                return expression;
            }

            String str = String.Empty;
            Int32 num, startIndex = 0, num3 = 0;
            while (num3 <= (len - 1))
            {
                var aStr = expression.Substring(startIndex, 1);
                num = aStr.ExtensionByteLen();
                if (1 == num)
                {
                    num3++;
                }
                else
                {
                    num3 += num;
                }
                if (num3 <= len)
                {
                    str = str + aStr;
                }
                startIndex++;
            }
            return str;
        }

        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String ExtensionSBCToDBCNumber(this String expression)
        {
            Char[] c = expression.ToCharArray();
            for (Int32 i = 0; i < c.Length; i++)
            {
                Byte[] b = Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (Byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new String(c);
        }

        /// <summary>
        /// String convert to bool
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Boolean ExtensionToBool(this String expression, Boolean defaultValue)
        {
            if (String.IsNullOrEmpty(expression))
            {
                return defaultValue;
            }

            Boolean n;
            return Boolean.TryParse(expression, out n) ? n : defaultValue;
        }

        /// <summary>
        /// T to int64
        /// </summary>
        public static Int64 ExtensionToInt64<T>(this T expression)
        {
            return ExtensionToInt64(expression, 0);
        }

        /// <summary>
        /// T to int64
        /// </summary>
        public static Int64 ExtensionToInt64<T>(this T expression, Int64 defaultValue)
        {
            string input = expression as string;
            if (input.ExtensionIsNullOrEmpty())
            {
                return defaultValue;
            }

            Int64 n;
            return Int64.TryParse(input, out n) ? n : defaultValue;
        }

        /// <summary>
        /// T to double
        /// </summary>
        public static Double ExtensionToDouble<T>(this T expression)
        {
            string input = expression as string;
            Double n;
            return Double.TryParse(input, out n) ? n : n;
        }

        /// <summary>
        /// T to integer
        /// </summary>
        public static Int32 ExtensionToInt<T>(this T expression)
        {
            return ExtensionToInt(expression, 0);
        }

        /// <summary>
        /// T to Integer
        /// </summary>
        public static Int32 ExtensionToInt<T>(this T expression, Int32 defaultValue)
        {
            string input = expression as string;
            if (input.ExtensionIsNullOrEmpty())
            {
                return defaultValue;
            }

            int n;
            return Int32.TryParse(input, out n) ? n : defaultValue;
        }

        /// <summary>
        /// T to float
        /// </summary>
        public static float ExtensionToFloat<T>(this T expression)
        {
            return ExtensionToFloat(expression, 0);
        }

        /// <summary>
        /// T to float
        /// </summary>
        public static float ExtensionToFloat<T>(this T expression, float defaultValue)
        {
            if (expression == null)
            {
                return defaultValue;
            }

            float n;
            return float.TryParse(expression.ToString(), out n) ? n : defaultValue;
        }

        /// <summary>
        ///  T to dateTime
        /// </summary>
        public static DateTime ExtensionToDateTime<T>(this T expression)
        {
            return ExtensionToDateTime(expression, DateTime.Now);
        }

        /// <summary>
        ///  Convert to yyyy/MM/dd String
        /// </summary>
        public static String ExtensionDateTimeFormat(this DateTime expression)
        {
            return expression.ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// T to dateTime
        /// </summary>
        public static DateTime ExtensionToDateTime<T>(this T expression, DateTime defaultValue)
        {
            string input = Convert.ToString(expression);
            if (input.ExtensionIsNullOrEmpty())
            {
                return defaultValue;
            }

            DateTime dateTime;
            return DateTime.TryParse(input, out dateTime) ? dateTime : defaultValue;
        }

        /// <summary>
        /// 删除重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExtensionDistinct<T>(T[] array)
        {
            return array.Distinct();
        }

        /// <summary>
        /// Array to List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IList<T> ExtensionToList<T>(T[] array)
        {
            if (array == null || array.Length <= 0) return null;
            IList<T> listT = new List<T>(array.Length);
            foreach (T obj in array.Where(obj => obj != null))
            {
                listT.Add(obj);
            }
            return listT;
        }

        /// <summary>
        /// 根据输入的分隔符,将数组输出成一串字符
        /// </summary>
        /// <param name="split"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static String ExtensionArrayToString(this String[] array, String split)
        {
            return String.Join(",", Array.ConvertAll(array, Convert.ToString));
        }

        /// <summary>
        /// Split a string
        /// </summary>
        /// <example>ExtensionSplit("1|2|3|4", "|")</example>
        public static String[] ExtensionSplit(this String expression, String split)
        {
            if (!expression.ExtensionIsNullOrEmpty())
            {
                if (expression.IndexOf(split, StringComparison.Ordinal) < 0)
                {
                    return new String[] { expression };
                }

                return Regex.Split(expression, Regex.Escape(split), RegexOptions.IgnoreCase);
            }

            return new String[] { };
        }

        /// <summary>
        /// Split a string
        /// </summary>
        /// <returns></returns>
        /// <example>ExtensionSplit("1|2|3|4", "|")</example>
        public static String[] ExtensionSplit(this String expression, String split, Int32 limit)
        {
            String[] result = new String[limit];
            String[] splited = ExtensionSplit(expression, split);

            for (Int32 i = 0; i < limit; i++)
            {
                result[i] = i < splited.Length ? splited[i] : String.Empty;
            }

            return result;
        }

        /// <summary>
        /// 统计字符串sin在str中出现的次数
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public static Int32 ExtensionSubStringTimes(this String expression, String sub)
        {
            Int32 i = 0, ibit = 0;
            while (true)
            {
                ibit = expression.IndexOf(sub, ibit, StringComparison.Ordinal);
                if (ibit > 0)
                {
                    ibit += sub.Length;
                    i++;
                }
                else
                {
                    break;
                }
            }
            return i;
        }

        /// <summary>
        /// Get byte length
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Int32 ExtensionByteLen(this String expression)
        {
            Int32 length = 0;
            if (expression != null)
            {
                length = Encoding.Default.GetBytes(expression).Length;
            }
            return length;
        }

        /// <summary>
        /// 判断指定符号在字符串中出现的位置
        /// </summary>
        /// <returns></returns>
        public static Int32[] FindPositions(this String expression, String sub)
        {
            MatchCollection matchs = Regex.Matches(expression, sub);
            String str = String.Empty;
            foreach (Match match in matchs)
            {
                str = str + match.Index + ",";
            }
            String[] strArray = str.Split(new Char[] { ',' });
            Int32[] numArray = new Int32[strArray.Length - 1];
            for (Int32 i = 0; i < (strArray.Length - 1); i++)
            {
                numArray[i] = Convert.ToInt32(strArray[i]);
            }
            return numArray;
        }
    }
}