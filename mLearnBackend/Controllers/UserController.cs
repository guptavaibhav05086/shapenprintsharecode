
using mLearnBackend.Domain;
using mLearnBackend.Helper;
using mLearnBackend.Models;
using SNPExtensons;
//using SNPExtensons;
//using ProjectExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace mLearnBackend.Controllers
{
    [Authorize(Roles = "Customer,Admin")]
    [RoutePrefix("api/customer")]
    public class UserController : ApiController
    {
        [Route("ScheduledTaskNotifications")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage ScheduledTaskNotifications()
        {
            ShareLearnService service = new ShareLearnService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                var result = service.ScheduledTaskNotifications();
                return Request.CreateResponse(HttpStatusCode.OK,result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("GetCustomerDataProfile")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUserData(string paramUserName)
        {
            ShareLearnService service = new ShareLearnService();
           //var result = "test";
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                ExtensionsUserData data = new ExtensionsUserData();
                var result = data.UpdateMasterFlags(paramUserName);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("UpdateUserData")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage UpdateUserData(string paramUserName)
        {
            ShareLearnService service = new ShareLearnService();
            //var result = "test";
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                ExtensionsUserData data = new ExtensionsUserData();
                var result = data.GetUserData(paramUserName);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("GetProductImages")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetProductImages(int proId)
        {
            ShareLearnService service = new ShareLearnService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                var result = service.GetProductImages(proId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        //SendEmailNotificationOrder(printers, null, "OrderNotification", "New Order To Accept", "printNoification", "printerBody.jpg",emailPrinter,"Accept New Order", "Accept Order");
        [Route("InviteFriend")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage InviteFriend(string email)
        {
           
            
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                List<string> frnd = new List<string> { email };
                ShareLearnService.SendEmailNotificationOrder(frnd, null, "InviteUser", "Welcome to shapeNprint", "printNoification", "https://drive.google.com/uc?export=view&id=1qDnXXAaXnxO9a2D-742bZbU78TlrZXEL", null, "Accept New Order", "Accept Order");
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("GetFlagValues")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetFlagValues(string  flagName)
        {
            ShareLearnService service = new ShareLearnService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                var result = service.GetFlagValues(flagName);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("generateorder")]
        [EnableCors("*", "*", "*")]
        [HttpPost]

        public string[] GenerateOrderId([FromBody]OrderDetails orderDetails)
        {
            string[] orders = new string[2];
            int transId;
            ShareLearnService service = new ShareLearnService();
            string orderId = service.GenerateOrder((orderDetails.OrderPrice.CalDiscountedTotal *100),out transId,orderDetails.UserId);
            int gorderId =service.CreateOrderDetails(orderDetails, transId);
            orders[0] = orderId;
            orders[1] = gorderId.ToString();
            return orders;

        }
        [Route("changeMobile")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage changeMobile(string email,string mobile)
        {
            ShareLearnService service = new ShareLearnService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                var orders = service.ChangeUserMobileNumber(email,mobile);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("getUserDetails")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage getUserDetails()
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                var orders = service.UserDeatials(userName);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        //[Route("updateUserCart")]
        //[EnableCors("*", "*", "*")]
        //[HttpGet]

        //public HttpResponseMessage updateUserCart(string email, string cart)
        //{
        //    ShareLearnService service = new ShareLearnService();
        //    //var userName = HttpContext.Current.User.Identity.Name;

        //    try
        //    {
        //        var orders = service.ChangeUserMobileNumber(email, mobile);
        //        return Request.CreateResponse(HttpStatusCode.OK, orders);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

        //    }

        //}
        [Route("getongoingorder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage GetOngoingOrder(string email, bool isAllOrdersRequired)
        {
            ShareLearnService service = new ShareLearnService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                var orders = service.GetOngoingOrdersCustomer(email, isAllOrdersRequired);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("downloadOngoingOrderFiles")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadOngoingOrders(string filename)
        {
            var responseStream = await AmazonS3Access.ReadFileDataAsync(filename, "createordercontent");
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;

            //Write the memory stream to HttpResponseMessage content
            response.Content = new StreamContent(responseStream);
            string contentDisposition = string.Concat("attachment; filename=", filename);
            response.Content.Headers.ContentDisposition =
                          ContentDispositionHeaderValue.Parse(contentDisposition);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            return response;
        }
        [Route("downloadDesigneCompletedFiles")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadDesigneCompletedFiles(string filename)
        {
            var responseStream = await AmazonS3Access.ReadFileDataAsync(filename, "finaldesignsourceandnormalfile");
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            filename = filename.Replace(" ", String.Empty);
            //Write the memory stream to HttpResponseMessage content
            response.Content = new StreamContent(responseStream);
            string contentDisposition = string.Concat("attachment; filename=", filename);
            response.Content.Headers.ContentDisposition =
                          ContentDispositionHeaderValue.Parse(contentDisposition);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            return response;
        }

        [Route("generateorder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public string GenerateOrderId(double amount,string userRegisId)
        {
            int transId;
            ShareLearnService service = new ShareLearnService();
            string orderId = service.GenerateOrder(amount,out transId, userRegisId);
            return orderId;

        }
        [Route("updateaddress")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage UpdateCreateAddress(AddressRequest address )
        {
            ShareLearnService service = new ShareLearnService();

            var result= service.addEditUserAddress(address);
            return Request.CreateResponse(HttpStatusCode.OK, result);

        }
        [Route("saveusercart")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage SaveUserCart(UserCartModel cart)
        {
            ShareLearnService service = new ShareLearnService();

            var result = service.UpdateCart(cart);
            return Request.CreateResponse(HttpStatusCode.OK, result);

        }
        [Route("getusercart")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetUserCart(string userEmail)
        {
            ShareLearnService service = new ShareLearnService();

            var result = service.FetchCart(userEmail);
            return Request.CreateResponse(HttpStatusCode.OK, result);

        }
        [Route("getaddress")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetAddress(string userId)
        {
            ShareLearnService service = new ShareLearnService();
            var listAdd = service.getUserAddress(userId);
            return Request.CreateResponse(HttpStatusCode.OK,listAdd);

        }
        [Route("deleteAddress")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage DeleteAddress(string userId,int addId)
        {
            ShareLearnService service = new ShareLearnService();
            var listAdd = service.DeleteAddress(userId,addId);
            return Request.CreateResponse(HttpStatusCode.OK, listAdd);

        }
        [Route("acceptDesign")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> FinishDesignerOrder(int? orderId)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var result =await  service.FinishDesignCustomer(orderId,false);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }

        [Route("acceptUserValid")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage AcceptUserValid(string orderId)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var result = service.AcceptUserValid(orderId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        [Route("sendNotificationPrinter")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> sendNotificationPrinter(int? orderId)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var result = await service.FinishDesignCustomer(orderId,true);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        //[Route("generateorder")]
        //[EnableCors("*", "*", "*")]
        //[HttpGet]

        //public string GenerateOrderId(double amount)
        //{
        //    ShareLearnService service = new ShareLearnService();
        //    string orderId = service.GenerateOrder(amount);
        //    return orderId;

        //}
        [Route("validateTransaction")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage validateTransaction(string paymentId, string orderId, string signature)
        {
            ShareLearnService service = new ShareLearnService();
            bool validation = service.ValidatePayments(paymentId, orderId, signature);
            if (validation)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Transaction validation successful");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error in transaction Validation");

                // return InternalServerError()
            }

        }
        [Route("updateFailedTransaction")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage updateFailedTransaction( int orderId)
        {
            try
            {
                ShareLearnService service = new ShareLearnService();
                bool validation = service.UpdateFailedTransctions(orderId);
                if (validation)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Transaction updated successful");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error in transaction updation");

                    // return InternalServerError()
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message + ex.InnerException;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err);

            }
           

        }
        [Route("sendNotification")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public async Task<HttpResponseMessage> sendNotification(string orderId)
        {
            try
            {
                ShareLearnService service = new ShareLearnService();
                var userId = HttpContext.Current.User.Identity.Name;
                bool validation = await service.GenerateNotifications(orderId);
                if (validation)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Transaction validation successful");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error in transaction Validation");

                    // return InternalServerError()
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message + ex.InnerException;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
            


        }
        [Route("rescheduleMeeting")]
        [EnableCors("*", "*", "*")]
        [HttpPost]

        public HttpResponseMessage RescheduleMeeting([FromUri] int orderId,[FromBody] MeetingDetails details)
        {
            ShareLearnService service = new ShareLearnService();
            bool validation = service.RescheduleMeetingByCustomer(orderId,details);
            if (validation)
            {
                return Request.CreateResponse(HttpStatusCode.OK, validation);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error in transaction Validation");

                // return InternalServerError()
            }

        }
        [Route("rescheduleMeetingNotifications")]
        [EnableCors("*", "*", "*")]
        [HttpPost]

        public HttpResponseMessage RescheduleMeetingNotification([FromUri]int orderId, [FromBody]MeetingDetails details)
        {
            ShareLearnService service = new ShareLearnService();
            bool validation = service.RescheduleMeetingNotifictionByCustomer(orderId,details);
            if (validation)
            {
                return Request.CreateResponse(HttpStatusCode.OK, validation);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error in transaction Validation");

                // return InternalServerError()
            }

        }
        [AllowAnonymous]
        [Route("GenerateOTP")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GenerateOTP(string email, string phoneNumber)
        {
            ShareLearnService service = new ShareLearnService();

            try
            {
                using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
                {
                    var user = (from a in dbConetxt.AspNetUsers where a.Email == email select a).FirstOrDefault();
                    string userId = user.Id;
                    service.GenerateOTP(userId, phoneNumber);
                    
                }
                  
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [AllowAnonymous]
        [Route("ValidateOTP")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage ValidateOTP(string email, string OTP)
        {
            ShareLearnService service = new ShareLearnService();
            bool result = false;
            try
            {
                using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
                {
                    var user = (from a in dbConetxt.AspNetUsers where a.Email == email select a).FirstOrDefault();
                    string userId = user.Id;
                    result = service.VerifyOTP( userId, OTP);

                }
               
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        [AllowAnonymous]
        [Route("GenerateOTPEmail")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GenerateOTPEmail(string email)
        {
            ShareLearnService service = new ShareLearnService();
            Random rnd = new Random();
            int OTP = rnd.Next(1001, 9999);

            try
            {
                using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
                {
                    var user = (from a in dbConetxt.Customers where a.emailId == email select a).FirstOrDefault();
                    user.verifyEmal = OTP.ToString();
                    //string userId = user.registrationId;
                    ShareLearnService.SendEmailOTP(email, OTP.ToString(), "VerifyEmailOTP", "Verification Mail", "verifyEmail",5);
                    dbConetxt.SaveChanges();
                    //service.GenerateOTP(userId, phoneNumber);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [AllowAnonymous]
        [Route("ValidateOTPEmail")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage ValidateOTPEmail(string email, string OTP)
        {
            ShareLearnService service = new ShareLearnService();
            bool result = false;
            try
            {
                using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
                {
                    var user = (from a in dbConetxt.Customers where a.emailId == email select a).FirstOrDefault();
                    string verifyOTP = user.verifyEmal;
                    if(verifyOTP == OTP)
                    {
                        var cust = (from b in dbConetxt.AspNetUsers
                                    where b.Email == email
                                    select b).FirstOrDefault();
                        cust.EmailConfirmed = true;
                        dbConetxt.SaveChanges();
                        result = true;

                    }
                    

                }

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }

        [AllowAnonymous]
        [Route("uploadimage")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
           Guid filesId=Guid.NewGuid();
            string fileName = "";

            try
            {

                var httpRequest = HttpContext.Current.Request;
                string name = HttpContext.Current.User.Identity.Name;
                string extension;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                   


                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 20; //Size = 20 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png", ".jpeg", ".pdf", ".doc", ".docx" };
                        IList<string> AllowedResumeFileExtensions = new List<string> { ".pdf", ".doc", ".docx" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.,.jpeg,.pdf,.doc,.docx");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 20 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                           
                            var stream = postedFile.InputStream;
                            bool result;
                            if (AllowedResumeFileExtensions.Contains(extension))
                            {
                                fileName = httpRequest.Files.Keys[0] + filesId + extension;
                                //fileName = httpRequest.Files.Keys[0] ;
                                result = await AmazonS3Access.UploadKYCData(stream, fileName, "createordercontent");

                            }
                            else
                            {
                                fileName = httpRequest.Files.Keys[0] + filesId + extension;
                                //fileName = httpRequest.Files.Keys[0];
                                result = await AmazonS3Access.UploadKYCData(stream, fileName, "createordercontent");
                            }                           

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateResponse(HttpStatusCode.Created, fileName); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("some Message");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }


    }
}
