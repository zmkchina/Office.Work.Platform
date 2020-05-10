using System.Collections.Generic;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppCodes
{
    public static class AppFuns
    {
        /// <summary>
        /// 设置主窗口底部状态栏信息
        /// </summary>
        /// <param name="Msg"></param>
        public static void SetStateBarText(string Msg)
        {
            AppSet.AppMainWindow.lblCursorPosition.Text = Msg;
        }
        /// <summary>
        /// 设置显示各类操作信息反馈窗口信息
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Caption"></param>
        /// <param name="isErr">是否是错误信息</param>
        /// <param name="showYesNo">是否显示Yes和No按钮</param>
        /// <param name="ShowOk"></param>
        /// <returns></returns>
        public static bool ShowMessage(string Message, string Caption = "信息", bool isErr = false, bool showYesNo = false, bool ShowOk = true)
        {
            return (new WinMsgDialog(Message, Caption, isErr, showYesNo, ShowOk)).ShowDialog().Value;
        }

        public static void ShowBalloonTip(string Message, string Caption = "消息", System.Windows.Forms.ToolTipIcon toolTipIcon = System.Windows.Forms.ToolTipIcon.Info, int TimeOut = 5)
        {
            AppSet.AppMainWindow.notifyIcon.ShowBalloonTip(TimeOut, Caption, Message, toolTipIcon);
        }
    }
}
