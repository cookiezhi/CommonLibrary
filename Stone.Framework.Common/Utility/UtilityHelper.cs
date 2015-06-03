
using Microsoft.International.Converters.PinYinConverter;
using Microsoft.International.Formatters;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace Stone.Framework.Common.Utility
{
    public class UtilityHelper
    {
        private UtilityHelper()
        {
        }

        /// <summary>
        /// 格式化字节数字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatBytesStr(int bytes)
        {
            if (bytes > 1073741824)
                return ((double)(bytes / 1073741824)).ToString("0") + "G";

            if (bytes > 1048576)
                return ((double)(bytes / 1048576)).ToString("0") + "M";

            if (bytes > 1024)
                return ((double)(bytes / 1024)).ToString("0") + "K";

            return bytes + "Bytes";
        }

        /// <summary>
        /// 截取指定长度字符串,会考虑汉字英文.例如:中华人民共和...
        /// </summary>
        /// <param name="text"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FormatTitle(string text, Int32 len)
        {
            if ((text == null) || (len <= 0))
            {
                return "...";
            }

            var encodeText = HttpUtility.HtmlEncode(text);

            if (len >= encodeText.ExtensionByteLen())
            {
                return encodeText;
            }
            if (len >= 3)
            {
                return text.ExtensionLeftAccurate(len - 3) + "...";
            }
            return text.ExtensionLeftAccurate(len);
        }

        /// <summary>
        /// 修正EastAsiaNumericFormatter.FormatWithCulture出現"三百十"之問題，
        /// 本函数会将其修正为三百一十的惯用写法
        /// </summary>
        /// <param name="n">要转换的数字</param>
        /// <param name="moneyChar">
        /// 是否使用金额大写，true时使用"一贰参肆...", false时则为"一二三四..."
        /// </param>
        /// <returns>转为中文大写的数字</returns>
        public static string GetChineseToNumber(decimal n, bool moneyChar)
        {
            var t = EastAsiaNumericFormatter.FormatWithCulture(moneyChar ? "L" : "Ln", n, null,
                new CultureInfo("zh-TW"));

            var pattern = moneyChar ? "[^壹貳參肆伍陸柒捌玖]拾" : "[^一二三四五六七八九]十";
            var one = moneyChar ? "壹" : "一";
            return Regex.Replace(t, pattern, m => m.Value.Substring(0, 1) + one + m.Value.Substring(1));
        }

        /// <summary>
        /// 將字串的数字部分转为中文大写
        /// </summary>
        /// <param name="s">原始字符串</param>
        /// <param name="moneyChar">
        /// 是否使用金额大写，true时使用"一贰参肆...", false时则为"一二三四..."
        /// </param>
        /// <returns>转为中文大写的数字</returns>
        public static string GetChineseToNumber(string s, bool moneyChar)
        {
            return Regex.Replace(s, "\\d+", m =>
            {
                int n = int.Parse(m.Value);
                return GetChineseToNumber(n, moneyChar);
            });
        }

        /// <summary> 
        /// 汉字转化为拼音首字母
        /// </summary> 
        /// <param name="str">汉字</param> 
        /// <returns>首字母</returns> 
        public static string GetFirstPinyin(string str)
        {
            var r = string.Empty;
            foreach (var obj in r)
            {
                try
                {
                    var chineseChar = new ChineseChar(obj);
                    var t = chineseChar.Pinyins[0];
                    r += t.Substring(0, 1);
                }
                catch (Exception)
                {
                    r += obj.ToString();
                    throw;
                }
            }
            return r;
        }

        /// <summary> 
        /// 汉字转化为拼音
        /// </summary> 
        /// <param name="str">汉字</param> 
        /// <returns>全拼</returns> 
        public static string GetPinyin(string str)
        {
            var r = string.Empty;
            foreach (var obj in str)
            {
                try
                {
                    var chineseChar = new ChineseChar(obj);
                    var t = chineseChar.Pinyins[0];
                    r += t.Substring(0, t.Length - 1);
                }
                catch (Exception)
                {
                    r += obj.ToString();
                }
            }
            return r;
        }
    }
}