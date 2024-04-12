using System.Security.Claims;
using LOS.Common.Requests;
using LOS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LimeOptimizationService.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpGet]
        public ActionResult Version()
        {
            return Ok(new { Version = "1.00" });
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult Login([FromBody] AccountRequest req)
        {
            var resp = _accountService.Login(req);

            var ret = resp == null ? (ActionResult)Unauthorized("User not found.") : Ok(resp);

            return ret;
        }

        [AllowAnonymous]
        [HttpPost("Refresh/{refreshToken}")]
        public ActionResult Refresh(string refreshToken)
        {
            var resp = _accountService.Refresh(refreshToken);
            var ret = resp == null ? (ActionResult)Unauthorized("User not found.") : Ok(resp);

            return ret;
        }

        
        [HttpPost("Logout")]
        public ActionResult Logout()
        {
            var token = GetToken();
            _accountService.Logout(token);

            return Ok();
        }

        [Authorize]
        [HttpPost()]
        public ActionResult CreateAccount(AccountRequest req)
        {
            ActionResult ret;

            var res = _accountService.CreateAccount(req);

            if (!res.ok)
            {
                ret = BadRequest($"Username {req.Username} already exists.");
            }
            else
            {
                ret = Ok(new { Id = res.id });
            }

            return ret;
        }

        /// <summary>
        /// A user can only delete their own account.  
        /// This logs out the user.
        /// </summary>
        [Authorize]
        [HttpDelete()]
        public ActionResult DeleteAccount()
        {
            ActionResult ret = Ok();

            var token = GetToken();
            _accountService.DeleteAccount(token);

            return ret;
        }

        /// <summary>
        /// A user can only change their own username and/or password.
        /// This logs out the user.
        /// </summary>
        [Authorize]
        [HttpPatch()]
        public ActionResult ChangeUsernameAndPassword(AccountRequest req)
        {
            ActionResult ret;

            var token = GetToken();
            bool ok = _accountService.ChangeUsernameAndPassword(token, req);
            ret = ok ? Ok() : BadRequest($"Username {req.Username} already exists.");

            return ret;
        }

        private string GetToken()
        {
            var claims = User.Identity as ClaimsIdentity;
            var token = claims.FindFirst("token").Value;

            return token;
        }

        // ---- for integration tests ----
#if DEBUG
        
        [Authorize]
        [HttpPost("expireToken")]
        public ActionResult ExpireToken()
        {
            var token = GetToken();
            _accountService.ExpireToken(token);

            return Ok();
        }

        [Authorize]
        [HttpPost("expireRefreshToken")]
        public ActionResult ExpireRefreshToken()
        {
            var token = GetToken();
            _accountService.ExpireRefreshToken(token);

            return Ok();
        }
#endif


    }
}
