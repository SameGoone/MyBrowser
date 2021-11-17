using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MyBrowser
{
    /// <summary>
    /// ������������ ����������� ������ ���� CSS ��� HTML
    /// </summary>
    public class Lexer
    {
        private StringReader reader;

        /// <summary>
        /// ������, ������� ��������������� StringReader
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
        /// ����������, �������� �� ������� ��� ����������
        /// </summary>
        private bool isEnd
        {
            get
            {
                return reader.Peek() == -1;
            }
        }

        /// <summary>
        /// �������, ��� ����������� ������� ������� �������� ���������� ������ ��� ��������������
        /// </summary>
        private List<char> specialSymbols = new List<char>() { '<', '>', '/', '=', '\"', '\'', ';', ':', '{', '}', '+', '~', ' ', ',' };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">������, ������� ������� ������� �� ������</param>
        public Lexer(string code)
        {
            reader = new StringReader(code);
        }

        /// <summary>
        /// �������� ������� ��������� ����� �� ���������� ������.
        /// </summary>
        /// <param name="token">�������� ��������. �������� int - ���� ����� �����, double - ���� ����� � ��������� ������, string - ���� ������������� ��� ������, char - ���� ����������� ������</param>
        /// <returns>false, ���� �� ���������� ������� ����������, �.�. ��������� ����� ������. ����� - true</returns>
        public bool TryGetToken(out object token)
        {
            token = null;
            if (isEnd)
                return false;

            while (char.IsWhiteSpace(ch) && ch != ' ')
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

            else if (char.IsLetter(ch))
            {
                string buf = ch.ToString();
                Shift();
                while (!specialSymbols.Contains(ch) && !char.IsWhiteSpace(ch) && !isEnd)
                {
                    buf += ch;
                    Shift();
                }

                token = buf;
            }

            else
            {
                token = ch;
                Shift();
            }

            return true;
        }

        /// <summary>
        /// ��������� ����� (int ��� double)
        /// </summary>
        /// <param name="positive">���� �����. true - ���� ����� �������������. ����� - false</param>
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
        /// ����������� ��������� ��������� � ������ �� ��������� ������.
        /// </summary>
        private void Shift()
        {
            reader.Read();
        }
    }
}