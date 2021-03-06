using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBrowser
{
    public class ElementParsingResult
    {
        public static ElementParsingResult GetOkResult()
        {
            return new OkResult();
        }

        public static ElementParsingResult GetCloseAnotherTagResult(string tag, HTMLElement firstElement, List<HTMLNode> potentialChildren)
        {
            return new CloseAnotherTagResult(tag, firstElement, potentialChildren);
        }
    }

    public class OkResult : ElementParsingResult
    {

    }

    public class CloseAnotherTagResult : ElementParsingResult
    {
        public string Tag { get; private set; }
        public List<HTMLNode> ElementsWithoutCloseTag { get; private set; } = new List<HTMLNode>();

        public CloseAnotherTagResult(string tag, HTMLElement firstElement, List<HTMLNode> potentialChildren)
        {
            Tag = tag;
            AddElementWithPotentialChildren(firstElement, potentialChildren);
        }

        public void AddElementWithPotentialChildren(HTMLElement element, List<HTMLNode> potentialChildren)
        {
            for (int i = potentialChildren.Count - 1; i >= 0; i--)
            {
                AddElement(potentialChildren[i]);
            }
            AddElement(element);
        }

        private void AddElement(HTMLNode element)
        {
            ElementsWithoutCloseTag.Insert(0, element);
        }
    }
}
