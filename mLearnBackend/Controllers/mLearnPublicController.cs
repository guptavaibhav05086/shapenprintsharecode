using mLearnBackend.Domain;
using mLearnBackend.Helper;
using mLearnBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace mLearnBackend.Controllers
{
    [RoutePrefix("api/Public")]
    public class mLearnPublicController : ApiController
    {

        [Route("dunzoWebHook")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage DunzoWebHook(DunzoWebHook payload)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                var result = service.UpdateDeliveryStatus(payload.task_id,payload.state);
                if (result == true)
                    return Request.CreateResponse(HttpStatusCode.OK, result);

                else
                    throw new Exception("Link is expired.Contact customer Support");


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        [Route("resendverifyemail")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage ResendVerifyEmail(string email, string guid, string role)
        {
            Guid emailVerify = Guid.NewGuid();
            int userRole = 0;
            if (role == Constants.role_Designer)
            {
                userRole = 3;

                using (mLearnDBEntities dbContext = new mLearnDBEntities())
                {
                    var item = (from a in dbContext.Designers where a.emailId == email select a).FirstOrDefault();
                    item.verifyEmal = emailVerify.ToString();
                    dbContext.SaveChanges();
                }
            }
            else if (role==Constants.role_Printer)
            {
                userRole = 4;
                using (mLearnDBEntities dbContext = new mLearnDBEntities())
                {
                    var item = (from a in dbContext.Printers where a.emailId == email select a).FirstOrDefault();
                    item.verifyEmal = emailVerify.ToString();
                    dbContext.SaveChanges();
                }
            }
            try
            {
                if(userRole !=0)
                ShareLearnService.SendEmail(email, emailVerify.ToString(), "VerifyEmail", "Verification Mail", "verifyEmail", userRole);
                else
                {
                    throw new Exception("role Not found");
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
        //ShareLearnService.SendEmail(model.Email, emailVerify.ToString(), "VerifyEmail", "Verification Mail", "verifyEmail",role);
        [Route("verifyemail")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage VerifyEmail(string userId, string guid,int role)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                var result =  service.VerifyEmail(userId, guid,role);
                if (result == true)
                    return Request.CreateResponse(HttpStatusCode.OK, result);

                else
                    throw new Exception("Link is expired.Contact customer Support");


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        [Route("GenerateOTP")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public  HttpResponseMessage GenerateOTP(string userId, string phoneNumber)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
               var result =  service.GenerateOTP(userId, phoneNumber);
                return Request.CreateResponse(HttpStatusCode.OK,result);


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        [Route("ValidateOTP")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage ValidateOTP(string userId, string OTP)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                var result = service.VerifyOTP(OTP, userId);
                return Request.CreateResponse(HttpStatusCode.OK,result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        [Route("getFile")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetGSTCertificate(string filename)
        {
            var responseStream = await AmazonS3Access.ReadFileDataAsync(filename, "userkycdata");
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
        [Route("validateTransaction")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage validateTransaction(string paymentId, string orderId, string signature)
        {
            ShareLearnService service = new ShareLearnService();
            bool validation = service.ValidatePayments(paymentId,orderId,signature);
            if(validation)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Transaction validation successful"); 
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,"Error in transaction Validation");

               // return InternalServerError()
            }

        }
        [AllowAnonymous]
        [Route("guestregister")]
        [EnableCors("*","*","*")]
        [HttpPost]
        public void GuestRegister(GuestUserModel model)
        {
            using(mLearnDBEntities dbConetx = new mLearnDBEntities())
            {
                GuestLogin login = new GuestLogin();
                login.email = model.Email;
                login.phonenumber = model.PhoneNumber;
                dbConetx.GuestLogins.Add(login);
                dbConetx.SaveChanges();
                
            }
        }
        [AllowAnonymous]
        [Route("userfeedback")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public IHttpActionResult UserFeedback(FeedbackModel model)
        {
            using (mLearnDBEntities dbConetx = new mLearnDBEntities())
            {

                UserFeedback feedback = new UserFeedback();
                feedback.feedback = model.feedback;
                feedback.courseId = model.courseId;
                feedback.starRating = model.starRating;
                feedback.userId = model.userId;
                dbConetx.UserFeedbacks.Add(feedback);
                dbConetx.SaveChanges();

            }
            mLearnServices service = new mLearnServices();
            var resposne = service.GetUserFeedback(model.courseId);
            return Ok(resposne);
        }
        [AllowAnonymous]
        [Route("getuserfeedback")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public IHttpActionResult GetUserFeedback(int courseId)
        {
            // List<FeedbackModel> resposne = null;
            mLearnServices service = new mLearnServices();
            var resposne = service.GetUserFeedback(courseId);
                return Ok(resposne);
        }
        [AllowAnonymous]
        [Route("resetPassword")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public IHttpActionResult ResetPassword(string email)
        {
            try
            {
                mLearnServices service = new mLearnServices();
                var message = service.ResetPassword(email);
                return Ok(message);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
           
        }
        [AllowAnonymous]
        [Route("GetStates")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public IHttpActionResult GetStates()
        {
            try
            {
                ShareLearnService service = new ShareLearnService();
                var state = service.GetStates();
                return Ok(state);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }

        }

        [AllowAnonymous]
        [Route("TestAWS")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task TestAWS()
        {
            try
            {
                await AmazonS3Access.WritingAnObjectAsync();
                //return Task.compe;
            }
            catch (Exception ex)
            {

              
            }

        }
    }
}
