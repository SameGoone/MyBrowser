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
        /// Строит Dom-дерево на основе переданного HTML-кода
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
        /// Считывает следующий элемент HTML
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
                        // Ошибка кода
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
                            // Ошибка кода
                        }
                        else;
                        // Ошибка кода
                    }

                    else if (token.GetType() == typeof(string))
                        newElement.Children.Add(ParseElement());

                    else if (token.Equals('!')) ;
                        // Потенциально комментарий

                    // TODO: добавить поддержку простых тегов
                }
            }
        }

        /// <summary>
        /// Считывает следующий HTML-атрибут 
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
                // TODO: Добавить поддержку атрибутов без кавычек
            }
            else
                attribute.Value = "";

            return attribute;
        }

        /// <summary>
        /// Считывает текст между тегами
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
        /// Пропускает комментарии
        /// </summary>
        private void ParseComment()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Получает следующий токен из лексера
        /// </summary>
        private void Shift()
        {
            lexer.TryGetToken(out curToken);
        }
    }
}