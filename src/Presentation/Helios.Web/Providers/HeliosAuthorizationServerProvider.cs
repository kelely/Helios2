using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Helios.Web.Providers
{
    public class HeliosAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 在 ValidateClientAuthentication() 方法中获取客户端的 client_id 与 client_secret 进行验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId, clientSecret;

            // 首先从 Basic Authorization 中获取客户端信息，获取不到就从POST数据中获取客户端信息
            // 客户端信息保存在 Basic Authorization 更加符合 OAuth 2.0 规范，增加对 POST 的支持
            // 仅仅是为了使用 Postman 进行调试时方便，客户端在传输客户端信息时，应该首选 Basic Authorization 方式。
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
                context.TryGetFormCredentials(out clientId, out clientSecret);

            // TODO: 调用后端业务逻辑对clientId以及clientSecret 进行配对校验
            if (clientId == "1234" && clientSecret == "5678")
            {
                context.Validated(clientId);
            }

            await base.ValidateClientAuthentication(context);
        }

        /// <summary>
        /// 在 GrantClientCredentials() 方法中对客户端进行授权，授了权就能发 access token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, "iOS App"));
            var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            context.Validated(ticket);

            await base.GrantClientCredentials(context);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //TODO:调用后台的登录服务验证用户名与密码

            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            context.Validated(ticket);

            await base.GrantResourceOwnerCredentials(context);
        }
    }
}