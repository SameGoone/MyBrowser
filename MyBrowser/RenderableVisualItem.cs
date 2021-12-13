using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyBrowser
{
    public class RenderableVisualItem : VisualItem
    {
        public Border Root { get; private set; }

        public FrameworkElement MainElement { get; private set; }

        public RenderableVisualItem(FrameworkElement mainElement)
        {
            MainElement = mainElement;
            Root = new Border();
            Root.Child = MainElement;
        }
    }
}
