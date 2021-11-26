using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class ParserHTML
    {
        private readonly List<string> emptyElements = new List<string> { "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr" };
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
                {
                    ElementParsingResult result;
                    dom.Document = ParseElement(out result);
                }
            }

            return dom;
        }

        /// <summary>
        /// ��������� ��������� ������� HTML
        /// </summary>
        /// <returns></returns>
        private HTMLElement ParseElement(out ElementParsingResult result)
        {
            List<HTMLElement> potentialChildren = new List<HTMLElement>();
            HTMLElement newElement = new HTMLElement();
            newElement.Tag = (string)token;
            bool insideOpeningTag = true;
            Shift();
            while (true)
            {
                if (token is NonToken)
                {
                    result = ElementParsingResult.GetOkResult();
                    newElement.AddChildrenRange(potentialChildren);
                    return newElement;
                }

                if (insideOpeningTag) // ������ ������������ ����
                {
                    if (token is string)
                        newElement.Attributes.Add(ParseAttribute());

                    else if (token.Equals('>'))
                    {
                        Shift();
                        if (IsEmptyElement(newElement.Tag)) // ���� ������� ��������� ���������� ������������ ���� (������ �������)
                        {
                            result = ElementParsingResult.GetOkResult();
                            return newElement;
                        }
                        else
                        {
                            insideOpeningTag = false;
                        }
                    }

                    else if (token.Equals('/'))
                    {
                        Shift();
                        Shift();
                        result = ElementParsingResult.GetOkResult();
                        return newElement;
                    }

                    else
                    {
                        Shift();
                    }
                }
                else // ��� ������������ ����
                {
                    if (token.Equals('<'))
                    {
                        Shift();

                        // ���� ����������� ���
                        if (token.Equals('/'))
                        {
                            Shift();
                            if (token is string)
                            {
                                var tag = (string)token;
                                ShiftToEndOfEndingTag();

                                if (tag.Equals(newElement.Tag)) // ����������� ��� �������� ��������, ������� ������� �����������
                                {
                                    result = ElementParsingResult.GetOkResult();
                                    newElement.AddChildrenRange(potentialChildren);
                                    return newElement;
                                }
                                else // �������� ����������� ��� ������� ��������, ������ �������� ����������� ��� ��������
                                {
                                    result = ElementParsingResult.GetCloseAnotherTagResult(tag, newElement, potentialChildren);
                                    return newElement;
                                }
                            }
                            else // ������ ����, ����������� �������� �������� ��������
                            {
                                result = ElementParsingResult.GetOkResult();
                                newElement.AddChildrenRange(potentialChildren);
                                return newElement;
                            }
                        }

                        // ���� ����������� ��� ������ ��������
                        else if (token is string)
                        {
                            ElementParsingResult childResult;
                            HTMLElement potentialChild = ParseElement(out childResult);
                            if (childResult is OkResult)
                            {
                                potentialChildren.Add(potentialChild);
                            }
                            else
                            {
                                var anotherTagResult = (CloseAnotherTagResult)childResult;
                                if (anotherTagResult.Tag == newElement.Tag)
                                {
                                    newElement.AddChildrenRange(potentialChildren);
                                    newElement.AddChildrenRange(anotherTagResult.ElementsWithoutCloseTag);
                                    result = ElementParsingResult.GetOkResult();
                                    return newElement;
                                }
                                else
                                {
                                    anotherTagResult.AddElementWithPotentialChildren(newElement, potentialChildren);
                                    result = anotherTagResult;
                                    return newElement;
                                }
                            }
                        }

                        else if (token.Equals('!'))
                            ParseComment();

                        // TODO: �������� ��������� ������ < � > � ������
                    }
                    else
                    {
                        newElement.InnerText += ParseInnerText();
                    }
                }
            }
        }

        /// <summary>
        /// �������� ��������� �� �����, ��������� �� ����������� �������� ����
        /// </summary>
        private void ShiftToEndOfEndingTag()
        {
            Shift();
            if (token.Equals('>'))
                Shift();
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
                if (token.Equals('\"') || token.Equals('\''))
                {
                    attribute.Value = GetStringData((char)token);
                }
                else;
                // TODO: �������� ��������� ��������� ��� �������
            }
            else
                attribute.Value = "";

            return attribute;
        }

        /// <summary>
        /// ��������� ������ (�������� ����� �������� ��� ���������� ���������)
        /// </summary>
        /// <param name="quoteChar">������ ������� (������� ��� ���������)</param>
        /// <returns>��������, ����������� ����� ���������</returns>
        private string GetStringData(char quoteChar)
        {
            string data = "";
            Shift();
            while (!token.Equals(quoteChar))
            {
                data += token;
                Shift();
            }
            Shift();

            return data;
        }

        /// <summary>
        /// ��������� ����� ����� ������
        /// </summary>
        /// <returns></returns>
        private string ParseInnerText()
        {
            string text = "";
            bool isFirst = true;
            while (!token.Equals('<') && !(token is NonToken))
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
            Shift();
            if (token.Equals('-'))
            {
                Shift();
                if (token.Equals('-'))
                {
                    Shift();
                    while (!(token is NonToken))
                    {
                        if (!token.Equals('-'))
                            Shift();
                        else
                        {
                            Shift();
                            if (token.Equals('-'))
                            {
                                Shift();
                                if (token.Equals('>'))
                                {
                                    Shift();
                                    return;
                                }
                            }
                        }
                    }
                }

                else return;

            }
            else return;
        }

        /// <summary>
        /// �������� ��������� ����� �� �������
        /// </summary>
        private void Shift()
        {
            if (!lexer.TryGetToken(out curToken))
                curToken = new NonToken();
        }

        private bool IsEmptyElement(string tag)
        {
            return emptyElements.Contains(tag);
        }
    }
}