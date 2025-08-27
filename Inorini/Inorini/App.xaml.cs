using System.Configuration;
using System.Data;
using System.Windows;

namespace Inorini
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = new MainWindow(e);
            try
            {
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                Current.Shutdown(1);
            }
        }
    }

}
