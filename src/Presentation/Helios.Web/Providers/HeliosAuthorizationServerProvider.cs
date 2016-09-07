using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Helios.Authentication.Domain;
using Helios.Authentication.Services;
using Helios.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
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
            // NOTE: 
            //   在方法内可以通过针对context 设置详细的错误信息，这些详细的错误信息会返回给客户端，例如
            //      context.SetError("invalid_clientId", "请求中并不包含 client_id 信息.");
            //   默认并不输出详细的错误信息给客户端

            string clientId, clientSecret;

            // 首先从 Basic Authorization 中获取客户端信息，获取不到就从POST数据中获取客户端信息
            // 客户端信息保存在 Basic Authorization 更加符合 OAuth 2.0 规范，增加对 POST 的支持
            // 仅仅是为了使用 Postman 进行调试时方便，客户端在传输客户端信息时，应该首选 Basic Authorization 方式。
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
                context.TryGetFormCredentials(out clientId, out clientSecret);

            if (string.IsNullOrEmpty(clientId))
                return;

            var authService = EngineContext.Current.Resolve<IAuthenticationService>();
            var client = authService.FindClient(clientId);

            if (client == null)
                return;

            // TODO:只有当客户端类型是本地验证模式???
            if (client.ApplicationType == ApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                    return;

                // 数据库中客户端信息的密钥是经过Hash加密保存的，所以此处需要把客户端传过来的密钥进行Hash加密
                if (client.Secret != clientSecret.Hashed())
                    return;
            }

            // 客户端被禁用
            if (!client.Active)
                return;

            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated(clientId);

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

            var props = new AuthenticationProperties(new Dictionary<string, string> {
                {"as:client_id", context.ClientId ?? string.Empty },
            });
            var ticket = new AuthenticationTicket(oAuthIdentity, props);


            context.Validated(ticket);

            await base.GrantClientCredentials(context);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // allowedOrigin 默认为 '*'
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null)
                allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var authService = EngineContext.Current.Resolve<IAuthenticationService>();
            if (!authService.ValidateUser(context.UserName, context.Password))
                return;

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            identity.AddClaim(new Claim("sub", context.UserName));

            var props = new AuthenticationProperties(new Dictionary<string, string> {
                {"as:client_id", context.ClientId ?? string.Empty },
                {"userName", context.UserName}
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

            await base.GrantResourceOwnerCredentials(context);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newClaim = newIdentity.Claims.FirstOrDefault(c => c.Type == "newClaim");
            if (newClaim != null)
            {
                newIdentity.RemoveClaim(newClaim);
            }
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 通过覆盖父类的此方法，达到在输出结果中增加附加属性的目的
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            // NOTE: 这些东西应该不是必要的
            //foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            //{
            //    context.AdditionalResponseParameters.Add(property.Key, property.Value);
            //}

            await base.TokenEndpoint(context);
        }
    }
}