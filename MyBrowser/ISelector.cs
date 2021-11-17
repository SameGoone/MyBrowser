using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public interface ISelector
    {
        bool IsElementMatch(HTMLElement element);
    }
}

