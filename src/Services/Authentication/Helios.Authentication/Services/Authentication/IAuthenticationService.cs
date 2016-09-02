using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helios.Domain.Authentication;

namespace Helios.Services.Authentication
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
