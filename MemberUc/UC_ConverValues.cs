using System;
using System.Windows.Data;

namespace Office.Work.Platform.MemberUc
{
    //指定转换器源类型和目标类型
    public class DateToToday : IValueConverter
    {
        //实现接口的两个方法
        public object Convert(object DateValue, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DateValue != null && DateTime.TryParse(DateValue.ToString(), out DateTime TheDate))
            {
                if (TheDate.Year == 1)
                    return "今";
                else
                    return TheDate;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }

}
