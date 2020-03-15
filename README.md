 Office.Work.Platform

这是一个 办公信息化系统 开发的客户端项目
### 关于权限

**计划权限**

A:计划修改 B:计划删除 C:计划进度更新 D:上传计划附件 E:计划完结 F:计划状态重置
+ 系统管理员：A—F。
+ 计划制订者：A—E。
+ 计划责任者：C—E。
+ 计划协助者：C、D。


### 开发技巧
1.一个类中 ObservableCollection 集合对象中的具体条目，如果是在不同的线程中生成的，则在绑定到具体UI上时可能会不能及时更新出。*可以如下解决。*

```
Task.Run(async () =>
      {
          ObservableCollection XX= await GetXXcollect();
          this.Dispatcher.Invoke(() =>
          {             
              DataContext = null;
              DataContext = _UC_PlanInfoVM;
          });
      });
```

2.IdentityServer4客户端、密码两种验证模式（服务器与受保护的API资源在一起）

(1)导入程序集

```
*IS4 服务端*
IdentityServer4
IdentityServer4.AspNetIdentity
Microsoft.AspNetCore.Authentication.JwtBearer

*客户端，只需下面一个*
IdentityModel

```

(2)创建IS4配置类IS４Config.cs

```c#
/// <summary>
    /// IdentityServer4的配置类
    /// </summary>
    public class IS4Config
    {
        /// <summary>
        /// 获取IS4自身内置的标准API资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
        /// <summary>
        /// 获取应用需调用的（用户自己编写的）API资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[] {
                new ApiResource("Apis","My Apis")
            };
        }
        /// <summary>
        /// 定义将访问IS4 的客户端
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new Client[] {
                new Client(){
                    //一个客户端支持两种验证模式
                    ClientId="WorkPlatformClient",
                    AllowedGrantTypes={
                        GrantType.ClientCredentials,  //客户端模式
                        GrantType.ResourceOwnerPassword //密码模式
                    },
                    ClientSecrets={new Secret("Work_Platform_ClientPwd".Sha256())},
                    AllowedScopes={ "Apis",
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.OfflineAccess
                    }
                },
            };
        }
    }
```

(3)创建IS4 用户查询类ResourceOwnerPasswordValidator.cs

```c#
  public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly DataUserRepository _UserRepository; //用户数据表操作对象
        public ResourceOwnerPasswordValidator(GHDbContext ghDbContet)
        {
            _UserRepository = new DataUserRepository(ghDbContet);
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var user = await _UserRepository.GetOneByIdPwdAsync(context.UserName, context.Password);
                //如用户名与密码都正确，User 才不会为 null
                if (user != null)
                {
                    context.Result = new GrantValidationResult(
                            subject: user.Id.ToString(),
                            authenticationMethod: "custom",
                            claims: GetUserClaims(user) //视情况，可以不需要此claims
                            );
                    return;
                }
                else
                {

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "无效的用户名或密码");
                    return;
                }
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.Message);
            }
        }

        //build claims array from user data  
        public static Claim[] GetUserClaims(ModelUser user)
        {
            return new Claim[]
            {
                new Claim("user_id", user.Id ?? ""),
                new Claim(JwtClaimTypes.Name,user.Id ?? ""),
                new Claim(JwtClaimTypes.Role, user.Post)
            };
        }
    }
```

(4)修改setup.cs类

```c#
 public void ConfigureServices(IServiceCollection services)
        {
            //注册IS4 服务
            var is4Buider = services.AddIdentityServer()
                  .AddDeveloperSigningCredential()
                  .AddInMemoryApiResources(IS4Config.GetApiResources()) //IS4 导入定义的应用资源API
                  .AddInMemoryIdentityResources(IS4Config.GetIdentityResources())  //IS4 自身API
                  .AddInMemoryClients(IS4Config.GetClients())  //IS4 导入定义的客户端
                  .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();//IS4 从数据表中验证用户类
                             
            //注册验证（*用于保护API资源，与IS4无关* ）
            services.AddAuthentication("Bearer").AddJwtBearer(r =>
            {
                //认证服务地址
                r.Authority = "http://localhost:5000";
                //权限标识
                r.Audience = "Apis";
                //是否必需HTTPS
                r.RequireHttpsMetadata = false;
            });
        }
        
 public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            //使用 IS4 中间件。
            app.UseIdentityServer();
            
            //添加验证（*用于保护API资源，与IS4无关* ）
            app.UseAuthentication(); //注意:验证中间件必须放在授权之前。**
            //添加授权（*用于保护API资源，与IS4无关* ）
            app.UseAuthorization();//
            //上两行要放在app.UseRouting、app.UseCors("cors")之后，并且在app.UseEndpoints之前
            
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
```

（5）欲进行验证的客户端代码

```c#
 /// <summary>
        /// 获取AccessToaken
        /// </summary>
        /// <returns></returns>
        public static async Task GetAccessToken(string P_UserName = null, string P_Pwd = null)
        {
            IS4_AccessToken = "";
            HttpClient _Client = new HttpClient();
            var disco = await _Client.GetDiscoveryDocumentAsync("http://localhost:5000");
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
```

