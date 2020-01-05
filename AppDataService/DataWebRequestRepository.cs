using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Office.Work.Platform.AppDataService
{
    public static class DataWebRequestRepository
    {
        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url">请求URL，如果需要传参，在URL末尾加上“？+参数名=参数值”即可</param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url)
        {
            Object TResult = await Task.Run(() =>
             {

                //创建
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //设置请求方法
                httpWebRequest.Method = "GET";
                //请求超时时间
                httpWebRequest.Timeout = 3000;
                //发送请求
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //利用Stream流读取返回数据
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                //获得最终数据，一般是json
                string responseContent = streamReader.ReadToEnd();
                 streamReader.Close();
                 httpWebResponse.Close();
                 TResult = JsonConvert.DeserializeObject<T>(responseContent);
                 return TResult;
             });
            return (T)TResult;
        }
        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public static string HttpPost(string url, string data)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //字符串转换为字节码
            byte[] bs = Encoding.UTF8.GetBytes(data);
            //参数类型，这里是json类型
            //还有别的类型如"application/x-www-form-urlencoded"，不过我没用过(逃
            httpWebRequest.ContentType = "application/json";
            //参数数据长度
            httpWebRequest.ContentLength = bs.Length;
            //设置请求类型
            httpWebRequest.Method = "POST";
            //设置超时时间
            httpWebRequest.Timeout = 20000;
            //将参数写入请求地址中
            httpWebRequest.GetRequestStream().Write(bs, 0, bs.Length);
            //发送请求
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //读取返回数据
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
            string responseContent = streamReader.ReadToEnd();
            streamReader.Close();
            httpWebResponse.Close();
            httpWebRequest.Abort();
            return responseContent;
        }

    }
}
