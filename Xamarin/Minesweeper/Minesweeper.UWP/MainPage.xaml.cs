using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace SqueakyApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadApplication(new Minesweeper.App());
        }
    }
}
