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

        public async Task<int> RunExternalKeepCommand(string applicationPath, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = applicationPath,
                Arguments = arguments,
                UseShellExecute = true,
                CreateNoWindow = false
            };
            try
            {
                Process.Start(startInfo);
                return 0;
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<string> RunExternalCommandResult(string applicationPath, string arguments)
        {
            xsCsharplib.DebugBar(Debugtag, $"目标:{applicationPath}", xsCsharplib.警告橙色);

            // 创建一个新的进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = applicationPath,
                Arguments = arguments,
                UseShellExecute = false,  // 必须设置为 false 以便能够重定向输入输出
                RedirectStandardOutput = true,  // 重定向标准输出
                RedirectStandardError = true,    // 可选：也重定向标准错误流
                CreateNoWindow = true            // 不创建窗口
            };

            try
            {
                // 启动进程
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;

                    // 启动进程
                    process.Start();

                    // 读取输出流
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync(); // 读取标准错误流

                    // 等待进程结束
                    await process.WaitForExitAsync();
                    /*
                    if (process.ExitCode != 0)
                    {
                        // 处理非零退出代码的情况
                        throw new Exception($"Process exited with code {process.ExitCode}: {error}");
                    }
                    */
                    
                    return output; // 返回标准输出内容
                }
            }
            catch (Exception ex)
            {
                // 可以记录或处理异常
                throw; // 重新抛出异常以便调用者捕获
            }
        }




        private async void Net类型dll_Click(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = System.IO.Path.Combine(appDirectory, @"ildasm\ildasm.exe");
            string exearg = filepath;

            try
            {
                if (await RunExternalKeepCommand(executablePath, exearg) >= 0)
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = System.IO.Path.Combine(appDirectory, @"dumpbin\dumpbin.exe");
            string exearg = $" /exports \"{filepath}\"";

            try
            {
                string result = await RunExternalCommandResult(executablePath, exearg);
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
                if (await RunExternalKeepCommand(executablePath, exearg) >= 0)
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
                if (await RunExternalKeepCommand(executablePath, exearg) >= 0)
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