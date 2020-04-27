using System;
using System.Linq;
using System.Windows.Data;

namespace Office.Work.Platform.AppCodes
{
    //指定转换器源类型和目标类型
    public class ConverExtentNameToImgeUri : IValueConverter
    {
        //实现接口的两个方法
        #region IValueConverter 成员
        public object Convert(object FileExtentName, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GetFileTypeImage(FileExtentName.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
        #endregion
        public static Uri GetFileTypeImage(string FileExtentName)
        {
            Uri V_DefaultDocImg = null;
            if ((new string[] { ".bmp", ".jpg", ".gif", ".jpeg", ".png", ".jpe", ".jfif", ".ico", ".tif", ".tiff" }).Contains(FileExtentName.ToLower()))
            {
                V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/PhotoImg.png", UriKind.Relative);
            }
            if (V_DefaultDocImg == null && (new string[] { ".xls", ".xlsx" }).Contains(FileExtentName.ToLower()))
            {
                V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/DocExcel.png", UriKind.Relative);
            }
            if (V_DefaultDocImg == null && (new string[] { ".doc", ".docx" }).Contains(FileExtentName.ToLower()))
            {
                return new Uri("/Office.Work.Platform;component/AppRes/Images/DocWord.png", UriKind.Relative);
            }
            if (V_DefaultDocImg == null && (new string[] { ".ppt", ".pptx" }).Contains(FileExtentName.ToLower()))
            {
                V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/DocPpt.png", UriKind.Relative);
            }
            if (V_DefaultDocImg == null && (new string[] { ".rar", ".zip" }).Contains(FileExtentName.ToLower()))
            {
                V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/DocRar.png", UriKind.Relative);
            }
            if (V_DefaultDocImg == null && (new string[] { ".pdf" }).Contains(FileExtentName.ToLower()))
            {
                V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/DocPdf.png", UriKind.Relative);
            }
            if (V_DefaultDocImg == null && (new string[] { ".mp4", ".mpeg", ".mpg", ".avi", ".mkv", ".ram" }).Contains(FileExtentName.ToLower()))
            {
                V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/Video.png", UriKind.Relative);
            }
            if (V_DefaultDocImg == null)
            {
                V_DefaultDocImg = new Uri("/Office.Work.Platform;component/AppRes/Images/DocImg.png", UriKind.Relative);
            }

            return V_DefaultDocImg;
        }
    }

    //指定转换器源类型和目标类型
    public class ConverByteLengthToMBLength : IValueConverter
    {
        //实现接口的两个方法
        #region IValueConverter 成员
        public object Convert(object BytesLength, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)(Double.Parse(BytesLength.ToString()) / 1024) / 1024).ToString("0.00");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
        #endregion
    }
}
