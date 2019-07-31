using System.Windows;
using Pascal.Ide.Wpf.Models;
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
            InitializeComponent();
        }

    }
}
