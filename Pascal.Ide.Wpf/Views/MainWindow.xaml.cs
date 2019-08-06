using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Minesweeper.Test;
using Unity;

namespace Pascal.Ide.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IUnityContainer container)
        {
            var ser = container.Resolve<IDocumentService>();
            InitializeComponent();
            ser.Initialize(RichTextBox);
        }

    }
}
