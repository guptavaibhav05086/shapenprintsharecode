using mLearnBackend.Domain;
using mLearnBackend.Helper;
using mLearnBackend.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
    [Authorize(Roles = "Designer")]
    [RoutePrefix("api/designer")]
    public class DesignerController : ApiController
    {
        [Route("getdesignerDashboard")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetDesignerDashboard(string email)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var orders = service.DesignerDashboardDetails(userName);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        [Route("finishDesignerOrder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage FinishDesignerOrder(int? orderId)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var result = service.FinishDesign(orderId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }

        [Route("updateBankDetails")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage UpdateBankDetails(BankAccountDetailsModel bankDetails)
        {
            var userName=HttpContext.Current.User.Identity.Name;
            ShareLearnService service = new ShareLearnService();
            try
            {
                 service.CreateUpdateAccountDetails(bankDetails);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }

        [Route("getBankDetails")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetBankDetails()
        {
            var userName = HttpContext.Current.User.Identity.Name;
            ShareLearnService service = new ShareLearnService();
            try
            {
                var details =service.GetBankDetails(userName);
                return Request.CreateResponse(HttpStatusCode.OK,details);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }


        [Route("getOngoingOrder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetOngoingOrdersDesigner(string email,bool isAllOrdersRequired)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var orders=service.GetOngoingOrdersDesigner(userName, isAllOrdersRequired);
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
        public async Task<HttpResponseMessage> DownloadOngoingOrdersDesigner(string filename)
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

        [Route("getDesignerpayouts")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetDesignerPayOut()
        {
            try
            {
                var userName = HttpContext.Current.User.Identity.Name;
                AdminService service = new AdminService();
                var printer = service.GetDesignerPayOutDetails();
                var designerPayout = (from a in printer where a.VendorEmail == userName select a).ToList();
                //var userAddress = service.GeUserAddress();

                return Request.CreateResponse(HttpStatusCode.OK, designerPayout);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }

        }
        [Route("AcceptOrder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage AcceptOrder(string email, int orderId)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                service.AcceptDesignOrder(userName, orderId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        [Route("GenerateOTP")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GenerateOTP(string userId,string phoneNumber)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                service.GenerateOTP(userId,phoneNumber);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("getNotificationDetails")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetNotification(bool isProfessional)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                var response = service.GetNotificationDetailsDesigner(isProfessional);
                return Request.CreateResponse(HttpStatusCode.OK,response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }

        [Route("getProfile")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetProfile(string userId)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                var profile = service.GetDesignerProfile(userId);
                return Request.CreateResponse(HttpStatusCode.OK, profile);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }


        }
        //[Route("generateorder")]
        //[EnableCors("*", "*", "*")]
        //[HttpGet]

        //public string GenerateOrderId(double amount, string userRegisId)
        //{
        //    ShareLearnService service = new ShareLearnService();
        //    int transId;
        //    string orderId = service.GenerateOrder(amount,out transId, userRegisId);
        //    return orderId;

        //}
        [Route("generateorder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public string[] GenerateOrderId()
        {
            int transId;
            double amount;
            string[] items = new string[2];
            string userRegisId = HttpContext.Current.User.Identity.Name;
            ShareLearnService service = new ShareLearnService();

            string orderId = service.GenerateOrderServiceProviderFees(out transId, userRegisId, out amount);
            items[0] = orderId;
            items[1] = amount.ToString();
            return items;

        }
        //[Route("validateTransaction")]
        //[EnableCors("*", "*", "*")]
        //[HttpGet]

        //public HttpResponseMessage validateTransaction(string paymentId, string orderId, string signature)
        //{
        //    ShareLearnService service = new ShareLearnService();
        //    bool validation = service.ValidatePayments(paymentId, orderId, signature);
        //    if (validation)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, "Transaction validation successful");
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error in transaction Validation");

        //        // return InternalServerError()
        //    }

        //}

        [Route("postDesignerFinalFolder")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostDesignerFinalFolder([FromUri] int orderId, [FromUri] string type)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            System.IO.Stream MyStream;

            try
            {

                var httpRequest = HttpContext.Current.Request;
                string name = HttpContext.Current.User.Identity.Name;
                string MappedFileName = httpRequest.Form["FileNames"];
                string extension;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];


                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        extension = ext.ToLower();
                          var stream = postedFile.InputStream;
                            bool result;

                        //fileName = name.Split('@')[0] + "GSTCertificate" + extension;
                        string key = "FinalDesignFile-" + orderId.ToString() + postedFile.FileName;
                                result = await AmazonS3Access.UploadKYCData(stream, key, "finaldesignsourceandnormalfile");
                            if (result)
                            {
                                using (mLearnDBEntities dbContxt = new mLearnDBEntities())
                                {
                                var orderDetails = (from a in dbContxt.orderDetails
                                                    where a.id == orderId
                                                    select a).FirstOrDefault(); 
                                if(orderDetails != null)
                                {
                                    if (type == "source")
                                    {
                                        orderDetails.sourceCodeFilePath = MappedFileName;
                                    }
                                    else if (type=="normal")
                                    {
                                        orderDetails.finalDesignFilepath = MappedFileName;
                                    }
                                }                                   
                                    dbContxt.SaveChanges();

                                }
                            }

                        //}
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateResponse(HttpStatusCode.Created, message1); ;
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

        [Route("updateprofile")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public void UpdateDesignerProfile(DesignerProfileRequest request)
        {
            ShareLearnService service = new ShareLearnService();
            service.CreateDesignerProfile(request);
        }
        [Route("uploaduserimage")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage PostScripts()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            string filePath = string.Empty;
            string imageFilePath = string.Empty;
            
            try
            {

                var httpRequest = HttpContext.Current.Request;
                string MappedFileName = httpRequest.Form["MappedName"];

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        //int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                       

                    }

                    var message1 = string.Format("Script Uploaded Successfully.");
                    //return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
               
                //var res = string.Format("Please Upload a image.");
                //dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.OK, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format(ex.StackTrace);
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, dict);
            }
        }
        [Route("validateTransaction")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage validateTransaction(string paymentId, string orderId, string signature)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            bool validation = service.ValidatePaymentsVendor(paymentId, orderId, signature, "D", userName);
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
        [Route("uploadimage")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            System.IO.Stream MyStream;

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

                        int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB  

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

                            var message = string.Format("Please Upload a file upto 10 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            string fileName = "";

                            if (AllowedResumeFileExtensions.Contains(extension))
                            {
                                fileName = name.Split('@')[0] + "Resume" + extension;

                            }
                            else
                            {
                                fileName = name.Split('@')[0] + "ProfilePic" + extension;
                            }


                            var stream = postedFile.InputStream;
                            var result = await AmazonS3Access.UploadFileStream(stream, fileName);
                            if (result)
                            {
                                using (mLearnDBEntities dbContxt = new mLearnDBEntities())
                                {
                                    var designerProfile = dbContxt.Designers.Where(x => x.emailId == name).Select(x => x).FirstOrDefault();
                                    if (fileName.Contains("ProfilePic"))
                                    {
                                        designerProfile.photolink = ConfigurationManager.AppSettings["S3BaseUrl"] + fileName;
                                    }
                                    else if (fileName.Contains("Resume"))
                                    {
                                        designerProfile.resumeUrl = ConfigurationManager.AppSettings["S3BaseUrl"] + fileName;
                                    }

                                    dbContxt.SaveChanges();

                                }
                            }

                        }
                    }

                    var message1 = string.Format("Document Updated Successfully.");
                    return Request.CreateResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a valid file.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("Error in upload");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }

    }
}
