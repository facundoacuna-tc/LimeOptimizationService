using LOS.Common.Constants;
using LOS.Common.Entities;
using LOS.Common.Hash;
using LOS.Common.Mapping;
using LOS.Common.Requests;
using LOS.Common.Responses;
using LOS.Data.Context;
using LOS.Services.Interfaces;

namespace LOS.Service
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext context;

        public AccountService(AppDbContext context)
        {
            this.context = context;
        }

        public LoginResponse Login(AccountRequest req)
        {
            LoginResponse response = null;

            var users = context.User.Where(u => u.UserName == req.Username && u.Deleted == false).ToList();

            //var u = context.User.Where(u => u.UserName == req.Username && u.Deleted == false).ToList().FirstOrDefault();
            //if (u != null) 
            //{

            //    //var salt1 = Hasher.GenerateSalt();
            //    var hashedInDb = Hasher.HashPassword("P5kdXCXkIGUF4gAOLzhCcA==", req.Password);

            //    var hashedInRequest = Hasher.HashPassword(u.Salt, req.Password);


            //}

            //var p = Hasher.HashPassword(u.Salt, u.Password) == "admin";

            var user = context.User
                .Where(u => u.UserName == req.Username && u.Deleted == false)
                .ToList()   // Because Hasher would otherwise be evaluated in the generated SQL expression.
                .SingleOrDefault(u => Hasher.HashPassword(u.Salt, req.Password) == u.Password);

            if (user != null)
            {
                var ts = GetEpoch();
                user.Login(ts);
                context.SaveChanges();
                response = user.CreateMapped<LoginResponse>();
            }

            return response;
        }

        public LoginResponse Refresh(string refreshToken)
        {
            LoginResponse response = null;

            var user = context.User
                .Where(u => u.RefreshToken == refreshToken && u.Deleted == false).SingleOrDefault();

            if (user != null)
            {
                var ts = GetEpoch();

                // Refresh token expires 90 days after when user logged in, thus ExpiresOn + (90 - 1) days
                if (user.ExpiresOn + (Constants.REFRESH_VALID_DAYS - 1) * Constants.ONE_DAY_IN_SECONDS > ts)
                {
                    user.Login(ts);
                    context.SaveChanges();
                    response = user.CreateMapped<LoginResponse>();
                }
            }

            return response;
        }

        public bool VerifyAccount(string token)
        {
            var user = context.User.Where(u => u.AccessToken == token).SingleOrDefault();
            var ts = GetEpoch();
            bool ok = (user?.ExpiresOn ?? 0) > ts;

            return ok;
        }

        public void Logout(string token)
        {
            var user = context.User.Single(u => u.AccessToken == token);
            user.Logout();
            context.SaveChanges();
        }

        public (bool ok, int id) CreateAccount(AccountRequest req)
        {
            bool ok = false;
            int id = -1;

            var existingUsers = context.User.Where(u => u.UserName == req.Username && !u.Deleted);

            if (existingUsers.Count() == 0)
            {
                var salt = Hasher.GenerateSalt();
                var hashedPassword = Hasher.HashPassword(salt, req.Password);
                var user = new User() { UserName = req.Username, Password = hashedPassword, Salt = salt };
                context.User.Add(user);
                context.SaveChanges();
                ok = true;
                id = user.Id;
            }

            return (ok, id);
        }

        public bool ChangeUsernameAndPassword(string token, AccountRequest req)
        {
            bool ok = false;
            var existingUsers = context.User.Where(u => u.UserName == req.Username && !u.Deleted);

            if (existingUsers.Count() == 0 || existingUsers.First().UserName == req.Username)
            {
                var user = context.User.Single(u => u.AccessToken == token);
                user.Logout();
                user.Salt = Hasher.GenerateSalt();
                user.UserName = req.Username ?? user.UserName;
                user.Password = Hasher.HashPassword(user.Salt, req.Password);
                context.SaveChanges();
                ok = true;
            }

            return ok;
        }

        public void DeleteAccount(string token)
        {
            var user = context.User.Single(u => u.AccessToken == token);
            user.Logout();
            user.Deleted = true;
            context.SaveChanges();
        }

        public void ExpireToken(string token)
        {
            var ts = GetEpoch();
            var user = context.User.SingleOrDefault(u => u.AccessToken == token);
            user.ExpiresOn = ts - Constants.ONE_DAY_IN_SECONDS;
            context.SaveChanges();
        }

        public void ExpireRefreshToken(string token)
        {
            var ts = GetEpoch();
            var user = context.User.SingleOrDefault(u => u.AccessToken == token);
            user.ExpiresOn = ts - Constants.REFRESH_VALID_DAYS * Constants.ONE_DAY_IN_SECONDS;
            context.SaveChanges();
        }

        private long GetEpoch()
        {
            var ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;   // We declare the epoch to be 1/1/1970.

            return ts;
        }

        public User GetUser(string token)
        {
            throw new NotImplementedException();
        }
    }
}