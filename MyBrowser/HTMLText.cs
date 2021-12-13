using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBrowser
{
    public class HTMLText : HTMLNode
    {
        public string Text { get; private set; }

        public HTMLText (string text)
        {
            Text = text;
        }

        public override bool Equals(object? obj)
        {
            var other = obj as HTMLText;
            if (other != null)
                return Text == other.Text;
            else
                return false;
        }

        public override string GetString()
        {
            return $"\"{Text}\"";
        }
    }
}
