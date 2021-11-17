using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MyBrowser.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void TestLexingHTML()
        {
            string code = "<body atr1=\"5\">Hello World!</body>";
            Lexer lexer = new Lexer(code);
            List<object> tokens = new List<object>();
            object[] expectedTokens = new object[] { '<', "body", ' ', "atr1", '=', '"', 5, '"', '>', "Hello", ' ', "World!", '<', '/', "body", '>' };

            object token;
            while (lexer.TryGetToken(out token))
            {
                tokens.Add(token);
            }

            Assert.AreEqual(expectedTokens.Length, tokens.Count);

            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.AreEqual(expectedTokens[i], tokens[i]);
            }
        }

        [TestMethod]
        public void TestLexingCSS()
        {
            string code = "body{font-family:Arial,Verdana,sans-serif; font-size:11pt;}";
            Lexer lexer = new Lexer(code);
            List<object> tokens = new List<object>();
            object[] expectedTokens = new object[] { "body", '{', "font-family", ':', "Arial", ',', "Verdana", ',' , "sans-serif", ';', ' ', "font-size", ':', 11, "pt", ';', '}' };

            object token;
            while (lexer.TryGetToken(out token))
            {
                tokens.Add(token);
            }

            Assert.AreEqual(expectedTokens.Length, tokens.Count);

            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.AreEqual(expectedTokens[i], tokens[i]);
            }
        }

        [TestMethod]
        public void TestLexingIntNumbers()
        {
            string stringWithPositiveInt = "margin: 10px;";
            string stringWithNegativeInt = "margin: -10px;";
            Lexer lexer;
            List<object> tokens;
            object token;

            //Тест положительных
            lexer = new Lexer(stringWithPositiveInt);
            tokens = new List<object>();
            while (lexer.TryGetToken(out token))
            {
                tokens.Add(token);
            }
            Assert.IsInstanceOfType(tokens[3], typeof(int));
            Assert.AreEqual(10, tokens[3]);

            //Тест отрицательных
            lexer = new Lexer(stringWithNegativeInt);
            tokens = new List<object>();
            while (lexer.TryGetToken(out token))
            {
                tokens.Add(token);
            }
            Assert.IsInstanceOfType(tokens[3], typeof(int));
            Assert.AreEqual(-10, tokens[3]);
        }

        [TestMethod]
        public void TestLexingDoubleNumbers()
        {
            string stringWithPositiveDouble = "margin: 2.5em;";
            string stringWithNegativeDouble = "margin: -2.5em;";
            Lexer lexer;
            List<object> tokens;
            object token;

            //Тест положительных
            lexer = new Lexer(stringWithPositiveDouble);
            tokens = new List<object>();
            while (lexer.TryGetToken(out token))
            {
                tokens.Add(token);
            }
            Assert.IsInstanceOfType(tokens[3], typeof(double));
            Assert.AreEqual(2.5, (double)tokens[3], 0.001);

            //Тест отрицательных
            lexer = new Lexer(stringWithNegativeDouble);
            tokens = new List<object>();
            while (lexer.TryGetToken(out token))
            {
                tokens.Add(token);
            }
            Assert.IsInstanceOfType(tokens[3], typeof(double));
            Assert.AreEqual(-2.5, (double)tokens[3], 0.001);
        }
    }
}