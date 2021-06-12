using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Google.Apis.Util.Store;

namespace mLearnBackend.Domain
{
    public class AppFlowMetadata : FlowMetadata
    {
        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "132073171968-2q1v9rrj5i5st6j29q70b790jnnrerlf.apps.googleusercontent.com",
                    ClientSecret = "xqiBPXCiSI9I3jGI1_nfF0Rz"
                    //ClientId = "240551158611-k8e10bsrt7jg6u4k23m5547uf8utou1t.apps.googleusercontent.com",
                    //ClientSecret = "9xlaNHROLjxVnjm641cLdNPG"
                },
                Scopes = new[] { CalendarService.Scope.CalendarReadonly,CalendarService.Scope.Calendar },
                //DataStore = new FileDataStore("Drive.Api.Auth.Store")
                DataStore = new TokenStore()
            });

        public override string GetUserId(Controller controller)
        {
            // In this sample we use the session to store the user identifiers.
            // That's not the best practice, because you should have a logic to identify
            // a user. You might want to use "OpenID Connect".
            // You can read more about the protocol in the following link:
            // https://developers.google.com/accounts/docs/OAuth2Login.
            var user = controller.Session["user"];
            if (user == null)
            {
                user = "noreply@shapenprint.com";
                controller.Session["user"] = user;
                
            }
            return user.ToString();

        }

        

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}