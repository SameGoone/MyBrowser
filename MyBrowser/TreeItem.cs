using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyBrowser
{
    internal class TreeItem : TreeViewItem
    {
        public HTMLNode AssociatedNode { get; private set; }
        public TreeItem(HTMLNode associatedNode)
        {
            AssociatedNode = associatedNode;
        }
    }
}
