using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class CSSRule
    {
        public ISelector Selector
        {
            get
            {
                return m_Selector;
            }
            set
            {
                m_Selector = value;
            }
        }
        private ISelector m_Selector;

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
    }
}
