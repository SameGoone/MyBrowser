using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MyBrowser
{
    public class VisualListItem : VisualItem
    {
        public ListItem Item { get; set; }
        public VisualListItem(string text)
        {
            Item = new ListItem(new Paragraph(new Run(text)));
        }
    }
}
