using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Helios.Web.Providers;

namespace Helios.Web
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        //public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }


        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // 针对基于 OAuth 的流配置应用程序
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new HeliosAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider(),

                // TODO: 改成配置项 appsettings
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
#if DEBUG
                //在生产模式下设 AllowInsecureHttp = false
                AllowInsecureHttp = true,       // 重要！！这里的设置包含整个流程通信环境是否启用ssl
#endif
            };

            

            // 使应用程序可以使用不记名令牌来验证用户身份
            //            app.UseOAuthBearerTokens(OAuthOptions);
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
    }
}
