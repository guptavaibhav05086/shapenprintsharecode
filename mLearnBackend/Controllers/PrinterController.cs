using mLearnBackend.Domain;
using mLearnBackend.Helper;
using mLearnBackend.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    [Authorize(Roles = "Printer")]
    [RoutePrefix("api/printer")]
    public class PrinterController : ApiController
    {
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
            
            string orderId = service.GenerateOrderServiceProviderFees(out transId, userRegisId,out amount);
            items[0] = orderId;
            items[1] = amount.ToString();
            return items;

        }
        [AllowAnonymous]
        [Route("GetDeliveryStatus")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetDeliveryStatus(string taskId)
        {
            
            try
            {
                ShareLearnService service = new ShareLearnService();
                var deliveryDetails = await CommonMethods.HTTPGetCallDunzoTask(taskId);
                service.UpdateDeliveryStatus(taskId, deliveryDetails.state);
                return Request.CreateResponse(HttpStatusCode.OK, deliveryDetails);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        [Route("SendDelivery")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> SendDelivery(int orderId, int customerId)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var deliveryDetails = await service.DeliveryAPICall(orderId,userName,customerId);
                return Request.CreateResponse(HttpStatusCode.OK, deliveryDetails);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        [Route("downloadOngoingOrderFiles")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadOngoingOrders(string filename,string type)
        {
            var responseStream = await AmazonS3Access.ReadFileDataAsync(filename, type);
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

        [Route("getprinterDashboard")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetPPrinterDashboard(string email)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var orders = service.PrinterDashboardDetails(userName);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
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
                service.AcceptPrintOrder(userName, orderId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        [Route("getOngoingOrder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetOngoingOrders(string email, bool isAllOrdersRequired)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            try
            {
                var orders = service.GetOngoingOrdersPrinter(userName, isAllOrdersRequired);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
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
                var details = service.GetBankDetailsPrinter(userName);
                return Request.CreateResponse(HttpStatusCode.OK, details);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        [Route("getPrinterpayouts")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetPrinterPayOut()
        {
            try
            {
                var userName = HttpContext.Current.User.Identity.Name;
                AdminService service = new AdminService();
                var printer = service.GetPrinterPayOutDetails();
                printer = (from a in printer where a.VendorEmail == userName select a).ToList();

                //var userAddress = service.GeUserAddress();

                return Request.CreateResponse(HttpStatusCode.OK, printer);
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
            var userName = HttpContext.Current.User.Identity.Name;
            ShareLearnService service = new ShareLearnService();
            try
            {
                service.CreateUpdatePrinterAccountDetails(bankDetails,userName);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }
        }
        [Route("postPrinterInvoice")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostDesignerFinalFolder([FromUri] int orderId)
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
                        string key = "FinalInvoice-" + orderId.ToString() + "-"+ postedFile.FileName.Replace(" ",string.Empty);
                        result = await AmazonS3Access.UploadKYCData(stream, key, "printerinvoices");
                        if (result)
                        {
                            using (mLearnDBEntities dbContxt = new mLearnDBEntities())
                            {
                                var orderDetails = (from a in dbContxt.Orders
                                                    where a.orderId == orderId
                                                    select a).FirstOrDefault();
                                if (orderDetails != null)
                                {
                                    orderDetails.printerInvoiceAddress =  key;
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

        [Route("getNotificationDetails")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetNotification()
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                var response = service.GetNotificationDetailsPrinter();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        [Route("GenerateOTP")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GenerateOTP(string userId, string phoneNumber)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                service.GenerateOTP(userId, phoneNumber);
                return Request.CreateResponse(HttpStatusCode.OK);
                

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
           var responseStream =await  AmazonS3Access.ReadFileDataAsync(filename, "userkycdata");
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;

            //Write the memory stream to HttpResponseMessage content
            response.Content = new StreamContent(responseStream);
            string contentDisposition = string.Concat("attachment; filename=", filename);
            response.Content.Headers.ContentDisposition =
                          ContentDispositionHeaderValue.Parse(contentDisposition);
            return response;
        }

        [Route("getProfile")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetProfile(string userId)
        {
            ShareLearnService service = new ShareLearnService();
            try
            {
                var profile = service.GetPrinterProfile(userId);
                return Request.CreateResponse(HttpStatusCode.OK, profile);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
            

        }

        [Route("validateTransaction")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage validateTransaction(string paymentId, string orderId, string signature)
        {
            ShareLearnService service = new ShareLearnService();
            var userName = HttpContext.Current.User.Identity.Name;
            bool validation = service.ValidatePaymentsVendor(paymentId, orderId, signature,"P",userName);
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
                        IList<string> AllowedResumeFileExtensions = new List<string> {".pdf", ".doc", ".docx" };
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
                            var stream = postedFile.InputStream;
                            bool result;
                            if (AllowedResumeFileExtensions.Contains(extension))
                            {
                                fileName = name.Split('@')[0] + "GSTCertificate" + extension;
                                result = await AmazonS3Access.UploadKYCData(stream, fileName, "userkycdata");

                            }
                            else
                            {
                                fileName = name.Split('@')[0] + "ProfilePic" + extension;
                                result = await AmazonS3Access.UploadFileStream(stream, fileName);
                            }

                            
                          
                            
                            if (result)
                            {
                                using (mLearnDBEntities dbContxt = new mLearnDBEntities())
                                {
                                    var designerProfile = dbContxt.Printers.Where(x => x.emailId == name).Select(x => x).FirstOrDefault();
                                    if (fileName.Contains("ProfilePic"))
                                    {
                                        designerProfile.photolink = ConfigurationManager.AppSettings["S3BaseUrl"] + fileName;
                                    }
                                    else if(fileName.Contains("GSTCertificate"))
                                    {
                                        designerProfile.GSTCertificateURL = fileName;
                                    }
                                    
                                    
                                    dbContxt.SaveChanges();

                                }
                            }

                        }
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
        //[Route("generateorder")]
        //[EnableCors("*", "*", "*")]
        //[HttpGet]

        //public string GenerateOrderId(double amount,string userRegisId)
        //{
        //    int transId;
        //    ShareLearnService service = new ShareLearnService();
        //    string orderId = service.GenerateOrder(amount,out transId, userRegisId);
        //    return orderId;

        //}
        [Route("updateprofile")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public void UpdateDesignerProfile(PrinterProfileRequest request)
        {
            ShareLearnService service = new ShareLearnService();
            service.CreatePrinterProfile(request);
        }
    }

    


}
