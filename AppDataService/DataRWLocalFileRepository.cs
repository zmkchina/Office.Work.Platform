using Newtonsoft.Json;
using Office.Work.Platform.Lib;
using System;
using System.IO;

namespace Office.Work.Platform.AppDataService
{
    public static class DataRWLocalFileRepository
    {
        /// <summary>
        /// 删除本地一个文件
        /// </summary>
        /// <param name="DeleFileName"></param>
        /// <returns></returns>
        public static bool DeleLocalFile(string DeleFileName)
        {
            string V_DeleFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DeleFileName);
            //创建一个文件流
            if (File.Exists(V_DeleFileName))
            {
                File.Delete(V_DeleFileName);
            }
            return true;
        }
        /// <summary>
        /// 将一个对象存入文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SaveDataObj"></param>
        /// <param name="SaveFileName"></param>
        /// <returns></returns>
        public static bool SaveObjToFile<T>(T SaveDataObj, string SaveFileName)
        {
            //加密，同时传换为字节数组。
            byte[] V_UserBytes = System.Text.Encoding.ASCII.GetBytes(FunStringCrypt.EnCrypt(JsonConvert.SerializeObject(SaveDataObj), "GangHang"));
            string V_SaveFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SaveFileName);
            //+ @"LocalSettings.ini";
            //创建一个文件流
            FileStream fs = new FileStream(V_SaveFileName, FileMode.Create, FileAccess.ReadWrite);
            //将byte数组写入文件中
            fs.Write(V_UserBytes, 0, V_UserBytes.Length);
            //所有流类型都要关闭流，否则会出现内存泄露问题
            fs.Close();
            return true;
        }
        /// <summary>
        ///  从文件中读取一个对象（列表）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ReadFromFileName"></param>
        /// <returns></returns>
        public static T ReadObjFromFile<T>(string ReadFromFileName) where T : new()
        {
            T targetObj;
            string V_LocalFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ReadFromFileName);
            if (System.IO.File.Exists(V_LocalFile))
            {
                FileStream fs = new FileStream(V_LocalFile, FileMode.Open);
                int V_FileLength = int.Parse(fs.Length.ToString());
                byte[] V_UserBytes = new byte[V_FileLength];
                fs.Read(V_UserBytes, 0, V_FileLength);
                fs.Close();
                //JObject V_Jobj = (JObject)JsonConvert.DeserializeObject(System.Text.Encoding.Default.GetString(V_UserBytes), SettingsLocal);
                //解密并转换为指定类型的对象
                targetObj = JsonConvert.DeserializeObject<T>(FunStringCrypt.DeCrypt(System.Text.Encoding.ASCII.GetString(V_UserBytes), "GangHang"));
            }
            else
            {
                targetObj = new T();
            }
            return targetObj;
        }
    }
}
