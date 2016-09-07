using System.ServiceModel;
using Helios.Authentication.Domain;

namespace Helios.Authentication.Services
{
    [ServiceContract]
    public interface IAuthenticationService
    {
        [OperationContract]
        Client FindClient(string clientId);

        [OperationContract]
        bool ValidateUser(string username, string password);

        [OperationContract]
        bool AddRefreshToken(RefreshToken token);

        [OperationContract]
        RefreshToken FindRefreshToken(string hashedTokenId);

        [OperationContract]
        bool RemoveRefreshToken(string hashedTokenId);
    }
}
