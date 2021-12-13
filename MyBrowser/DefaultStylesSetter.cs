using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBrowser
{
    public class DefaultStylesSetter
    {
        public void SetDefaultStyles(DOM dom)
        {
            var body = dom.Body;
            SetDefaultStylesToElement(body);
        }

        private void SetDefaultStylesToElement(HTMLElement element)
        {
            element.Styles = element.Tag switch
            {
                "body" => new List<Style>() { new Style("margin", new Value(ValueType.Absolute, 8)) },

                "h1" => GetDefaultHeaderStyles(2, 0.67),
                "h2" => GetDefaultHeaderStyles(1.5, 0.83),
                "h3" => GetDefaultHeaderStyles(1.17, 1),
                "h4" => GetDefaultHeaderStyles(1, 1.33),
                "h5" => GetDefaultHeaderStyles(0.83, 1.67),
                "h6" => GetDefaultHeaderStyles(0.67, 2.33),

                "ul" => GetDefaultStyles(),
                "ol" => GetDefaultStyles(),
                "p" => GetDefaultStyles(),

                _ => new List<Style>()
            }; 
            
            foreach (var child in element.Children)
            {
                if (child is HTMLElement)
                {
                    var childElem = (HTMLElement)child;
                    SetDefaultStylesToElement(childElem);
                }
            }
        }

        private List<Style> GetDefaultHeaderStyles(double fontSize, double margin)
        {
            return new List<Style>() { new Style("font-size", new Value(ValueType.Em, fontSize)),
                                       new Style("margin-top", new Value(ValueType.Em, margin)),
                                       new Style("margin-bottom", new Value(ValueType.Em, margin)),
            };
        }

        private List<Style> GetDefaultStyles()
        {
            return new List<Style>() { new Style("margin-top", new Value(ValueType.Em, 1.0)),
                                       new Style("margin-bottom", new Value(ValueType.Em, 1.0)), };
        }
    }
}
