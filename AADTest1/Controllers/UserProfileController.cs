﻿using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AADTest1.Helpers;
using AADTest1.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json;

// The following using statements were added for this sample.

namespace AADTest1.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        public async Task<ActionResult> Index()
        {
            //
            // Retrieve the user's name, tenantID, and access token since they are parameters used to query the Graph API.
            //
            UserProfile profile;

            try
            {
                string tenantId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                string userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                AuthenticationContext authContext = new AuthenticationContext(AuthenticationHelper.Authority, new NaiveSessionCache(userObjectId, HttpContext));
                ClientCredential credential = new ClientCredential(AuthenticationHelper.ClientId, AuthenticationHelper.AppKey);
                var result = await authContext.AcquireTokenSilentAsync(AuthenticationHelper.GraphResourceId, credential, new UserIdentifier(userObjectId, UserIdentifierType.UniqueId));

                //
                // Call the Graph API and retrieve the user's profile.
                //
                string requestUrl = string.Format(CultureInfo.InvariantCulture, AuthenticationHelper.GraphUserUrl, HttpUtility.UrlEncode(tenantId));
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                //
                // Return the user's profile in the view.
                //
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    profile = JsonConvert.DeserializeObject<UserProfile>(responseString);
                }
                else
                {
                    //
                    // If the call failed, then drop the current access token and show the user an error indicating they might need to sign-in again.
                    //
                    var todoTokens = authContext.TokenCache.ReadItems().Where(a => a.Resource == AuthenticationHelper.GraphResourceId);
                    foreach (TokenCacheItem tci in todoTokens)
                    {
                        authContext.TokenCache.DeleteItem(tci);
                    }

                    profile = new UserProfile
                    {
                        DisplayName = " ",
                        GivenName = " ",
                        Surname = " "
                    };
                    ViewBag.ErrorMessage = "UnexpectedError";
                }

                return View(profile);
            }
            catch (AdalException)
            {
                //
                // If the user doesn't have an access token, they need to re-authorize.
                //

                //
                // If refresh is set to true, the user has clicked the link to be authorized again.
                //
                if (Request.QueryString["reauth"] == "True")
                {
                    //
                    // Send an OpenID Connect sign-in request to get a new set of tokens.
                    // If the user still has a valid session with Azure AD, they will not be prompted for their credentials.
                    // The OpenID Connect middleware will return to this controller after the sign-in response has been handled.
                    //
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties(), OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }

                //
                // The user needs to re-authorize.  Show them a message to that effect.
                //
                profile = new UserProfile
                {
                    DisplayName = " ",
                    GivenName = " ",
                    Surname = " "
                };
                ViewBag.ErrorMessage = "AuthorizationRequired";

                return View(profile);
            }
        }
    }
}