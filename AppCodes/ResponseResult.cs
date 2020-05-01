using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Office.Work.Platform.AppCodes
{
    /// <summary>
    /// 系统用户类
    /// </summary>
    public class ResponseResult
    {
        public string status { get; set; }
        public string errors { get; set; }
        public string type { get; set; }
        public string traceId { get; set; }
    }
}
