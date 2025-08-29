using Ramitta;
using static Ramitta.lib.Basic;

using IniParser;
using IniParser.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Inorini
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataItem NowSelectedItem;
        string filepath;
        IniData data;

        public MainWindow(StartupEventArgs e)
        {
            Startupe = ParseCommandLineArgs(e.Args);
            InitializeComponent();
            InitializeDataGrid();

            if (Startupe.TryGetValue("getfile", out string filePath))
            {
                this.filepath = filePath;
                // 创建解析器实例
                FileIniDataParser parser = new FileIniDataParser();
                parser.Parser.Configuration.AllowDuplicateKeys = true;  // 允许重复键
                parser.Parser.Configuration.OverrideDuplicateKeys = true;  // 后面的值覆盖前面的
                data = parser.ReadFile(filePath);

                // 获取INI文件中所有的段名
                foreach (SectionData section in data.Sections)
                {
                    AddItem(section.SectionName);
                }

                //                LoadIniFromFile(filePath);
                DebugBar(Debugtag, $"加载文件: {filePath}", 正常绿色);
            }
        }

        // 保存 INI 数据到文件
        private void SaveIniDataToFile(string path)
        {
            try
            {
                if (data != null && !string.IsNullOrEmpty(path))
                {
                    var parser = new FileIniDataParser();
                    parser.WriteFile(path, data);
                    DebugBar(Debugtag, "INI 文件已保存", 正常绿色);
                }
            }
            catch (Exception ex)
            {
                DebugBar(Debugtag, $"保存失败: {ex.Message}", 错误红色);
            }
        }


        #region 表格数据结构
        public class DataItem : INotifyPropertyChanged
        {
            private string _key;
            private string _value = "";

            public string Key
            {
                get => _key;
                set
                {
                    if (_key != value)
                    {
                        _key = value;
                        OnPropertyChanged(nameof(Key));
                    }
                }
            }

            public string Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        OnPropertyChanged(nameof(Value));
                        OnValueChanged?.Invoke(this); // 触发值改变事件
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public event Action<DataItem> OnValueChanged; // 值改变事件


            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<DataItem> dataForm; // 移动到类的成员范围内

        private void InitializeDataGrid()
        {
            dataForm = new ObservableCollection<DataItem>();

            // 设置 DataGrid 的 ItemsSource
            main_data.ItemsSource = dataForm;

        }

        private void flashItemChange(string head) {
            dataForm.Clear();

            // 遍历指定段名（head）下的所有键值对
            if (data.Sections.ContainsSection(head))
            {
                // 使用字典方式遍历
                foreach (var keyValue in data[head])
                {
                    dataForm.Add(new DataItem
                    {
                        Key = keyValue.KeyName,
                        Value = keyValue.Value
                    });
                }

                // 添加数据
                foreach (var item in dataForm)
                {
                    var i = 0;
                    item.OnValueChanged += OnDataItemValueChanged;
                }
            }
        }

        private void OnDataItemValueChanged(DataItem changedItem)
        {
            data[GetSelectedItem().ToString()][changedItem.Key] = changedItem.Value;
            DebugBar(Debugtag, $"{GetSelectedItem().ToString()} >= {changedItem.Key}:{changedItem.Value}", 警告橙色);
        }

        #endregion


        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            DebugBar(Debugtag, $"删除键: {GetSelectedItem().ToString()} => {NowSelectedItem.Key}", 警告橙色);
            data[GetSelectedItem().ToString()].RemoveKey(NowSelectedItem.Key);
            flashItemChange(GetSelectedItem().ToString());

        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try {

                if (!data[GetSelectedItem().ToString()].AddKey(增加键.Text)) 
                    throw new NotImplementedException("存在重复的键");

                data[GetSelectedItem().ToString()][增加键.Text] = 增加值.Text;
                flashItemChange(GetSelectedItem().ToString());
            }
            catch (Exception ex) {
                DebugBar(Debugtag, $"错误: {ex.Message}", 错误红色);
            }

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Content.ToString() == "编辑")
                {
                    数据增加功能区.Visibility = Visibility.Visible;
                    删除.IsEnabled = true;
                    增加.IsEnabled = true;
                    // 切换到编辑模式
                    main_data.IsReadOnly = false;
                    button.Content = "完成";

                    // 可选：更改按钮样式或提示
                    button.ToolTip = "完成编辑";
                    DebugBar(Debugtag, "进入编辑模式", 警告橙色);
                }
                else
                {
                    数据增加功能区.Visibility = Visibility.Collapsed;
                    删除.IsEnabled = false;
                    增加.IsEnabled = false;
                    // 切换回只读模式
                    main_data.IsReadOnly = true;
                    button.Content = "编辑";

                    // 可选：更改按钮样式或提示
                    button.ToolTip = "编辑键值";
                    DebugBar(Debugtag, "退出编辑模式", 正常绿色);

                    // 可选：保存修改
                    SaveIniDataToFile(filepath);
                }
            }
        }

        private void Main_data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (main_data.SelectedItem is DataItem selectedItem)
            {
                // 行选中触发
                this.NowSelectedItem = selectedItem;
            }
            else if (main_data.SelectedItems.Count == 0)
            {
                this.NowSelectedItem = null;

            }
        }


    }
}