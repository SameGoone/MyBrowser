namespace MyBrowser
{
    public class DOM
    {
        public HTMLElement Document { get; set; }

        public HTMLElement Body
        {
            get
            {
                return body;
            }
            set
            {
                body = value;
                Document.AddChild(body);
            }
        }
        private HTMLElement body;

        public HTMLElement Head
        {
            get
            {
                return head;
            }
            set
            {
                head = value;
                Document.AddChild(head);
            }
        }
        private HTMLElement head;
    }
}

