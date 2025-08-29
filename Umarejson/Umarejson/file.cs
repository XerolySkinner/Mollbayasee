using Ramitta;
using static Ramitta.lib.Basic;

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
                DebugBar(Debugtag, $"保存成功", 正常绿色);
                if (loadMode == "大族模式") { 
                Close();
                }
            }
            catch (Exception ex)
            {
                DebugBar(Debugtag, $"保存失败: {ex.Message}", 错误红色);
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
                DebugBar(Debugtag, $"保存成功", 正常绿色);
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
            JsonTreeView.Items.Clear();
            JsonTextBox.Clear();
            try
            {
                _currentFilePath = filePath; // 记录当前文件路径
                string jsonText = File.ReadAllText(filePath);

                _currentJsonData = JToken.Parse(jsonText);


                // 检查当前解析的数据是否有效
                if (_currentJsonData != null &&(
                    _currentJsonData.Type == JTokenType.Object ||
                    _currentJsonData.Type == JTokenType.Array)) // 简单的检查，可以根据需求调整
                {
                    RefreshTreeView();

                    DebugBar(Debugtag, $"加载文件: {filePath}", 正常绿色);
                }
                else
                {
                    DebugBar(Debugtag, $"解析结果为空或无效: {filePath}", 错误红色);
                }


            }
            catch (JsonReaderException jrex)
            {
                DebugBar(Debugtag, $"JSON 解析失败: {jrex.Message}", 错误红色);
            }
            catch (Exception ex)
            {
                DebugBar(Debugtag, $"加载文件失败: {ex.Message}", 错误红色);
            }
        }

    }
}
