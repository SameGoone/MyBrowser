using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace MyBrowser
{
    public class HTMLAttribute
    {
        public string Name { get; private set; }

        public string Value { get; private set; }

        public HTMLAttribute (string name, string value)
        {
            Name = name; 
            Value = value; 
        }

        public override bool Equals(object? obj)
        {
            var other = obj as HTMLAttribute;
            if (other != null)
            {
                return Name == other.Name && Value == other.Value;
            }
            else
                return false;
        }
    }
}
