﻿using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNet.Authentication.OpenIdConnect;

namespace AureliaAspNetCoreAuth.Providers
{
    public class AuthenticationProvider : OpenIdConnectServerProvider
    {
        public override Task ValidateClientAuthentication(ValidateClientAuthenticationContext context)
        {
            if (context.ClientId == "AureliaNetAuthApp")
            {
                // Note: the context is marked as skipped instead of validated because the client
                // is not trusted (JavaScript applications cannot keep their credentials secret).
                context.Skipped();
            }

            else {
                // If the client_id doesn't correspond to the
                // intended identifier, reject the request.
                context.Rejected();
            }

            return Task.FromResult(0);
        }

        public override Task GrantResourceOwnerCredentials(GrantResourceOwnerCredentialsContext context)
        {
            var user = new { Id = "users-123", Email = "alex@123.com", Password = "AureliaNetAuth" };

            if (context.UserName != user.Email || context.Password != user.Password)
            {
                context.Rejected("Invalid username or password.");

                return Task.FromResult(0);
            }

            var identity = new ClaimsIdentity(OpenIdConnectDefaults.AuthenticationScheme);
            identity.AddClaim(ClaimTypes.NameIdentifier, user.Id, "id_token token");
            identity.AddClaim(ClaimTypes.Name, user.Email, "id_token token");

            context.Validated(new ClaimsPrincipal(identity));

            return Task.FromResult(0);
        }
    }
}
