using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.AppDataService;
using Office.Work.Platform.Lib;

namespace Office.Work.Platform.Member
{
    /// <summary>
    /// PageAddMembers.xaml 的交互逻辑
    /// </summary>
    public partial class PageAddMembers : Page, INotifyPropertyChanged
    {
        private string _ExcelName;
        private string _AddCount;

        public PageAddMembers()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private async void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            int AddCount = 0;
            // 在WPF中， OpenFileDialog位于Microsoft.Win32名称空间
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    System.IO.FileInfo theExcelFile = new System.IO.FileInfo(dialog.FileName);
                    this.TB_ExcelName.Text = theExcelFile.FullName;
                    DataTable UserTable = NPOIOffice.ReadStreamToDataTable(theExcelFile.OpenRead(), null, true);

                    if (UserTable != null && UserTable.Rows.Count > 0)
                    {
                        List<Lib.Member> memberList = new List<Lib.Member>();
                        Lib.Member tempMember = new Lib.Member();
                        PropertyInfo[] Attri = tempMember.GetType().GetProperties();
                        for (int i = 0; i < UserTable.Rows.Count; i++)
                        {
                            tempMember = new Lib.Member();
                            foreach (PropertyInfo item in Attri)
                            {
                                if (UserTable.Columns.Contains(item.Name) && UserTable.Rows[i][item.Name] != null)
                                {
                                    if (item.PropertyType.Name.Contains("DateTime"))
                                    {
                                        //处理一下Date值，因为Excel表中的数值格式可能不符合要求。
                                        System.Text.StringBuilder dateStr = new StringBuilder(UserTable.Rows[i][item.Name].ToString());
                                        if (!DateTime.TryParse(dateStr.ToString(), out DateTime tempDateTime))
                                        {
                                            dateStr = dateStr.Replace('.', '/');
                                            dateStr = dateStr.Replace('-', '/');
                                            dateStr = dateStr.Replace('—', '/');
                                            dateStr = dateStr.Replace('。', '/');
                                            if (dateStr.Length < 8) dateStr.Append("/01");
                                        }
                                        if (DateTime.TryParse(dateStr.ToString(), out tempDateTime))
                                        {
                                            item.SetValue(tempMember, tempDateTime);
                                        }
                                    }
                                    else
                                    {
                                        item.SetValue(tempMember, UserTable.Rows[i][item.Name].ToString());
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(tempMember.Id))
                            {
                                if (!memberList.Any(x => x.Id != tempMember.Id))
                                {
                                    ExcuteResult excuteResult = await DataMemberRepository.AddMember(tempMember).ConfigureAwait(false);
                                    if (excuteResult.State == 0)
                                    {
                                        AddCount++;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception VE)
                {
                    (new WinMsgDialog("出现错误：" + VE.Message, Caption: "错误", isErr: true)).ShowDialog();

                    return;
                }
            }
        }

        public string ExcelName
        {
            get { return _ExcelName; }
            set { _ExcelName = value; OnPropertyChanged(); }
        }
        public string AddCount
        {
            get { return _AddCount; }
            set { _AddCount = value; OnPropertyChanged(); }
        }
    }
}
