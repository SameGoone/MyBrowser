using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class ParserHTML
    {
        private Lexer lexer;

        private object token
        {
            get { return curToken; }
        }
        private object curToken;

        /// <summary>
        /// ������ Dom-������ �� ������ ����������� HTML-����
        /// </summary>
        /// <param name="HTMLCode"></param>
        /// <returns></returns>
        public DOM Parse(string HTMLCode)
        {
            HTMLCode = HTMLCode.Replace("<!DOCTYPE html>", "");
            lexer = new Lexer(HTMLCode);
            Shift();
            DOM dom = new DOM();


            if (token.Equals('<'))
            {
                Shift();
                if (token.GetType() == typeof(string))
                    dom.Document = ParseElement();
            }

            return dom;
        }

        /// <summary>
        /// ��������� ��������� ������� HTML
        /// </summary>
        /// <returns></returns>
        private HTMLElement ParseElement()
        {
            HTMLElement newElement = new HTMLElement();
            newElement.Tag = (string)token;
            bool isOpening = true;
            Shift();
            while (true)
            {
                if (token.GetType() == typeof(string) && isOpening)
                    newElement.Attributes.Add(ParseAttribute());

                else if (token.Equals('>'))
                {
                    if (isOpening)
                    {
                        isOpening = false;
                        Shift();
                        newElement.InnerText = ParseInnerText();
                    }
                    else;
                        // ������ ����
                }

                else if (token.Equals('<'))
                {
                    Shift();

                    if (token.Equals('/'))
                    {
                        Shift();
                        if (token.GetType() == typeof(string))
                        {
                            if (((string)token).Equals(newElement.Tag))
                            {
                                Shift();
                                Shift();
                                return newElement;
                            }
                            else;
                            // ������ ����
                        }
                        else;
                        // ������ ����
                    }

                    else if (token.GetType() == typeof(string))
                        newElement.Children.Add(ParseElement());

                    else if (token.Equals('!')) ;
                        // ������������ �����������

                    // TODO: �������� ��������� ������� �����
                }
            }
        }

        /// <summary>
        /// ��������� ��������� HTML-������� 
        /// </summary>
        /// <returns></returns>
        private HTMLAttribute ParseAttribute()
        {
            HTMLAttribute attribute = new HTMLAttribute();
            attribute.Name = (string)token;

            Shift();
            if (token.Equals('='))
            {
                Shift();
                if (token.GetType() == typeof(char[]))
                {
                    attribute.Value = new string((char[])token);
                    Shift();
                }
                else;
                // TODO: �������� ��������� ��������� ��� �������
            }
            else
                attribute.Value = "";

            return attribute;
        }

        /// <summary>
        /// ��������� ����� ����� ������
        /// </summary>
        /// <returns></returns>
        private string ParseInnerText()
        {
            string text = "";
            bool isFirst = true;
            while (!token.Equals('<'))
            {
                if (isFirst)
                    isFirst = false;
                else
                    text += " ";

                if (token.GetType() == typeof(char[]))
                    text += new string((char[])token);
                else
                    text += token.ToString();

                Shift();
            }

            return text;
        }

        /// <summary>
        /// ���������� �����������
        /// </summary>
        private void ParseComment()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// �������� ��������� ����� �� �������
        /// </summary>
        private void Shift()
        {
            lexer.TryGetToken(out curToken);
        }
    }
}