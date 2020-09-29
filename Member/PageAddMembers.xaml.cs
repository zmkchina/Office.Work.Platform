using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
        private string _OperationMsg;
        public bool _CanExportAll { get; set; }
        public bool _CanImportAll { get; set; }
        public PageAddMembers()
        {
            InitializeComponent();
            _CanExportAll = AppSet.LoginUser.Grants.Contains("MemberExportAll");
            _CanImportAll = AppSet.LoginUser.Grants.Contains("MemberImportAll");
            this.DataContext = this;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private async void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {

            Button CurBtn = sender as Button;
            int AddCount = 0;
            // 在WPF中， OpenFileDialog位于Microsoft.Win32名称空间
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx"
            };
            if (dialog.ShowDialog() == true)
            {
                CurBtn.IsEnabled = false;
                try
                {
                    System.IO.FileInfo theExcelFile = new System.IO.FileInfo(dialog.FileName);
                    OperationMsg = $"正在从文件[{theExcelFile.FullName}]中导入人员信息，请稍候....";

                    await Task.Run(async () =>
                    {
                        DataTable UserTable = NpoiExcel.ReadStreamToDataTable(theExcelFile.OpenRead(), null, true);

                        if (UserTable != null && UserTable.Rows.Count > 0)
                        {
                            Lib.MemberInfoEntity tempMember = new Lib.MemberInfoEntity();
                            PropertyInfo[] Attri = tempMember.GetType().GetProperties();

                            for (int i = 0; i < UserTable.Rows.Count; i++)
                            {
                                tempMember = new Lib.MemberInfoEntity();
                                foreach (PropertyInfo item in Attri)
                                {
                                    //获取该属性的描述特性值（比如：[Description("身份证号")]中的 "身份证号" ）
                                    object[] objs = item.GetCustomAttributes(typeof(DescriptionAttribute), true);
                                    string PropDescription = ((DescriptionAttribute)objs[0]).Description;
                                    if (UserTable.Columns.Contains(PropDescription) && UserTable.Rows[i][PropDescription] != null)
                                    {
                                        StringBuilder dateStr = new StringBuilder(UserTable.Rows[i][PropDescription].ToString());
                                        switch (item.PropertyType.FullName)
                                        {
                                            case "System.DateTime":
                                                if (!DateTime.TryParse(dateStr.ToString(), out DateTime tempDateTime))
                                                {
                                                    //处理Excel中数据不规范情况。
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
                                                break;
                                            case "System.Int32":
                                                if (int.TryParse(dateStr.ToString(), out int tempIntValue))
                                                {
                                                    item.SetValue(tempMember, tempIntValue);
                                                }
                                                break;
                                            case "System.String":
                                                item.SetValue(tempMember, UserTable.Rows[i][PropDescription].ToString());
                                                break;
                                            default:
                                                continue;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(tempMember.Id))
                                {
                                    tempMember.UserId = AppSet.LoginUser.Id;
                                    ExcuteResult excuteResult = await DataMemberRepository.AddOrUpdate(tempMember).ConfigureAwait(false);
                                    if (excuteResult.State == 0)
                                    {
                                        AddCount++;
                                        OperationMsg = $"正在从文件[{theExcelFile.FullName}]中导入人员信息，已导入[{AddCount}]条信息。";
                                    }
                                }
                            }
                        }
                    });//Task Finish
                    OperationMsg = $"人员信息导入成功，共导入[{AddCount}]条信息。";
                    CurBtn.IsEnabled = true;
                }
                catch (Exception VE)
                {
                    AppFuns.ShowMessage("出现错误：" + VE.Message, Caption: "错误", isErr: true);
                    CurBtn.IsEnabled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 操作反馈信息
        /// </summary>
        public string OperationMsg
        {
            get { return _OperationMsg; }
            set { _OperationMsg = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnExport_ClickAsync(object sender, RoutedEventArgs e)
        {
            Button CurBtn = sender as Button;
            List<Lib.MemberInfoEntity> MemberList = await DataMemberRepository.ReadMembers(new MemberSearch()
            {
            });

            if (MemberList == null)
            {
                return;
            }
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.Filter = "Excel|*.xls";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            try
            {
                CurBtn.IsEnabled = false;
                OperationMsg = "正在导出人员信息,请稍候....";
                await Task.Run(() =>
                {
                    NpoiExcel.ExportExcels(fileDialog.FileName, "员工信息", MemberList);
                });
                OperationMsg = $"人员信息导出....完成，文件为[{fileDialog.FileName}]。";
                CurBtn.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                AppFuns.ShowMessage(Ex.Message, Caption: "失败");
                CurBtn.IsEnabled = true;
            }
        }
    }
}
