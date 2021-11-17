namespace MyBrowser
{
    public class DOM
    {
        public HTMLElement Document
        {
            get
            {
                return m_Document;
            }
            set
            {
                m_Document = value;
            }
        }
        private HTMLElement m_Document;

        public HTMLElement Body
        {
            get
            {
                return m_Body;
            }
            set
            {
                m_Body = value;
            }
        }
        private HTMLElement m_Body;

        public HTMLElement Head
        {
            get
            {
                return m_Head;
            }
            set
            {
                m_Head = value;
            }
        }
        private HTMLElement m_Head;
    }
}

