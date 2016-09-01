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
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetBasicCredentials(out clientId, out clientSecret);

            // TODO: 调用后端业务逻辑对clientId以及clientSecret 进行配对校验
            if (clientId == "1234" && clientSecret == "5678")
            {
                context.Validated(clientId);
            }

            return base.ValidateClientAuthentication(context);
        }

        /// <summary>
        /// 在 GrantClientCredentials() 方法中对客户端进行授权，授了权就能发 access token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, "iOS App"));
            var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            context.Validated(ticket);

            return base.GrantClientCredentials(context);
        }
    }
}