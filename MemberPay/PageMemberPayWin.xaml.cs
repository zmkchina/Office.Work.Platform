﻿using System;
using System.Collections.Generic;
using System.Windows;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.MemberPay
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class PageMemberPayWin : Window
    {
        public Lib.MemberSalary CurMemberSalary { get; set; }
        public List<MemberPayItem> MemberPayItems { get; set; }
        public Lib.MemberPayItem SelectPayItem { get; set; }
        public DateTime SelectPayDate { get; set; }

        public PageMemberPayWin()
        {
            this.Owner = AppSet.AppMainWindow;
            InitializeComponent();
        }
        private void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            MemberPayItems.Sort((x, y) => x.OrderIndex - y.OrderIndex);
            DataContext = this;
            ComboBox_PayItems.SelectedIndex = 0;
        }
        /// <summary>
        /// 保存发放信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            CurMemberSalary.PayUnitName = AppSet.LoginUser.UnitName;
            CurMemberSalary.PayYear = SelectPayDate.Year;
            CurMemberSalary.PayMonth = SelectPayDate.Month;
            //CurMemberPay.AddOrCut = SelectPayItem.AddOrCut;
            CurMemberSalary.TableType = SelectPayItem.InTableType;
            DialogResult = true;
            this.Close();
        }
        /// <summary>
        /// 取消发放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {

            CurMemberSalary = null;
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// 发放项目选择发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_PayItems.SelectedItem != null)
            {
                SelectPayItem = ComboBox_PayItems.SelectedItem as Lib.MemberPayItem;
                TextBlock_ItemInfo.DataContext = SelectPayItem;
            }
        }

        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => this.DragMove()));
            }
        }
    }
}
