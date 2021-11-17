using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public abstract class SimpleSelector : ISelector
    {
        public abstract bool IsElementMatch(HTMLElement element);
    }
}