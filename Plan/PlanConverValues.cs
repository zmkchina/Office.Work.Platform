using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Office.Work.Platform.Plan
{
    //指定转换器源类型和目标类型
    public class ConverPlanStateToImgeUri : IValueConverter
    {
        //实现接口的两个方法
        #region IValueConverter 成员
        public object Convert(object FileExtentName, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GetStateToImage(FileExtentName.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
        #endregion
        public static Uri GetStateToImage(string PlanState)
        {
            Uri V_DefaultDocImg = null;
            switch (PlanState)
            {
                case "等待执行":
                    V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/PlanNew.png", UriKind.Relative);
                    break;
                case "正在实施":
                    V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/PlanHandle.png", UriKind.Relative);
                    break;
                case "已经完成":
                    V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/PlanFinish.png", UriKind.Relative);
                    break;
                case "计划取消":
                    V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/DocRar.png", UriKind.Relative);
                    break;
            }
            return V_DefaultDocImg;
        }
    }
}
