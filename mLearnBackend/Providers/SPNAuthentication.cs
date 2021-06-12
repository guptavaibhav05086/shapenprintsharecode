using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SNPExtensons;

namespace mLearnBackend.Providers
{
    public class SPNAuthentication : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
   HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentUser = HttpContext.Current.User.Identity.Name;
            var result = ExtensionsUserData.ValidateRequest(currentUser);
            if(result == false)
            {
                var res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                var taskCompletion = new TaskCompletionSource<HttpResponseMessage>();
                taskCompletion.SetResult(res);
                return taskCompletion.Task;
            }
            else
            {
                return  base.SendAsync(request, cancellationToken);
            }
            
        }

       
    }
}