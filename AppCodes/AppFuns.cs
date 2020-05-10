using System.Collections.Generic;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.AppCodes
{
    public static class AppFuns
    {
        public static void SetStateBarText(string Msg)
        {
            AppSet.AppMainWindow.lblCursorPosition.Text = Msg;
        }
        public static bool ShowMessage(string Message, string Caption = "信息", bool isErr = false, bool showYesNo = false, bool ShowOk = true)
        {
            return (new WinMsgDialog(Message, Caption, isErr, showYesNo, ShowOk)).ShowDialog().Value;
        }
    }
}
