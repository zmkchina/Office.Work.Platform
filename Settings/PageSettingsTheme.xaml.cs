﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Settings
{
    /// <summary>
    /// WinUpLoadFile.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettingsTheme : Page
    {
        private PageViewModel _PageViewModel = null;
        public PageSettingsTheme()
        {
            InitializeComponent();
        }
        private void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            _PageViewModel = new PageViewModel();
            this.DataContext = _PageViewModel;
        }

        /// <summary>
        /// 保存主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SaveTheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataRWLocalFileRepository.SaveObjToFile<SettingLocal>(_PageViewModel.LocalSettings, AppSet.LocalSettingFileName))
            {
                AppFuns.ShowMessage("颜色主题保存成功！");
            }
        }
        /// <summary>
        /// 恢复默认主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_RestoreTheme_Click(object sender, RoutedEventArgs e)
        {
            _PageViewModel.LocalSettings.ResetColorTheme();
        }


        /// <summary>
        /// 本页面的视图模型
        /// </summary>
        private class PageViewModel : NotificationObject
        {
            private bool _SetAlphaValue;
            private bool _SetRedValue;
            private bool _SetGreenValue;
            private bool _SetBlueValue;
            private byte _RedColorValue;
            private byte _GreenColorValue;
            private byte _BlueColorValue;
            private SolidColorBrush _RedColorBrush;
            private SolidColorBrush _GreenColorBrush;
            private SolidColorBrush _BlueColorBrush;
            private byte _AlphaValue = 255;
            private bool _SelectColor1;
            private bool _SelectColor2;
            private bool _SelectColor3;
            private bool _SelectColor4;
            private bool _SelectColor5;

            public string[] FileContentTypes => AppSet.ServerSetting.WorkContentType.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            public SettingServer EntitySettingServer { get; set; }

            public byte AlphaValue
            {
                get { return _AlphaValue; }
                set
                {
                    _AlphaValue = value;
                    if (!_SetAlphaValue) { SetMixColorBrush(); }
                    _SetAlphaValue = false;
                    RaisePropertyChanged();
                }
            }

            public byte RedColorValue
            {
                get { return _RedColorValue; }
                set
                {
                    _RedColorValue = value;
                    if (!_SetRedValue) { SetMixColorBrush(); }
                    _SetRedValue = false;
                    RedColorBrush.Color = Color.FromArgb(0xFF, value, 0, 0);
                    RaisePropertyChanged();
                }
            }
            public SolidColorBrush RedColorBrush
            {
                get { return _RedColorBrush; }
                set
                {
                    _RedColorBrush = value; RaisePropertyChanged();
                }
            }



            public byte GreenColorValue
            {
                get { return _GreenColorValue; }
                set
                {
                    _GreenColorValue = value;
                    if (!_SetGreenValue) { SetMixColorBrush(); }
                    _SetGreenValue = false;
                    GreenColorBrush.Color = Color.FromArgb(0xFF, 0, value, 0);
                    RaisePropertyChanged();
                }
            }
            public SolidColorBrush GreenColorBrush
            {
                get { return _GreenColorBrush; }
                set
                {
                    _GreenColorBrush = value; RaisePropertyChanged();
                }
            }

            public byte BlueColorValue
            {
                get { return _BlueColorValue; }
                set
                {
                    _BlueColorValue = value;
                    if (!_SetBlueValue) { SetMixColorBrush(); }
                    _SetBlueValue = false;
                    BlueColorBrush.Color = Color.FromArgb(0xFF, 0, 0, value);
                    RaisePropertyChanged();
                }
            }
            public SolidColorBrush BlueColorBrush
            {
                get { return _BlueColorBrush; }
                set
                {
                    _BlueColorBrush = value; RaisePropertyChanged();
                }
            }

            public bool SelectColor1
            {
                get { return _SelectColor1; }
                set
                {
                    _SelectColor1 = value;
                    if (value)
                    {
                        SetArgbValueTrue();
                        SetSoliderValue(LocalSettings.ColorMainWinTitle);
                    }
                }
            }
            public bool SelectColor2
            {
                get { return _SelectColor2; }
                set
                {
                    _SelectColor2 = value;
                    if (value)
                    {
                        SetArgbValueTrue();
                        SetSoliderValue(LocalSettings.ColorMainWinTopMenu);
                    }
                }
            }
            public bool SelectColor3
            {
                get { return _SelectColor3; }
                set
                {
                    _SelectColor3 = value;
                    if (value)
                    {
                        SetArgbValueTrue();
                        SetSoliderValue(LocalSettings.ColorMainWinState);
                    }
                }
            }
            public bool SelectColor4
            {
                get { return _SelectColor4; }
                set
                {
                    _SelectColor4 = value;
                    if (value)
                    {
                        SetArgbValueTrue();
                        SetSoliderValue(LocalSettings.ColorMainWinLeftMenu);
                    }
                }
            }
            public bool SelectColor5
            {
                get { return _SelectColor5; }
                set
                {
                    _SelectColor5 = value;
                    if (value)
                    {
                        SetArgbValueTrue();
                        SetSoliderValue(LocalSettings.ColorPageNavBar);
                    }
                }
            }

            private void SetArgbValueTrue()
            {
                _SetAlphaValue = _SetRedValue = _SetGreenValue = _SetBlueValue = true;
            }

            public SettingLocal LocalSettings { get; set; }
            /// <summary>
            /// 构造函数
            /// </summary>
            public PageViewModel()
            {
                LocalSettings = DataRWLocalFileRepository.ReadObjFromFile<SettingLocal>(AppSet.LocalSettingFileName);
                RedColorBrush = new SolidColorBrush();
                GreenColorBrush = new SolidColorBrush();
                BlueColorBrush = new SolidColorBrush();
            }
            private void SetSoliderValue(string ColorValueStr)
            {
                Color tempColor = (Color)ColorConverter.ConvertFromString(ColorValueStr);
                AlphaValue = tempColor.A;
                RedColorValue = tempColor.R;
                GreenColorValue = tempColor.G;
                BlueColorValue = tempColor.B;
            }
            private void SetMixColorBrush()
            {
                if (SelectColor1)
                {
                    LocalSettings.ColorMainWinTitle = Color.FromArgb(AlphaValue, RedColorValue, GreenColorValue, BlueColorValue).ToString();
                }
                if (SelectColor2)
                {
                    LocalSettings.ColorMainWinTopMenu = Color.FromArgb(AlphaValue, RedColorValue, GreenColorValue, BlueColorValue).ToString();
                }
                if (SelectColor3)
                {
                    LocalSettings.ColorMainWinState = Color.FromArgb(AlphaValue, RedColorValue, GreenColorValue, BlueColorValue).ToString();
                }
                if (SelectColor4)
                {
                    LocalSettings.ColorMainWinLeftMenu = Color.FromArgb(AlphaValue, RedColorValue, GreenColorValue, BlueColorValue).ToString();
                }
                if (SelectColor5)
                {
                    LocalSettings.ColorPageNavBar = Color.FromArgb(AlphaValue, RedColorValue, GreenColorValue, BlueColorValue).ToString();
                }
            }
        }

    }
}

