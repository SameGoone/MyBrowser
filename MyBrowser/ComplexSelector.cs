using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MyBrowser
{
    public class ComplexSelector : ISelector
    {
        private ISelector leftSelector;
        private ISelector rightSelector;
        private CombinatorTypes combinator;

        public bool IsElementMatch(HTMLElement element)
        {
            throw new NotImplementedException();
        }
    }
}

