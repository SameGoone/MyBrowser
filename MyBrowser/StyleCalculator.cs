using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyBrowser
{
    public static class StyleListExtensions
    {
        public static bool TryGetStyleWithName(this List<Style> styles, string name, out Style style)
        {
            bool isContained = false;
            style = null;
            foreach (var st in styles)
            {
                if (st.Name == name)
                {
                    isContained = true;
                    style = st;
                }
            }

            return isContained;
        }
    }

    public enum Sides
    {
        Top,
        Right,
        Bottom,
        Left,
        All
    }

    internal class StyleCalculator
    {
        Dictionary<string, int> borderWidthKeywords = new Dictionary<string, int>() { { "thin", 2 }, { "medium", 5 }, { "thick", 10 } };
        Dictionary<string, Color> colorKeywords = new Dictionary<string, Color> { { "red", Colors.Red }, { "blue", Colors.Blue }, { "yellow", Colors.Yellow }, { "green", Colors.Green }, { "purple", Colors.Purple }, { "black", Colors.Black }, { "white", Colors.White } };
        HTMLElement currElement;

        public void CalculateStyles(DOM dom)
        {
            CalculateElementStyles(dom.Document);
        }

        private void CalculateElementStyles(HTMLElement element)
        {
            currElement = element;
            currElement.BorderWidth = GetBorderWidthWithKeyword("medium");
            currElement.BorderColor = Colors.Transparent;
            currElement.BackgroundColor = Colors.Transparent;

            if (element.Parent != null)
            {
                currElement.FontSize = element.Parent.FontSize;
                currElement.FontColor = element.Parent.FontColor;
                currElement.FontFamily = element.Parent.FontFamily;
            }
            else
            {
                currElement.FontSize = 16;
                currElement.FontColor = Colors.Black;
                currElement.FontFamily = new FontFamily("Calibri");
            }

            if (currElement.Styles.TryGetStyleWithName("font-size", out Style fontSize))
            {
                currElement.FontSize = GetAbsoluteValue(fontSize.Values[0]);
                currElement.Styles.Remove(fontSize);
            }

            foreach (var style in currElement.Styles)
            {
                if (style.Name.Contains("margin"))
                    CalculateMargin(style);
                else if (style.Name.Contains("padding"))
                    CalculatePadding(style);
                else if (style.Name == "border")
                    CalculateBorder(style);
                else if (style.Name == "font-family")
                    CalculateFontFamily(style);
                else if (style.Name == "color")
                    CalculateFontColor(style.Values[0]);
                else if (style.Name == "background-color")
                    CalculateBackgroundColor(style.Values[0]);
            }

            foreach (HTMLNode child in element.Children)
            {
                if (child is HTMLElement)
                    CalculateElementStyles((HTMLElement)child);
            }
        }

        private void CalculateMargin(Style style)
        {
            var margin = GetQuadrupleProperty("margin", style, out Sides side);
            if (side == Sides.All && margin.Length == 4)
            {
                currElement.MarginTop = margin[0];
                currElement.MarginRight = margin[1];
                currElement.MarginBottom = margin[2];
                currElement.MarginLeft = margin[3];
            }
            else if (side == Sides.Top)
                currElement.MarginTop = margin[0];

            else if (side == Sides.Right)
                currElement.MarginRight = margin[0];

            else if (side == Sides.Bottom)
                currElement.MarginBottom = margin[0];

            else if (side == Sides.Left)
                currElement.MarginLeft = margin[0];

            else
                throw new Exception("Ошибка разбора свойства margin");
        }

        private void CalculatePadding(Style style)
        {
            var padding = GetQuadrupleProperty("padding", style, out Sides side);
            if (side == Sides.All && padding.Length == 4)
            {
                currElement.PaddingTop = padding[0];
                currElement.PaddingRight = padding[1];
                currElement.PaddingBottom = padding[2];
                currElement.PaddingLeft = padding[3];
            }
            else if (side == Sides.Top)
                currElement.PaddingTop = padding[0];

            else if (side == Sides.Right)
                currElement.PaddingRight = padding[0];

            else if (side == Sides.Bottom)
                currElement.PaddingBottom = padding[0];

            else if (side == Sides.Left)
                currElement.PaddingLeft = padding[0];

            else
                throw new Exception("Ошибка разбора свойства padding");
        }

        private int[] GetQuadrupleProperty(string propertyName, Style style, out Sides sideOfProperty)
        {
            int[] result;
            string styleName = style.Name;
            styleName = styleName.Replace(propertyName, "");
            if (styleName == "")
            {
                sideOfProperty = Sides.All;
                result = new int[4];
                if (style.Values.Count == 1)
                {
                    int value = GetAbsoluteValue(style.Values[0]);
                    result[0] = value;
                    result[1] = value;
                    result[2] = value;
                    result[3] = value;
                }
                else if (style.Values.Count == 2)
                {
                    int verticalValue = GetAbsoluteValue(style.Values[0]);
                    result[0] = verticalValue;
                    result[2] = verticalValue;
                    int horizontalValue = GetAbsoluteValue(style.Values[1]);
                    result[1] = horizontalValue;
                    result[3] = horizontalValue;
                }
                else if (style.Values.Count == 3)
                {
                    result[0] = GetAbsoluteValue(style.Values[0]);
                    int horizontalValue = GetAbsoluteValue(style.Values[1]);
                    result[1] = horizontalValue;
                    result[3] = horizontalValue;
                    result[2] = GetAbsoluteValue(style.Values[2]);
                }
                else if (style.Values.Count == 4)
                {
                    result[0] = GetAbsoluteValue(style.Values[0]);
                    result[1] = GetAbsoluteValue(style.Values[1]);
                    result[2] = GetAbsoluteValue(style.Values[2]);
                    result[3] = GetAbsoluteValue(style.Values[3]);
                }
            }
            else
            {
                result = new int[1];
                result[0] = GetAbsoluteValue(style.Values[0]);

                styleName = styleName.Replace("-", "");
                if (styleName == "top")
                    sideOfProperty = Sides.Top;

                else if (styleName == "right")
                    sideOfProperty = Sides.Right;

                else if (styleName == "bottom")
                    sideOfProperty = Sides.Bottom;

                else if (styleName == "left")
                    sideOfProperty = Sides.Left;

                else
                    sideOfProperty = Sides.Left;
            }

            return result;
        }

        private void CalculateBorder(Style style)
        {
            var values = style.Values;
            foreach (var value in values)
            {
                if (value.Type == ValueType.BorderWidth)
                {
                    currElement.BorderWidth = GetBorderWidthWithKeyword((string)value.Val);
                }
                else if (value.Type == ValueType.Absolute || value.Type == ValueType.Em)
                {
                    currElement.BorderWidth = GetAbsoluteValue(value);
                }
                else if (value.Type == ValueType.ColorName)
                {
                    currElement.BorderColor = GetColorWithColorName((string)value.Val);
                }
                else if (value.Type == ValueType.ColorHexRGB)
                {
                    currElement.BorderColor = GetColorHexRGB((string)value.Val);
                }
            }
        }

        private int GetBorderWidthWithKeyword(string keyword)
        {
            return borderWidthKeywords[keyword];
        }

        private void CalculateFontFamily(Style style)
        {
            var converter = new FontFamilyConverter();
            var fontFamily = converter.ConvertFromString((string)style.Values[0].Val) as FontFamily;
            currElement.FontFamily = fontFamily;
        }

        private void CalculateBackgroundColor(Value value)
        {
            currElement.BackgroundColor = GetColor(value);
        }

        private void CalculateFontColor(Value value)
        {
            currElement.FontColor = GetColor(value);
        }

        private Color GetColor(Value value)
        {
            if (value.Type == ValueType.ColorName)
                return GetColorWithColorName((string)value.Val);

            else if (value.Type == ValueType.ColorHexRGB)
                return GetColorHexRGB((string)value.Val);

            else
                return Colors.Black;
        }

        private Color GetColorWithColorName(string colorName)
        {
            if (colorKeywords.ContainsKey(colorName))
                return colorKeywords[colorName];
            else
                return Colors.Transparent;
        }

        private Color GetColorHexRGB(string hexRGB)
        {
            byte r = Convert.ToByte(hexRGB[1].ToString() + hexRGB[2].ToString(), 16);
            byte g = Convert.ToByte(hexRGB[3].ToString() + hexRGB[4].ToString(), 16);
            byte b = Convert.ToByte(hexRGB[5].ToString() + hexRGB[6].ToString(), 16);
            return Color.FromRgb(r, g, b);
        }

        private int GetAbsoluteValue(Value value)
        {
            var parent = currElement.Parent;
            if (value.Type == ValueType.Absolute)
                return (int)value.Val;
            else if (value.Type == ValueType.Em)
                return (int)(currElement.FontSize * (double)value.Val);

            else 
                return 0;
        }
    }
}
