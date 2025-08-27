using System.Configuration;
using System.Data;
using System.Windows;

namespace Umarejson
{
    public partial class App : Application
    {
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = new MainWindow(e);
            mainWindow.Show();
        }
        
    }

}
