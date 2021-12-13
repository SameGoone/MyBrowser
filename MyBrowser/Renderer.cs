using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

namespace MyBrowser
{
    public class Renderer
    {
        public void Render(DOM dom)
        {
            BuildLayout(dom);
            Draw(dom);
        }

        private void BuildLayout(DOM dom)
        {
            CalculateVisual(dom.Body);
        }

        private void CalculateVisual(HTMLNode node)
        {
            if (node is HTMLText)
            {
                node.VisualItem = new TextVisualItem(new Run((node as HTMLText).Text));
            }
            else
            {
                bool calculateChildren = true;
                HTMLElement element = node as HTMLElement;

                string text;
                switch (element.Tag)
                {
                    case "li":
                        if (TryGetText(element, out text))
                            element.VisualItem = new VisualListItem(text);
                        else
                            element.VisualItem = new VisualListItem("");
                        calculateChildren = false;
                        break;

                    case "ul":
                        element.VisualItem = new RenderableVisualItem(new ListTextBox(TextMarkerStyle.Circle));
                        break;
                    case "ol":
                        element.VisualItem = new RenderableVisualItem(new ListTextBox(TextMarkerStyle.Decimal));
                        break;

                    case "b":
                        if (TryGetText(element, out text))
                            element.VisualItem = new TextVisualItem(new Bold(new Run(text)));
                        else
                            element.VisualItem = new TextVisualItem(new Bold(new Run("")));
                        calculateChildren = false;
                        break;

                    case "i":
                        if (TryGetText(element, out text))
                            element.VisualItem = new TextVisualItem(new Italic(new Run(text)));
                        else
                            element.VisualItem = new TextVisualItem(new Italic(new Run("")));
                        calculateChildren = false;
                        break;

                    case "br":
                        element.VisualItem = new BreakLineItem();
                        break;

                    case "body":
                        element.VisualItem = new RenderableVisualItem(new StackPanel());
                        break;

                    case "div":
                        element.VisualItem = new RenderableVisualItem(new StackPanel());
                        break;

                    case "h1":
                        element.VisualItem = new RenderableVisualItem(new TextBlock());
                        calculateChildren = false;
                        break;
                    case "h2":
                        element.VisualItem = new RenderableVisualItem(new TextBlock());
                        calculateChildren = false;
                        break;
                    case "h3":
                        element.VisualItem = new RenderableVisualItem(new TextBlock());
                        calculateChildren = false;
                        break;
                    case "h4":
                        element.VisualItem = new RenderableVisualItem(new TextBlock());
                        calculateChildren = false;
                        break;
                    case "h5":
                        element.VisualItem = new RenderableVisualItem(new TextBlock());
                        calculateChildren = false;
                        break;
                    case "h6":
                        element.VisualItem = new RenderableVisualItem(new TextBlock());
                        calculateChildren = false;
                        break;

                    case "p":
                        element.VisualItem = new RenderableVisualItem(new TextBlock());
                        calculateChildren = false;
                        break;

                    default:
                        element.VisualItem = null;
                        break;

                }

                SetRenderableParams(element);

                foreach(var child in node.Children)
                {
                    CalculateVisual(child);
                }
            }
        }

        private bool TryGetText(HTMLElement element, out string text)
        {
            text = "";
            bool contains = false;
            if (element.Children.Count > 0)
            {
                if (element.Children[0] is HTMLText)
                {
                    text = (element.Children[0] as HTMLText).Text;
                    contains = true;
                }
            }

            return contains;
        }

        private void SetRenderableParams(HTMLElement element)
        {
            if (element.VisualItem == null || !(element.VisualItem is RenderableVisualItem))
                return;

            var renderable = element.VisualItem as RenderableVisualItem;
            renderable.Root.BorderThickness = new Thickness(element.BorderWidth);
            renderable.Root.BorderBrush = new SolidColorBrush(element.BorderColor);
            renderable.Root.Margin = new Thickness(element.MarginLeft, element.MarginTop, element.MarginRight, element.MarginBottom);
            renderable.Root.Padding = new Thickness(element.PaddingLeft, element.PaddingTop, element.PaddingRight, element.PaddingBottom);

            var main = renderable.MainElement;
            if (main is TextBlock)
            {
                TextBlock tb = main as TextBlock;
                tb.FontSize = element.FontSize;
                tb.FontFamily = element.FontFamily;
                tb.Foreground = new SolidColorBrush(element.FontColor);
            }
        }

        private void Draw(DOM dom)
        {
            DrawNode(dom.Body, null);
        }

        private void DrawNode(HTMLNode node, RenderableVisualItem parent)
        {
            VisualItem visualItem = node.VisualItem;
            if (visualItem == null)
                return;

            if (parent == null)
            {
                if (visualItem is RenderableVisualItem)
                {
                    var renderable = visualItem as RenderableVisualItem;
                    MainWindow.Singleton.Viewport.Children.Add(renderable.Root);
                }

                else if (visualItem is TextVisualItem)
                {
                    var text = visualItem as TextVisualItem;
                    MainWindow.Singleton.Viewport.Children.Add(new TextBlock(text.Text));
                }
            }
            else if (parent.MainElement is StackPanel)
            {
                StackPanel stackPanel = parent.MainElement as StackPanel;
                if (visualItem is RenderableVisualItem)
                {
                    var renderable = visualItem as RenderableVisualItem;
                    stackPanel.Children.Add(renderable.Root);
                }

                else if (visualItem is TextVisualItem)
                {
                    var text = visualItem as TextVisualItem;
                    stackPanel.Children.Add(new TextBlock(text.Text));
                }
            }

            else if (parent.MainElement is TextBlock)
            {
                TextBlock textBlock = parent.MainElement as TextBlock;
                if (visualItem is TextVisualItem)
                    textBlock.Inlines.Add((visualItem as TextVisualItem).Text);
                else if (visualItem is BreakLineItem)
                    textBlock.Text += Environment.NewLine;
            }

            else if (parent.MainElement is ListTextBox)
            {
                ListTextBox listTextBox = parent.MainElement as ListTextBox;
                if (visualItem is VisualListItem)
                    listTextBox.AddListItem((visualItem as VisualListItem).Item);
            }

            if (node is HTMLElement && node.VisualItem is RenderableVisualItem)
            {
                var element = node as HTMLElement;
                var renderable = node.VisualItem as RenderableVisualItem;
                foreach (var child in element.Children)
                {
                    DrawNode(child, renderable);
                }
            }
        }
    }
}