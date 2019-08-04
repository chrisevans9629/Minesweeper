using Pascal.Ide.Wpf.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using Akavache;

namespace Pascal.Ide.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        public override void Initialize()
        {
            Akavache.Registrations.Start("PascalIDE");
            base.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            BlobCache.Shutdown().Wait();
            base.OnExit(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
