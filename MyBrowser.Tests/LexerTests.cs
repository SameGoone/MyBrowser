using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MyBrowser.Tests
{
    [TestClass]
    public class LexerTests
    {
        /// <summary>
        /// Проверяет правильность разбиения на токены HTML-тега
        /// </summary>
        [TestMethod]
        public void TestLexingHTMLTag()
        {
            string code = "<body attr1=\"5\" attr2=\"string1\" attr3 = '5' attr4 = 'string2' attr5=5px>Hello World!</body>";
            object[] expectedTokens = new object[] { '<', "body", 
                                                     "attr1", '=', new char[] { '5' }, 
                                                     "attr2", '=', "string1".ToCharArray(), 
                                                     "attr3", '=', new char[] { '5' }, 
                                                     "attr4", '=', "string2".ToCharArray(), 
                                                     "attr5", '=', 5, "px", 
                                                     '>', "Hello", "World!", '<', '/', "body", '>' };

            TestLex(code, expectedTokens);
        }

        /// <summary>
        /// Проверяет правильность разбиения на токены CSS-правила
        /// </summary>
        [TestMethod]
        public void TestLexingCSSRule()
        {
            string code = "body{font-family:Arial,Verdana,sans-serif; font-size:11pt;}";
            object[] expectedTokens = new object[] { "body", '{', "font-family", ':', "Arial", ',', "Verdana", ',' , "sans-serif", ';', "font-size", ':', 11, "pt", ';', '}' };

            TestLex(code, expectedTokens);
        }

        /// <summary>
        /// Проверяет правильность разбиения на токены различных CSS-селекторов
        /// </summary>
        public void TestLexingSelectors()
        {
            string selector1 = ".class1.class2";
            object[] expectedObjs1 = new object[] { ".class1.class2" };

            string selector2 = ".class1 .class2";
            object[] expectedObjs2 = new object[] { ".class1", ".class2" };

            string selector3 = "p.className1.className2#someId";
            object[] expectedObjs3 = new object[] { "p.className1.className2#someId" };

            string selector4 = "div span";
            object[] expectedObjs4 = new object[] { "div", "span" };

            string selector5 = "div>span";
            object[] expectedObjs5 = new object[] { "div", '>', "span" };

            TestLex(selector1, expectedObjs1);
            TestLex(selector2, expectedObjs2);
            TestLex(selector3, expectedObjs3);
            TestLex(selector4, expectedObjs4);
            TestLex(selector5, expectedObjs5);
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
            Assert.AreEqual(expectedTokens.Length, tokens.Count);

            for(int i = 0; i < expectedTokens.Length; i++)
            {
                if (expectedTokens[i].GetType() == typeof(char[]))
                    CollectionAssert.AreEqual((char[])expectedTokens[i], (char[])tokens[i]);
                else
                    Assert.AreEqual(expectedTokens[i], tokens[i]);
            }
        }

        /// <summary>
        /// Проверяет правильность выделения целых чисел из кода
        /// </summary>
        [TestMethod]
        public void TestLexingIntNumbers()
        {
            string stringWithPositiveInt = "margin: 10px;";
            string stringWithNegativeInt = "margin: -10px;";

            TestNums(stringWithPositiveInt, 2, 10);
            TestNums(stringWithNegativeInt, 2, -10);
        }

        /// <summary>
        /// Проверяет правильность выделения чисел с плавающей точкой из кода
        /// </summary>
        [TestMethod]
        public void TestLexingDoubleNumbers()
        {
            string stringWithPositiveDouble = "margin: 2.5em;";
            string stringWithNegativeDouble = "margin: -2.5em;";

            TestNums(stringWithPositiveDouble, 2, 2.5);
            TestNums(stringWithNegativeDouble, 2, -2.5);
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