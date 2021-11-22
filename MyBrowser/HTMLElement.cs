using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class HTMLElement
    {
        public string Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }
        private string m_Tag;

        public HTMLElement Parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                m_Parent = value;
            }
        }
        private HTMLElement m_Parent;

        public List<HTMLElement> Children
        {
            get
            {
                return m_Children;
            }
            set
            {
                m_Children = value;
            }
        }
        private List<HTMLElement> m_Children;

        public List<HTMLElement> Descendants
        {
            get
            {
                return m_Descendants;
            }
            set
            {
                m_Descendants = value;
            }
        }
        private List<HTMLElement> m_Descendants;

        public string InnerText
        {
            get
            {
                return m_InnerText;
            }
            set
            {
                m_InnerText = value;
            }
        }
        private string m_InnerText;

        public List<HTMLAttribute> Attributes
        {
            get
            {
                return m_Attributes;
            }
            set
            {
                m_Attributes = value;
            }
        }
        private List<HTMLAttribute> m_Attributes;

        public List<Style> Styles
        {
            get
            {
                return m_Styles;
            }
            set
            {
                m_Styles = value;
            }
        }
        private List<Style> m_Styles;

        public List<HTMLElement> Siblings
        {
            get
            {
                return m_Siblings;
            }
            set
            {
                m_Siblings = value;
            }
        }
        private List<HTMLElement> m_Siblings;

        public HTMLElement NextSibling
        {
            get
            {
                return m_NextSibling;
            }
            set
            {
                m_NextSibling = value;
            }
        }
        private HTMLElement m_NextSibling;

        public HTMLElement PreviousSibling
        {
            get
            {
                return m_PreviousSibling;
            }
            set
            {
                m_PreviousSibling = value;
            }
        }
        private HTMLElement m_PreviousSibling;

        public HTMLElement()
        {
            m_Attributes = new List<HTMLAttribute>();
            m_Children = new List<HTMLElement>();
        }


        public bool TryGetAttributeWithName(string attributeName, HTMLAttribute attribute)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasAttributeWithName(string attributeName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsChildOfElement(string elementTag)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsDescendantOfElement(string elementTag)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsRenderable()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}