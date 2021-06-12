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
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/admin")]
    public class AdminController : ApiController
    {


        [OverrideAuthorization]
        [Authorize(Roles = "Admin,Support")]
        [Route("getongoingorder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]

        public HttpResponseMessage GetOngoingOrder(string email, bool isAllOrdersRequired)
        {
            AdminService service = new AdminService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                var orders = service.GetOngoingOrdersCustomer(email, isAllOrdersRequired);
                return Request.CreateResponse(HttpStatusCode.OK, orders);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Error=ex.StackTrace.ToString()});

            }

        }
        [OverrideAuthorization]
        [Authorize(Roles = "Admin,Support")]
        [Route("downloaFiles")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadOngoingOrdersDesigner(string filename,string location)
        {
            var responseStream = await AmazonS3Access.ReadFileDataAsync(filename, location);
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
        [Route("adminVerifyVendor")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage AdminVerifyVendor(string type, string email,bool isProf,bool isAdminVerified)
        {
            try
            {
                ShareLearnService service = new ShareLearnService();
                List<BankAccountDetailsModel> model = new List<BankAccountDetailsModel>();
                bool result = false;
                if (type == "Designer")
                {
                    result = service.verifyVendor(type, email, isProf, isAdminVerified);
                    //model = service.GetBankDetails(vendorEmail);
                }
                else if (type == "Printer")
                {
                    result = service.verifyVendor(type, email, isProf, isAdminVerified);
                    //model = service.GetBankDetailsPrinter(vendorEmail);
                }


                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }

        }
        [Route("getAccountDetailsVendor")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetVendorAccountDetails(string type,string vendorEmail)
        {
            try
            {
                ShareLearnService service = new ShareLearnService();
                List<BankAccountDetailsModel> model = new List<BankAccountDetailsModel>();
                if (type== "Designer")
                {
                    model = service.GetBankDetails(vendorEmail);
                }
                else if (type == "Printer")
                {
                    model = service.GetBankDetailsPrinter(vendorEmail);
                }
                

                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }

        }
        [Route("getDesignerpayouts")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public  HttpResponseMessage GetDesignerPayOut()
        {
            try
            {
                AdminService service = new AdminService();
                var printer =  service.GetDesignerPayOutDetails();
                //var userAddress = service.GeUserAddress();

                return Request.CreateResponse(HttpStatusCode.OK, printer);
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
                AdminService service = new AdminService();
                var printer = service.GetPrinterPayOutDetails();
                //var userAddress = service.GeUserAddress();

                return Request.CreateResponse(HttpStatusCode.OK, printer);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }

        }
        [Route("makePrinterPaymentVPA")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> CreatePayoutPrinterVPA(int orderId, string vendorEmail, decimal amount)
        {
            try
            {
                AdminService service = new AdminService();
                var printer = await service.MakeVendorPayoutVPAPrinter(orderId, vendorEmail,amount);
                //var userAddress = service.GeUserAddress();

                return Request.CreateResponse(HttpStatusCode.OK, printer);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }

        }
        [Route("makeDesignerPaymentVPA")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> CreatePayoutDesignerVPA(int orderId, string vendorEmail,decimal amount)
        {
            try
            {
                AdminService service = new AdminService();
                var printer = await service.MakeVendorPayoutVPADesigner(orderId, vendorEmail, amount);
                //var userAddress = service.GeUserAddress();

                return Request.CreateResponse(HttpStatusCode.OK, printer);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }

        }
        [Route("makeDesignerPayment")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> CreatePayoutDesigner(int orderId, string vendorEmail,decimal amount)
        {
            try
            {
                AdminService service = new AdminService();
                var printer = await service.MakeVendorPayoutDesigner(orderId, vendorEmail, amount);
                //var userAddress = service.GeUserAddress();

                return Request.CreateResponse(HttpStatusCode.OK,printer);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }
          
        }
        [Route("makePrinterPayment")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> CreatePayoutPrinter(int orderId, string vendorEmail, decimal amount)
        {
            try
            {
                AdminService service = new AdminService();
                var printer = await service.MakeVendorPayoutPrinter(orderId, vendorEmail, amount);
                //var userAddress = service.GeUserAddress();

                return Request.CreateResponse(HttpStatusCode.OK, printer);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }
           
        }
        [Route("getAllAddress")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage GetAddress()
        {
            AdminService service = new AdminService();
            var printer = service.GetPrinterAddress();
            var userAddress = service.GeUserAddress();
            AddressResponse res = new AddressResponse();
            res.printerAdd = printer;
            res.userAdd = userAddress;
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [Route("deleteProducttList")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage DeleteProducttList(int Id)
        {
            AdminService service = new AdminService();
            var resposne = service.DeleteProductDetails(Id);

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }
        [Route("deleteProduct")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage DeleteProduct(int Id)
        {
            AdminService service = new AdminService();
            try
            {
                var resposne = service.DeleteProduct(Id);
                return Request.CreateResponse(HttpStatusCode.OK, resposne);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, ex);
            }
            

            
        }

        [Route("getDisountList")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage getDisountList()
        {
            AdminService service = new AdminService();
            var resposne = service.GetDiscountList();

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }
        [Route("getPinCode")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage getPinCode()
        {
            AdminService service = new AdminService();
            var resposne = service.GetPincodes();

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }
        [Route("addPinCode")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage AddPinCode(int pinCode)
        {
            AdminService service = new AdminService();
            var resposne = service.AddPincode(pinCode);

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }
        [Route("deletePinCode")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage DeletePinCode(int Pincode)
        {
            AdminService service = new AdminService();
            var resposne = service.DeletePinCode(Pincode);

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }
        [Route("setDisountList")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage settDisountList(DiscountsModel discount)
        {
            AdminService service = new AdminService();
            var resposne = service.CreateUpdateDiscount(discount);

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }
        //[Route("rescheduleMeeting")]
        //[EnableCors("*", "*", "*")]
        //[HttpPost]
        //public HttpResponseMessage AcceptOrder(string Designeremail, int orderId,[FromBody] MeetingDetails meetDetails)
        //{
        //    ShareLearnService service = new ShareLearnService();
        //    var userName = HttpContext.Current.User.Identity.Name;
        //    try
        //    {
        //        service.RescheduleDesignOrder(userName, orderId, meetDetails);
        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

        //    }
        //}
        [Route("deleteDisountList")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage deletetDisountList(int Id)
        {
            AdminService service = new AdminService();
            var resposne = service.DeleteDiscount(Id);

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }

        [Route("setMaxGap")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public HttpResponseMessage setMaxGap(int gap)
        {
            AdminService service = new AdminService();
            var resposne = service.SetMaxGap(gap);

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }
        [Route("getregisteredprofiles")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
       
        public HttpResponseMessage GetRegisteredProfiles()
        {
            AdminService service = new AdminService();
            RegisteredProfiles profiles = new RegisteredProfiles();
            profiles.dprofile = service.GetDesignerProfile();
            profiles.pprofile = service.GetPrinterProfile();
            profiles.customerProfile = service.GetCustomerProfiles();

            return Request.CreateResponse(HttpStatusCode.OK, profiles);
        }
        [Route("cancelVendorOrder")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        public async Task<HttpResponseMessage> CancelVendorOrder(int? orderId,string type)
        {
            try
            {
                ShareLearnService service = new ShareLearnService();
                var result = await service.VendorCancelOrder(orderId, type);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {

                  return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
           
        }
        [Route("checkpincodes")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage CheckPincodes(int pincode)
        {
            AdminService service = new AdminService();
            var resposne = service.CheckServicablePinCode(pincode);

            return Request.CreateResponse(HttpStatusCode.OK, resposne);
        }
        [Route("getproducts")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetProducts()
        {
            AdminService service = new AdminService();
            try
            {
                var resposne = service.GetproductList();

                return Request.CreateResponse(HttpStatusCode.OK, resposne);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        [Route("delproductImg")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DeleteProductImg(int imgId)
        {
            AdminService service = new AdminService();
            try
            {
               service.DeleteProductImage(imgId);

                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }

        [Route("updateproducts")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage UpdateProducts(ProductModel product)
        {
            AdminService service = new AdminService();
            try
            {
               int id= service.UpdateProducts(product);
                
                service.createUpdatepricePerUnit(product.printPrice,id);

                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }

        [Route("updateproductslist")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public HttpResponseMessage UpdateProductsList(ProductModel product)
        {
            AdminService service = new AdminService();
            try
            {
                service.CreateUpdateProducts(product);
                

                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }

        }
        [Route("SetFlagValues")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SetFlagValues(Flags flagName)
        {
            ShareLearnService service = new ShareLearnService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
               service.SetFlagValues(flagName);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("DeleteFlagValues")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DeleteFlagValues(int flagName)
        {
            ShareLearnService service = new ShareLearnService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                service.DeleteFlagValues(flagName);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }
        [Route("GetAllFlags")]
        [EnableCors("*", "*", "*")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllFlags()
        {
            ShareLearnService service = new ShareLearnService();
            //var userName = HttpContext.Current.User.Identity.Name;

            try
            {
                var result = service.GetAllFlagValues();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);

            }

        }

        [Route("uploadproductimage")]
        [EnableCors("*", "*", "*")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostProductImage()
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

                    string imgpath = "";
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB  

                        
                        IList<string> AllowedIamgeFileExtensions = new List<string> { ".jpg", ".gif", ".png", ".jpeg" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        extension = ext.ToLower();
                        if (!AllowedIamgeFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.,.jpeg");

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
                            
                            var stream = postedFile.InputStream;
                            var result = await AmazonS3Access.UploadFileStream(stream, postedFile.FileName);
                            if (result)
                            {
                                imgpath= ConfigurationManager.AppSettings["S3BaseUrl"] + postedFile.FileName;
                            }

                        }
                    }

                   // var message1 = string.Format("Document Updated Successfully.");
                    return Request.CreateResponse(HttpStatusCode.Created,  imgpath); ;
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
