using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Office.Work.Platform.AppCodes
{
    public static class CloseScreen
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const uint WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xf170;
        public static void Close()
        {
            //放入新线程运用很重要。否则会造成任务栏中的托盘不响应。
            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1000);
                SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
            });
        }
    }
}
