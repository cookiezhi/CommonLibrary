using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace Stone.Framework.Common.Utility
{
    public class SecurityHelper
    {
        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns></returns>
        public static bool IsPost
        {
            get { return HttpContext.Current.Request.HttpMethod.Equals("POST"); }
        }

        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns></returns>
        public static bool IsGet
        {
            get { return HttpContext.Current.Request.HttpMethod.Equals("GET"); }
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

        /// <summary>
        /// Receive the post value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RequestFormStr(string key)
        {
            var str = HttpContext.Current.Request.Form[key];
            return str.ExtensionIsNullOrEmpty() ? string.Empty : str.Trim().Replace("'", "''");
        }

        /// <summary>
        /// Receive the get value Int
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int RequestQueryNum(string key)
        {
            var str = HttpContext.Current.Request.QueryString[key];
            return str.ExtensionIsNumeric() ? str.ExtensionToInt() : 0;
        }

        /// <summary>
        /// Receive the get value Float
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float RequestQueryFloat(string key)
        {
            var str = HttpContext.Current.Request.QueryString[key];
            return str.ExtensionIsNumeric() ? str.ExtensionToFloat() : 0.0F;
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
        /// 生成不可逆的Hash加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Md5(string text)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5").ToLower();
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
        /// 清除UBB标签
        /// </summary>
        /// <param name="sDetail"></param>
        /// <returns></returns>
        public static String UBBFilter(String html)
        {
            return Regex.Replace(html, @"\[[^\]]*?\]", String.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 将文本中所有的HTML标记给移除掉,只保留存文本
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static String HTMLFilter(Object html)
        {
            String input = html as String;
            if (String.IsNullOrEmpty(input))
            {
                return String.Empty;
            }

            input = new Regex(@"<!--(.|\n)*?-->", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            input = new Regex(@"<script[^>]*>(.|\n)*?<\/script>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            input = new Regex(@"<style[^>]*>(.|\n)*?<\/style>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, " ");
            input = new Regex("\\son[a-zA-Z]+=[\\\"|\\']?[^\\'\\\"]*[\\\"|\\']?", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            input = new Regex("</?[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            input = new Regex("&[a-zA-Z]+;", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, String.Empty);
            Regex regex = new Regex(@"\s\s{1,}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return regex.Replace(input, "");
        }

        /// <summary>
        /// 仅将移除一些敏感的HTML标签元素.例如:iframe、script、Object、javascript等.
        /// </summary>
        /// <param name="htmltext"></param>
        /// <returns></returns>
        public static String FilterScript(Object html)
        {
            String input = html as String;
            if (String.IsNullOrEmpty(input))
            {
                return String.Empty;
            }

            input = new Regex(@"<script[^>]*>(.|\n)*<\/script>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            input = new Regex("<meta[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            input = new Regex(@"<!--[^>]*>(.|\n)*<\-->", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            input = new Regex(@"<iframe[^>]*>(.|\n)*<\/iframe>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            input = new Regex(@"<Object[^>]*>(.|\n)*<\/Object>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
            Regex regex = new Regex("javascript:", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return regex.Replace(input, "javascript :");
        }

        /// <summary>
        /// 移除HTML文档中的空格
        /// </summary>
        /// <param name="contentInBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public static String RemoveWhitespace(String contentInBuffer)
        {
            Regex tabsRe = new Regex("\\t", RegexOptions.Compiled | RegexOptions.Multiline);
            Regex carriageReturnRe = new Regex(">[\\s]*\\r\\n[\\s]*<", RegexOptions.Compiled | RegexOptions.Multiline);
            Regex carriageReturnSafeRe = new Regex("\\r\\n\\s", RegexOptions.Compiled | RegexOptions.Multiline);
            Regex multipleSpaces = new Regex("  ", RegexOptions.Compiled | RegexOptions.Multiline);
            Regex spaceBetweenTags = new Regex(">\\s<", RegexOptions.Compiled | RegexOptions.Multiline);

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
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static String RemoveHtml(String content)
        {
            content = Regex.Replace(content, @"<[^>]*>", String.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"&ldquo;", String.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"&rdquo;", String.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"&nbsp;", String.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"<br/>", String.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"<br>", String.Empty, RegexOptions.IgnoreCase);
            return Regex.Replace(content, @"&mdash;", String.Empty, RegexOptions.IgnoreCase);

            //content = Regex.Replace(content, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            //return Regex.Replace(content, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, @"&#(\d+);", "", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static String RemoveUnsafeHtml(String content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static String GetTextFromHTML(String html)
        {
            Regex regex = new Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return regex.Replace(html, "");
        }

        /// <summary>
        /// UBB转HTML
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String UBBToHTML(String str)
        {
            Regex r;
            Match m;
            str = Regex.Replace(str, " ", "&nbsp;", RegexOptions.IgnoreCase);   //处理空格
            str = Regex.Replace(str, "'", "’", RegexOptions.IgnoreCase);       //单引号
            str = Regex.Replace(str, "\"", "&quot;", RegexOptions.IgnoreCase);  //双引号
            str = Regex.Replace(str, "<", "&lt;", RegexOptions.IgnoreCase);     //html标记符
            str = Regex.Replace(str, ">", "&gt;", RegexOptions.IgnoreCase);     //html标记符

            #region 处理换行

            //处理换行，在每个新行的前面添加两个全角空格
            r = new Regex(@"(\r\n((&nbsp;)|　)+)(?<正文>\S+)", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "<BR>　　" + m.Groups["正文"].ToString());
            }
            //处理换行，在每个新行的前面添加两个全角空格
            str = str.Replace("\r\n", "<br/>");

            #endregion 处理换行

            str = Regex.Replace(str, @"(\[b\])(.+?)(\[\/b\])", "<b>$2</b>", RegexOptions.IgnoreCase);                   //[b]
            str = Regex.Replace(str, @"(\[i\])([ \S\t]*?)(\[\/i\])", "<i>$2</i>", RegexOptions.IgnoreCase);             //[i]
            str = Regex.Replace(str, @"(\[u\])([ \S\t]*?)(\[\/u\])", "<u>$2</u>", RegexOptions.IgnoreCase);             //[u]
            str = Regex.Replace(str, @"((\r\n)*\[p\])(.*?)((\r\n)*\[\/p\])", "<p>$3</p>", RegexOptions.IgnoreCase);     //[p]
            str = Regex.Replace(str, @"(\[sup\])([ \S\t]*?)(\[\/sup\])", "<sup>$2</sup>", RegexOptions.IgnoreCase);     //[sup]
            str = Regex.Replace(str, @"(\[sub\])([ \S\t]*?)(\[\/sub\])", "<sub>$2</sub>", RegexOptions.IgnoreCase);     //[sub]
            str = Regex.Replace(str, @"(\[del\])([ \S\t]*?)(\[\/del\])", "<del>$2</del>", RegexOptions.IgnoreCase);     //[del]
            str = Regex.Replace(str, @"(\[url\])([ \S\t]*?)(\[\/url\])", "<a href='$2' target='_blank'>$2</a>", RegexOptions.IgnoreCase);               //[url=http://www.sohu.com][/url]
            str = Regex.Replace(str, @"(\[url=([ \S\t]+)\])([ \S\t]*?)(\[\/url\])", "<a href='$2' target='_blank'>$3</a>", RegexOptions.IgnoreCase);     //[url=http://www.sohu.com]sohu[/url]
            str = Regex.Replace(str, @"(\[email\])([ \S\t]*?)(\[\/email\])", "<a href='mailto:' target='_blank'>$2</a>", RegexOptions.IgnoreCase);      //[email]
            str = Regex.Replace(str, @"(\[email=([ \S\t]+)\])([ \S\t]*?)(\[\/email\])", "<a href='mailto:$2' target='_blank'>$3</a>", RegexOptions.IgnoreCase);    //[email]
            str = Regex.Replace(str, @"(\[size=([1-7])\])([ \S\t]*?)(\[\/size\])", "<font size='$2'>$3</font>", RegexOptions.IgnoreCase);       //[size]
            str = Regex.Replace(str, @"(\[color=([\S]+)\])([ \S\t]*?)(\[\/color\])", "<font color='$2'>$3</font>", RegexOptions.IgnoreCase);        //[color=x][/color]
            str = Regex.Replace(str, @"(\[font=([\S]+)\])([ \S\t]*?)(\[\/font\])", "<font face='$2'>$3</font>", RegexOptions.IgnoreCase);           //[font=x][/font]
            str = Regex.Replace(str, @"(\[align=([\S]+)\])(.+?)(\[\/align\])", "<p align='$2'>$3</p>", RegexOptions.IgnoreCase);           //[align=x][/align]
            str = Regex.Replace(str, @"(\[img\])(http|https|ftp):\/\/([ \S\t]*?)(\[\/img\])", "<a onfocus='this.blur()' href='$2://$3' target='_blank'><img src=$2://$3 border='0' alt='按此在新窗口浏览图片' onload='javascript:if(screen.width-333<this.width)this.width=screen.width-333' /></a>", RegexOptions.IgnoreCase);   //[img][/img]
            str = Regex.Replace(str, @"(\[img=(http|https|ftp):\/\/([ \S\t]*?)])(\[\/img\])", "<a onfocus='this.blur()' href='$2://$3' target='_blank'><img src=$2://$3 border='0' alt='按此在新窗口浏览图片' onload='javascript:if(screen.width-333<this.width)this.width=screen.width-333' /></a>", RegexOptions.IgnoreCase);   //[img][/img]
            str = Regex.Replace(str, @"(\[image\])([ \S\t]*?)(\[\/image\])", "<img src='$2' border='0' align='middle' /><br>", RegexOptions.IgnoreCase);        //[image][/image]

            #region 处理图片链接

            //处理图片链接
            r = new Regex("\\[picture\\](\\d+?)\\[\\/picture\\]", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<A href=\"ShowImage.aspx?Type=ALL&Action=forumImage&ImageID=" + m.Groups[1].ToString() +
                 "\" target=\"_blank\"><IMG border=0 Title=\"点击打开新窗口查看\" src=\"ShowImage.aspx?Action=forumImage&ImageID=" + m.Groups[1].ToString() +
                 "\"></A>");
            }

            #endregion 处理图片链接

            #region 处[H=x][/H]标记

            //处[H=x][/H]标记
            r = new Regex(@"(\[H=([1-6])\])([ \S\t]*?)(\[\/H\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<H" + m.Groups[2].ToString() + ">" +
                 m.Groups[3].ToString() + "</H" + m.Groups[2].ToString() + ">");
            }

            #endregion 处[H=x][/H]标记

            #region 处理[list=x][*][/list]

            //处理[list=x][*][/list]
            r = new Regex(@"(\[list(=(A|a|I|i| ))?\]([ \S\t]*)\r\n)((\[\*\]([ \S\t]*\r\n))*?)(\[\/list\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                String strLI = m.Groups[5].ToString();
                Regex rLI = new Regex(@"\[\*\]([ \S\t]*\r\n?)", RegexOptions.IgnoreCase);
                Match mLI;
                for (mLI = rLI.Match(strLI); mLI.Success; mLI = mLI.NextMatch())
                {
                    strLI = strLI.Replace(mLI.Groups[0].ToString(), "<LI>" + mLI.Groups[1]);
                }
                str = str.Replace(m.Groups[0].ToString(),
                 "<UL TYPE=\"" + m.Groups[3].ToString() + "\"><B>" + m.Groups[4].ToString() + "</B>" +
                 strLI + "</UL>");
            }

            #endregion 处理[list=x][*][/list]

            #region 处[SHADOW=x][/SHADOW]标记

            //处[SHADOW=x][/SHADOW]标记
            r = new Regex(@"(\[SHADOW=)(\d*?),(#*\w*?),(\d*?)\]([\S\t]*?)(\[\/SHADOW\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<TABLE WIDTH=" + m.Groups[2].ToString() + "  STYLE=FILTER:SHADOW(COLOR=" + m.Groups[3].ToString() + ", STRENGTH=" + m.Groups[4].ToString() + ")>" +
                 m.Groups[5].ToString() + "</TABLE>");
            }

            #endregion 处[SHADOW=x][/SHADOW]标记

            #region 处[glow=x][/glow]标记

            //处[glow=x][/glow]标记
            r = new Regex(@"(\[glow=)(\d*?),(#*\w*?),(\d*?)\]([\S\t]*?)(\[\/glow\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<TABLE WIDTH=" + m.Groups[2].ToString() + "  STYLE=FILTER:GLOW(COLOR=" + m.Groups[3].ToString() + ", STRENGTH=" + m.Groups[4].ToString() + ")>" +
                 m.Groups[5].ToString() + "</TABLE>");
            }

            #endregion 处[glow=x][/glow]标记

            #region 处[center][/center]标记

            r = new Regex(@"(\[center\])([ \S\t]*?)(\[\/center\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "<CENTER>" + m.Groups[2].ToString() + "</CENTER>");
            }

            #endregion 处[center][/center]标记

            #region 处[em]标记

            /*
            r = new Regex(@"(\[em([\S\t]*?)\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "<img src=" + ApplicationRootPath + "/ubb/images/post/pic/" + m.Groups[2].ToString() + ".gif border=0 align=middle>");
            }*/

            #endregion 处[em]标记

            #region 处[flash=x][/flash]标记

            //处[mp=x][/mp]标记
            r = new Regex(@"(\[flash=)(\d*?),(\d*?)\]([\S\t]*?)(\[\/flash\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<a href=" + m.Groups[4].ToString() + " TARGET=_blank><IMG SRC=images/post/swf.gif border=0 alt=点击开新窗口欣赏该FLASH动画!> [全屏欣赏]</a><br><br><OBJECT codeBase=http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0 classid=clsid:D27CDB6E-AE6D-11cf-96B8-444553540000 width=" + m.Groups[2].ToString() + " height=" + m.Groups[3].ToString() + "><PARAM NAME=movie VALUE=" + m.Groups[4].ToString() + "><PARAM NAME=quality VALUE=high><param name=menu value=false><embed src=" + m.Groups[4].ToString() + " quality=high menu=false pluginspage=http://www.macromedia.com/go/getflashplayer type=application/x-shockwave-flash width=" + m.Groups[2].ToString() + " height=" + m.Groups[3].ToString() + ">" + m.Groups[4].ToString() + "</embed></OBJECT>");
            }

            #endregion 处[flash=x][/flash]标记

            #region 处[dir=x][/dir]标记

            //处[dir=x][/dir]标记
            r = new Regex(@"(\[dir=)(\d*?),(\d*?)\]([\S\t]*?)(\[\/dir\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<Object classid=clsid:166B1BCA-3F9C-11CF-8075-444553540000 codebase=http://download.macromedia.com/pub/shockwave/cabs/director/sw.cab#version=7,0,2,0 width=" + m.Groups[2].ToString() + " height=" + m.Groups[3].ToString() + "><param name=src value=" + m.Groups[4].ToString() + "><embed src=" + m.Groups[4].ToString() + " pluginspage=http://www.macromedia.com/shockwave/download/ width=" + m.Groups[2].ToString() + " height=" + m.Groups[3].ToString() + "></embed></Object>");
            }

            #endregion 处[dir=x][/dir]标记

            #region 处[rm=x][/rm]标记

            //处[rm=x][/rm]标记
            r = new Regex(@"(\[rm=)(\d*?),(\d*?)\]([\S\t]*?)(\[\/rm\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<OBJECT classid=clsid:CFCDAA03-8BE4-11cf-B84B-0020AFBBCCFA class=OBJECT id=RAOCX width=" + m.Groups[2].ToString() + " height=" + m.Groups[3].ToString() + "><PARAM NAME=SRC VALUE=" + m.Groups[4].ToString() + "><PARAM NAME=CONSOLE VALUE=Clip1><PARAM NAME=CONTROLS VALUE=imagewindow><PARAM NAME=AUTOSTART VALUE=true></OBJECT><br><OBJECT classid=CLSID:CFCDAA03-8BE4-11CF-B84B-0020AFBBCCFA height=32 id=video2 width=" + m.Groups[2].ToString() + "><PARAM NAME=SRC VALUE=" + m.Groups[4].ToString() + "><PARAM NAME=AUTOSTART VALUE=-1><PARAM NAME=CONTROLS VALUE=controlpanel><PARAM NAME=CONSOLE VALUE=Clip1></OBJECT>");
            }

            #endregion 处[rm=x][/rm]标记

            #region 处[mp=x][/mp]标记

            //处[mp=x][/mp]标记
            r = new Regex(@"(\[mp=)(\d*?),(\d*?)\]([\S\t]*?)(\[\/mp\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<Object align=middle classid=CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95 class=OBJECT id=MediaPlayer width=" + m.Groups[2].ToString() + " height=" + m.Groups[3].ToString() + " ><param name=ShowStatusBar value=-1><param name=Filename value=" + m.Groups[4].ToString() + "><embed type=application/x-oleobject codebase=http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701 flename=mp src=" + m.Groups[4].ToString() + "  width=" + m.Groups[2].ToString() + " height=" + m.Groups[3].ToString() + "></embed></Object>");
            }

            #endregion 处[mp=x][/mp]标记

            #region 处[qt=x][/qt]标记

            //处[qt=x][/qt]标记
            r = new Regex(@"(\[qt=)(\d*?),(\d*?)\]([\S\t]*?)(\[\/qt\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(),
                 "<embed src=" + m.Groups[4].ToString() + " width=" + m.Groups[2].ToString() + " height=" + m.Groups[3].ToString() + " autoplay=true loop=false controller=true playeveryframe=false cache=false scale=TOFIT bgcolor=#000000 kioskmode=false targetcache=false pluginspage=http://www.apple.com/quicktime/>");
            }

            #endregion 处[qt=x][/qt]标记

            #region 处[QUOTE][/QUOTE]标记

            r = new Regex(@"(\[QUOTE\])([ \S\t]*?)(\[\/QUOTE\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "<table cellpadding=2 cellspacing=1 border=0 WIDTH=94% bgcolor=#CCCCCC align=center  style=FONT-SIZE: 9pt><tr><td bgcolor=#F3F3F3><table width=100% cellpadding=5 cellspacing=1 border=0><TR><TD >" + m.Groups[2].ToString() + "</table></table><br>");
            }

            #endregion 处[QUOTE][/QUOTE]标记

            #region 处[move][/move]标记

            r = new Regex(@"(\[move\])([ \S\t]*?)(\[\/move\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "<MARQUEE scrollamount=3>" + m.Groups[2].ToString() + "</MARQUEE>");
            }

            #endregion 处[move][/move]标记

            #region 处[FLY][/FLY]标记

            r = new Regex(@"(\[FLY\])([ \S\t]*?)(\[\/FLY\])", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "<MARQUEE width=80% behavior=alternate scrollamount=3>" + m.Groups[2].ToString() + "</MARQUEE>");
            }

            #endregion 处[FLY][/FLY]标记

            return str;
        }

        public static String XmlDecode(String data)
        {
            return Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(data, "&apos;", "'", RegexOptions.IgnoreCase), "&quot;", "\"", RegexOptions.IgnoreCase), "&lt;", "<", RegexOptions.IgnoreCase), "&gt;", ">", RegexOptions.IgnoreCase), "&amp;", "&", RegexOptions.IgnoreCase);
        }

        public static String XmlEncode(String data)
        {
            return data.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        /// <summary>
        /// 判断是否为IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Boolean IsIP(String ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static Boolean IsIPSect(String ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }

        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static Boolean InIPArray(String ip, String[] iparray)
        {
            String dot = ".";
            String[] userip = ip.ExtensionSplit(dot);

            for (Int32 n = 0; n < iparray.Length; n++)
            {
                String[] tmp = iparray[n].ExtensionSplit(dot);
                Int32 r = 0;
                for (Int32 i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i] == "*") return true;

                    if (userip.Length > i)
                    {
                        if (tmp[i] == userip[i])
                        {
                            r++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (r == 4)
                {
                    return true;
                }
            }
            return false;
        }
    }
}