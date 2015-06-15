using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Stone.Framework.Common.Utility
{
    public class UrlHelper
    {
        private const string Http = "http://";
        private const string RegexQuery = @"(?<=(\&|\?|^)({0})\=).*?(?=\&|$)";

        private UrlHelper()
        {
        }

        /// <summary>
        /// 返回当前完整主机头.http://www.baidu.com:8088
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost
        {
            get
            {
                var request = HttpContext.Current.Request;

                if (!request.Url.IsDefaultPort)
                {
                    return Http + string.Format("{0}:{1}", request.Url.Host, request.Url.Port).ToLower();
                }
                return string.Concat(Http, request.Url.Host).ToLower();
            }
        }

        /// <summary>
        /// Return to the host header
        /// </summary>
        /// <returns></returns>
        public static string GetHost
        {
            get { return string.Concat(Http, HttpContext.Current.Request.Url.Host).ToLower(); }
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp
        {
            get
            {
                var result = SecurityHelper.GetServerString("REMOTE_ADDR");
                if (result.ExtensionIsNullOrEmpty())
                {
                    result = SecurityHelper.GetServerString("HTTP_X_FORWARDED_FOR");
                }

                if (result.ExtensionIsNullOrEmpty())
                {
                    result = HttpContext.Current.Request.UserHostAddress;
                }

                //todo:SecurityHelper.IsIp(result)
                if (result.ExtensionIsNullOrEmpty())
                {
                    result = "127.0.0.1";
                }
                return result;
            }
        }

        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns></returns>
        public static string GetPageName
        {
            get
            {
                var urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
                return urlArr[urlArr.Length - 1].ToLower();
            }
        }

        /// <summary>
        /// 返回表单或Url参数的总个数
        /// </summary>
        /// <returns></returns>
        public static int GetParamCount
        {
            get { return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count; }
        }

        ///// <summary>
        ///// 获取重写之后的URL
        ///// </summary>
        public static string GetRewriterUrl
        {
            get { return SecurityHelper.GetServerString("HTTP_X_REWRITE_URL") ?? string.Empty; }
        }

        /// <summary>
        /// 获取不带参数的URL地址.例如:http://www.sohu.com/news.aspx
        /// </summary>
        /// <returns></returns>
        public static string GetThisShortUrl
        {
            get
            {
                return Http + (SecurityHelper.GetServerString("SERVER_NAME")) +
                       (HttpContext.Current.Request.Url.IsDefaultPort
                           ? string.Empty
                           : SecurityHelper.GetServerString("Server_Port")) +
                       SecurityHelper.GetServerString("URL").ToLower();
            }
        }

        /// <summary>
        /// 获取完整URL地址.例如:http://www.sohu.com/news.aspx?type=1
        /// </summary>
        /// <returns></returns>
        public static string GetThisUrl
        {
            get
            {
                var str = Http + (SecurityHelper.GetServerString("SERVER_NAME")) +
                          (HttpContext.Current.Request.Url.IsDefaultPort
                              ? string.Empty
                              : SecurityHelper.GetServerString("Server_Port")) +
                          SecurityHelper.GetServerString("URL");
                if (SecurityHelper.GetServerString("QUERY_STRING").Length > 0)
                {
                    str = str + SecurityHelper.GetServerString("QUERY_STRING");
                }
                return str.ToLower();
            }
        }

        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <returns></returns>
        public static string GetUrlReferrer
        {
            get
            {
                string str = string.Empty;
                try
                {
                    if (HttpContext.Current.Request.UrlReferrer != null)
                        str = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
                return str;
            }
        }

        /// <summary>
        /// 根据URL获取文件名
        /// </summary>
        /// <param name="url"></param>
        /// <returns>如:http://sh.cdfcc.cn/pic/2432_090827222230181.jpg返回2432_090827222230181.jpg</returns>
        public static string GetFileName(string url)
        {
            GroupCollection group = Regex.Match(url, "^(?i)http://.*/(.*)$", RegexOptions.IgnoreCase).Groups;
            return group[1].Value;
        }

        /// <summary>
        /// 获取域名.例如:http://www.sohu.com/default.aspx 返回 www.sohu.com
        /// </summary>
        /// <param name="url"></param>
        /// <returns>http://blog.csdn.net/shatamadedongxi/article/details/8000829</returns>
        public static string GetUrlFullDomainName(string url)
        {
            string str;
            try
            {
                int start = url.IndexOf(Http, 7, StringComparison.Ordinal);
                var index = url.IndexOf("/", start, StringComparison.Ordinal);

                if (index == -1)
                {
                    index = url.Length;
                }
                str = url.Substring(start, index - start);
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return str.ToLower();
        }

        /// <summary>
        /// 提取Url中Querystring指定键的值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="queryKey"></param>
        public static string GetUrlParameter(string input, string queryKey)
        {
            return Regex.Match(input, string.Format(RegexQuery, queryKey), RegexOptions.Singleline).Value.Trim();
        }

        /// <summary>
        /// 提取Url中Querystring指定键的值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="queryKey"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetUrlParameters(string input, string queryKey, RegexOptions options)
        {
            var value = string.Empty;
            var orKeys = queryKey.Split('|');

            foreach (var andKeys in orKeys.Select(item => item.Split('&')))
            {
                value = andKeys.Aggregate(value, (current, and) => current + (GetUrlParameter(input, and) + " "));
                if (value != string.Empty)
                {
                    break;
                }
            }
            return value.Trim();
        }

        /// <summary>
        /// 将集合元素拼接成QueryString后面的参数
        /// e.g.: key=中国&tag=地铁
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static string JoinUrlParameters<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return string.Join("&", dictionary.Select(kv => kv.Key.ToString() + "=" + kv.Value.ToString()).ToArray());
        }

        /// <summary>
        /// 移除Url中QueryString中指定的参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RemoveUrlParameters(string url, string key)
        {
            return Regex.Replace(url, "[?&]" + key + "=[^&]*[&]{0,1}",
                o =>
                    (o.Value.StartsWith("?") && o.Value.EndsWith("&") ? "?" : "") +
                    (o.Value.StartsWith("&") && o.Value.EndsWith("&") ? "&" : ""));
        }

        /// <summary>
        /// 获取搜索引擎蜘蛛名称
        /// </summary>
        public static string GetSearchEnginesName
        {
            get
            {
                if (HttpContext.Current.Request.UrlReferrer == null) return String.Empty;

                string[] searchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
                var tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
                return searchEngine.Any(t => tmpReferrer.IndexOf(t, StringComparison.Ordinal) >= 0) ? tmpReferrer : string.Empty;
            }
        }

        /// <summary>
        /// 根据URL获取文件名.如:http://sh.cdfcc.cn/pic/2432_090827222230181.jpg返回2432_090827222230181.jpg
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlFileName(string url)
        {
            var groups = Regex.Match(url, "^(?i)http://.*/(.*)$", RegexOptions.IgnoreCase).Groups;
            return groups[1].Value;
        }

        /// <summary>
        /// 合并完整Url.如:http://bj.centanet.com/、/index.aspx合并成http://bj.centanet.com/index.aspx
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string UrlCombine(string baseUrl, string relativeUrl)
        {
            Uri uri = null;
            return Uri.TryCreate(new Uri(baseUrl), relativeUrl, out uri) ? uri.AbsoluteUri : string.Empty;
        }
    }
}