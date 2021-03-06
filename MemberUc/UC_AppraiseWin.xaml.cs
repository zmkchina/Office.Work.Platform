﻿using Office.Work.Platform.AppCodes;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Office.Work.Platform.MemberUc
{
    /// <summary>
    /// 此类用于新增或编辑月度待遇发放记录。
    /// 此窗体作为对话框使用，当DialogResult不被设为true时，将始终为false
    /// </summary>
    public partial class UC_AppraiseWin : Window
    {
        public ReadOnlyCollection<string> _AppraiseTypeArr { get; set; }
        public Lib.MemberAppraise _CurRecord { get; set; }
        public UC_AppraiseWin(Lib.MemberAppraise ParamRecord)
        {
            InitializeComponent();
            this.Owner = AppSet.AppMainWindow;
            _AppraiseTypeArr = AppSet.ServerSetting.AppraiseTypeArr;
            _CurRecord = ParamRecord;
            DataContext = this;
        }
        private void BtnSaveClickAsync(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _CurRecord.UpDateTime = DateTime.Now;
            this.Close();
        }

        private void BtnCancelClickAsync(object sender, RoutedEventArgs e)
        {
            _CurRecord = null;
            this.Close();
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
