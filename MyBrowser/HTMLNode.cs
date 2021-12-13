using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBrowser
{
    public abstract class HTMLNode
    {
        public VisualItem VisualItem { get; set; }
        public HTMLElement Parent { get; set; }

        public List<HTMLNode> Children { get; private set; } = new List<HTMLNode>();

        public List<HTMLNode> Descendants { get; private set; }

        public List<HTMLNode> Siblings
        {
            get
            {
                if (Parent != null)
                {
                    var childrenOfParent = new List<HTMLNode>(Parent.Children);
                    if (childrenOfParent.Count > 0)
                    {
                        childrenOfParent.Remove(this);
                        return childrenOfParent;
                    }
                    else
                        return new List<HTMLNode>();
                }
                else
                    return new List<HTMLNode>();
            }
        }

        public HTMLNode NextSibling { get; set; }

        public HTMLNode PreviousSibling { get; set; }

        public abstract string GetString();
    }
}
