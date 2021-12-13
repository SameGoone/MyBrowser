using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    /// <summary>
    /// ���������� ��� ������� ������ �� �������� ��������� � �����
    /// </summary>
    public static class ListExtensions
    {
        public static bool TryGetElementWithTag(this List<HTMLElement> elements, string tag, out HTMLElement element)
        {
            bool result = false;
            element = null;

            foreach (HTMLElement elem in elements)
            {
                if (elem.Tag.Equals(tag))
                {
                    element = elem;
                    result = true;
                    break;
                }
            }

            return result;
        }

        public static bool HasElementWithTag(this List<HTMLElement> elements, string tag)
        {
            bool result = false;

            foreach (HTMLElement elem in elements)
            {
                if (elem.Tag.Equals(tag))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public static bool TryGetElementWithTag(this List<HTMLNode> nodes, string tag, out HTMLElement element)
        {
            bool result = false;
            element = null;

            foreach (HTMLNode node in nodes)
            {
                if (node is HTMLElement)
                {
                    var elem = node as HTMLElement;
                    if (elem.Tag.Equals(tag))
                    {
                        element = elem;
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
    }

    public class ParserHTML
    {
        private readonly string htmlTag = "html";
        private readonly string headTag = "head";
        private readonly string bodyTag = "body";
        private readonly List<string> emptyElements = new List<string> { "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr" };
        private readonly List<string> onlyHeadElements = new List<string> { "title", "link", "style", "bgsound", "base", "meta" };
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
                    List<HTMLElement> roots = new List<HTMLElement>();
                    while (!(token is NonToken))
                    {
                        ElementParsingResult result;
                        roots.Add(ParseElement(out result));
                    }
                    CorrectDOM(dom, roots);
                }
            }

            return dom;
        }

        /// <summary>
        /// ������������ ��������, ������� ��������� ��� �����������, � ������ ����������
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="potentialChildren"></param>
        private void CorrectDOM(DOM dom, List<HTMLElement> roots)
        {
            if (roots.Count == 1 && roots[0].Tag == htmlTag)
            {
                var tempHtml = roots[0];
                dom.Document = new HTMLElement(htmlTag);
                dom.Document.Attributes = tempHtml.Attributes;

                List<HTMLNode> elementsToAnalyze = new List<HTMLNode>(tempHtml.Children);
                HTMLElement body;
                if (tempHtml.Children.TryGetElementWithTag(bodyTag, out body))
                    elementsToAnalyze.Remove(body);
                else
                    body = new HTMLElement(bodyTag);
                dom.Body = body;

                HTMLElement head;
                if (tempHtml.Children.TryGetElementWithTag(headTag, out head))
                    elementsToAnalyze.Remove(head);
                else
                    head = new HTMLElement(headTag);
                dom.Head = head;

                dom.Document.AddChild(dom.Head);
                dom.Document.AddChild(dom.Body);

                if (elementsToAnalyze.Count > 0)
                {
                    foreach (HTMLNode node in elementsToAnalyze)
                    {
                        if (node is HTMLElement)
                        {
                            var element = node as HTMLElement;
                            if (element.Tag == headTag || element.Tag == bodyTag || element.Tag == htmlTag)
                                continue;

                            if (onlyHeadElements.Contains(element.Tag))
                                dom.Head.AddChild(element);
                            else
                                dom.Body.AddChild(element);
                        }
                        else
                            dom.Body.AddChild(node);
                    }
                }
            }
            else
            {
                HTMLElement elem;
                
                if (roots.TryGetElementWithTag(htmlTag, out HTMLElement html))
                {
                    dom.Document = html;
                    if (html.Children.TryGetElementWithTag(headTag, out HTMLElement head))
                        dom.Head = head;

                    if (html.Children.TryGetElementWithTag(headTag, out HTMLElement body))
                        dom.Body = body;
                }
                else
                {
                    dom.Document = new HTMLElement(htmlTag);
                }

                if (roots.TryGetElementWithTag(headTag, out elem))
                {
                    if (dom.Head == null)
                    {
                        dom.Head = elem;
                        dom.Document.AddChild(dom.Head);
                    }
                }

                if (roots.TryGetElementWithTag(bodyTag, out elem))
                {
                    if (dom.Body == null)
                    {
                        dom.Body = elem;
                        dom.Document.AddChild(dom.Body);
                    }
                }

                if (dom.Head == null)
                {
                    dom.Head = new HTMLElement(headTag);
                    dom.Document.AddChild(dom.Head);
                }

                if (dom.Body == null)
                {
                    dom.Body = new HTMLElement(bodyTag);
                    dom.Document.AddChild(dom.Body);
                }

                foreach (HTMLElement element in roots)
                {
                    if (element.Tag == headTag || element.Tag == bodyTag || element.Tag == htmlTag)
                        continue;

                    if (onlyHeadElements.Contains(element.Tag))
                        dom.Head.AddChild(element);
                    else
                        dom.Body.AddChild(element);
                }
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

            List<HTMLNode> potentialChildren = new List<HTMLNode>();
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
                        newElement.AddChild(ParseText());
                }
            }
        }

        /// <summary>
        /// ��������� ������������ �������� ��������
        /// </summary>
        /// <param name="element">������� ������������� �������</param>
        /// <param name="potentialChildren">��������, ������� ����������, ��� ������������ �������� � ��������</param>
        /// <returns>����������, ��� �� ��������������� ����������� ��� ������� ��������, ��� �������</returns>
        private ElementParsingResult ParseCloseTag(HTMLElement element, List<HTMLNode> potentialChildren)
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

        /// <summary>
        /// �������� ��������� �� �����, ��������� �� ����������� �������� ����
        /// </summary>
        private void ShiftToEndOfEndingTag()
        {
            Shift();
            if (token.Equals('>'))
                Shift();
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
                        result = ParseCloseTag(element, new List<HTMLNode>());
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

            element.AddChild(new HTMLText(script));
            return result;
        }

        /// <summary>
        /// ��������� ��������� HTML-������� 
        /// </summary>
        /// <returns></returns>
        private HTMLAttribute ParseAttribute()
        {
            string attributeName = ToLower(token);
            string attributeValue = "";

            Shift();
            if (token.Equals('='))
            {
                Shift();
                if (token.Equals('\"') || token.Equals('\''))
                {
                    attributeValue = GetStringData((char)token);
                }
                else;
                // TODO: �������� ��������� ��������� ��� �������
            }
            else
                attributeValue = "";

            return new HTMLAttribute(attributeName, attributeValue);
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
        private HTMLText ParseText()
        {
            string text = "";
            bool isFirst = true;
            while (!token.Equals('<') && !(token is NonToken))
            {
                if (isFirst)
                    isFirst = false;
                else
                    text += " ";

                text += token.ToString();
                Shift();
            }

            return new HTMLText(text);
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