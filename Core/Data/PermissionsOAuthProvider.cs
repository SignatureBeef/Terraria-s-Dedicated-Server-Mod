using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using TDSM.Core.Data.Management;
using OTA.Logging;
using OTA.Web;
using System.Security.Claims;
using TDSM.Core.Net.Web;

namespace TDSM.Core.Data
{
    public class PermissionsOAuthProvider : Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerProvider
    {
        //            public override async System.Threading.Tasks.Task ValidateClientAuthentication(Microsoft.Owin.Security.OAuth.OAuthValidateClientAuthenticationContext context)
        //            {
        //                await Task.FromResult(context.Validated());
        //                //                return base.ValidateClientAuthentication(context);
        //            }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(Microsoft.Owin.Security.OAuth.OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            if (String.IsNullOrEmpty(context.UserName))
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                context.Rejected();
                return;
            }

            if (OTA.Protection.IpLimiting.Register(context.Request.RemoteIpAddress, ServerManager.MaxRequestsPerLapse, ServerManager.RequestLockoutDuration))
            {
                //Prevent console spamming
                if (OTA.Protection.IpLimiting.GetJustLockedOut(context.Request.RemoteIpAddress))
                {
                    ProgramLog.Web.Log("API client reached request limit for user/ip {0}", context.UserName, context.Request.RemoteIpAddress);
                }

                context.SetError("request_limit", "You have reached the service limit");
                context.Rejected();
                return;
            }

            var user = await APIAccountManager.FindByNameAsync(context.UserName);
            if (user != null && user.ComparePassword(context.Password))
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

                //Load permissions for user
                foreach (var role in await APIAccountManager.GetRolesForAccount(user.Id))
                {
                    identity.AddClaim(new Claim(role.Type, role.Value));
                    //                    identity.AddClaim(new Claim(ClaimTypes.Role, "player"));
                }

                //                    var ticket = new AuthenticationTicket(identity, new AuthenticationProperties()
                //                        {
                //                            IsPersistent = true,
                //                            IssuedUtc = DateTime.UtcNow
                //                        });
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                context.Rejected();
            }
        }
    }
}

