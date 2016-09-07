using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Helios.Authentication.Domain;

namespace Helios.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private static readonly IDictionary<string, RefreshToken> _refreshTokens = new Dictionary<string, RefreshToken>();

        private static readonly IList<Client> _clients = new List<Client>
        {
            new Client {Id = 1, Key = "client#1", Secret = "secret#1", Name = "安卓客户端", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.NativeConfidential},
            new Client {Id = 2, Key = "client#2", Secret = "secret#2", Name = "IOS客户端", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.NativeConfidential},
            new Client {Id = 3, Key = "client#3", Secret = "secret#3", Name = "Web商户管理端", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.JavaScript},
            new Client {Id = 4, Key = "client#4", Secret = "secret#4", Name = "客户端#4", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.NativeConfidential},
            new Client {Id = 5, Key = "client#5", Secret = "secret#5", Name = "客户端#5", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.NativeConfidential},
            new Client {Id = 6, Key = "client#6", Secret = "secret#6", Name = "客户端#6", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.NativeConfidential},
            new Client {Id = 7, Key = "client#7", Secret = "secret#7", Name = "客户端#7", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.NativeConfidential},
            new Client {Id = 8, Key = "client#8", Secret = "secret#8", Name = "客户端#8", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.NativeConfidential},
            new Client {Id = 9, Key = "client#9", Secret = "secret#9", Name = "客户端#9", Active = true, AllowedOrigin = "*", RefreshTokenLifeTime = 1440, ApplicationType = ApplicationTypes.NativeConfidential},
        };

        static AuthenticationService()
        {
            foreach (var client in _clients)
            {
                client.Secret = GetClientSecretHashed(client.Secret);
            }
        }

        private static string GetClientSecretHashed(string secret)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(secret);
            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public Client FindClient(string clientId)
        {
            return _clients.FirstOrDefault(c => c.Key == clientId);
        }

        public bool ValidateUser(string username, string password)
        {
            return username == "博客园团队" && password == "cnblogs.com";
        }

        public bool AddRefreshToken(RefreshToken token)
        {
            var key = $"Helios.RefreshToken.{token.Token}";

            if(_refreshTokens.ContainsKey(key))
                _refreshTokens.Remove(key);

            _refreshTokens.Add(key, token);

            return _refreshTokens.ContainsKey(key);
        }

        public RefreshToken FindRefreshToken(string hashedTokenId)
        {
            var key = $"Helios.RefreshToken.{hashedTokenId}";
            if (!_refreshTokens.ContainsKey(key))
                return null;

            return _refreshTokens[key];
        }

        public bool RemoveRefreshToken(string hashedTokenId)
        {
            var key = $"Helios.RefreshToken.{hashedTokenId}";
            if (_refreshTokens.ContainsKey(key))
                _refreshTokens.Remove(key);

            return _refreshTokens.ContainsKey(key);
        }
    }
}
