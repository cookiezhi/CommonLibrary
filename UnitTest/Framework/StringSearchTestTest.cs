using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stone.Framework.Common.Utility;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework
{
    /// <summary>
    /// StringSearchTestTest 的摘要说明
    /// </summary>
    [TestClass]
    public class StringSearchTestTest
    {
        public StringSearchTestTest()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        private class IndexOfSearch : IStringSearchAlgorithm
        {
            #region IStringSearchAlgorithm Members

            private string[] _keywords;

            public string[] Keywords
            {
                get { return _keywords; }
                set { _keywords = value; }
            }

            public StringSearchResult[] FindAll(string text)
            {
                throw new NotImplementedException();
            }

            public StringSearchResult FindFirst(string text)
            {
                foreach (string kwd in _keywords)
                {
                    int i = text.IndexOf(kwd);
                    if (i != -1) return new StringSearchResult(i, kwd);
                }
                return StringSearchResult.Empty;
            }

            public bool ContainsAny(string text)
            {
                foreach (string kwd in _keywords)
                {
                    int i = text.IndexOf(kwd);
                    if (i != -1) return true;
                }
                return false;
            }

            #endregion IStringSearchAlgorithm Members
        }

        /// <summary>
        /// Search using regular expressions (for comparsion)
        /// </summary>
        private class RegexSearch : IStringSearchAlgorithm
        {
            #region IStringSearchAlgorithm Members

            private string[] _keywords;
            private Regex _reg;

            public string[] Keywords
            {
                get { return _keywords; }
                set
                {
                    _keywords = value;
                    _reg = new Regex("(" + string.Join("|", value) + ")", RegexOptions.None);
                }
            }

            public StringSearchResult[] FindAll(string text)
            {
                throw new NotImplementedException();
            }

            public StringSearchResult FindFirst(string text)
            {
                throw new NotImplementedException();
            }

            public bool ContainsAny(string text)
            {
                return _reg.Match(text).Success;
            }

            #endregion IStringSearchAlgorithm Members
        }

        /// <summary>
        /// Minimal and maximal word length
        /// </summary>
        private const int MaxWordLength = 10;

        private const int MinWordLength = 3;

        /// <summary>
        /// Allowed letters in word
        /// </summary>
        private const string AllowedLetters = "abcdefghijklmnopqrstuvwxyz";

        private static Random rnd = new Random(12345);

        /// <summary>
        /// Generate random word
        /// </summary>
        public static string GetRandomWord()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rnd.Next(MaxWordLength - MinWordLength) + MinWordLength; i++)
                sb.Append(AllowedLetters[rnd.Next(AllowedLetters.Length)]);
            return sb.ToString();
        }

        /// <summary>
        /// Generate list of random keywords
        /// </summary>
        public static string[] GetRandomKeywords(int count)
        {
            string[] ret = new string[count];
            for (int i = 0; i < count; i++)
                ret[i] = GetRandomWord();
            return ret;
        }

        /// <summary>
        /// Generate random text
        /// </summary>
        public static string GetRandomText(int count)
        {
            StringBuilder sb = new StringBuilder();
            while (sb.Length < count) { sb.Append(GetRandomWord()); sb.Append(" "); }
            return sb.ToString();
        }

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性

        //
        // 编写测试时，可以使用以下附加特性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion 附加测试特性

        [TestMethod]
        public void EncryptDecryptTest()
        {
            string str1 = string.Empty;

            //Assert.IsInstanceOfType(str1, String);
        }
    }
}