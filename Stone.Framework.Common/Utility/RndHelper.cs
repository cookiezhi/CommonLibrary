
using System;
using System.Threading;

namespace Stone.Framework.Common.Utility
{
    public class RndHelper
    {
        private const string CharLower = "abcdefghijklmnopqrstuvwxyz";
        private const string CharUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digit = "0123456789";
        private RndHelper()
        {
            //
            // TODO: 
            //
        }

        /// <summary>
        /// 生成特定长度的随机数字
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RndNumber(int length)
        {
            return Builder(length, Digit);
        }

        /// <summary>
        /// 生成特定长度的随机英文字符
        /// </summary>
        /// <param name="legth"></param>
        /// <returns></returns>
        public static string RndChar(int legth)
        {
            return Builder(legth, CharLower + CharUpper);
        }

        /// <summary>
        /// 生成特定长度的随机数字和字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RndNumberChar(int length)
        {
            return Builder(length, string.Concat(Digit, CharLower, CharUpper));
        }

        private static string Builder(int length, string constant)
        {
            Thread.Sleep(3); //线程挂起的时间是3毫秒
            var result = string.Empty;
            var n = constant.Length;
            var random = new Random(~unchecked((int)System.DateTime.Now.Ticks));
            for (var i = 0; i < length; i++)
            {
                var rnd = random.Next(0, n);
                result += constant[rnd];
            }
            return result;
        }
    }
}
