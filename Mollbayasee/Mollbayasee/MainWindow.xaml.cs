using Csharplib.basic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mollbayasee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string? filepath;
        public MainWindow(StartupEventArgs e)
        {
            xsCsharplib.Startupe = xsCsharplib.ParseCommandLineArgs(e.Args);
            InitializeComponent();

            try
            {
                xsCsharplib.DebugBar(Debugtag,$"目标:{e.Args[0]}", xsCsharplib.经典紫色);
                filepath = e.Args[0];
                xsCsharplib.DebugBar(Debugtag, $"后缀:{System.IO.Path.GetExtension(filepath)}", xsCsharplib.经典紫色);
                if (System.IO.Path.GetExtension(filepath) == ".json") {
                    json文件_Click(null, null);
                    Close();
                }
                if (System.IO.Path.GetExtension(filepath) == ".ini")
                {
                    ini文件_Click(null, null);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("本程序仅支持调用使用,附带被操作文件路径!","错误");
                Close();
            }
        }

        private async void Net类型dll_Click(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = System.IO.Path.Combine(appDirectory, @"ildasm\ildasm.exe");
            string exearg = filepath;

            try
            {
                await xsCsharplib.RunExternalCommand(executablePath, exearg, true);
            }
            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"错误:{ex.Message}", xsCsharplib.错误红色);
                MessageBox.Show($"错误:{ex.Message}","错误");
            }
            Close();
        }

        private async void 常规dll_Click(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = System.IO.Path.Combine(appDirectory, @"dumpbin\dumpbin.exe");
            string exearg = $" /exports \"{filepath}\"";

            try
            {
                string result = await xsCsharplib.RunExternalCommandResult(executablePath, exearg);
                winDumpbin form = new winDumpbin(result);
                form.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}","Error");
            }
        }
        

        private async void json文件_Click(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = System.IO.Path.Combine(appDirectory, @"Umarejson\Umarejson.exe");
            string exearg = $"-getfile \"{filepath}\"";

            try
            {
                if (await xsCsharplib.RunExternalCommand(executablePath, exearg,true) >= 0)
                {
                    xsCsharplib.DebugBar(Debugtag, $"成功", xsCsharplib.正常绿色);
                }
            }
            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"错误:{ex.Message}", xsCsharplib.错误红色);
            }
            Close();
        }

        private async void ini文件_Click(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = System.IO.Path.Combine(appDirectory, @"Inorini\Inorini.exe");
            string exearg = $"-getfile \"{filepath}\"";

            try
            {
                if (await xsCsharplib.RunExternalCommand(executablePath, exearg,true) >= 0)
                {
                    xsCsharplib.DebugBar(Debugtag, $"成功", xsCsharplib.正常绿色);
                }
            }
            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"错误:{ex.Message}", xsCsharplib.错误红色);
            }
            Close();
        }
    }
}