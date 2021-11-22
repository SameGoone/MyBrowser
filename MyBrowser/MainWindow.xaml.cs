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
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ParserHTML parserHTML = new ParserHTML();
            var dom = parserHTML.Parse(TextBox1.Text);
            HTMLElement doc = dom.Document;
            TreeViewItem treeRoot = new TreeViewItem() { Header = doc.Tag };
            if (doc.Children.Count > 0)
            {
                treeRoot.IsExpanded = true;
                foreach (var child in doc.Children)
                    ShowItem(child, treeRoot);
            }


            treeView.Items.Clear();
            treeView.Items.Add(treeRoot);
        }

        private void ShowItem(HTMLElement element, TreeViewItem parent)
        {
            var newTreeItem = new TreeViewItem() { Header = element.Tag };
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
            parent.Items.Add(newTreeItem);
        }
    }
}
