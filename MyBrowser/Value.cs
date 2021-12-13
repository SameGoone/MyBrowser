using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBrowser
{
    public enum ValueType
    {
        Em,
        Absolute,
        ColorName,
        ColorHexRGB,
        Id,
        BorderWidth,
    }

    public class Value
    {
        public ValueType Type { get; set; }
        public object Val { get; set; }
        public Value(ValueType type, object val)
        {
            Type = type;
            Val = val;
        }
    }
}
