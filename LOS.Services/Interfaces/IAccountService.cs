using LOS.Common.Entities;
using LOS.Common.Requests;
using LOS.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Services.Interfaces
{
    public interface IAccountService
    {
        LoginResponse Login(AccountRequest req);
        LoginResponse Refresh(string refreshToken);
        (bool ok, int id) CreateAccount(AccountRequest req);
        bool VerifyAccount(string token);
        User GetUser(string token);
        void Logout(string token);
        void DeleteAccount(string token);
        bool ChangeUsernameAndPassword(string token, AccountRequest req);
        void ExpireToken(string token);
        void ExpireRefreshToken(string token);
    }
}
