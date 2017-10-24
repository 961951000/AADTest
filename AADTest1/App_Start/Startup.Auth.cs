using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using AADTest1.Authorization;

namespace AADTest1
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = AuthenticationHelper.ClientId,
                    Authority = AuthenticationHelper.Authority,
                    PostLogoutRedirectUri = AuthenticationHelper.PostLogoutRedirectUri,
                    RedirectUri = AuthenticationHelper.RedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        //
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        //
                        AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                        AuthenticationFailed = OnAuthenticationFailed
                    }

                });
        }

        private static async Task<int> OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/Home/Error?message=" + context.Exception.Message);
            return await Task.FromResult(0);
        }

        private async Task<AuthenticationResult> OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
        {
            var code = context.Code;

            ClientCredential credential = new ClientCredential(AuthenticationHelper.ClientId, AuthenticationHelper.AppKey);
            string userObjectId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            AuthenticationContext authContext = new AuthenticationContext(AuthenticationHelper.Authority, new NaiveSessionCache(userObjectId, context.OwinContext.Environment["System.Web.HttpContextBase"] as HttpContextBase));

            // If you create the redirectUri this way, it will contain a trailing slash.  
            // Make sure you've registered the same exact Uri in the Azure Portal (including the slash).
            Uri uri = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));

            AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code, uri, credential, AuthenticationHelper.GraphResourceId);
            return result;
        }
    }
}