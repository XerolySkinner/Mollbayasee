using Ramitta;
using static Ramitta.lib.Basic;

using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Inorini
{
    public partial class MainWindow : Window
    {
        // 插入Item的方法
        public void AddItem(string item)
        {
            HeadListBox.Items.Add(item);
        }

        // 插入自定义对象的方法（示例）
        public void AddItem(object item)
        {
            HeadListBox.Items.Add(item);
        }

        // 读取当前选中项
        public object GetSelectedItem()
        {
            return HeadListBox.SelectedItem;
        }

        // 获取选中项的索引
        public int GetSelectedIndex()
        {
            return HeadListBox.SelectedIndex;
        }

        // 选择变化事件处理
        private void HeadListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 这里处理选择变化后的逻辑
            object selectedItem = HeadListBox.SelectedItem;
            int selectedIndex = HeadListBox.SelectedIndex;

            // 调用你的处理函数
            OnSelectionChanged(selectedItem, selectedIndex);
        }


        // 自定义的选择变化处理函数（供你实现）
        private void OnSelectionChanged(object selectedItem, int selectedIndex)
        {
            // 先清空现有数据
            flashItemChange(selectedItem.ToString());
        }

        // 清空所有项
        public void ClearItems()
        {
            HeadListBox.Items.Clear();
        }

        // 移除指定项
        public void RemoveItem(object item)
        {
            HeadListBox.Items.Remove(item);
        }

        // 移除指定索引的项
        public void RemoveItemAt(int index)
        {
            HeadListBox.Items.RemoveAt(index);
        }
    }
}
