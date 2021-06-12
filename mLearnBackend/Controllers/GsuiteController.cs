using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using mLearnBackend.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading;
using Google.Apis.Services;

namespace mLearnBackend.Controllers
{
    [RoutePrefix("api/gsuite")]
    public class GsuiteController : ApiController
    {
        private CancellationToken cancellationToken;

        [AllowAnonymous]
        [Route("guestregister")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public string GetAuthUrl()
        {
            return "demo";  
        }

        [AllowAnonymous]
        [Route("GetAccessToken")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public async Task<string> GetAccessToken(string code="",string error="")
        {
            
            GsuitService service = new GsuitService();
            service.getEvents();
            return "demo";
        }
    }
}
