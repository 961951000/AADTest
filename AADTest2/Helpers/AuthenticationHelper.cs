using System.Globalization;
using Microsoft.Azure;

namespace AADTest2.Helpers
{
    public class AuthenticationHelper
    {
        //
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The App Key is a credential used to authenticate the application to Azure AD.  Azure AD supports password and certificate credentials.
        // The Metadata Address is used by the application to retrieve the signing keys used by Azure AD.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        // The Post Logout Redirect Uri is the URL where the user will be redirected after they sign out.
        //
        public static readonly string ClientId = CloudConfigurationManager.GetSetting("ida:ClientId");
        public static readonly string AppKey = CloudConfigurationManager.GetSetting("ida:AppKey");
        public static readonly string AadInstance = CloudConfigurationManager.GetSetting("ida:AADInstance");
        public static readonly string Tenant = CloudConfigurationManager.GetSetting("ida:Tenant");
        public static readonly string RedirectUri = CloudConfigurationManager.GetSetting("ida:RedirectUri");
        public static readonly string PostLogoutRedirectUri = CloudConfigurationManager.GetSetting("ida:PostLogoutRedirectUri");
        public static readonly string Authority = string.Format(CultureInfo.InvariantCulture, AadInstance, Tenant);

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        public static readonly string GraphResourceId = CloudConfigurationManager.GetSetting("ida:GraphResourceId");
        public static readonly string GraphUserUrl = CloudConfigurationManager.GetSetting("ida:GraphUserUrl");

        public static readonly string ClaimsSchemas = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    }
}