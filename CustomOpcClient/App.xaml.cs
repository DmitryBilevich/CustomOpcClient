using System.Windows;
using CustomOpcClient.Domain;
using OpcDAClient;
using SimpleInjector;

namespace CustomOpcClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Container container = CreateContainer();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = container.GetInstance<MainWindow>();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            container.Dispose();
        }

        private static Container CreateContainer()
        {
            var container = new Container();
            container.Register<MainWindow>();
            container.Register<IMotorService, MotorService>(Lifestyle.Singleton);
            container.Verify();

            return container;
        }
    }
}
