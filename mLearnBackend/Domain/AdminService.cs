using mLearnBackend.Helper;
using mLearnBackend.Models;
using mLearnBackend.Models.Response;
using mLearnBackend.Models.VPAPayment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace mLearnBackend.Domain
{
    public class AdminService
    {
        
        public List<OngoingOrder> GetOngoingOrdersCustomer(string userEmail, bool isAllordersneeded)
        {
            List<OngoingOrder> designerOrderList = new List<OngoingOrder>();
            

            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var customerData = (from a in dbConetxt.Customers select a).ToList();
                var addressUser = (from a in dbConetxt.AddressUsers select a).ToList();
                var designers = (from a in dbConetxt.Designers select a).ToList();
                var printers = (from a in dbConetxt.Printers select a).ToList();
                var orderCustomer = (from a in dbConetxt.Orders                                                                       
                                     select a).ToList();
                var prodDetails = (from a in dbConetxt.ProductDetails select a).ToList();
                var products = (from a in dbConetxt.Products select a).ToList();
                var orderDetailsList = (from a in dbConetxt.orderDetails select a).ToList();
                var meetingsList = (from a in dbConetxt.Meetings select a).ToList();
                foreach (var item in orderCustomer)
                {
                    bool isDesignCompleted = false;
                    
                    var orderDetails = (from b in orderDetailsList
                                        where b.orderid == item.orderId
                                        select b).ToList();
                    OngoingOrder designerOrder = new OngoingOrder();
                    designerOrder.OrderId = item.orderId;
                    designerOrder.printerInvoiceFilepath = item.printerInvoiceAddress;
                    designerOrder.CreatedDate = item.createdDate;
                    designerOrder.DunzoTaskId = item.DunzoTaskId;
                    var meetingDetails = (from a in meetingsList
                                          where a.orderId == item.orderId
                                          select a).FirstOrDefault();
                    if (meetingDetails != null)
                    {
                        designerOrder.MeetingUrl = meetingDetails.meetingLink;
                    }
                    //var customerDetails = designers.Where(x=>x.userId == item.designerId).FirstOrDefault();
                    var designerDetails = designers.Where(x => x.userId == item.designerId).FirstOrDefault();
                    var printerDetails = printers.Where(x => x.userId == item.printerId).FirstOrDefault();
                    var customerDetails = customerData.Where(x => x.userId == item.customerId).FirstOrDefault();
                    var addressDetails = addressUser.Where(x=>x.addressId == item.deliveryAddress).FirstOrDefault();
                    if(designerDetails != null)
                    {
                        designerOrder.assignedDesignerEmail = designerDetails.emailId;
                        designerOrder.assignerDesignerMobile = designerDetails.mobileNumber;
                    }
                    if(printerDetails != null)
                    {
                        designerOrder.assignedPrinterEmail = printerDetails.emailId;
                        designerOrder.assgnedPrinterMobile = printerDetails.mobileNumber;
                    }
                    
                   
                    

                    List<OngoingOrderList> orders = new List<OngoingOrderList>();
                    foreach (var order in orderDetails)
                    {
                        OngoingOrderList orderList = new OngoingOrderList();
                        orderList.orderid = item.orderId;
                        #region Get Details of Meeting Reschedule...
                        if (order.isMeetingRescheduledOpted == true && (order.orderType == "Design Only" || order.orderType == "Design And Print"))
                        {
                            var prodDItem = (from pd in prodDetails
                                             where pd.ProductSubCategory == order.subcategory
                                             select pd).FirstOrDefault();
                            if (prodDItem != null)
                            {
                                designerOrder.gap += prodDItem.SlotTimeGap;
                                designerOrder.eligibleForReschedule = true;
                                designerOrder.duration += prodDItem.meetingDuration;

                            }
                            

                        }
                        #endregion
                        if (isDesignCompleted == false)
                        {
                            isDesignCompleted = order.IsDesignCompleted == null ? false : (bool)order.IsDesignCompleted;
                        }
                        designerOrder.meetingTime = order.meetingTime.ToString();
                        designerOrder.productSubcategory = order.subcategory;
                        
                        
                        //OngoingOrderList orderList = new OngoingOrderList();
                        var productDetails = (from a in products
                                              where a.productName == order.category
                                              select a).FirstOrDefault();
                        orderList.ProductImageURL = productDetails.productImage;
                        
                        if (customerDetails != null)
                        {
                            designerOrder.CustomerName = customerDetails.firstName + customerDetails.lastName;
                            designerOrder.CustomerId = customerDetails.userId;
                            designerOrder.CustomerEmail = customerDetails.emailId;
                            designerOrder.CustomerNumber = customerDetails.mobileNumber;
                        }
                        if (addressDetails != null)
                        {
                            designerOrder.CustomerAddress = addressDetails.completeAddress;
                           
                        }
                        orderList.id = order.id;
                        orderList.subcategory = order.subcategory;
                        orderList.industry = order.industry;
                        orderList.size = order.size;
                        orderList.paperGSM = order.paperGSM;
                        orderList.quantity = order.quantity;
                        orderList.designerCost = order.designerCost;
                        orderList.TotalDesignCost = order.TotalDesignCost;
                        orderList.TotalPrintCost = order.TotalPrintCost;
                        orderList.BaseDesignPrice = order.BaseDesignPrice;
                        orderList.DesignGST = order.DesignGST;
                        orderList.Price = order.Price;
                        orderList.GST = order.GST;
                        orderList.Total = order.Total;
                        orderList.BaseDesignPrice = order.BaseDesignPrice;
                        orderList.SourceFileFees = order.SourceFileFees;
                        orderList.ProfessionDesignerFees = order.ProfessionDesignerFees;
                        orderList.PrintGST = order.PrintGST;
                        orderList.ProfessiondesignerFeesAfterCommision = order.ProfessiondesignerFeesAfterCommision;
                        orderList.printercost = order.printercost;
                        orderList.BillingName = order.BillingName;
                        orderList.GSTNumber = order.GSTNumber;
                        orderList.UserDesignAcceptance = order.UserAcceptanceTime;
                       // orderList.printerInvoiceFilepath = order.printerInvoiceAddress;

                        //orderList.DesignGST = order.DesignGST;
                        //orderList.TotalDesignCost = order.TotalDesignCost;
                        //orderList.TotalPrintCost = order.TotalPrintCost;
                        orderList.orderType = order.orderType;
                        orderList.content = order.content;
                        orderList.contentpath = order.contentpath;
                        orderList.orientation = order.orientation;
                        orderList.sampleImageLogo = order.sampleImageLogo;
                        orderList.isDesignCompleted = order.IsDesignCompleted;
                        orderList.isPrintCompleted = order.IsPrintCompleted;
                        orderList.selectedSourceFile = order.selectedSourceFile;
                        orderList.isDesignerFinishedOrder = order.IsDesignerFinishedOrder;
                        if (orderList.sampleImageLogo != null)
                        {
                            var images = orderList.sampleImageLogo.Split(';');
                            orderList.sourceLogoFile = images;
                        }
                        if (order.sourceCodeFilePath != null)
                        {
                            var images = order.sourceCodeFilePath.Split(';');
                            orderList.sourcecodeFinalDesignFile = images;
                        }
                        if (order.finalDesignFilepath != null)
                        {
                            var images = order.finalDesignFilepath.Split(';');

                            orderList.FinalNormalDesignFile = images;
                        }
                        //orderList.MeetingUrl = meetingDetails.meetingLink;
                        orderList.referenceImageURL = order.referenceImageURL;
                        orderList.designerCost = order.designerCost;
                        orders.Add(orderList);


                    }
                    //designerOrder.ongoingOrders = new List<OngoingOrderList>();
                    //logic to add only active orders
                    if (isAllordersneeded == false && isDesignCompleted == false)
                    {
                        designerOrder.ongoingOrders = orders;
                        designerOrderList.Add(designerOrder);
                    }
                    else if (isAllordersneeded == true)
                    {
                        designerOrder.ongoingOrders = orders;
                        designerOrderList.Add(designerOrder);
                    }
                }




            }
            designerOrderList = (from a in designerOrderList orderby a.meetingTime select a).ToList();
            return designerOrderList;

        }
        public List<VendorPayout> GetPrinterPayOutDetails()
        {
            List<VendorPayout> payout = new List<VendorPayout>();

            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var GetOrdersGroup = (from a in dbcontext.Orders
                                      where a.IsOrderCompleted == true && a.printerId !=null
                                      group a by new { a.orderId, a.printerId } into designerOrders
                                      select new { key = designerOrders.Key }).ToList();

                foreach (var item in GetOrdersGroup)
                {
                    VendorPayout itempayout = new VendorPayout();
                    itempayout.orderId = item.key.orderId;
                    itempayout.VendorId = (int)item.key.printerId;
                    itempayout.VendorType = "Printer";
                    var vendorDetails = (from a in dbcontext.Printers
                                         where a.userId == itempayout.VendorId
                                         select a).FirstOrDefault();
                    itempayout.VendorEmail = vendorDetails.emailId;
                    itempayout.VendorName = $"{vendorDetails.firstName}  {vendorDetails.lastName}";

                    var orderdetails = (from b in dbcontext.orderDetails
                                        where b.orderid == item.key.orderId
                                        group b by b.orderid into oDetails
                                        select new { id = oDetails.Key, printerCost = oDetails.Sum(x => x.printercost) }).FirstOrDefault();
                    if (orderdetails != null)
                    {
                        itempayout.TotalPayout = orderdetails.printerCost;
                    }
                    var payoutDetails = (from c in dbcontext.TransactionsPAYOUTs
                                         where c.OrderId == itempayout.orderId && c.vendorId == itempayout.VendorId && c.VendorType == "Printer"
                                         group c by new { c.OrderId, c.vendorId } into pOut
                                         select new
                                         {
                                             key = pOut.Key,
                                             paymentDone = pOut.Sum(x => x.PayOutAmount),
                                             payouttaxes = pOut.Sum(x => x.RazorPayTax),
                                             payoutFees = pOut.Sum(x => x.RazorPayFees)
                                         }).FirstOrDefault();
                    if (payoutDetails != null)
                    {
                        itempayout.AmountTransfered = payoutDetails.paymentDone;
                        itempayout.PayoutTaxes = payoutDetails.payouttaxes;
                        itempayout.PayoutFees = payoutDetails.payoutFees;
                    }
                    payout.Add(itempayout);
                }

            }
            return payout;
        }
        public List<VendorPayout> GetDesignerPayOutDetails()
        {
            List<VendorPayout> payout = new List<VendorPayout>();

            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var GetOrdersGroup = (from a in dbcontext.Orders where a.IsOrderCompleted==true
                                      group a by new { a.orderId, a.designerId } into designerOrders
                                      select new { key = designerOrders.Key }).ToList();

                foreach(var item in GetOrdersGroup)
                {
                    VendorPayout itempayout = new VendorPayout();
                    itempayout.orderId = item.key.orderId;
                    itempayout.VendorId = (int)item.key.designerId;
                    itempayout.VendorType = "Designer";
                    var vendorDetails = (from a in dbcontext.Designers
                                         where a.userId == itempayout.VendorId
                                         select a).FirstOrDefault();
                    itempayout.VendorEmail = vendorDetails.emailId;
                    itempayout.VendorName = $"{vendorDetails.firstName}  {vendorDetails.lastName}";

                    var orderdetails = (from b in dbcontext.orderDetails
                                        where b.orderid == item.key.orderId
                                        group b by b.orderid into oDetails
                                        select new { id = oDetails.Key, designerCost = oDetails.Sum(x => x.designerCost) }).FirstOrDefault();
                    if(orderdetails != null)
                    {
                        itempayout.TotalPayout = orderdetails.designerCost;
                    }
                    var payoutDetails = (from c in dbcontext.TransactionsPAYOUTs
                                         where c.OrderId == itempayout.orderId && c.vendorId == itempayout.VendorId && c.VendorType == "Designer"
                                         group c by new { c.OrderId, c.vendorId } into pOut
                                         select new
                                         {
                                             key = pOut.Key,
                                             paymentDone = pOut.Sum(x => x.PayOutAmount),
                                             payouttaxes = pOut.Sum(x => x.RazorPayTax),
                                             payoutFees = pOut.Sum(x => x.RazorPayFees)
                                         }).FirstOrDefault();
                    if(payoutDetails != null)
                    {
                        itempayout.AmountTransfered = payoutDetails.paymentDone;
                        itempayout.PayoutTaxes = payoutDetails.payouttaxes;
                        itempayout.PayoutFees = payoutDetails.payoutFees;
                    }
                    payout.Add(itempayout);
                }

            }
            return payout;
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public async Task<bool> MakeVendorPayoutDesigner(int orderId,string vendorEmail,decimal amount)
        {
            bool transactionStatus = false;
            RazorPayoutModel request = new RazorPayoutModel();
            request.fund_account = new Models.FundAccount();
            request.fund_account.contact = new Models.Contact();
            request.fund_account.bank_account = new Models.BankAccount();
            request.account_number = ConfigurationManager.AppSettings["payoutAccount"];
            request.fund_account.contact.notes = new Dictionary<string, string>();
            request.fund_account.contact.notes.Add("notes_key_1", "Designer payout");
            request.notes = new Dictionary<string, string>();
            request.notes.Add("notes_key_2", "Designer payout");
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var designerCost = (from a in dbcontext.orderDetails
                                    where a.orderid == orderId
                                    group a by a.orderid into newGroup
                                    select new
                                    {
                                        key=newGroup.Key,
                                       Cost=newGroup.Sum(x=> x.designerCost)
                                    }).FirstOrDefault();
                //request.amount = ((decimal)designerCost.Cost) * 100;
                request.amount = ((decimal)amount) * 100;
                request.currency = "INR";
                request.purpose = "vendor bill";
                request.mode = "NEFT";
                var vendorAccDetails = (from a in dbcontext.Designers
                                        join b in dbcontext.BankAccountDetailsDesigners
                                        on a.userId equals b.DesignerId
                                        where a.emailId == vendorEmail
                                        select new {
                                            BankAccountNumber=b.BankAccountNumber,
                                            AccountHolderName=b.AccountHolderName,
                                            IFSC=b.IFSC,
                                            Name=a.firstName + " " + a.lastName,
                                            number=a.mobileNumber,
                                            vendorId=a.userId
                                           

                                        }).FirstOrDefault();
                if (vendorAccDetails != null)
                {
                    request.fund_account.account_type = "bank_account";
                    request.fund_account.bank_account.account_number = vendorAccDetails.BankAccountNumber;
                    request.fund_account.bank_account.name = vendorAccDetails.AccountHolderName;
                    request.fund_account.bank_account.ifsc = vendorAccDetails.IFSC;
                    request.fund_account.contact.contact = vendorAccDetails.number;
                    request.fund_account.contact.name = vendorAccDetails.Name;
                    request.fund_account.contact.email = vendorEmail;
                    request.fund_account.contact.reference_id = $"{vendorEmail} - {orderId}";
                   
                }
                request.queue_if_low_balance = false;
                request.reference_id = $"{vendorEmail} - {orderId}";
                request.narration = "Payout From shapeNprint";
                var json = JsonConvert.SerializeObject(request);
                var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var result = await Helper.CommonMethods.HTTPPostCallsRazorVendorPayout("https://api.razorpay.com/v1/payouts", data);
                transactionStatus=SaveVendorPayoutResponse(result, orderId, "Designer", vendorAccDetails.vendorId);

            }
            
            return transactionStatus;
        }
        public async Task<bool> MakeVendorPayoutVPADesigner(int orderId, string vendorEmail,decimal amount)
        {
            bool transactionStatus = false;
            RazorPayoutVPA request = new RazorPayoutVPA();
            request.fund_account = new Models.VPAPayment.FundAccount();
            request.fund_account.contact = new Models.VPAPayment.Contact();
            request.fund_account.vpa = new VPA();
            request.account_number = ConfigurationManager.AppSettings["payoutAccount"];
            request.fund_account.contact.notes = new Dictionary<string, string>();
            request.fund_account.contact.notes.Add("notes_key_1", "Designer payout");
            request.notes = new Dictionary<string, string>();
            request.notes.Add("notes_key_2", "Designer payout");
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var designerCost = (from a in dbcontext.orderDetails
                                    where a.orderid == orderId
                                    group a by a.orderid into newGroup
                                    select new
                                    {
                                        key = newGroup.Key,
                                        Cost = newGroup.Sum(x => x.designerCost)
                                    }).FirstOrDefault();
                //request.amount = ((decimal)designerCost.Cost) * 100;
                request.amount = ((decimal)amount) * 100;
                request.currency = "INR";
                request.purpose = "vendor bill";
                request.mode = "UPI";
                var vendorAccDetails = (from a in dbcontext.Designers
                                        join b in dbcontext.BankAccountDetailsDesigners
                                        on a.userId equals b.DesignerId
                                        where a.emailId == vendorEmail
                                        select new
                                        {
                                            UPI = b.UPI,
                                            
                                            Name = a.firstName + " " + a.lastName,
                                            number = a.mobileNumber,
                                            vendorId = a.userId


                                        }).FirstOrDefault();
                if (vendorAccDetails != null)
                {
                    request.fund_account.account_type = "vpa";
                    request.fund_account.vpa.address = vendorAccDetails.UPI;
                   
                    request.fund_account.contact.contact = vendorAccDetails.number;
                    request.fund_account.contact.name = vendorAccDetails.Name;
                    request.fund_account.contact.email = vendorEmail;
                    request.fund_account.contact.reference_id = $"{vendorEmail} - {orderId}";

                }
                request.queue_if_low_balance = false;
                request.reference_id = $"{vendorEmail} - {orderId}";
                request.narration = "Payout From shapeNprint";
                var json = JsonConvert.SerializeObject(request);
                var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var result = await Helper.CommonMethods.HTTPPostCallsRazorVendorPayout("https://api.razorpay.com/v1/payouts", data);
                transactionStatus = SaveVendorPayoutResponse(result, orderId, "Designer", vendorAccDetails.vendorId);

            }

            return transactionStatus;
        }
        public async Task<bool> MakeVendorPayoutVPAPrinter(int orderId, string vendorEmail, decimal amount)
        {
            bool transactionStatus = false;
            RazorPayoutVPA request = new RazorPayoutVPA();
            request.fund_account = new Models.VPAPayment.FundAccount();
            request.fund_account.contact = new Models.VPAPayment.Contact();
            request.fund_account.vpa = new VPA();
            request.account_number = ConfigurationManager.AppSettings["payoutAccount"];
            request.fund_account.contact.notes = new Dictionary<string, string>();
            request.fund_account.contact.notes.Add("notes_key_1", "Designer payout");
            request.notes = new Dictionary<string, string>();
            request.notes.Add("notes_key_2", "Designer payout");
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var designerCost = (from a in dbcontext.orderDetails
                                    where a.orderid == orderId
                                    group a by a.orderid into newGroup
                                    select new
                                    {
                                        key = newGroup.Key,
                                        Cost = newGroup.Sum(x => x.printercost)
                                    }).FirstOrDefault();
                //request.amount = ((decimal)designerCost.Cost) * 100;
                request.amount = ((decimal)amount) * 100;
                request.currency = "INR";
                request.purpose = "vendor bill";
                request.mode = "UPI";
                var vendorAccDetails = (from a in dbcontext.Printers
                                        join b in dbcontext.BankAccountDetailsPrinters
                                        on a.userId equals b.PrinterId
                                        where a.emailId == vendorEmail
                                        select new
                                        {
                                            UPI = b.UPI,

                                            Name = a.firstName + " " + a.lastName,
                                            number = a.mobileNumber,
                                            vendorId = a.userId


                                        }).FirstOrDefault();
                if (vendorAccDetails != null)
                {
                    request.fund_account.account_type = "vpa";
                    request.fund_account.vpa.address = vendorAccDetails.UPI;

                    request.fund_account.contact.contact = vendorAccDetails.number;
                    request.fund_account.contact.name = vendorAccDetails.Name;
                    request.fund_account.contact.email = vendorEmail;
                    request.fund_account.contact.reference_id = $"{vendorEmail} - {orderId}";

                }
                request.queue_if_low_balance = false;
                request.reference_id = $"{vendorEmail} - {orderId}";
                request.narration = "Payout From shapeNprint";
                var json = JsonConvert.SerializeObject(request);
                var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var result = await Helper.CommonMethods.HTTPPostCallsRazorVendorPayout("https://api.razorpay.com/v1/payouts", data);
                transactionStatus = SaveVendorPayoutResponse(result, orderId, "Printer", vendorAccDetails.vendorId);

            }

            return transactionStatus;
        }

        private bool SaveVendorPayoutResponse(string result,int orderId,string vendorType,int vendorId)
        {
            bool transactionStatus = false;
           
            var responseObject = JsonConvert.DeserializeObject<RazorPayoutResponse>(result);

            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                TransactionsPAYOUT transaction = new TransactionsPAYOUT();
                transaction.AccountDeductedfrom = ConfigurationManager.AppSettings["payoutAccount"];
                if (responseObject.fund_account.bank_account !=null)
                {
                    transaction.AccountTransferedTo = responseObject.fund_account.bank_account.account_number;
                    transaction.TransactionMode = "NEFT";
                }
                else if(responseObject.fund_account.vpa.address !=null)
                {
                    transaction.AccountTransferedTo = responseObject.fund_account.vpa.address;
                    transaction.TransactionMode = "UPI";
                }
               
                transaction.OrderId = orderId;
                transaction.PayOutAmount = responseObject.amount/100;
                transaction.razorPayOutID = responseObject.id;
                transaction.RazorPayTax = responseObject.tax;
                transaction.RazorPayFees = responseObject.fees;
                transaction.PayOutStatus = responseObject.status;
                transaction.PayoutPurpose = responseObject.purpose;
                if(responseObject.utr != null)
                {
                    transaction.PayOutTransactionId = responseObject.utr.ToString();
                }
                if(responseObject.failure_reason != null)
                {
                    transaction.FailureNote = responseObject.failure_reason.ToString();
                }
                transaction.createdDate = UnixTimeStampToDateTime(responseObject.created_at);
               
                transaction.VendorType = vendorType;
                transaction.vendorId = (int)vendorId;
                dbcontext.TransactionsPAYOUTs.Add(transaction);
                dbcontext.SaveChanges();
                transactionStatus = true;

            }

                return transactionStatus;
        }
         
        public async Task<bool> MakeVendorPayoutPrinter(int orderId,string vendorEmail, decimal amount)
        {
            bool transactionStatus = false;
            RazorPayoutModel request = new RazorPayoutModel();
            request.fund_account = new Models.FundAccount();
            request.fund_account.contact = new Models.Contact();
            request.fund_account.bank_account = new Models.BankAccount();
            request.account_number = ConfigurationManager.AppSettings["payoutAccount"];
            request.fund_account.contact.notes = new Dictionary<string, string>();
            request.fund_account.contact.notes.Add("notes_key_1", "Designer payout");
            request.notes = new Dictionary<string, string>();
            request.notes.Add("notes_key_2", "Printer payout");
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var designerCost = (from a in dbcontext.orderDetails
                                    where a.orderid == orderId
                                    group a by a.orderid into newGroup
                                    select new
                                    {
                                        key = newGroup.Key,
                                        Cost = newGroup.Sum(x => x.printercost)
                                    }).FirstOrDefault();
                //request.amount = ((decimal)designerCost.Cost) * 100;
                request.amount = ((decimal)amount) * 100;
                request.currency = "INR";
                request.purpose = "vendor bill";
                request.mode = "NEFT";
                var vendorAccDetails = (from a in dbcontext.Printers
                                        join b in dbcontext.BankAccountDetailsPrinters
                                        on a.userId equals b.PrinterId
                                        where a.emailId == vendorEmail
                                        select new
                                        {
                                            BankAccountNumber = b.BankAccountNumber,
                                            AccountHolderName = b.AccountHolderName,
                                            IFSC = b.IFSC,
                                            Name = a.firstName + " " + a.lastName,
                                            number = a.mobileNumber,
                                            vendorId = a.userId


                                        }).FirstOrDefault();
                if (vendorAccDetails != null)
                {
                    request.fund_account.account_type = "bank_account";
                    request.fund_account.bank_account.account_number = vendorAccDetails.BankAccountNumber;
                    request.fund_account.bank_account.name = vendorAccDetails.AccountHolderName;
                    request.fund_account.bank_account.ifsc = vendorAccDetails.IFSC;
                    request.fund_account.contact.contact = vendorAccDetails.number;
                    request.fund_account.contact.name = vendorAccDetails.Name;
                    request.fund_account.contact.email = vendorEmail;
                    request.fund_account.contact.reference_id = $"{vendorEmail} - {orderId}";

                }
                request.queue_if_low_balance = false;
                request.reference_id = $"{vendorEmail} - {orderId}";
                request.narration = "Payout From shapeNprint";
                var json = JsonConvert.SerializeObject(request);
                var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var result = await Helper.CommonMethods.HTTPPostCallsRazorVendorPayout("https://api.razorpay.com/v1/payouts", data);
                transactionStatus = SaveVendorPayoutResponse(result, orderId, "Printer", vendorAccDetails.vendorId);

            }

            return transactionStatus;
        }
        public List<AddressRequest> GetPrinterAddress()
        {
            List<AddressRequest> add = new List<AddressRequest>();
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                add = (from a in dbcontext.AddressPrinters
                               where a.longitute != null && a.latitude != null select
                               new AddressRequest {printer=a.printerId,longitude=a.longitute,lattitude=a.latitude }).ToList<AddressRequest>();
            }
            return add;
        }
        public AddressRequest GeUserAddress()
        {
            AddressRequest add = new AddressRequest();
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                add = (from a in dbcontext.AddressUsers
                       where a.longitute != null && a.latitude != null
                       select
new AddressRequest { printer = a.userId, longitude = a.longitute, lattitude = a.latitude }).FirstOrDefault();
            }
            return add;
        }
        public bool DeleteProduct(int id)
        {
            bool status = false;
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var item = dbcontext.Products.Find(id);
                if(item.IsDisabled == true)
                {
                    item.IsDisabled = false;
                }
                else
                {
                    item.IsDisabled = true;
                }
                
                status = true;
                dbcontext.SaveChanges();

            }
            return status;
        }
        public bool DeleteProductDetails(int id)
        {
            bool status = false;
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var item = dbcontext.ProductDetails.Find(id);
                if (item.IsDisabled == true)
                {
                    item.IsDisabled = false;
                }
                else
                {
                    item.IsDisabled = true;
                }
                status = true;
                dbcontext.SaveChanges();

            }
            return status;
        }
        public List<DiscountsModel> GetDiscountList()
        {
            List<DiscountsModel> discount = new List<DiscountsModel>();
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                discount = (from a in dbcontext.Discounts select new DiscountsModel {
                    DiscountPercentage = a.DiscountPercentage,
                    CartAmount=a.CartAmount,
                    DiscountId=a.DiscountId

                }).ToList();
            }
            return discount;

        }

        public bool DeleteDiscount(int id)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                Discount item = null;
               
                    item = (from a in dbcontext.Discounts where a.DiscountId == id select a).FirstOrDefault();
                dbcontext.Discounts.Remove(item);


                
                
                dbcontext.SaveChanges();
                return true;
            }
        }
        public bool CreateUpdateDiscount(DiscountsModel discount)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                Discount item = null;
                if(discount.DiscountId == 0)
                {
                    item = new Discount();
                    dbcontext.Discounts.Add(item);
                }
                else
                {
                    item = (from a in dbcontext.Discounts where a.DiscountId == discount.DiscountId select a).FirstOrDefault();
                   

                }
                if(item != null)
                {
                    item.CartAmount = discount.CartAmount;
                    item.DiscountPercentage = discount.DiscountPercentage;
                }
                dbcontext.SaveChanges();
                return true;
            }
        }

        public List<CustomerData> GetCustomerProfiles()
        {
            List<CustomerData> data = new List<CustomerData>();

            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                 data = (from a in dbContext.Customers
                                    join b in dbContext.AspNetUsers on a.registrationId equals b.Id
                                    select new CustomerData
                                    {
                                        firstName=a.firstName,
                                        lastName=a.lastName,
                                        emailId=a.emailId,
                                        mobileNumber=a.mobileNumber,
                                        cretaedDate=a.createdDate == null ? null:a.createdDate.ToString(),
                                        isEmailVerified=b.EmailConfirmed,
                                        isMobileVerified=b.PhoneNumberConfirmed

                                    }).ToList();
            }

                return data;
        }
       
        public List<PrinterProfileRequest> GetPrinterProfile()
        {
            List<PrinterProfileRequest> printers = new List<PrinterProfileRequest>();
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var printsList = (from a in dbcontext.Printers orderby a.createdDate select a).ToList();
                var userList = (from a in dbcontext.AspNetUsers select a).ToList();
                int i = 1;
                foreach (var request in printsList)
                {
                    var user = userList.Where(x => x.Id == request.registrationId).Select(x => x).FirstOrDefault();
                    var newDesinger = new PrinterProfileRequest();
                    
                        newDesinger.location = request.location;
                        newDesinger.pan = request.PAN;
                        //newDesinger.createdDate = DateTime.Now;
                        newDesinger.dob = request.dateOfBirth;
                    //newDesinger.emailId = request.email;
                        newDesinger.isVerified = request.IsProfileVerified == null?false :(bool)request.IsProfileVerified;
                        newDesinger.firstName = request.firstName;
                        newDesinger.lastName = request.lastName;
                        newDesinger.mobileNumber = request.mobileNumber;

                        newDesinger.profileUrl = request.profileUrl;
                        //newDesinger.profileImage = request.photolink;
                        newDesinger.gstFileName = request.GSTCertificateURL;
                        newDesinger.gender = request.gender;
                        //s newDesinger.gstFileName = request.GSTCertificateURL;
                        newDesinger.gst = request.GST;
                        newDesinger.aadhar = request.Aadhar;
                    newDesinger.isPaymentDone = request.isRegistrationFeesPaid == null ? false : (bool) request.isRegistrationFeesPaid;
                    newDesinger.Registrationnumber = request.lastName == null ? 0 : i++;
                    if (newDesinger.Registrationnumber == 0)
                    {
                        newDesinger.Registrationnumber = null;
                    }

                    //newDesinger.a

                    var printAdd = request.AddressPrinters.FirstOrDefault();
                        newDesinger.address = printAdd == null ? "" : printAdd.completeAddress;
                        newDesinger.profileImage = request.photolink;
                        if (user != null)
                        {
                            newDesinger.isMobileVerified = user.PhoneNumberConfirmed;
                        newDesinger.email = user.Email;
                    }

                    printers.Add(newDesinger);

                    
                }
                
                return printers;
            }
        }
        public List<DesignerProfileRequest> GetDesignerProfile()
        {
            List<DesignerProfileRequest> designerList = new List<DesignerProfileRequest>();

            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var designers = (from a in dbcontext.Designers orderby a.createdDate  select a).ToList();
                var userList = (from a in dbcontext.AspNetUsers select a).ToList();
                int i = 1;
                foreach (var request in designers)
                {
                    //var user = dbcontext.AspNetUsers.Find(request.registrationId);
                    var user = userList.Where(x => x.Id == request.registrationId).Select(x => x).FirstOrDefault();
                    DesignerProfileRequest newDesinger = new DesignerProfileRequest();
                    if (request != null)
                    {
                        newDesinger.city = request.city;
                        newDesinger.pan = request.PAN;
                        //newDesinger.updateDate = DateTime.Now;
                        newDesinger.dob = request.dateOfBirth;
                        newDesinger.isVerified = request.IsProfileVerified == null ? false : (bool)request.IsProfileVerified;
                        newDesinger.isProfessional = request.IsProfessional == null ? false:(bool) request.IsProfessional;
                        //newDesinger.emailId = request.email;
                        newDesinger.exp = request.experience;
                        newDesinger.firstName = request.firstName;
                        newDesinger.lastName = request.lastName;
                        newDesinger.mobileNumber = request.mobileNumber;
                        newDesinger.postalCode = request.postalCode;
                        newDesinger.profileUrl = request.profileUrl;
                        newDesinger.qualification = request.qualification;
                        newDesinger.gender = request.gender;
                        newDesinger.state = request.state;
                        newDesinger.softwares = request.softwares;
                        newDesinger.profileImage = request.photolink;
                        newDesinger.Registrationnumber = request.lastName == null ? 0 : i++;
                        newDesinger.isPaymentDone = request.isRegistrationFeesPaid == null ? false : (bool)request.isRegistrationFeesPaid;
                        if (newDesinger.Registrationnumber == 0)
                        {
                            newDesinger.Registrationnumber = null;
                        }
                        
                        if (user != null)
                        {
                            newDesinger.emailId = user.Email;
                            newDesinger.isMobileVerified = user.PhoneNumberConfirmed;
                        }
                        designerList.Add(newDesinger);
                    }
                }
               
                return designerList;
            }
        }
        public bool createUpdatepricePerUnit(List<ProductprintPrice> items,int detailsId)
        {
            bool status = false;
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var rows = (from a in dbContext.PrintPrcQuantities
                           where a.prodDetailsId == detailsId
                           select a);
                foreach (var row in rows)
                {
                    dbContext.PrintPrcQuantities.Remove(row);
                }
                foreach (var item in items)
                {
                    
                        var row = new PrintPrcQuantity();
                       
                        row.prodDetailsId = detailsId;
                    
                    row.deliveryDays = item.deliveryDays;
                    row.pricePerUnit = item.pricePerUnit;
                    row.quantity = item.qunatity;
                    row.printCommision = item.printCommission;
                    //row.pr
                    dbContext.PrintPrcQuantities.Add(row);
                }
                dbContext.SaveChanges();
            }
            return status;


        }
        public bool CheckServicablePinCode(int pinCode)
        {
             bool result = false;
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var item = (from a in dbContext.ServiceablePinCodes
                            where a.PinCode == pinCode select a).FirstOrDefault();
                if(item != null)
                {
                    result = true;
                }
            }
            return result;
        }
        public void CreateUpdateProducts(ProductModel model)
        {
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var allproducts = (from a in dbContext.Products orderby a.preference descending select a).ToList();
                var product = (from a in allproducts
                               where a.productId == model.productId
                               select a).FirstOrDefault();
               
                if(product != null)
                {
                    ResetPrefProducts(model.producPreference, dbContext, product);
                    product.productName = model.productName;
                    //product.preference = allproducts[0].preference + 1;
                    product.productPrice = "68";
                    product.productLinkedTo = model.productCategory;
                    product.productImage = model.productImage;
                    product.ProductIcon = model.productIcon;
                    //product.IsDisabled = model.IsDisabled;
                    dbContext.SaveChanges();
                    
                }
                else
                {
                    Product prod = new Product();
                    prod.productName = model.productName;
                    if(allproducts.Count > 0)
                    {
                        prod.preference = allproducts[0].preference + 1;
                    }
                    
                    prod.productPrice = "68";
                    prod.productLinkedTo = model.productCategory;
                    prod.productImage = model.productImage;
                    prod.ProductIcon = model.productIcon;
                    prod.IsDisabled =false;
                    dbContext.SaveChanges();
                    dbContext.Products.Add(prod);

                    dbContext.SaveChanges();
                    //ResetPrefProducts(model.producPreference, dbContext, prod);

                    
                }
                dbContext.SaveChanges();
                UploadProductSampleImages(model.imageUpload, model.productId);
            }
                
        }

        public List<int> GetPincodes()
        {
            List<int> pincodes = null;
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                pincodes = (from a in dbContext.ServiceablePinCodes select a.PinCode).ToList();
            }
            return pincodes;
                
        }

        public List<int> AddPincode(int pinCode)
        {
            List<int> pincodes = null;
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var pincode = (from a in dbContext.ServiceablePinCodes where a.PinCode == pinCode select a).FirstOrDefault();
                
                if (pincode == null)
                {
                    ServiceablePinCode sPinCode = new ServiceablePinCode();
                    sPinCode.PinCode = pinCode;
                    dbContext.ServiceablePinCodes.Add(sPinCode);
                    dbContext.SaveChanges();
                    pincodes = (from a in dbContext.ServiceablePinCodes select a.PinCode).ToList();
                }
            }
            return pincodes;
        }
        public List<int> DeletePinCode(int pinCode)
        {
            List<int> pincodes = null;
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var pincode = (from a in dbContext.ServiceablePinCodes where a.PinCode == pinCode select a).FirstOrDefault();

                if (pincode != null)
                {
                   
                    dbContext.ServiceablePinCodes.Remove(pincode);
                    dbContext.SaveChanges();
                   
                }
                pincodes = (from a in dbContext.ServiceablePinCodes select a.PinCode).ToList();
            }
            return pincodes;
        }
        public void UploadProductSampleImages(List<ImageUpload> imageUpload,int productId)
        {
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var images = (from a in dbContext.ProductImages select a).ToList();
                images.RemoveAll(x=>x.productId == productId);
                dbContext.SaveChanges();
                foreach (var item in imageUpload)
                {
                    ProductImage image = new ProductImage();
                    image.productId = productId;
                    image.imageUrl = item.serverFileName;
                    dbContext.ProductImages.Add(image);
                }
                dbContext.SaveChanges();
                
            }
        }

        /// <summary>
        /// This method set the value in products table price column of all rows
        /// </summary>
        /// <param name="maxGap"></param>
        public bool SetMaxGap(int maxGap)
        {
            bool result = false;
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var list = (from a in dbContext.Products select a).ToList();
                foreach (var item in list)
                {
                    item.productPrice = maxGap.ToString();
                }
                result = true;
            }
            return result;
        }
        public int UpdateProducts(ProductModel model)
        {
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                
                var product = (from a in dbContext.ProductDetails
                               where a.ProdDetailsId == model.productsubId
                               select a).FirstOrDefault();
                if(product == null)
                {
                    product = new ProductDetail();
                    product.productId = model.productId;
                    dbContext.ProductDetails.Add(product);
                    model.IsDisabled = false;
                }
                product.IsDisabled = model.IsDisabled == null ? false : model.IsDisabled;
                product.Orientation = model.orientation;
                product.PaperGSM = model.paperGSM;
                //product.ProductCategory = model.productCategory;
                product.ProductSubCategory = model.productSubcategory;
                product.ProductSize = model.productSize;
                product.productCode = model.productCode;
                product.quantities = model.quantities;
                product.productPrice = model.price;
                product.DesignCommision = model.DesignCommision;
                product.DesignGST = model.DesignGST;
                product.DesignPrice = model.DesignPrice;
                product.PrintCommision = model.PrintCommision;
                product.PrintGST = model.PrintGST;
                product.PrintPrice = model.PrintPrice;
                product.deliveryFees = model.deliveryFees;
                product.deliveryTime = model.deliveryTime;
                product.meetingDuration = model.meetingDuration;
                product.IsPriceInSqFt = model.IsPriceInSqFt;
                product.meetingDuration = model.meetingDuration;
                product.profDesignerFee = model.profDesignerFee;
                product.sourceFileFees = model.sourceFileFees;
                product.SlotTimeGap = model.SlotTimeGap;
                if(product.producPreference != model.producPreference)
                {
                    ResetPreference(model.producPreference, dbContext,product);
                }
                //product.producPreference = model.producPreference;
                product.productDescription = model.productDescription;
                dbContext.SaveChanges();
                return product.ProdDetailsId;
            }
        }

        private void ResetPreference(int? pre,mLearnDBEntities dbContext,ProductDetail product)
        {
            var items = (from a in dbContext.ProductDetails where a.producPreference >= pre  && a.producPreference < product.producPreference  select a).ToList();
            foreach (var item in items)
            {
                item.producPreference += 1;
            }
           
            product.producPreference = pre;


        }
        private void ResetPrefProducts(int? targetpref, mLearnDBEntities dbContext, Product product)
        {
            int? currentPref = product.preference;
            var items = (from a in dbContext.Products
                         where a.preference >= targetpref && a.preference < currentPref
                         orderby a.preference
                         select a).ToList();
            foreach (var item in items)
            {
                item.preference += 1;
            }

            product.preference = targetpref;


        }
        public void DeleteProductImage(int imgId)
        {
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var item = (from a in dbContext.ProductImages where a.imageId == imgId select a).FirstOrDefault();
                dbContext.ProductImages.Remove(item);
                dbContext.SaveChanges();
            }
        }
        public ProductList GetproductList()
        {
            ProductList data = new ProductList();
            data.productList = new List<ProductModel>();
           

            using(mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var productImages = (from a in dbContext.ProductImages select a).ToList();
                data.productList = (from a in dbContext.Products
                                    join b in dbContext.ProductDetails on a.productId equals b.productId
                                    
                            orderby b.producPreference ascending
                            select new ProductModel
                            {
                                poductDetailsId=b.ProdDetailsId,
                                productId=a.productId,
                                productsubId = b.ProdDetailsId,
                                productCategory=a.productLinkedTo,
                                productName=a.productName,
                                productSize =b.ProductSize,
                                productCode = b.productCode,
                                orientation=b.Orientation,
                                paperGSM=b.PaperGSM,
                                quantities=b.quantities,
                                productImage=a.productImage,
                                productIcon=a.ProductIcon,
                                productSubcategory=b.ProductSubCategory,
                                producPreference=b.producPreference,
                                DesignCommision=b.DesignCommision,
                                DesignGST=b.DesignGST,
                                DesignPrice=b.DesignPrice,
                                PrintCommision=b.PrintCommision,
                                PrintGST=b.PrintGST,
                                PrintPrice=b.PrintPrice,
                                productDescription=b.productDescription, 
                                deliveryFees=b.deliveryFees,
                                deliveryTime=b.deliveryTime,
                                meetingDuration=b.meetingDuration,
                                IsPriceInSqFt=b.IsPriceInSqFt,
                                profDesignerFee=b.profDesignerFee,
                                sourceFileFees=b.sourceFileFees,
                                SlotTimeGap=b.SlotTimeGap,
                                IsDisabled = b.IsDisabled                                                           


                            }).ToList<ProductModel>();
                data.products = (from a in dbContext.Products
                                
                                 orderby a.preference
                                select new UniqueProduct
                                {
                                    key = a.productId,
                                    value = a.productName,
                                    category=a.productLinkedTo,
                                    producPreference=a.preference,
                                    price=a.productPrice,
                                    productIcon=a.ProductIcon,
                                    productImage=a.productImage,
                                    IsDisabled=a.IsDisabled
                                }).ToList();

                //Taking max gap value fromt the price column of products table
                data.maxGap = String.IsNullOrEmpty(data.products[0].price) ? 0 : int.Parse(data.products[0].price);

                data.printPrice = (from a in dbContext.PrintPrcQuantities
                                   orderby  a.quantity 
                                   select new ProductprintPrice
                                   {
                                       prodDetailsId = a.prodDetailsId,
                                       qunatity = a.quantity,
                                       pricePerUnit=a.pricePerUnit,
                                       deliveryDays=a.deliveryDays,
                                       printCommission=a.printCommision,


                                   }).ToList();
                data.videoUrl = (from a in dbContext.MasterFlags where a.FlagName == "VideoURL" select a.FlagValue).FirstOrDefault();
                data.promotionImageURL = (from a in dbContext.MasterFlags where a.FlagName == "PromotionImageURL" select a.FlagValue).FirstOrDefault();
                data.displayPromotion = (from a in dbContext.MasterFlags where a.FlagName == "DisplayPromotion" select a.FlagValue).FirstOrDefault();
                foreach (var item in data.products)
                {
                    item.productImages = (from a in productImages
                               where a.productId==item.key
                               select new ProductCarouselImages
                               {
                                Id=a.imageId,
                                Url=a.imageUrl
                        
                    }).ToList();
                    

                }
                //data.products = new Dictionary<int, string>();
                //foreach (var item in products)
                //{
                //    data.products.Add(item.key, item.value);
                //}
            }
            data.discountList = GetDiscountList();
            return data;

        }
        
    }
}