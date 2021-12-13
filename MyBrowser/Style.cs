using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class Style
    {
        public string Name { get; set; }

        public List<Value> Values { get; set; } = new List<Value>();

        public Style(string name, params Value[] values)
        {
            Name = name;
            Values.AddRange(new List<Value>(values));
        }

        public void AddValue(ValueType type, object value)
        {
            Values.Add(new Value(type, value));
        }
    }
}