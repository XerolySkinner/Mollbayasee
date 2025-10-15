using NPOI.SS.Formula.Functions;
using SharpCompress.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static Ramitta.lib.Basic;
using static Ramitta.winDataGrid;

namespace Suwatou
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string filepath = "";
        static string directoryPath = "";
        static string fileNameWithoutExtension = "";


        public MainWindow(StartupEventArgs e)
        {
            Startupe = ParseCommandLineArgs(e.Args);

            InitializeComponent();

            try
            {
                filepath = e.Args[0];
                DebugBar(Debugtag, $"文件:{filepath}", 经典紫色);

                // 检查文件是否存在
                if (!File.Exists(filepath))
                {
                    MessageBox.Show($"文件不存在: {filepath}", "错误");
                    Close();
                    return;
                }

                // 还可以检查路径是否有效
                if (string.IsNullOrWhiteSpace(filepath))
                {
                    MessageBox.Show("文件路径为空", "错误");
                    Close();
                    return;
                }

                directoryPath = System.IO.Path.GetDirectoryName(filepath);
                fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filepath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("本程序仅支持调用使用,附带被操作文件路径!", "错误");
                Close();
            }
        }

        public static string AddSuffixToFilename(string suffix)
        {
            if (string.IsNullOrEmpty(filepath))
                return filepath;

            try
            {
                string directory = System.IO.Path.GetDirectoryName(filepath);
                string filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filepath);
                string extension = System.IO.Path.GetExtension(filepath);

                // 组合新的文件名
                string newFilename = $"{filenameWithoutExtension}{suffix}{extension}";

                // 返回完整路径
                return System.IO.Path.Combine(directory ?? "", newFilename);
            }
            catch
            {
                // 如果路径处理出错，返回原路径
                return filepath;
            }
        }

        private async void 转换成ico_Click(object sender, RoutedEventArgs e)
        {
            string selectedSize = icoSizeComboBox.Text;
            string outputPath = $"{directoryPath}\\{fileNameWithoutExtension}_{selectedSize.Replace(":", "x")}.ico";

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = System.IO.Path.Combine(appDirectory, @"ffmpeg.exe");
            string exearg = $" -i {filepath} -vf scale={selectedSize} {outputPath}";

            // 在这里添加转换逻辑
            // 可以验证输入格式是否为 "数字x数字"
            if (System.Text.RegularExpressions.Regex.IsMatch(selectedSize, @"^\d+:\d+$"))
            {
                try
                {
                    await RunExternalCommand(executablePath, exearg, true);
                    DebugBar(Debugtag, $"输出到:{directoryPath}\\{fileNameWithoutExtension}.ico", 正常绿色);

                }
                catch (Exception ex)
                {
                    DebugBar(Debugtag, $"错误:{ex.Message}", 错误红色);
                    MessageBox.Show($"错误:{ex.Message}", "错误");
                }

            }
            else
            {
                DebugBar(Debugtag, $"请输入有效的尺寸格式（如：16:16）", 错误红色);

            }
            return;
        }
        private async void 尺寸缩变_Click(object sender, RoutedEventArgs e)
        {
            string selectedSize = outSizeComboBox.Text;
            string outputPath = AddSuffixToFilename("_"+selectedSize.Replace(":", "x"));

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = System.IO.Path.Combine(appDirectory, @"ffmpeg.exe");
            string exearg = $"-i {filepath} -vf \"scale={selectedSize}:flags=lanczos\" {outputPath}";

            // 在这里添加转换逻辑
            // 可以验证输入格式是否为 "数字x数字"
            if (System.Text.RegularExpressions.Regex.IsMatch(selectedSize, @"^\d+:\d+$"))
            {
                try
                {
                    await RunExternalCommand(executablePath, exearg, true);
                    DebugBar(Debugtag, $"输出到:{outputPath}", 正常绿色);

                }
                catch (Exception ex)
                {
                    DebugBar(Debugtag, $"错误:{ex.Message}", 错误红色);
                    MessageBox.Show($"错误:{ex.Message}", "错误");
                }

            }
            else
            {
                DebugBar(Debugtag, $"请输入有效的尺寸格式（如：16:16）", 错误红色);

            }
            return;
        }

    }
}