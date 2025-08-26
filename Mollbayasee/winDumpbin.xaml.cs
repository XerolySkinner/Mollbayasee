using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace Mollbayasee
{
    /// <summary>
    /// winDumpbin.xaml 的交互逻辑
    /// </summary>
    public partial class winDumpbin : Window
    {
        public winDumpbin(string msg)
        {
            InitializeComponent();
            msg_result.Text = msg;
            InitializeDataGrid(msg); // 添加对初始化方法的调用

        }


        #region 表格数据结构
        public class DataItem : INotifyPropertyChanged
        {
            private int _index;
            private string _func = "";

            public int Index
            {
                get => _index;
                set
                {
                    if (_index != value)
                    {
                        _index = value;
                        OnPropertyChanged(nameof(Index)); // 使用 Index 而不是 _index
                    }
                }
            }

            public string Func
            {
                get => _func;
                set
                {
                    if (_func != value)
                    {
                        _func = value;
                        OnPropertyChanged(nameof(Func)); // 使用 Func 而不是 _func
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<DataItem> dataList; // 移动到类的成员范围内

        private void InitializeDataGrid()
        {
            dataList = new ObservableCollection<DataItem>();

            // 添加初始数据项
            dataList.Add(new DataItem
            {
                Index = 0,
                Func = "初始功能"  // 添加功能字段的初始值
            });

            // 设置 DataGrid 的 ItemsSource
            func_data.ItemsSource = dataList;
        }

        private void InitializeDataGrid(string msg)
        {
            dataList = new ObservableCollection<DataItem>();


            var exportNames = new List<string>();

            // 使用 regex 匹配导出名称信息
            Regex regex = new Regex(@"^\s*(\d+)\s+\d+\s+([A-F0-9]+)\s+([A-Za-z0-9_]+)", RegexOptions.Multiline);
            MatchCollection matches = regex.Matches(msg);

            foreach (Match match in matches)
            {
                // 提取序号和名称
                string ordinal = match.Groups[1].Value; // 序号
                string name = match.Groups[3].Value;    // 名称
                                                        // 添加初始数据项
                dataList.Add(new DataItem
                {
                    Index = int.Parse(ordinal),
                    Func = name  // 添加功能字段的初始值
                });

                
            }
            // 设置 DataGrid 的 ItemsSource
            func_data.ItemsSource = dataList;
        }
        #endregion
    }
}
