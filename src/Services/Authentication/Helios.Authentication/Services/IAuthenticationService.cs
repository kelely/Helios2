using Helios.Authentication.Domain;

namespace Helios.Authentication.Services
{
    public interface IAuthenticationService
    {
        Client FindClient(string clientId);

        bool ValidateUser(string username, string password);
        bool AddRefreshToken(RefreshToken token);
        RefreshToken FindRefreshToken(string hashedTokenId);
        bool RemoveRefreshToken(string hashedTokenId);
    }
}
