using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyBrowser
{
    public class ListTextBox : RichTextBox
    {
        private List list;

        public ListTextBox(TextMarkerStyle markerStyle)
        {
            IsReadOnly = true;
            BorderBrush = Brushes.Transparent;
            list = new List();
            list.MarkerStyle = markerStyle;
            Document = new FlowDocument();
            Document.Blocks.Add(list);
        }
        
        public void AddListItem(ListItem listItem)
        {
            list.ListItems.Add(listItem);
        }
    }
}
