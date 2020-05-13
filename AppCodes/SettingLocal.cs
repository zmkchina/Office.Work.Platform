using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Office.Work.Platform.AppCodes
{
    /// <summary>
    /// 系统用户类
    /// </summary>
    public class SettingLocal : INotifyPropertyChanged
    {
       
        private string _LoginUserId;
        private int _RestInterval;
        private string _ColorMainWinTitle;
        private string _ColorMainWinTopMenu;
        private string _ColorMainWinState;
        private string _ColorMainWinLeftMenu;
        private string _ColorPageNavBar;
        private string _ResApiUrl;
        private string _IS4SeverUrl;

        /// <summary>
        /// 用户Id号
        /// </summary>
        public string LoginUserId
        {
            get { return _LoginUserId; }
            set { _LoginUserId = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 系统授权认证服务器地址
        /// </summary>
        public string IS4SeverUrl
        {
            get { return _IS4SeverUrl; }
            set { _IS4SeverUrl = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 资源API访问根地上
        /// </summary>
        public string ResApiUrl
        {
            get { return _ResApiUrl; }
            set { _ResApiUrl = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 该人员设定的休息时间间隔
        /// </summary>
        public int RestInterval
        {
            get { return _RestInterval; }
            set { _RestInterval = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 主窗口顶部标题栏背景色
        /// </summary>
        public string ColorMainWinTitle
        {
            get { return _ColorMainWinTitle; }
            set
            {
                _ColorMainWinTitle = value;
                Application.Current.Resources["ColorMainWinTitle"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 主窗口顶部菜单栏背景色
        /// </summary>
        public string ColorMainWinTopMenu
        {
            get { return _ColorMainWinTopMenu; }
            set
            {
                _ColorMainWinTopMenu = value;
                Application.Current.Resources["ColorMainWinTopMenu"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 主窗口底部状态（信息）栏背景色
        /// </summary>
        public string ColorMainWinState
        {
            get { return _ColorMainWinState; }
            set
            {
                _ColorMainWinState = value;
                Application.Current.Resources["ColorMainWinState"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 主窗口左则菜单栏背景色
        /// </summary>
        public string ColorMainWinLeftMenu
        {
            get { return _ColorMainWinLeftMenu; }
            set
            {
                _ColorMainWinLeftMenu = value;
                Application.Current.Resources["ColorMainWinLeftMenu"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 各子页面顶部导航信息栏背景色
        /// </summary>
        public string ColorPageNavBar
        {
            get { return _ColorPageNavBar; }
            set
            {
                _ColorPageNavBar = value;
                Application.Current.Resources["ColorPageNavBar"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SettingLocal()
        {
            IS4SeverUrl = "http://localhost:8989";
            ResApiUrl = "http://localhost:8989/Api/";
            RestInterval = 20;
            ResetColorTheme();
        }
        public void ResetColorTheme()
        {
            ColorMainWinTitle = "#EAEAEAEA";
            ColorMainWinTopMenu = "#EADFEECB";
            ColorMainWinState = "#EAF1EED8";
            ColorMainWinLeftMenu = "#FFEFEFEF";
            ColorPageNavBar = "#EAFFFCDA";
        }
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
