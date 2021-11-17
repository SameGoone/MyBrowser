using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class CombinedSelector : ISelector
    {
        private List<SimpleSelector> simpleSelectors;

        public bool IsElementMatch(HTMLElement element)
        {
            throw new NotImplementedException();
        }
    }
}

