using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class HTMLElement
    {
        public string Tag { get; set; }

        public HTMLElement Parent { get; set; }

        public List<HTMLElement> Children { get; private set; }

        public List<HTMLElement> Descendants { get; private set; }

        public string InnerText { get; set; }

        public List<HTMLAttribute> Attributes { get; private set; }

        public List<Style> Styles { get; private set; }

        public List<HTMLElement> Siblings { get; private set; }

        public HTMLElement NextSibling { get; set; }

        public HTMLElement PreviousSibling { get; set; }

        public HTMLElement()
        {
            Attributes = new List<HTMLAttribute>();
            Children = new List<HTMLElement>();
            InnerText = "";
        }


        public bool TryGetAttributeWithName(string attributeName, out HTMLAttribute attribute)
        {
            bool isContained = false;
            attribute = null;
            foreach(var attr in Attributes)
            {
                if (attribute.Name == attributeName)
                {
                    isContained = true;
                    attribute = attr;
                }
            }

            return isContained;
        }

        public bool HasAttributeWithName(string attributeName)
        {
            throw new Exception("The method or operation is not implemented.");
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

        public void AddChild(HTMLElement newChild)
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

        public void AddChildrenRange(List<HTMLElement> children)
        {
            foreach (var child in children)
            {
                AddChild(child);
            }
        }
    }
}