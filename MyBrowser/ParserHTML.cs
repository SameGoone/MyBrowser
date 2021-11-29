using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class ParserHTML
    {
        private readonly string htmlTag = "html";
        private readonly string headTag = "head";
        private readonly string bodyTag = "body";
        private readonly List<string> emptyElements = new List<string> { "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr" };
        private readonly List<string> onlyHeadElements = new List<string> { "title", "link", "style", "bgsound", "base" };
        private readonly List<string> bodyOrHeadElements = new List<string> { "script", "basefront" };
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
                if (token is string)
                {
                    var tag = ToLower(token);
                    if (tag.Equals(htmlTag))
                    {
                        ElementParsingResult result;
                        dom.Document = ParseElement(out result);
                    }
                    else
                    {
                        dom.Document = new HTMLElement(htmlTag);
                        List<HTMLElement> potentialChildren = new List<HTMLElement>();
                        while (!(token is NonToken))
                        {
                            ElementParsingResult result;
                            potentialChildren.Add(ParseElement(out result));
                        }
                        CorrectDOM(dom, potentialChildren);
                    }
                }
            }

            return dom;
        }

        /// <summary>
        /// ������������ ��������, ������� ��������� ��� �����������, � ������ ����������
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="potentialChildren"></param>
        private void CorrectDOM(DOM dom, List<HTMLElement> potentialChildren)
        {
            if (dom.Document == null)
                dom.Document = new HTMLElement(htmlTag);

            List<HTMLElement> elementsWithoutContainer = new List<HTMLElement>(potentialChildren);

            foreach (HTMLElement element in potentialChildren)
            {
                if (element.Tag.Equals(headTag))
                {
                    dom.Head = element;
                    elementsWithoutContainer.Remove(element);
                }
                else if (element.Tag.Equals(bodyTag))
                {
                    dom.Body = element;
                    elementsWithoutContainer.Remove(element);
                }
            }

            if (dom.Head == null)
            {
                dom.Head = new HTMLElement(headTag);
            }
            if (dom.Body == null)
            {
                dom.Body = new HTMLElement(bodyTag);
            }

            foreach (HTMLElement element in elementsWithoutContainer)
            {
                if (onlyHeadElements.Contains(element.Tag))
                    dom.Head.AddChild(element);
                else
                    dom.Body.AddChild(element);
            }
        }

        /// <summary>
        /// ��������� ��������� ������� HTML
        /// </summary>
        /// <returns></returns>
        private HTMLElement ParseElement(out ElementParsingResult result)
        {
            if (!(token is string))
                while (!(token is NonToken))
                {
                    if (token.Equals('<'))
                    {
                        Shift();
                        if (token is string)
                            break;
                    }
                    else
                        Shift();
                }

            List<HTMLElement> potentialChildren = new List<HTMLElement>();
            HTMLElement newElement = new HTMLElement(ToLower(token));
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
                            if (newElement.Tag.Equals("script"))
                            {
                                result = ReadScript(newElement);
                                return newElement;
                            }

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
                            result = ParseCloseTag(newElement, potentialChildren);
                            return newElement;
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
        /// ��������� ������������ �������� ��������
        /// </summary>
        /// <param name="element">������� ������������� �������</param>
        /// <param name="potentialChildren">��������, ������� ����������, ��� ������������ �������� � ��������</param>
        /// <returns>����������, ��� �� ��������������� ����������� ��� ������� ��������, ��� �������</returns>
        private ElementParsingResult ParseCloseTag(HTMLElement element, List<HTMLElement> potentialChildren)
        {
            Shift();
            ElementParsingResult result;
            if (token is string)
            {
                var tag = ToLower(token);
                ShiftToEndOfEndingTag();

                if (tag.Equals(element.Tag)) // ����������� ��� �������� ��������, ������� ������� �����������
                {
                    result = ElementParsingResult.GetOkResult();
                    element.AddChildrenRange(potentialChildren);
                }
                else // �������� ����������� ��� ������� ��������, ������ �������� ����������� ��� ��������
                {
                    result = ElementParsingResult.GetCloseAnotherTagResult(tag, element, potentialChildren);
                }
            }
            else // ������ ����, ����������� �������� �������� ��������
            {
                result = ElementParsingResult.GetOkResult();
                element.AddChildrenRange(potentialChildren);
            }

            return result;
        }

        private ElementParsingResult ReadScript(HTMLElement element)
        {
            string script = "";
            ElementParsingResult result = ElementParsingResult.GetOkResult();
            while (!(token is NonToken))
            {
                if (token.Equals('<'))
                {
                    Shift();
                    if (token.Equals('/'))
                    {
                        Shift();
                        result = ParseCloseTag(element, new List<HTMLElement>());
                        break;
                    }
                    else
                    {
                        script += "<" + token;
                    }

                }
                else
                {
                    script += token;
                    Shift();
                }
            }

            element.InnerText = script;
            return result;
        }

        /// <summary>
        /// ��������� ��������� HTML-������� 
        /// </summary>
        /// <returns></returns>
        private HTMLAttribute ParseAttribute()
        {
            HTMLAttribute attribute = new HTMLAttribute();
            attribute.Name = ToLower(token);

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

        private string ToLower(object tag)
        {
            return ((string)tag).ToLower();
        }
    }
}