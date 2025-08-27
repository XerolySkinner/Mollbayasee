using Csharplib.basic;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Umarejson
{
    public partial class MainWindow : Window
    {
        private string? AskForJsonFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "选择 JSON 文件"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadJsonFromFile(openFileDialog.FileName);
                xsCsharplib.DebugBar(Debugtag, $"加载文件: {openFileDialog.FileName}", xsCsharplib.经典紫色);
                return openFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 如果没有加载过文件，则使用"另存为"逻辑
                if (_currentFilePath == null)
                {
                    SaveAsJson();
                    return;
                }
                // 直接保存到原文件
                SaveToFile(_currentFilePath);
                xsCsharplib.DebugBar(Debugtag, $"保存成功", xsCsharplib.正常绿色);
                if (loadMode == "大族模式") { 
                Close();
                }
            }
            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"保存失败: {ex.Message}", xsCsharplib.错误红色);
            }
        }

        private void SaveAsJson()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "保存 JSON 文件"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToFile(saveFileDialog.FileName);
                _currentFilePath = saveFileDialog.FileName; // 记住当前文件路径
                xsCsharplib.DebugBar(Debugtag, $"保存成功", xsCsharplib.正常绿色);
            }
        }

        private void SaveToFile(string filePath)
        {
            // 确保 JSON 是有效的
            var text = JsonTextBox.Text;
            JToken.Parse(text); // 验证格式

            // 写入文件（带缩进格式化）
            File.WriteAllText(filePath, _currentJsonData.ToString(Formatting.Indented));
        }

        private void LoadJsonFromFile(string filePath)
        {
            try
            {
                _currentFilePath = filePath; // 记录当前文件路径
                string jsonText = File.ReadAllText(filePath);
                _currentJsonData = JToken.Parse(jsonText);
                RefreshTreeView();
            }
            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"加载文件失败: {ex.Message}", xsCsharplib.错误红色);
            }
        }

    }
}
