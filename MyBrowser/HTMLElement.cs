using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MyBrowser
{
    public class HTMLElement : HTMLNode
    {
        public string Tag { get; private set; }
        public List<HTMLAttribute> Attributes { get; set; } = new List<HTMLAttribute>();
        public List<Style> Styles { get; set; } = new List<Style>();

        public int FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public Color FontColor { get; set; }

        public int MarginTop { get; set; }
        public int MarginRight { get; set; }
        public int MarginBottom { get; set; }
        public int MarginLeft { get; set; }

        public int PaddingTop { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingBottom { get; set; }
        public int PaddingLeft { get; set; }

        public int BorderWidth { get; set; }
        public Color BorderColor { get; set; }

        public Color BackgroundColor { get; set; }


        public HTMLElement(string tag)
        {
            Tag = tag;
        }

        public bool TryGetAttributeWithName(string attributeName, out HTMLAttribute attribute)
        {
            bool isContained = false;
            attribute = null;
            foreach (var attr in Attributes)
            {
                if (attribute.Name == attributeName)
                {
                    isContained = true;
                    attribute = attr;
                }
            }

            return isContained;
        }

        public bool IsChildOfElement(string elementTag)
        {
            bool result;
            if (Parent != null)
            {
                if (Parent.Tag == elementTag)
                    result = true;
                else
                    result = false;
            }

            else
                result = false;

            return result;
        }

        public bool IsDescendantOfElement(string elementTag)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsRenderable()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddChild(HTMLNode newChild)
        {
            if (Children.Count > 0)
            {
                var previousChild = Children[Children.Count - 1];
                previousChild.NextSibling = newChild;
                newChild.PreviousSibling = previousChild;
            }
            Children.Add(newChild);
            newChild.Parent = this;
        }

        public void AddChildrenRange(List<HTMLNode> children)
        {
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        public override bool Equals(object? obj)
        {
            var other = obj as HTMLElement;
            if (other != null)
                return Tag == other.Tag && AttributesAreEqual(other);
            else
                return false;
        }

        private bool AttributesAreEqual(HTMLElement other)
        {
            bool result = true;
            if (Attributes.Count == other.Attributes.Count)
            {
                for (int i = 0; i < Attributes.Count; i++)
                {
                    if (!Attributes[i].Equals(other.Attributes[i]))
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override string GetString()
        {
            string result = $"<{Tag}";
            foreach (var attr in Attributes)
            {
                result += $" {attr.Name}=\"{attr.Value}\"";
            }
            result += ">";
            return result;
        }
    }
}