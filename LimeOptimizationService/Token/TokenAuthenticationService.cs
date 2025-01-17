﻿using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using LOS.Services.Interfaces;
using LOS.Common.Constants;
using LOS.Common.ExtensionMethods;

namespace LimeOptimizationService.Token
{
    public class TokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
    }

    public class TokenAuthenticationService : AuthenticationHandler<TokenAuthenticationSchemeOptions>
    {
        private readonly IAccountService acctSvc;

        public TokenAuthenticationService(
            IAccountService accountService,
            IOptionsMonitor<TokenAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            acctSvc = accountService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Task<AuthenticateResult> result = Task.FromResult(AuthenticateResult.Fail("Not authorized."));

            // Authentication confirms that users are who they say they are.
            // Authorization gives those users permission to access a resource.

            if (Request.Headers.ContainsKey(Constants.AUTHORIZATION))
            {
                
                var token = Request.Headers[Constants.AUTHORIZATION][0]
                    .RightOf(Constants.TOKEN_PREFIX).Trim();

                bool verified = acctSvc.VerifyAccount(token);

                if (verified)
                {
                    var request = Request.HttpContext.Request;
                    var basePath = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}";

                    // If verified, optionally add some claims about the user...
                    var claims = new[]
                    {
                        new Claim("token", token),
                    };

                    // Generate claimsIdentity on the name of the class:
                    var claimsIdentity = new ClaimsIdentity(claims, nameof(TokenAuthenticationService));

                    // Generate AuthenticationTicket from the Identity
                    // and current authentication scheme.
                    var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

                    result = Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }

            return result;
        }
    }
}
