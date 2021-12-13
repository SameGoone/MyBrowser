using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBrowser.Tests
{
    [TestClass]
    public class HTMLElementTests
    {
        [TestMethod]
        public void TestAddChild()
        {
            HTMLElement parent = new HTMLElement("a");
            HTMLElement child1 = new HTMLElement("b");
            HTMLElement child2 = new HTMLElement("c");

            parent.AddChild(child1);
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreEqual(child1, parent.Children[0]);
            Assert.AreEqual(parent, child1.Parent);

            Assert.AreEqual(null, child1.PreviousSibling);
            Assert.AreEqual(null, child1.NextSibling);


            parent.AddChild(child2);
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreEqual(child1, parent.Children[0]);
            Assert.AreEqual(child2, parent.Children[1]);
            Assert.AreEqual(parent, child2.Parent);

            Assert.AreEqual(null, child1.PreviousSibling);
            Assert.AreEqual(child2, child1.NextSibling);

            Assert.AreEqual(child1, child2.PreviousSibling);
            Assert.AreEqual(null, child2.NextSibling);
        }

        private void CheckChildren(HTMLElement element, List<HTMLElement> expectedChildren)
        {

        }

        [TestMethod]
        public void TestAddChildrenRange()
        {

        }

        [TestMethod]
        public void TestGetSiblings()
        {
            HTMLElement parent = new HTMLElement("a");
            HTMLElement child1 = new HTMLElement("b");
            HTMLElement child2 = new HTMLElement("c");
            HTMLElement child3 = new HTMLElement("d");
            HTMLElement child4 = new HTMLElement("e");
            HTMLElement child5 = new HTMLElement("f");

            List<HTMLElement> children = new List<HTMLElement>() { child1, child2, child3, child4, child5 };

            parent.AddChildrenRange(children);
            foreach(HTMLElement child in children)
            {
                CheckSiblings(child, children);
            }
        }

        private void CheckSiblings(HTMLElement element, List<HTMLElement> allChildren)
        {
            var expectedSiblings = new List<HTMLElement>(allChildren);
            expectedSiblings.Remove(element);

            CollectionAssert.AreEqual(expectedSiblings, element.Siblings);
        }
    }
}
