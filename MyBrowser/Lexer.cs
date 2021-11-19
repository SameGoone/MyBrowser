using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MyBrowser
{
    /// <summary>
    /// Осуществляет лексический разбор кода CSS или HTML
    /// </summary>
    public class Lexer
    {
        private StringReader reader;

        /// <summary>
        /// Символ, который рассматривается StringReader
        /// </summary>
        private char ch
        {
            get
            {
                int nextCharCode = reader.Peek();
                char nextChar;
                if (nextCharCode == -1)
                {
                    nextChar = (char)0;
                }
                else
                {
                    nextChar = (char)nextCharCode;
                }
                return nextChar;
            }
        }

        /// <summary>
        /// Определяет, остались ли символы для считывания
        /// </summary>
        private bool isEnd
        {
            get
            {
                return reader.Peek() == -1;
            }
        }

        /// <summary>
        /// Символы, при обнаружении которых следует прервать считывание идентификатора
        /// </summary>
        private List<char> stopIdentSymbols = new List<char>() { '<', '>', '/', '=', '\"', '\'', ';', ':', '{', '}', '+', '~', ',' };

        /// <summary>
        /// Символы, с которых может начинаться селектор
        /// </summary>
        private List<char> startOfSelectors = new List<char>() { '.', '#', };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">Строка, которую следует разбить на токены</param>
        public Lexer(string code)
        {
            reader = new StringReader(code);
        }

        /// <summary>
        /// Пытается считать следующий токен из внутренней строки.
        /// </summary>
        /// <param name="token">Выходной параметр. Получает int - если целое число, double - если число с плавающей точкой, string - если идентификатор, char - если специальный символ</param>
        /// <returns>false, если не получилось считать информацию, т.к. достигнут конец строки. Иначе - true</returns>
        public bool TryGetToken(out object token)
        {
            token = null;
            if (isEnd)
                return false;

            while (char.IsWhiteSpace(ch))
            {
                Shift();
            }

            if (ch == '-')
            {
                Shift();
                if (char.IsDigit(ch))
                    token = GetNumber(false);
                else
                    token = '-';
            }

            else if (char.IsDigit(ch))
            {
                token = GetNumber(true);
            }

            else if (char.IsLetter(ch) || startOfSelectors.Contains(ch))
            {
                token = GetIdent();
            }

            else
            {
                token = ch;
                Shift();
            }

            return true;
        }

        /// <summary>
        /// Считывает число (int или double)
        /// </summary>
        /// <param name="positive">Знак числа. true - если число положительное. Иначе - false</param>
        /// <returns></returns>
        private object GetNumber(bool positive)
        {
            string intPart = ch.ToString();
            string floatPart = "";
            bool isFloat = false;
            while (true)
            {
                Shift();
                if (char.IsDigit(ch))
                {
                    if (isFloat)
                    {
                        floatPart += ch;
                    }
                    else
                    {
                        intPart += ch;
                    }
                }
                else if (ch == '.')
                {
                    if (isFloat)
                        break;
                    else
                        isFloat = true;

                }
                else
                    break;
            }

            if (isFloat)
            {
                double result = int.Parse(intPart) + (int.Parse(floatPart) * Math.Pow(10, -floatPart.Length));
                if (!positive)
                    result *= -1;
                return result;
            }
            else
            {
                int result = int.Parse(intPart); 
                if (!positive)
                    result *= -1;
                return result;
            }
        }

        /// <summary>
        /// Считывает идентификатор
        /// </summary>
        /// <returns></returns>
        private string GetIdent()
        {
            string buf = ch.ToString();
            Shift();
            while (!stopIdentSymbols.Contains(ch) && !char.IsWhiteSpace(ch) && !isEnd)
            {
                buf += ch;
                Shift();
            }

            return buf;
        }

        /// <summary>
        /// Передвигает положение указателя в строке на следующий символ.
        /// </summary>
        private void Shift()
        {
            reader.Read();
        }
    }
}