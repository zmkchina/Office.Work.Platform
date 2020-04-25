using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Office.Work.Platform.AppCodes
{
    /// <summary>
    /// 系统用户类
    /// </summary>
    public class SettingLocal : INotifyPropertyChanged
    {
        #region 事件
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region 字段
        private string _LoginUserId;
        private int _RestInterval;
        #endregion

        #region 属性
        /// <summary>
        /// 用户Id号
        /// </summary>
        public string LoginUserId
        {
            get { return _LoginUserId; }
            set { _LoginUserId = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 该人员设定的休息时间间隔
        /// </summary>
        public int RestInterval
        {
            get { return _RestInterval; }
            set { _RestInterval = value; OnPropertyChanged(); }
        }
        #endregion
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SettingLocal()
        {
            RestInterval = 20;
        }

        #endregion
        #region 方法
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
