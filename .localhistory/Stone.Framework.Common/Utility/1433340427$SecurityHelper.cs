
﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;



namespace Stone.Framework.Common.Utility
{
    public class SecurityHelper
    {
        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns></returns>
        public static bool IsGet
        {
            get { return HttpContext.Current.Request.HttpMethod.Equals("GET"); }
        }

        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns></returns>
        public static bool IsPost
        {
            get { return HttpContext.Current.Request.HttpMethod.Equals("POST"); }
        }

        /// <summary>
        /// 判断是否为IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Boolean IsIp(String ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>
        /// 仅将移除一些敏感的HTML标签元素.例如:iframe、script、Object、javascript等.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string FilterScript(Object html)
        {
            var input = html as string;
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            input = new Regex(@"<script[^>]*>(.|\n)*<\/script>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            input = new Regex("<meta[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            input = new Regex(@"<!--[^>]*>(.|\n)*<\-->", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            input = new Regex(@"<iframe[^>]*>(.|\n)*<\/iframe>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            input = new Regex(@"<Object[^>]*>(.|\n)*<\/Object>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            var regex = new Regex("javascript:", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return regex.Replace(input, "javascript :");
        }

        /// <summary>
        /// 根据cookie名称获取cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static string GetCookie(string cookieName)
        {
            var str = string.Empty;
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                str = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.Cookies[cookieName].Value);
            }
            return str;
        }

        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieKey"></param>
        /// <returns></returns>
        public static string GetCookie(string cookieName, string cookieKey)
        {
            var str = string.Empty;
            if ((HttpContext.Current.Request.Cookies[cookieName] != null) &&
                (HttpContext.Current.Request.Cookies[cookieName][cookieKey] != null))
            {
                str = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.Cookies[cookieName][cookieKey]);
            }
            return str;
        }

        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetServerString(string name)
        {
            return HttpContext.Current.Request.ServerVariables[name] ?? string.Empty;
        }

        /// <summary>
        /// 将文本中所有的HTML标记给移除掉,只保留存文本
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlFilter(Object html)
        {
            var input = html as String;
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            input = new Regex(@"<!--(.|\n)*?-->", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            input = new Regex(@"<script[^>]*>(.|\n)*?<\/script>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            input = new Regex(@"<style[^>]*>(.|\n)*?<\/style>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, " ");
            input = new Regex("\\son[a-zA-Z]+=[\\\"|\\']?[^\\'\\\"]*[\\\"|\\']?", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            input = new Regex("</?[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            input = new Regex("&[a-zA-Z]+;", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            var regex = new Regex(@"\s\s{1,}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return regex.Replace(input, "");
        }

        /// <summary>
        /// 生成不可逆的Hash加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Md5(string text)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5").ToLower();
        }

        /// <summary>
        /// 过滤NamingContainer(删除ASP.NET控件的垃圾UniqueID名称方法)
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string NamingContainerFilter(string html)
        {
            var opt = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant |
                               RegexOptions.Compiled;
            var regex = new Regex("( name=\")(?=.*(" + Regex.Escape("$") + "))([^\"]+?)(\")", opt);
            html = regex.Replace(html, delegate(Match m)
            {
                var lastDollarSignIndex = m.Value.LastIndexOf('$');
                if (lastDollarSignIndex >= 0)
                {
                    return m.Groups[1].Value + m.Value.Substring(lastDollarSignIndex + 1);
                }
                else
                {
                    return m.Value;
                }
            });
            return html;
        }

        /// <summary>
        /// 301 跳转
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static int Redirect301(string url)
        {
            if (!url.ExtensionIsNullOrEmpty())
            {
                url = string.Empty;
            }

            HttpContext.Current.Response.StatusCode = 0x12d; //状态码
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", url);
            HttpContext.Current.Response.End();
            return 0;
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(String content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        /// <summary>
        /// 移除HTML文档中的空格
        /// </summary>
        /// <param name="contentInBuffer"></param>
        public static string RemoveWhitespace(string contentInBuffer)
        {
            var tabsRe = new Regex("\\t", RegexOptions.Compiled | RegexOptions.Multiline);
            var carriageReturnRe = new Regex(">[\\s]*\\r\\n[\\s]*<", RegexOptions.Compiled | RegexOptions.Multiline);
            var carriageReturnSafeRe = new Regex("\\r\\n\\s", RegexOptions.Compiled | RegexOptions.Multiline);
            var multipleSpaces = new Regex("  ", RegexOptions.Compiled | RegexOptions.Multiline);
            var spaceBetweenTags = new Regex(">\\s<", RegexOptions.Compiled | RegexOptions.Multiline);

            //去掉<script>前的换行
            Regex spaceScript = new Regex(">([\\s]*)<script ", RegexOptions.Compiled | RegexOptions.Multiline);

            // Strip out all whitespace... kill all tabs, replace carriage returns with a space, and compress multiple spaces
            contentInBuffer = tabsRe.Replace(contentInBuffer, string.Empty);

            contentInBuffer = carriageReturnRe.Replace(contentInBuffer, "><");
            contentInBuffer = carriageReturnSafeRe.Replace(contentInBuffer, " ");

            while (multipleSpaces.IsMatch(contentInBuffer))
                contentInBuffer = multipleSpaces.Replace(contentInBuffer, " ");

            contentInBuffer = spaceBetweenTags.Replace(contentInBuffer, "><");

            //去掉<script>前的换行
            contentInBuffer = spaceScript.Replace(contentInBuffer, "><script ");
            contentInBuffer = contentInBuffer.Replace("\r\n<!DOCTYPE", "<!DOCTYPE");

            //去掉<title></title>换行
            contentInBuffer = contentInBuffer.Replace("<title>\r\n", "<title>");
            contentInBuffer = contentInBuffer.Replace("\r\n</title>", "</title>");
            return contentInBuffer;
        }

        /// <summary>
        /// Receive the post value Float
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float RequestFormFloat(string key)
        {
            string str = HttpContext.Current.Request.Form[key];
            return str.ExtensionIsNumeric() ? str.ExtensionToFloat() : 0;
        }

        /// <summary>
        /// Receive the post value Int
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int RequestFormNum(string key)
        {
            string str = HttpContext.Current.Request.Form[key];
            return str.ExtensionIsNumeric() ? str.ExtensionToInt() : 0;
        }

        /// <summary>
        /// Receive the post value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RequestFormStr(string key)
        {
            string str = HttpContext.Current.Request.Form[key];
            return str.ExtensionIsNullOrEmpty() ? string.Empty : str.Trim().Replace("'", "''");
        }

        /// <summary>
        /// Receive the get value Float
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float RequestQueryFloat(string key)
        {
            string str = HttpContext.Current.Request.QueryString[key];
            return str.ExtensionIsNumeric() ? str.ExtensionToFloat() : 0.0F;
        }

        /// <summary>
        /// Receive the get value Int
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int RequestQueryNum(string key)
        {
            string str = HttpContext.Current.Request.QueryString[key];
            return str.ExtensionIsNumeric() ? str.ExtensionToInt() : 0;
        }

        /// <summary>
        /// Receive the get value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RequestQueryStr(string key)
        {
            return RequestQuertStr(key, false);
        }

        /// <summary>
        /// Object to Int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToNum(object obj)
        {
            if (obj != null)
            {
                try
                {
                    return obj.ExtensionToInt();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// String to Int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToNum(string str)
        {
            if (!str.ExtensionIsNullOrEmpty())
            {
                return str.ExtensionToInt();
            }
            return 0;
        }

        /// <summary>
        /// String to Long
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ToNum64(string str)
        {
            if (!str.ExtensionIsNullOrEmpty())
            {
                return str.ExtensionToInt64();
            }
            return 0;
        }

        /// <summary>
        /// Object to String
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStr(object obj)
        {
            if (obj != null)
            {
                try
                {
                    return obj.ToString().Trim();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// String to String
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToStr(string str)
        {
            if (!str.ExtensionIsNullOrEmpty())
            {
                return str.Trim();
            }

            return string.Empty;
        }

        /// <summary>
        /// 过滤掉UBB标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string UbbFilter(string html)
        {
            return Regex.Replace(html, @"\[[^\]]*?\]", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤ViewState
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ViewStateFilter(string html)
        {
            var mstr1 = "type=\"hidden\" name=\"__VIEWSTATE\" id=\"__VIEWSTATE\"";
            var mstr2 = "type=\"hidden\" name=\"__EVENTVALIDATION\" id=\"__EVENTVALIDATION\"";
            var mstr3 = "type=\"hidden\" name=\"__EVENTTARGET\" id=\"__EVENTTARGET\"";
            var mstr4 = "type=\"hidden\" name=\"__EVENTARGUMENT\" id=\"__EVENTARGUMENT\"";

            var positiveLookahead1 = "(?=.*(" + Regex.Escape(mstr1) + "))";
            var positiveLookahead2 = "(?=.*(" + Regex.Escape(mstr2) + "))";
            var positiveLookahead3 = "(?=.*(" + Regex.Escape(mstr3) + "))";
            var positiveLookahead4 = "(?=.*(" + Regex.Escape(mstr4) + "))";

            var opt = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled;

            var arrRegex = new[] 
            {   
                new Regex("\\s*<div>" + positiveLookahead1 + "(.*?)</div>\\s*", opt),   
                new Regex("\\s*<div>" + positiveLookahead2 + "(.*?)</div>\\s*", opt),   
                new Regex("\\s*<div>" + positiveLookahead3 + "(.*?)</div>\\s*", opt),   
                new Regex("\\s*<div>" + positiveLookahead3 + "(.*?)</div>\\s*", opt),   
                new Regex("\\s*<div>" + positiveLookahead4 + "(.*?)</div>\\s*", opt)   
            };

            return arrRegex.Aggregate(html, (current, item) => item.Replace(current, string.Empty));
        }

        /// <summary>
        /// 删除页面空白
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string WhitespaceFilter(string html)
        {
            var tabRe = new Regex("\\t", RegexOptions.Compiled | RegexOptions.Multiline);
            var carriageReturnRe = new Regex(">\\r\\\n<", RegexOptions.Compiled | RegexOptions.Multiline);
            var carriageReturnSafeRe = new Regex("\\r\\n", RegexOptions.Compiled | RegexOptions.Multiline);
            var multipleSpaces = new Regex("  ", RegexOptions.Compiled | RegexOptions.Multiline);
            var spaceBetweenTags = new Regex(">\\s<", RegexOptions.Compiled | RegexOptions.Multiline);

            html = tabRe.Replace(html, string.Empty);
            html = carriageReturnRe.Replace(html, string.Empty);
            html = carriageReturnSafeRe.Replace(html, string.Empty);

            while (multipleSpaces.IsMatch(html))
            {
                html = multipleSpaces.Replace(html, " ");
            }

            html = spaceBetweenTags.Replace(html, "><");

            html = html.Replace("//<![CDATA[", "");
            html = html.Replace("//]]>", "");
            return html;
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ?? new HttpCookie(strName);
            cookie.Value = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="key"></param>
        /// <param name="strValue"></param>
        public static void WriteCookie(string strName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ?? new HttpCookie(strName);
            cookie[key] = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        /// <param name="expires">过期时间 (Min)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName] ?? new HttpCookie(strName);
            cookie.Value = strValue;
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// Receive the get value
        /// </summary> 
        /// <param name="key"></param>
        /// <param name="isSafeCheck">是否进行SQL安全检查</param>
        /// <returns></returns>
        private static string RequestQuertStr(string key, bool isSafeCheck)
        {
            string str = HttpContext.Current.Request.QueryString[key];

            if (str.ExtensionIsNullOrEmpty())
            {
                return string.Empty;
            }

            if (isSafeCheck && !str.ExtensionIsSafeSqlString())
            {
                return "unsafe";
            }

            return str.ExtensionTrim().Replace("'", "''");
        }
    }
}

