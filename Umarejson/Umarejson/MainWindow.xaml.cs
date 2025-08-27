using Csharplib.basic;
using Csharplib.Titlebar;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Printing.IndexedProperties;
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
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Umarejson
{
    public partial class MainWindow : Window
    {
        string loadMode = "正常模式";

        private JToken? _currentJsonData = null;
        // 在类顶部添加字段记录当前文件路径
        private string? _currentFilePath = null;


        public MainWindow(StartupEventArgs e)
        {
            xsCsharplib.Startupe = xsCsharplib.ParseCommandLineArgs(e.Args);
            InitializeComponent();

            
            if (xsCsharplib.Startupe.TryGetValue("getfile", out string filePath))
            {
                LoadJsonFromFile(filePath);
                loadMode = "指令模式";
            }

            if (xsCsharplib.Startupe.TryGetValue("mode", out string mode))
            {
                loadMode = mode;
            }
            else
            {
                loadMode = "正常模式";
            }

            if (loadMode == "大族模式")
            {
                // 通过遍历 ComboBox 的 items 找到并删除
                foreach (var item in 添加类型.Items.OfType<ComboBoxItem>().ToList())
                {
                    if (item.Content.ToString() == "对象" || 
                        item.Content.ToString() == "数组"){
                        添加类型.Items.Remove(item);
                    }
                }

                LoadButton.IsEnabled = false;
                //JsonTextBox.Visibility = Visibility.Collapsed;
                //textsplitter.Visibility = Visibility.Collapsed;

            }


            xsCsharplib.DebugBar(Debugtag, $"{loadMode} 加载文件: {filePath ?? "无"}", xsCsharplib.经典紫色);
        }

        string GetJsonPath(JToken selectedToken)
        {
            var pathParts = new List<string>();

            // 从选中的节点开始，向上遍历父节点
            while (selectedToken != null && selectedToken.Parent != null)
            {
                if (selectedToken.Parent is JProperty parentProperty)
                {
                    // JProperty 对象，取其 Name 来构建路径
                    pathParts.Insert(0, parentProperty.Name);
                }
                else if (selectedToken.Parent is JArray parentArray)
                {
                    // JArray 对象，根据索引构建路径
                    var index = parentArray.IndexOf(selectedToken);
                    selectedToken = selectedToken.Parent;
                    if (selectedToken.Parent is JProperty parentPropertyP) {
                        pathParts.Insert(0, $"{parentPropertyP.Name}[{index}]");
                    }

                }

                // 移动到父节点
                selectedToken = selectedToken.Parent;
            }

            // 拼接所有路径部分
            return string.Join(".", pathParts);
        }
        private void AddNodeToTree(JToken token, ItemCollection items)
        {
            if (token == null) return;

            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {

                    if (property.Value is JValue value)
                    {
                        var node = new TreeViewItem
                        {
                            Header = $"{property.Name}:{value.ToString()}",
                            Tag = value
                        };
                        items.Add(node);
                        continue;
                    }
                    else
                    {
                        var node = new TreeViewItem
                        {
                            Header = $"{property.Name} ({GetTypeName(property.Value)})",
                            Tag = property.Value
                        };
                        items.Add(node);
                        AddNodeToTree(property.Value, node.Items);
                    }

                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {

                    if (array[i] is JValue value)
                    {
                        var node = new TreeViewItem
                        {
                            Header = $"[{i}]:{value.ToString()}",
                            Tag = value
                        };
                        items.Add(node);
                        continue;
                    }
                    else
                    {
                        var node = new TreeViewItem
                        {
                            Header = $"[{i}] ({GetTypeName(array[i])})",
                            Tag = array[i]
                        };
                        items.Add(node);
                        AddNodeToTree(array[i], node.Items);
                    }
                }
            }
        }
        private void RefreshTreeView()
        {
            JsonTreeView.Items.Clear();
            AddNodeToTree(_currentJsonData, JsonTreeView.Items);
            JsonTextBox.Text = _currentJsonData.ToString(Formatting.Indented);
        }
        private string GetTypeName(JToken token)
        {
            if (token == null) return "null";
            if (token is JObject) return "Object";
            if (token is JArray) return "Array";

            if (token is JValue value)
            {
                if (value.Type == JTokenType.String) return "String";
                if (value.Type == JTokenType.Integer) return "Number";
                if (value.Type == JTokenType.Float) return "Float";
                if (value.Type == JTokenType.Boolean) return "Boolean";
                if (value.Type == JTokenType.Null) return "null";
            }
            return token.Type.ToString();
        }
        private bool ExpandTreeToPath(ItemCollection items, string path)
        {
            foreach (TreeViewItem item in items)
            {
                if (item.Tag is JToken token)
                {
                    var tokenPath = token.Path;
                    if (path.StartsWith(tokenPath))
                    {
                        item.IsExpanded = true;
                        if (path == tokenPath)
                        {
                            item.IsSelected = true;
                            item.BringIntoView();
                            return true;
                        }

                        if (ExpandTreeToPath(item.Items, path))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static object AutoParse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // 尝试解析为布尔值
            if (bool.TryParse(input, out bool boolValue))
                return boolValue;

            // 尝试解析为整数
            if (int.TryParse(input, out int intValue))
                return intValue;

            // 尝试解析为浮点数
            if (double.TryParse(input, out double doubleValue))
                return doubleValue;

            // 如果是 null 或空字符串的特殊表示
            if (input.Equals("null", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                return null;

            // 默认返回字符串
            return input;
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentJsonData = JToken.Parse(JsonTextBox.Text);
                RefreshTreeView();
                xsCsharplib.DebugBar(Debugtag, $"刷新成功", xsCsharplib.正常绿色);
            }
            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"刷新失败: {ex.Message}", xsCsharplib.错误红色);
            }
        }

        private void JsonTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (JsonTextBox.IsFocused)
            {
                try
                {

                    _currentJsonData = JToken.Parse(JsonTextBox.Text);

                    JsonTreeView.Items.Clear();
                    AddNodeToTree(_currentJsonData, JsonTreeView.Items);

                    xsCsharplib.DebugBar(Debugtag, $"刷新成功", xsCsharplib.正常绿色);
                }
                catch (Exception ex)
                {
                    xsCsharplib.DebugBar(Debugtag, $"刷新失败: {ex.Message}", xsCsharplib.错误红色);
                }
            }
        }



        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            AskForJsonFile();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(_currentFilePath))
            {
                try
                {
                    // 使用 ShellExecute 方法打开文件
                    System.Diagnostics.Process.Start("explorer.exe", _currentFilePath);
                    xsCsharplib.DebugBar(Debugtag, $"打开:{_currentFilePath}", xsCsharplib.正常绿色);
                }
                catch (Exception ex)
                {
                    xsCsharplib.DebugBar(Debugtag, "无法打开文件: " + ex.Message, xsCsharplib.错误红色);
                }
            }
            else
            {
                xsCsharplib.DebugBar(Debugtag, "目标不存在！", xsCsharplib.错误红色);
            }
        }



        // 设置初始的区域高度
        public double TopRowHeight { get; set; }
        public double BottomRowHeight { get; set; }

        // GridSplitter 拖动事件
        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            // 获取当前拖动的偏移量
            double newTopHeight = TopRowHeight + e.VerticalChange;
            double newBottomHeight = BottomRowHeight - e.VerticalChange;

            // 限制上下区域的最小高度，避免区域消失
            if (newTopHeight >= 50 && newBottomHeight >= 50)
            {
                TopRowHeight = newTopHeight;
                BottomRowHeight = newBottomHeight;
            }
        }
    }
}