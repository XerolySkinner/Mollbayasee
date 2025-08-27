using Csharplib.basic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Umarejson
{
    public partial class MainWindow : Window
    {
        #region 增
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try {
                var selectedItem = JsonTreeView.SelectedItem as TreeViewItem;
                if (selectedItem == null) throw new InvalidOperationException($"请先选择一个节点");
                var selectedToken = selectedItem.Tag as JToken;
                if (selectedToken == null) return;

                string PathText = GetJsonPath(selectedToken);

                //  JObject
                if (selectedToken is JObject obj)
                {
                    Add_Obj_Click(obj);
                }
                //  JValue
                else if (selectedToken is JValue value)
                {
                    //  父JProperty
                    if (value.Parent is JProperty parentProperty)
                    {
                        JObject? vobj = parentProperty.Parent as JObject;
                        if (vobj == null)
                        {
                            throw new xsCsharplib.DebugbarException("错误对象", xsCsharplib.错误红色);
                        }
                        Add_Obj_Click(vobj);
                    }
                    //  父JArray
                    else if (value.Parent is JArray parentArray)
                    {
                        Add_Array_Click(parentArray);
                    }
                }
                //  JArray
                else if (selectedToken is JArray array)
                {
                    Add_Array_Click(array);
                }

                RefreshTreeView();
                ExpandTreeToPath(JsonTreeView.Items, PathText);
            }
            catch (xsCsharplib.DebugbarException ex)
            {
                xsCsharplib.DebugBar(Debugtag, ex.Message,ex.Brush);
            }
            catch (Exception ex) {
                if (ex.Message.Contains("with the same name already exists"))
                {
                    xsCsharplib.DebugBar(Debugtag, $"添加失败: 已存在重复键名", xsCsharplib.错误红色);
                }
                else {
                    xsCsharplib.DebugBar(Debugtag, $"添加失败: {ex.Message}", xsCsharplib.错误红色);
                }
            }
            
            return;
        }
        private void Add_Obj_Click(JObject obj) {
            if (添加类型.SelectedValue.ToString() == "数据")
            {
                string key = 添加键名.Text;
                object val = (添加值字符串.IsChecked == true) ? 添加值名.Text : AutoParse(添加值名.Text);
                if (string.IsNullOrWhiteSpace(key))
                    throw new xsCsharplib.DebugbarException("键名为空", xsCsharplib.警告橙色);
                if (val == null)
                    throw new xsCsharplib.DebugbarException("值为空", xsCsharplib.警告橙色);

                obj.Add(new JProperty(key, val));
                xsCsharplib.DebugBar(Debugtag, $"在{PathTextBox.Text}处添加 {key}:{val}", xsCsharplib.正常绿色);
            }
            else if (添加类型.SelectedValue.ToString() == "对象")
            {
                string key = 添加键名.Text;
                if (string.IsNullOrWhiteSpace(key))
                    throw new xsCsharplib.DebugbarException("键名为空", xsCsharplib.警告橙色);
                obj.Add(new JProperty(key, new JObject()));
                xsCsharplib.DebugBar(Debugtag, $"在{PathTextBox.Text}处添加对象{key}", xsCsharplib.正常绿色);
            }
            else if (添加类型.SelectedValue.ToString() == "数组")
            {
                string key = 添加键名.Text;
                if (string.IsNullOrWhiteSpace(key))
                    throw new xsCsharplib.DebugbarException("键名为空", xsCsharplib.警告橙色);
                obj.Add(new JProperty(key, new JArray()));
                xsCsharplib.DebugBar(Debugtag, $"在{PathTextBox.Text}处添加数组{key}", xsCsharplib.正常绿色);
            }
        }
        private void Add_Array_Click(JArray array)
        {
            if (添加类型.SelectedValue.ToString() == "数据")
            {
                object val = (添加值字符串.IsChecked == true) ? 添加值名.Text : AutoParse(添加值名.Text);
                if (val == null)
                    throw new xsCsharplib.DebugbarException("值为空", xsCsharplib.警告橙色);
                array.Insert(int.Parse(添加序号.Text), new JValue(val));
                xsCsharplib.DebugBar(Debugtag, $"在{PathTextBox.Text}[{添加序号.Text}]添加{val}", xsCsharplib.正常绿色);
            }
            else if (添加类型.SelectedValue.ToString() == "对象")
            {
                array.Insert(int.Parse(添加序号.Text), new JObject());
                xsCsharplib.DebugBar(Debugtag, $"在{PathTextBox.Text}[{添加序号.Text}]添加一个对象", xsCsharplib.正常绿色);
            }
            else if (添加类型.SelectedValue.ToString() == "数组")
            {
                array.Insert(int.Parse(添加序号.Text), new JArray());
                xsCsharplib.DebugBar(Debugtag, $"在{PathTextBox.Text}[{添加序号.Text}]添加一个数组", xsCsharplib.正常绿色);
            }
        }
        #endregion

        #region 删
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = JsonTreeView.SelectedItem as TreeViewItem;
                if (selectedItem == null) throw new InvalidOperationException($"请先选择一个节点");
                var selectedToken = selectedItem.Tag as JToken;
                if (selectedToken == null) return;

                string PathText = GetJsonPath(selectedToken);

                try
                {
                    selectedToken.Remove();
                }
                catch
                {
                    try
                    {
                        selectedToken.Parent.Remove();
                    }
                    catch (Exception ex)
                    {
                        xsCsharplib.DebugBar(Debugtag, $"删除失败: {ex.Message}", xsCsharplib.错误红色);
                    }
                }
                RefreshTreeView();
            }
            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"删除失败: {ex.Message}", xsCsharplib.错误红色);
            }
        }
        #endregion

        #region 改
        //  改
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (JsonTreeView.SelectedItem is not TreeViewItem selectedItem)
                    throw new InvalidOperationException($"先选中才能编辑");
                var selectedToken = selectedItem.Tag as JToken;
                if (selectedToken == null) return;
                string PathText = GetJsonPath(selectedToken);

                //  JObject
                if (selectedToken is JObject obj)
                {
                    if (obj.Parent is JProperty parentProperty)
                    {
                        parentProperty.Replace(new JProperty(键名.Text.ToString(), parentProperty.Value));
                        PathText = (string.Join(".", PathText.Split('.').Take(PathText.Split('.').Length - 1)) + "." + 键名.Text.ToString()).TrimStart('.');
                    }
                }

                //  JValue
                else if (selectedToken is JValue value)
                {
                    if (value.Parent is JProperty parentProperty)
                    {
                        object val = (值字符串.IsChecked == true) ? 值名.Text : AutoParse(值名.Text);
                        parentProperty.Replace(new JProperty(键名.Text.ToString(), val));
                        PathText = (string.Join(".", PathText.Split('.').Take(PathText.Split('.').Length - 1)) + "." + 键名.Text.ToString()).TrimStart('.');
                    }

                    else if (value.Parent is JArray array)
                    {
                        object val = (值字符串.IsChecked==true) ? 值名.Text : AutoParse(值名.Text);
                        value.Replace(new JValue(val));

                    }
                }

                //  JArray
                else if (selectedToken is JArray array)
                {
                    if (array.Parent is JProperty parentProperty)
                    {
                        parentProperty.Replace(new JProperty(键名.Text.ToString(), parentProperty.Value));
                        PathText = (string.Join(".", PathText.Split('.').Take(PathText.Split('.').Length - 1)) + "." + 键名.Text.ToString()).TrimStart('.');
                    }
                }

                RefreshTreeView();
                ExpandTreeToPath(JsonTreeView.Items, PathText);
            }

            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"编辑失败: {ex.Message}", xsCsharplib.错误红色);
            }
        }
        #endregion

        #region 查
        //  查
        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = PathTextBox.Text.Trim();
                if (string.IsNullOrEmpty(path)) return;

                var token = _currentJsonData.SelectToken(path);
                if (token == null)
                {
                    xsCsharplib.DebugBar(Debugtag, $"未找到指定路径的节点", xsCsharplib.警告橙色);
                    return;
                }

                // 展开树视图到指定节点
                ExpandTreeToPath(JsonTreeView.Items, path);
            }
            catch (Exception ex)
            {
                xsCsharplib.DebugBar(Debugtag, $"导航错误: {ex.Message}", xsCsharplib.错误红色);
            }
        }
        #endregion

    }
}
