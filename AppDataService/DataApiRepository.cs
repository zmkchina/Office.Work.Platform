using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Office.Work.Platform.AppDataService
{
    public static class DataApiRepository
    {
        private static string IS4_AccessToken = "";
        private static HttpClient CreateHttpClient(ProgressMessageHandler processMessageHander = null)
        {
            HttpClient _Client;
            if (processMessageHander != null)
            {
                _Client = HttpClientFactory.Create(processMessageHander);
            }
            else
            {
                _Client = HttpClientFactory.Create();
            }
            _Client.Timeout = TimeSpan.FromSeconds(60);

            //如果accessToken有值，则携带之。
            if (!string.IsNullOrWhiteSpace(IS4_AccessToken))
            {
                _Client.SetBearerToken(IS4_AccessToken);
            }
            return _Client;
        }
        /// <summary>
        /// 获取AccessToaken
        /// </summary>
        /// <returns></returns>
        public static async Task GetAccessToken(string P_UserName = null, string P_Pwd = null)
        {
            IS4_AccessToken = "";
            HttpClient _Client = CreateHttpClient();
            var disco = await _Client.GetDiscoveryDocumentAsync("http://localhost:5000").ConfigureAwait(false);
            //发现 IS4 各类功能所在的网址和其他相关信息 
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            ////ClientCredentials 客户端凭据许可
            //var tokenResponse = await _Client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            //{
            //    Address = disco.TokenEndpoint, //令牌端点 = http://localhost:5000/connect/token
            //    ClientId = "WorkPlatformClient",    //id
            //    ClientSecret = "Work_Platform_ClientPwd", //pwd
            //    Scope = "Apis",  //请求的api
            //});

            //ClientPassword 客户端加用户密码模式
            var tokenResponse = await _Client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "WorkPlatformClient",
                ClientSecret = "Work_Platform_ClientPwd",
                Scope = "Apis",

                //以下为用户与密码
                UserName = P_UserName,
                Password = P_Pwd
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            //JObject TokenJsonObj = tokenResponse.Json;
            if (tokenResponse.AccessToken != null)
            {
                IS4_AccessToken = tokenResponse.AccessToken;
            }

            //// call api
            //var Apiclient = new HttpClient();
            //Apiclient.SetBearerToken(tokenResponse.AccessToken);

            //var response = await Apiclient.GetAsync("http://localhost:5001/identity");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.StatusCode);
            //}
            //else
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(JArray.Parse(content));
            //}
        }

        /// <summary>
        /// 使用GET方法，获取服务器资源。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ApiUri">资源Uri</param>
        /// <param name="processMessageHander">进度报告委托</param>
        /// <returns></returns>
        public static async Task<T> GetApiUri<T>(string ApiUri, ProgressMessageHandler processMessageHander = null)
        {
            using HttpClient _Client = CreateHttpClient(processMessageHander);
            Object TResult = null;
            HttpResponseMessage ResultResponse = await _Client.GetAsync(ApiUri).ConfigureAwait(false);
            if (typeof(T) == typeof(HttpResponseMessage))
            {
                TResult = ResultResponse;
            }
            else if (typeof(T) == typeof(Stream))
            {
                TResult = await ResultResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            else if (typeof(T) == typeof(byte[]))
            {
                TResult = await ResultResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            else
            {
                string ResponseString = await ResultResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                TResult = JsonConvert.DeserializeObject<T>(ResponseString);
            }
            return (T)TResult;
        }
        public static async Task<T> DeleteApiUri<T>(string ApiUri, ProgressMessageHandler processMessageHander = null)
        {
            HttpClient _Client = CreateHttpClient(processMessageHander);
            Object TResult = null;
            try
            {

                HttpResponseMessage ResponseMsg = await _Client.DeleteAsync(ApiUri).ConfigureAwait(false);
                string ResponseString = ResponseMsg.Content.ReadAsStringAsync().Result;
                TResult = JsonConvert.DeserializeObject<T>(ResponseString);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _Client?.Dispose();
            }
            return (T)TResult;
        }
        /// <summary>
        /// Post 数据到指定的Url，带进度报告委托
        /// </summary>
        /// <typeparam name="T">返回指定类型数据，需服务器配合</typeparam>
        /// <param name="ApiUri">Url</param>
        /// <param name="PostFormData">MultipartFormDataContent类型数据</param>
        /// <param name="processMessageHander">进度报告委托</param>
        /// <returns></returns>
        public static async Task<T> PostApiUri<T>(string ApiUri, MultipartFormDataContent PostFormData, ProgressMessageHandler processMessageHander = null)
        {
            HttpClient _Client = CreateHttpClient(processMessageHander);
            Object TResult = null;
            try
            {
                HttpResponseMessage ResponseMsg = await _Client.PostAsync(ApiUri, PostFormData).ConfigureAwait(false);
                string ResponseString = await ResponseMsg.Content.ReadAsStringAsync().ConfigureAwait(false);
                TResult = JsonConvert.DeserializeObject<T>(ResponseString);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _Client?.Dispose();
            }
            return (T)TResult;
        }
        /// <summary>
        /// PUT 数据到指定的Url,一般用于更新一个实体
        /// </summary>
        /// <typeparam name="T">返回指定类型数据，需服务器配合</typeparam>
        /// <param name="ApiUri">Url</param>
        /// <param name="PostFormData">MultipartFormDataContent 类型数据</param>
        /// <returns></returns>
        public static async Task<T> PutApiUri<T>(string ApiUri, MultipartFormDataContent PostFormData)
        {
            HttpClient _Client = CreateHttpClient();
            Object TResult = null;
            try
            {
                HttpResponseMessage ResponseMsg = await _Client.PutAsync(ApiUri, PostFormData).ConfigureAwait(false);
                string ResponseString = ResponseMsg.Content.ReadAsStringAsync().Result;
                TResult = JsonConvert.DeserializeObject<T>(ResponseString);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _Client?.Dispose();
            }
            return (T)TResult;
        }



        /// <summary>
        /// 将任意一个对象转换成JSON格式，POST到指定的URL
        /// </summary>
        /// <typeparam name="T">返回指定类型数据，需服务器配合</typeparam>
        /// <param name="ApiUri">Url</param>
        /// <param name="PostData">Object类型数据</param>
        /// <returns></returns>
        public static async Task<T> PostApiUri<T>(string ApiUri, object PostData)
        {
            HttpClient _Client = CreateHttpClient();
            Object TResult = null;
            //表头参数  (添加此Headers 会导致返回的数据在解析时出问题，估计.net core 已默认为json格式，再加会画蛇添足。
            //_Client.DefaultRequestHeaders.Accept.Clear();
            //_Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            //转为链接需要的格式
            HttpContent httpContent = new JsonContent(PostData);
            //请求
            HttpResponseMessage ResponseMsg = await _Client.PostAsync(ApiUri, httpContent).ConfigureAwait(false);
            if (ResponseMsg.IsSuccessStatusCode)
            {
                string ResponseMsgString = ResponseMsg.Content.ReadAsStringAsync().Result;
                if (ResponseMsgString != null)
                {
                    TResult = JsonConvert.DeserializeObject<T>(ResponseMsgString);
                }
            }
            return (T)TResult;
        }


        /// <summary>
        /// 将实体数据，转换成MultipartFormDataContent类型，以便进行POST或PUT
        /// </summary>
        /// <typeparam name="T">待转换的实体类型</typeparam>
        /// <param name="PostOrPutEntity">待转换的实体对象</param>
        /// <param name="PostFileStream">上传的文件流</param>
        /// <param name="PostFileKey">上传的文件Key,供服务器程序检索使用，比如：Request.Form.Files[Key]</param>
        /// <param name="PostFileName">上传文件的文件名</param>
        /// <returns></returns>
        public static MultipartFormDataContent SetFormData<T>(T PostOrPutEntity, Stream PostFileStream = null, string PostFileKey = null, string PostFileName = null)
        {
            MultipartFormDataContent V_MultFormDatas = new MultipartFormDataContent();

            PropertyInfo[] Attri = PostOrPutEntity.GetType().GetProperties();
            foreach (PropertyInfo item in Attri)
            {
                object itemValue = item.GetValue(PostOrPutEntity);
                //Type AttriType = itemValue.GetType();获取数据类型再作进一步处理。
                if (itemValue != null)
                {
                    V_MultFormDatas.Add(new StringContent(itemValue.ToString()), item.Name);
                }
                else
                {
                    V_MultFormDatas.Add(new StringContent(""), item.Name);
                }
            }
            if (PostFileStream != null)
            {
                if (PostFileKey == null)
                {
                    V_MultFormDatas.Add(new StreamContent (PostFileStream));
                }
                else if (PostFileName == null)
                {
                    V_MultFormDatas.Add(new StreamContent(PostFileStream), PostFileKey);
                }
                else
                {
                    V_MultFormDatas.Add(new StreamContent(PostFileStream), PostFileKey, PostFileName);
                }
            }
            return V_MultFormDatas;
        }

        //public static async Task<string> PostFileAsync(Stream filestream, string filename, int filesize)
        //{
        //    var progress = new System.Net.Http.Handlers.ProgressMessageHandler();

        //    //Progress tracking
        //    progress.HttpSendProgress += (object sender, System.Net.Http.Handlers.HttpProgressEventArgs e) =>
        //    {
        //        int progressPercentage = (int)(e.BytesTransferred * 100 / filesize);
        //        //Raise an event that is used to update the UI
        //        UploadProgressMade(sender, new System.Net.Http.Handlers.HttpProgressEventArgs(progressPercentage, null, e.BytesTransferred, null));
        //    };

        //    using (var client = HttpClientFactory.Create(progress))
        //    {
        //        using (var content = new MultipartFormDataContent("------" + DateTime.Now.Ticks.ToString("x")))
        //        {
        //            content.Add(new StreamContent(filestream), "Filedata", filename);
        //            using (var message = await client.PostAsync("http://MyUrl.example", content))
        //            {
        //                var result = await message.Content.ReadAsStringAsync();
        //                System.Diagnostics.Debug.WriteLine("Upload done");
        //                return result;
        //            }
        //        }
        //    }
        //}

        public static string CreateUrlParams<T>(T Entity)
        {
            System.Text.StringBuilder urlParams = new StringBuilder();
            PropertyInfo[] Attri = Entity.GetType().GetProperties();
            foreach (PropertyInfo item in Attri)
            {
                object itemValue = item.GetValue(Entity);
                //Type AttriType = itemValue.GetType();获取数据类型再作进一步处理。
                if (itemValue != null)
                {
                    if (urlParams.Length < 1)
                    {
                        urlParams.Append($"?" + item.Name + "=" + itemValue.ToString());
                    }
                    else
                    {
                        urlParams.Append($"&" + item.Name + "=" + itemValue.ToString());
                    }
                }
                //else
                //{
                //    if (urlParams.Length < 1)
                //    {
                //        urlParams.Append($"?" + item.Name + "=");
                //    }
                //    else
                //    {
                //        urlParams.Append($"&" + item.Name + "=");
                //    }
                //}
            }
            return urlParams.ToString();
        }
    }
    public class JsonContent : StringContent
    {
        public JsonContent(object obj) : base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        { }
    }
}
