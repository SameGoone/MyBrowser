using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MyBrowser.Tests
{
    [TestClass]
    public class LexerTests
    {
        /// <summary>
        /// Проверяет правильность разбиения на токены фрагментов HTML-кода
        /// </summary>
        [TestMethod]
        public void TestLexingHTML()
        {
            string code = "<body atr1=\"5\">Hello World!</body>";
            object[] expectedTokens = new object[] { '<', "body", ' ', "atr1", '=', '"', 5, '"', '>', "Hello", ' ', "World!", '<', '/', "body", '>' };

            TestLex(code, expectedTokens);
        }

        /// <summary>
        /// Проверяет правильность разбиения на токены фрагментов CSS-кода
        /// </summary>
        [TestMethod]
        public void TestLexingCSS()
        {
            string code = "body{font-family:Arial,Verdana,sans-serif; font-size:11pt;}";
            object[] expectedTokens = new object[] { "body", '{', "font-family", ':', "Arial", ',', "Verdana", ',' , "sans-serif", ';', ' ', "font-size", ':', 11, "pt", ';', '}' };

            TestLex(code, expectedTokens);
        }

        /// <summary>
        /// Проверяет правильность значений и последовательности токенов введённой строки
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedTokens"></param>
        [TestMethod]
        private void TestLex(string code, object[] expectedTokens)
        {
            var tokens = Analyze(code);
            CollectionAssert.AreEqual(expectedTokens, tokens);
        }

        /// <summary>
        /// Проверяет правильность выделения целых чисел из кода
        /// </summary>
        [TestMethod]
        public void TestLexingIntNumbers()
        {
            string stringWithPositiveInt = "margin: 10px;";
            string stringWithNegativeInt = "margin: -10px;";

            TestNums(stringWithPositiveInt, 3, 10);
            TestNums(stringWithNegativeInt, 3, -10);
        }

        /// <summary>
        /// Проверяет правильность выделения чисел с плавающей точкой из кода
        /// </summary>
        [TestMethod]
        public void TestLexingDoubleNumbers()
        {
            string stringWithPositiveDouble = "margin: 2.5em;";
            string stringWithNegativeDouble = "margin: -2.5em;";

            TestNums(stringWithPositiveDouble, 3, 2.5);
            TestNums(stringWithNegativeDouble, 3, -2.5);
        }

        /// <summary>
        /// Проверяет правильность выделения чисел из кода
        /// </summary>
        /// <param name="code">Строка для лексического разбора</param>
        /// <param name="index">Ожидаемый индекс, по которому располагается число</param>
        /// <param name="value">Ожидаемое значение числа</param>
        [TestMethod]
        private void TestNums(string code, int index, object value)
        {
            var tokens = Analyze(code);
            Assert.AreEqual(value, tokens[index]);
        }

        /// <summary>
        /// Разбивает строку на токены
        /// </summary>
        /// <param name="code">Строка для разбиения</param>
        /// <returns>Коллекция полученных из строки токенов</returns>
        private List<object> Analyze(string code)
        {
            Lexer lexer = new Lexer(code);
            List<object> tokens = new List<object>();
            object token;
            while (lexer.TryGetToken(out token))
            {
                tokens.Add(token);
            }

            return tokens;
        }
    }
}