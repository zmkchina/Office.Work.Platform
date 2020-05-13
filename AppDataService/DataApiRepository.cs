using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;
using Office.Work.Platform.AppCodes;
using Office.Work.Platform.Lib;

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
        public static async Task<string> GetAccessToken(string P_UserName = null, string P_Pwd = null)
        {
            IS4_AccessToken = "";
            HttpClient _Client = CreateHttpClient();
            //var disco = await _Client.GetDiscoveryDocumentAsync(AppSet.LocalSetting.IS4SeverUrl).ConfigureAwait(false);
            var disco = await _Client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = AppSet.LocalSetting.IS4SeverUrl,
                Policy = new DiscoveryPolicy()
                {
                    RequireHttps = false
                }
            }).ConfigureAwait(false);
            //发现 IS4 各类功能所在的网址和其他相关信息 
            if (disco.IsError)
            {
                //Console.WriteLine(disco.Error);
                return disco.Error;
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
                return tokenResponse.Error;
            }
            //JObject TokenJsonObj = tokenResponse.Json;
            if (tokenResponse.AccessToken != null)
            {
                IS4_AccessToken = tokenResponse.AccessToken;
                return "Ok";
            }
            return "未知错误！";
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
            HttpClient _Client = CreateHttpClient(processMessageHander);

            try
            {
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
                else if (typeof(T) == typeof(string))
                {
                    TResult = await ResultResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    string ResponseString = await ResultResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    TResult = JsonConvert.DeserializeObject<T>(ResponseString);
                }
                return (T)TResult;
            }
            catch (Exception)
            {
            }
            finally
            {
                _Client.Dispose();
            }
            return default(T);
        }
        public static async Task<T> DeleteApiUri<T>(string ApiUri, ProgressMessageHandler processMessageHander = null)
        {
            HttpClient _Client = CreateHttpClient(processMessageHander);
            Object TResult = new ExcuteResult();
            try
            {

                HttpResponseMessage ResponseMsg = await _Client.DeleteAsync(ApiUri).ConfigureAwait(false);
                string ResponseString = ResponseMsg.Content.ReadAsStringAsync().Result;
                TResult = JsonConvert.DeserializeObject<T>(ResponseString);
            }
            catch (Exception err)
            {
                (new WinMsgDialog(err.Message, isErr: true)).ShowDialog();
            }
            finally
            {
                _Client?.Dispose();
            }
            return (T)TResult;
        }
        /// <summary>
        /// Post 数据到指定的Url（注意：此方式可同时传输 文件）
        /// </summary>
        /// <typeparam name="T">返回指定类型数据，需服务器配合</typeparam>
        /// <param name="ApiUri">Url</param>
        /// <param name="PostFormData">MultipartFormDataContent类型数据</param>
        /// <param name="processMessageHander">进度报告委托</param>
        /// <returns></returns>
        public static async Task<ExcuteResult> PostApiUriAsync(string ApiUri, MultipartFormDataContent PostFormData, ProgressMessageHandler processMessageHander = null)
        {
            HttpClient _Client = CreateHttpClient(processMessageHander);
            ExcuteResult TResult = new ExcuteResult();
            try
            {
                HttpResponseMessage ResponseMsg = await _Client.PostAsync(ApiUri, PostFormData).ConfigureAwait(false);
                string ResponseString = await ResponseMsg.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (ResponseString.Contains("errors"))
                {
                    ResponseResult ResResult = JsonConvert.DeserializeObject<ResponseResult>(ResponseString);
                    TResult = new ExcuteResult() { State = -1, Msg = ResResult.errors, Tag = ResResult.traceId };
                }
                else
                {
                    TResult = JsonConvert.DeserializeObject<ExcuteResult>(ResponseString);
                }
            }
            catch (Exception err)
            {
                (new WinMsgDialog(err.Message, isErr: true)).ShowDialog();
            }
            finally
            {
                _Client?.Dispose();
            }
            return TResult;
        }
        /// <summary>
        /// 将任意一个对象转换成JSON格式，POST到指定的URL（注意：此方式无法传输 文件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ApiUri"></param>
        /// <param name="PostObject"></param>
        /// <param name="processMessageHander">进度报告委托</param>
        /// <returns></returns>
        public static async Task<ExcuteResult> PostApiUriAsync<T>(string ApiUri, T PostObject, ProgressMessageHandler processMessageHander = null)
        {
            HttpClient _Client = CreateHttpClient(processMessageHander);
            ExcuteResult TResult = new ExcuteResult();
            //表头参数  (添加此Headers 会导致返回的数据在解析时出问题，估计.net core 已默认为json格式，再加会画蛇添足。
            //_Client.DefaultRequestHeaders.Accept.Clear();
            //_Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //转为待传输的Json字符串
            string PostObjectJsonStr = JsonConvert.SerializeObject(PostObject);
            HttpContent httpContent = new StringContent(PostObjectJsonStr, Encoding.UTF8, "application/json");
            //请求
            HttpResponseMessage ResponseMsg = await _Client.PostAsync(ApiUri, httpContent).ConfigureAwait(false);

            if (ResponseMsg.IsSuccessStatusCode)
            {
                string ResponseString = ResponseMsg.Content.ReadAsStringAsync().Result;
                if (ResponseString.Contains("errors"))
                {
                    ResponseResult ResResult = JsonConvert.DeserializeObject<ResponseResult>(ResponseString);
                    TResult = new ExcuteResult() { State = -1, Msg = ResResult.errors, Tag = ResResult.traceId };
                }
                else
                {
                    TResult = JsonConvert.DeserializeObject<ExcuteResult>(ResponseString);
                }
            }
            return TResult;
        }

        /// <summary>
        /// PUT 数据到指定的Url,一般用于更新一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ApiUri"></param>
        /// <param name="PostObject"></param>
        /// <param name="processMessageHander"></param>
        /// <returns></returns>
        public static async Task<ExcuteResult> PutApiUriAsync<T>(string ApiUri, T PostObject, ProgressMessageHandler processMessageHander = null)
        {
            HttpClient _Client = CreateHttpClient(processMessageHander);
            ExcuteResult TResult = new ExcuteResult();
            //表头参数  (添加此Headers 会导致返回的数据在解析时出问题，估计.net core 已默认为json格式，再加会画蛇添足。
            //_Client.DefaultRequestHeaders.Accept.Clear();
            //_Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            //转为待传输的Json字符串
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(PostObject), Encoding.UTF8, "application/json");
            //请求
            HttpResponseMessage ResponseMsg = await _Client.PutAsync(ApiUri, httpContent).ConfigureAwait(false);
            if (ResponseMsg.IsSuccessStatusCode)
            {
                string ResponseString = ResponseMsg.Content.ReadAsStringAsync().Result;
                if (ResponseString.Contains("errors"))
                {
                    ResponseResult ResResult = JsonConvert.DeserializeObject<ResponseResult>(ResponseString);
                    TResult = new ExcuteResult() { State = -1, Msg = ResResult.errors, Tag = ResResult.traceId };
                }
                else
                {
                    TResult = JsonConvert.DeserializeObject<ExcuteResult>(ResponseString);
                }
            }
            return TResult;
        }

        /// <summary>
        /// 将实体数据，转换成MultipartFormDataContent类型，以便进行POST或PUT（可以包含文件）
        /// </summary>
        /// <typeparam name="T">待转换的实体类型</typeparam>
        /// <param name="PostOrPutEntity">待转换的实体对象</param>
        /// <param name="PostFileStream">上传的文件流</param>
        /// <param name="PostFileKey">上传的文件Key,供服务器程序检索使用，比如：Request.Form.Files[Key]</param>
        /// <param name="PostFileName">上传文件的文件名</param>
        /// <returns></returns>
        public static MultipartFormDataContent SetFormData<T>(T PostOrPutEntity, Stream PostFileStream, string PostFileKey = null, string PostFileName = null)
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
                    V_MultFormDatas.Add(new StreamContent(PostFileStream));
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
            }
            return urlParams.ToString();
        }
    }
}
