using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Helios.Web.Providers
{
    public class HeliosAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// �� ValidateClientAuthentication() �����л�ȡ�ͻ��˵� client_id �� client_secret ������֤
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetBasicCredentials(out clientId, out clientSecret);

            // TODO: ���ú��ҵ���߼���clientId�Լ�clientSecret �������У��
            if (clientId == "1234" && clientSecret == "5678")
            {
                context.Validated(clientId);
            }

            await base.ValidateClientAuthentication(context);
        }

        /// <summary>
        /// �� GrantClientCredentials() �����жԿͻ��˽�����Ȩ������Ȩ���ܷ� access token
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
            //TODO:���ú�̨�ĵ�¼������֤�û���������

            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            context.Validated(ticket);

            await base.GrantResourceOwnerCredentials(context);
        }
    }
}