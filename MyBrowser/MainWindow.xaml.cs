using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Singleton
        {
            get
            {
                return singleton;
            }
        }
        private static MainWindow singleton;

        public Panel Viewport { get; private set; }
        public int ViewportWidth
        {
            get
            {
                return (int)Viewport.Width;
            }
        }
        public int ViewportHeight
        {
            get
            {
                return (int)Viewport.Height;
            }
        }

        DOM dom;
        public MainWindow()
        {
            InitializeComponent();
            if (singleton == null)
                singleton = this;

            Viewport = StackPanel;
        }

        private void ParseButton_Click(object sender, RoutedEventArgs e)
        {
            ParserHTML parserHTML = new ParserHTML();
            dom = parserHTML.Parse(TextBox1.Text);
            HTMLElement doc = dom.Document;

            DefaultStylesSetter setter = new DefaultStylesSetter();
            setter.SetDefaultStyles(dom);

            StyleCalculator calculator = new StyleCalculator();
            calculator.CalculateStyles(dom);

            ShowItem(doc, null);
        }

        private void ShowItem(HTMLNode element, TreeItem parent)
        {
            var newTreeItem = new TreeItem(element) { Header = element.GetString() };
            if (element.Children.Count == 0)
            {
                newTreeItem.IsExpanded = false;
            }
            else
            {
                newTreeItem.IsExpanded = true;
                foreach (var child in element.Children)
                {
                    ShowItem(child, newTreeItem);
                }
            }

            if (parent != null)
                parent.Items.Add(newTreeItem);
            else
            {
                treeView.Items.Clear();
                treeView.Items.Add(newTreeItem);
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            HTMLNode node = ((TreeItem)((TreeView)sender).SelectedItem).AssociatedNode;
            string nodeInfo = "";
            NodeInfoTextBlock.Text = "";
            if (node is HTMLText)
            {
                HTMLText text = node as HTMLText;
                nodeInfo += "#text" + Environment.NewLine;
                nodeInfo += text.Text;
            }
            else
            {
                HTMLElement element = node as HTMLElement;
                nodeInfo += $"<{element.Tag}>" + Environment.NewLine;
                nodeInfo += $"font-size: {element.FontSize}" + Environment.NewLine;
                nodeInfo += $"font-family: {element.FontFamily.FamilyNames[Language]}" + Environment.NewLine;
                nodeInfo += $"font-color: {element.FontColor}" + Environment.NewLine;

                nodeInfo += $"margin-top: {element.MarginTop}" + Environment.NewLine;
                nodeInfo += $"margin-right: {element.MarginRight}" + Environment.NewLine;
                nodeInfo += $"margin-bottom: {element.MarginBottom}" + Environment.NewLine;
                nodeInfo += $"margin-left: {element.MarginLeft}" + Environment.NewLine;

                nodeInfo += $"padding-top: {element.PaddingTop}" + Environment.NewLine;
                nodeInfo += $"padding-right: {element.PaddingRight}" + Environment.NewLine;
                nodeInfo += $"padding-bottom: {element.PaddingBottom}" + Environment.NewLine;
                nodeInfo += $"padding-left: {element.PaddingLeft}" + Environment.NewLine;

                nodeInfo += $"border-width: {element.BorderWidth}" + Environment.NewLine;
                nodeInfo += $"border-color: {element.BorderColor}" + Environment.NewLine;
                nodeInfo += $"background-color: {element.BackgroundColor}";
            }
            NodeInfoTextBlock.Text = nodeInfo;
        }

        private void RenderButton_Click(object sender, RoutedEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            StackPanel.Children.Clear();
            Renderer renderer = new Renderer();
            renderer.Render(dom);
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{(int)28.9384}");
        }
    }
}
