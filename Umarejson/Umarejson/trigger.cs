using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Umarejson
{
    public partial class MainWindow : Window
    {
        //  条目变化
        private void JsonTreeView_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(null, null);
            JsonTreeView_SelectedItemChanged(null, args);
        }

        private void JsonTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (添加功能框 != null) 添加功能框.Visibility = Visibility.Hidden;
            if (更删功能框 != null) 更删功能框.Visibility = Visibility.Hidden;

            if (JsonTreeView.SelectedItem is not TreeViewItem selectedItem)return;
            var selectedToken = selectedItem.Tag as JToken;
            if (selectedToken == null) {
                选中类型.Text = "Other";
                return;
            }
            PathTextBox.Text = GetJsonPath(selectedToken);


            string parentType = "Other";

            值改条.Visibility = Visibility.Collapsed;
            值类型改条.Visibility = Visibility.Collapsed;
            序号条.Visibility = Visibility.Collapsed;
            键改条.Visibility = Visibility.Collapsed;

            添加值改条.Visibility = Visibility.Collapsed;
            添加序号条.Visibility = Visibility.Collapsed;
            添加键改条.Visibility = Visibility.Collapsed;

            //  JObject
            if (selectedToken is JObject obj)
            {
                parentType = "JObject";
                添加功能框.Visibility = Visibility.Visible;
                更删功能框.Visibility = Visibility.Visible;

                //  父JProperty
                if (obj.Parent is JProperty parentProperty)
                {
                    键名.Text = parentProperty.Name;
                    键改条.Visibility = Visibility.Visible;
                }
                //  父Array
                else if (obj.Parent is JArray parentArray)
                {
                    序号.Text = parentArray.IndexOf(obj).ToString();
                    序号条.Visibility = Visibility.Visible;
                }
                //  仅本征
                {
                    添加键改条.Visibility = Visibility.Visible;
                    if (添加类型.SelectedValue.ToString() == "数据") 添加值改条.Visibility = Visibility.Visible;
                }

                if (loadMode == "大族模式") {
                    更删功能框.Visibility = Visibility.Collapsed;
                }
            }
            //  JValue
            else if (selectedToken is JValue value)
            {
                parentType = "JValue";
                添加功能框.Visibility = Visibility.Visible;
                更删功能框.Visibility = Visibility.Visible;

                值名.Text = value.ToString();
                值类型.Content = GetTypeName(value);

                值改条.Visibility = Visibility.Visible;
                值类型改条.Visibility = Visibility.Visible;
                //  父JProperty
                if (value.Parent is JProperty parentProperty)
                {
                    键改条.Visibility = Visibility.Visible;
                    键名.Text = parentProperty.Name;

                    添加键改条.Visibility = Visibility.Visible;
                    if (添加类型.SelectedValue.ToString() == "数据") 添加值改条.Visibility = Visibility.Visible;

                }
                //  父Array
                else if (value.Parent is JArray parentArray)
                {
                    序号条.Visibility = Visibility.Visible;
                    序号.Text = parentArray.IndexOf(value).ToString();

                    添加序号.Text = (parentArray.Count).ToString();
                    添加序号条.Visibility = Visibility.Visible;
                    if (添加类型.SelectedValue.ToString() == "数据") 添加值改条.Visibility = Visibility.Visible;
                }


            }
            //  JArray
            else if (selectedToken is JArray array)
            {
                parentType = "JArray";
                添加功能框.Visibility = Visibility.Visible;
                更删功能框.Visibility = Visibility.Visible;

                //  父JProperty
                if (array.Parent is JProperty parentProperty)
                {
                    键改条.Visibility = Visibility.Visible;
                    键名.Text = parentProperty.Name;
                }
                //  父Array
                else if (array.Parent is JArray parentArray)
                {
                    序号条.Visibility = Visibility.Visible;
                    序号.Text = parentArray.IndexOf(array).ToString();
                }
                //  仅本征
                {
                    添加序号.Text = (array.Count).ToString();
                    添加序号条.Visibility = Visibility.Visible;
                    if (添加类型.SelectedValue.ToString() == "数据") 添加值改条.Visibility = Visibility.Visible;
                }

                if (loadMode == "大族模式")
                {
                    更删功能框.Visibility = Visibility.Collapsed;
                }
            }
            //  其他
            else
            {
                parentType = "Other";
            }

            选中类型.Text = parentType;

            if (selectedToken == _currentJsonData) {
                更删功能框.Visibility = Visibility.Hidden;
            }
        }

        // 键盘触发
        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 获取 TextBox 的 Tag 属性，这里是方法名
                string methodName = (string)((TextBox)sender).Tag;

                // 根据方法名调用相应的函数
                if (methodName == "EditButton_Click")
                {
                    // 你希望调用的函数
                    EditButton_Click(sender, e);
                }
                if (methodName == "AddButton_Click")
                {
                    // 你希望调用的函数
                    AddButton_Click(sender, e);
                }
            }
        }
        private void Del_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteButton_Click(null, null);
            }
        }

    }
}
