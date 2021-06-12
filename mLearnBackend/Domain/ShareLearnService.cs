using mLearnBackend.Helper;
using mLearnBackend.Models;
using Newtonsoft.Json;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace mLearnBackend.Domain
{

    public class ShareLearnService
    {
        public List<string> GetFlagValues(string flagName)
        {
            List<string> flags = null;
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                flags = (from a in dbConetxt.MasterFlags where a.FlagName == flagName select a.FlagValue).ToList();
            }
            return flags;
        }
        public void SetFlagValues(Flags flag)
        {
           
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var flagdata = (from a in dbConetxt.MasterFlags where a.Id == flag.FlagId select a).FirstOrDefault();
                if(flagdata != null)
                {
                    flagdata.FlagValue = flag.FlagValue;
                }
                else
                {
                    MasterFlag flagObj = new MasterFlag();
                    flagObj.FlagName = flag.FlagName;
                    flagObj.FlagValue = flag.FlagValue;
                    dbConetxt.MasterFlags.Add(flagObj);

                }
                dbConetxt.SaveChanges();
            }
            
        }
        public void DeleteFlagValues(int flagId)
        {

            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var flagdata = (from a in dbConetxt.MasterFlags where a.Id == flagId select a).FirstOrDefault();
                if (flagdata != null)
                {
                    dbConetxt.MasterFlags.Remove(flagdata);
                }
              
                dbConetxt.SaveChanges();
            }

        }
        public List<Flags> GetAllFlagValues()
        {
            List<Flags> FlagValues = new List<Flags>();
            var result = new List<Tuple<string, string>>()
          .Select(t => new { Key = t.Item1, Value = t.Item2 }).ToList();
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                FlagValues = (from a in dbConetxt.MasterFlags select new Flags
                 {

                   FlagName=a.FlagName,
                   FlagValue=a.FlagValue,
                   FlagId=a.Id

                }).ToList();
            }
            return FlagValues;
        }
        public List<ImageUpload> GetProductImages(int productId)
        {
            List<ImageUpload> images = new List<ImageUpload>();
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var prodImages = (from a in dbConetxt.ProductImages where a.productId == productId select a).ToList();
                var prod = dbConetxt.Products.Find(productId);
                foreach (var item in prodImages)
                {
                    ImageUpload objImgUp = new ImageUpload();
                    objImgUp.serverFileName = item.imageUrl;
                    objImgUp.id = (int)item.productId;
                    images.Add(objImgUp);
                }

                ImageUpload objImgUpDisplay = new ImageUpload();
                objImgUpDisplay.serverFileName = prod.productImage;
                objImgUpDisplay.id= productId;
                images.Add(objImgUpDisplay);
            }
            return images;
        }
        public bool UpdateCart(UserCartModel cart)
        {
            bool status = false;
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                UserCart usercart = new UserCart();
                var item = (from a in dbConetxt.UserCarts where a.UserEmail == cart.UserEmail select a).FirstOrDefault();
                if(item != null)
                {
                    item.UserCart1 = cart.UserCart;
                    item.IsPrintonly = cart.IsPrintonly;
                }
                else
                {
                    usercart.UserCart1 = cart.UserCart;
                    usercart.UserEmail = cart.UserEmail;
                    usercart.IsPrintonly = cart.IsPrintonly;
                    dbConetxt.UserCarts.Add(usercart);
                }
                dbConetxt.SaveChanges();
                status = true;
            }


                return status;
        }
        public UserCartModel FetchCart(string emailId)
        {
            UserCartModel usercart = new UserCartModel();
            AdminService service = new AdminService();
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                
                var item = (from a in dbConetxt.UserCarts where a.UserEmail == emailId select a).FirstOrDefault();
                if (item != null)
                {
                    usercart.UserCart = item.UserCart1;
                    usercart.UserEmail = item.UserEmail;
                    usercart.IsPrintonly = (bool)item.IsPrintonly;
                    usercart.prodList = new ProductList();
                    usercart.prodList.discountList = service.GetDiscountList();
                }
                else
                {
                    usercart.UserCart = "[]";
                }                              
                
            }
            return usercart;
        }
        public bool verifyVendor(string type, string email, bool isProf, bool isAdminVerified)
        {
            bool result = false;
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                if (type == "Designer")
                {
                    var item = (from a in dbConetxt.Designers where a.emailId == email select a).FirstOrDefault();
                    item.IsProfessional = isProf;
                    item.IsProfileVerified = isAdminVerified;
                    result = true;

                    //model = service.GetBankDetails(vendorEmail);
                }
                else if (type == "Printer")
                {
                    var item = (from a in dbConetxt.Printers where a.emailId == email select a).FirstOrDefault();
                    //item.IsProfessional = isProf;
                    item.IsProfileVerified = isAdminVerified;
                    result = true;
                    //model = service.GetBankDetailsPrinter(vendorEmail);
                }
                dbConetxt.SaveChanges();
            }

           return result;
        }
        public async Task<bool> VendorCancelOrder(int? orderNumber,string type)
        {
            bool result = false;
            List<string> designers = new List<string>();
            List<string> designersPhone = new List<string>();
        
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                if (type == "D")
                {
                    var orderDetails = (from a in dbConetxt.orderDetails
                                        where a.orderid == orderNumber && (a.orderType == "Design Only" || a.orderType == "Design And Print") && a.IsDesignCompleted == false
                                        select a).ToList();
                    if (orderDetails == null)
                    {
                        throw new Exception("No Design is in Progress for this Order");
                    }
                    //string subCategory = "";
                    //decimal? amount = 0;
                    //string meetingTime = "";
                    var customerDetails = (from a in dbConetxt.Orders
                                           join b in dbConetxt.Customers on a.customerId equals b.userId
                                           where a.orderId == orderNumber
                                           select b).FirstOrDefault();
                    var smsText = Constants.SMS_Customer_MeetingCancelled.Replace("$link", Constants.link_user_orders);
                    var emailText = Constants.Email_Order_Designer_NotAvaibale;
                    SendEmailNotificationOrder(designers, null, "OrderNotification", "Meeting Cancelled", "FinishdesignNoification", "https://drive.google.com/uc?export=view&id=11ddXfKw-Zz4osrD5lFtTnIavmi9ZGzhZ", emailText, "Sorry! Please choose another time slot.", "Select Another Meeting Slot");
                    sendSMS.sendSMSClient(smsText, customerDetails.mobileNumber);
                    foreach (var orderD in orderDetails)
                    {
                        orderD.isMeetingRescheduledOpted = true;
                    }
                    dbConetxt.SaveChanges();
                }
                else if (type == "P")
                {
                    var orderDetails = (from a in dbConetxt.orderDetails
                                        where a.orderid == orderNumber && (a.orderType == "Print Only" || a.orderType == "Design And Print") && (a.IsDesignCompleted == true) && (a.IsPrintCompleted == false)
                                        select a).ToList();
                   
                    var notification = (from a in dbConetxt.Notificationprnters where a.OrderId == orderNumber select a).FirstOrDefault();
                    if(orderDetails == null || notification == null)
                    {
                        throw new Exception("No Print is in Progress for this Order");
                    }
                    notification.isValid = true;
                    notification.updateDate = DateTime.Now;
                    foreach (var item in orderDetails)
                    {
                      
                        var smsPrinter = Constants.SMS_Printer_Order_Placed.
                            Replace("$quantity", item.quantity.ToString()).
                            Replace("$gsm", item.paperGSM.ToString()).
                            Replace("$subcategory", item.subcategory).
                            Replace("$link", Constants.link_printer_notifications).
                            Replace("$amount", item.printercost.ToString()).
                            Replace("$size", item.size.ToString());
                        var emailPrinter = Constants.Email_Order_Printer.
                          Replace("$quantity", item.quantity.ToString()).
                          Replace("$gsm", item.paperGSM.ToString()).
                          Replace("$subcategory", item.subcategory).
                          Replace("$size", item.size).
                          Replace("$amount", item.printercost.ToString());
                        await SendNotificationsPrinters(smsPrinter, emailPrinter, orderNumber);
                    }

                    dbConetxt.SaveChanges();
                    
                }

                result = true;
            }

            return result;
        }
        public bool ScheduledTaskNotifications()
        {
            bool result = false;
            List<string> designers = new List<string>();
            List<string> designersPhone = new List<string>();
            string smsEmailDesigner = "";
            //string link = "https://www.shapenprint.com/designer/notifications";
            string orderId = "";
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var notifications = (from a in dbConetxt.NotificationDesigners where a.isValid == true select a).ToList();
                var randomNumbersForSendingNotifications = (from a in dbConetxt.MasterFlags
                                                            where
                                                            a.FlagName == "RandomDesignerNumbers" select a).FirstOrDefault();
                int count = 0;
                int.TryParse(randomNumbersForSendingNotifications.FlagValue, out count);

                var currentDate = DateTime.Now;
                foreach (var item in notifications)
                {
                    var notificateTime = item.createdDate;
                    var diffInTime = currentDate - notificateTime;
                    if(diffInTime.Value.TotalMinutes > 15 && diffInTime.Value.TotalMinutes < 30)
                    {
                        //send notification to 
                        var totalDesigners = (from a in dbConetxt.Designers where a.IsProfessional == item.IsProfessional && a.IsProfileVerified == true select a).ToList();
                        Random rnd = new Random();
                        int skip = 0;
                        if (totalDesigners.Count > count)
                        {
                             skip = rnd.Next(1, (totalDesigners.Count - count));
                        }
                        
                       
                        var notificationDesignerLists = (from a in totalDesigners
                                                         select a).Skip(skip).
                                                         Take(count).ToList();
                        var orderDetails = (from a in dbConetxt.orderDetails
                                            where a.orderid == item.OrderId && (a.orderType == "Design Only" || a.orderType == "Design And Print")
                                            select a).ToList();
                        foreach (var designr in notificationDesignerLists)
                        {
                            //this will change to emailId later once we go to prod
                            if(designr.emailId != null)
                            {
                                designers.Add(designr.emailId);
                            }
                           
                            designersPhone.Add(designr.mobileNumber);
                        }
                        string subCategory = "";
                        decimal? amount = 0;
                        string meetingTime = "";
                        foreach(var order in orderDetails)
                        {
                            orderId = order.orderid.ToString();
                            subCategory = subCategory + order.subcategory;
                            amount = amount + order.designerCost;
                            meetingTime = GetSlot((DateTime) order.meetingTime);
                            if (item.IsProfessional == true)
                            {
                                amount = amount + order.ProfessiondesignerFeesAfterCommision;
                                smsEmailDesigner = Constants.Email_Order_Desing_Prof.
                      Replace("$meetingslot",  meetingTime).
                      Replace("$subcategory", subCategory).
                      Replace("$link", Constants.link_designer_notifications).Replace("$amount", amount.ToString());
                            }
                            else
                            {
                                smsEmailDesigner = Constants.Email_Order_Desing.
                      Replace("$meetingslot", meetingTime).
                      Replace("$subcategory", subCategory).
                      Replace("$link", Constants.link_designer_notifications).Replace("$amount", amount.ToString());
                            }
                            
                           
                        }
                        var smsTextDesigner = Constants.SMS_Order_Placed_Prof.
                         Replace("$meetingslot", meetingTime).
                        Replace("$orderid", orderId.ToString()).
                         Replace("$link", Constants.link_designer_notifications).Replace("$amount", amount.ToString());
                        


                        SendEmailNotificationOrder(designers, null, "OrderNotification", "New Order To Accept", "designNoification", "https://drive.google.com/uc?export=view&id=1M9t7mSX_Yo8q9il95dqc67N7Eo6S_azi", smsEmailDesigner, "Accept New Order","Accept Order");
                        foreach (var phone in designersPhone)
                        {
                            sendSMS.sendSMSClient(smsTextDesigner, phone);
                        }

                    }
                    else if (diffInTime.Value.TotalMinutes > 30)
                    {
                        item.isValid = false;
                        var customerDetails = (from a in dbConetxt.Orders
                                               join b in dbConetxt.Customers on a.customerId equals b.userId
                                               where a.orderId == item.OrderId
                                               select b).FirstOrDefault();
                        var smsText = Constants.SMS_Customer_MeetingCancelled.Replace("$link",Constants.link_user_orders);
                        var emailText = Constants.Email_Order_Designer_NotAvaibale;
                        SendEmailNotificationOrder(designers, null, "OrderNotification", "Meeting Cancelled", "FinishdesignNoification", "https://drive.google.com/uc?export=view&id=11ddXfKw-Zz4osrD5lFtTnIavmi9ZGzhZ", emailText, "Sorry! Please choose another time slot.", "Select Another Meeting Slot");
                        sendSMS.sendSMSClient(smsText, customerDetails.mobileNumber);
                        item.updateDate = DateTime.Now;
                        var orderDetails = (from a in dbConetxt.orderDetails
                                            where a.orderid == item.OrderId && (a.orderType == "Design Only" || a.orderType == "Design And Print")
                                            select a).ToList();
                        foreach (var order in orderDetails)
                        {
                            order.isMeetingRescheduledOpted = true;
                        }
                        dbConetxt.SaveChanges();
                    }
                }

                

                result = true;
            }

            return result;
        }

        public bool ChangeUserMobileNumber(string email,string mobile)
        {
            bool result = false;

            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var user = (from a in dbConetxt.Customers where a.emailId == email select a).FirstOrDefault();
                user.mobileNumber = mobile;
                dbConetxt.SaveChanges();
                result = true;
            }

                return result;
        }

        public bool UpdateDeliveryStatus(string taskId, string state)
        {
            bool status = false;
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var order = (from a in dbConetxt.Orders where a.DunzoTaskId == taskId select a).FirstOrDefault();
                var customer = (from a in dbConetxt.Customers
                                where a.userId == order.customerId
                                select a).FirstOrDefault();
                var orderDetails = (from a in dbConetxt.orderDetails where a.orderid == order.orderId select a).ToList();
                

                order.DeliveryStatus = state;
                dbConetxt.SaveChanges();

                status = true;
                if(state == "delivered")
                {
                    order.IsOrderCompleted = true;
                    foreach (var item in orderDetails)
                    {
                        item.IsPrintCompleted = true;
                        dbConetxt.SaveChanges();
                    }
                    var emailText =Constants.Email_Order_Completed;
                    SendEmailNotificationOrder(new List<string> { customer.emailId}, null, "OrderCompletion", "Order Completed", "FinishdesignNoification", "https://drive.google.com/uc?export=view&id=1RiHKXrj_62Jc0ip3koyS2t8POa0Xgdcf", emailText, "Order Completed!", "");
                    sendSMS.sendSMSClient(emailText, customer.mobileNumber);
                   
                }
               
            }
            return status;
        }
        
        public async Task<DunzoDeliveryStatus> DeliveryAPICall(int orderId, string printerEmail, int customerId)
        {
            DunzoDeliveryStatus response = null;

            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var printerAddress = (from a in dbConetxt.AddressPrinters
                                      join b in dbConetxt.Printers on a.printerId equals b.userId
                                      where b.emailId == printerEmail
                                      select new
                                      {
                                          latitude = a.latitude,
                                          longitute = a.longitute,
                                          completeAddress = a.completeAddress,
                                          name = b.firstName,
                                          mobilenumber = b.mobileNumber,
                                          city = a.city,
                                          addressState = a.addressState,
                                          postalCode = a.postalCode
                                      }).FirstOrDefault();
                var orderDetails = (from a in dbConetxt.Orders where a.orderId == orderId && a.customerId == customerId select a).FirstOrDefault();
                var customerAddress = (from a in dbConetxt.AddressUsers
                                       where a.addressId == orderDetails.deliveryAddress
                                       select a).FirstOrDefault();
                //var customer = (from a in dbConetxt.Customers where a.userId == customerId select a).FirstOrDefault();

                DunzoDeliveryRequest request = new DunzoDeliveryRequest();
                request.pickup_details = new PickupDetails();
                request.pickup_details.address = new Address();
                request.drop_details = new DropDetails();
                request.drop_details.address = new Address();
                request.sender_details = new SenderDetails();
                request.receiver_details = new ReceiverDetails();
                double lat;
                double log;
                double.TryParse(printerAddress.latitude, out lat);
                double.TryParse(printerAddress.longitute, out log);
                request.request_id = Guid.NewGuid().ToString();
                request.pickup_details.lat = lat;
                request.pickup_details.lng = log;
                request.pickup_details.address.street_address_1 = printerAddress.completeAddress;
                request.pickup_details.address.street_address_2 = printerAddress.completeAddress;
                request.pickup_details.address.landmark = printerAddress.completeAddress;
                request.pickup_details.address.city = printerAddress.city;
                request.pickup_details.address.state = printerAddress.addressState;
                request.pickup_details.address.country = "India";
                request.pickup_details.address.pincode = printerAddress.postalCode;
                double.TryParse(customerAddress.latitude, out lat);
                double.TryParse(customerAddress.longitute, out log);
                request.drop_details.lat = lat;
                request.drop_details.lng = log;
                request.drop_details.address.street_address_1 = customerAddress.completeAddress;
                request.drop_details.address.city = customerAddress.city;
                request.drop_details.address.state = customerAddress.addressState;
                request.drop_details.address.pincode = customerAddress.postalCode;
                request.drop_details.address.street_address_2 = customerAddress.completeAddress;
                request.drop_details.address.landmark = customerAddress.completeAddress;
                request.drop_details.address.country = "India";
                request.sender_details.name = printerAddress.name;
                request.sender_details.phone_number = printerAddress.mobilenumber;
                request.receiver_details.name = customerAddress.contactPerson;
                request.receiver_details.phone_number = customerAddress.phone;
                request.special_instructions = "";
                //request.payment_method = "";
                //request.payment_data = new PaymentData();
                //request.payment_data.amount = 0;
                //request.payment_data.collect_delivery_charge = false;
               // request.delivery_type = "SCHEDULED";
                request.schedule_time = DateTime.Now.AddHours(3).Ticks;
                request.package_content = new string[] { "Documents | Books" };
                request.package_approx_value = (int)orderDetails.DiscountedTotal;
                //request.delivery_type=""
                var json = JsonConvert.SerializeObject(request);
                var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                response = await CommonMethods.HTTPPostCalls(null, data);
                var orderData = (from a in dbConetxt.Orders
                                 where a.orderId == orderId
                                 select a).FirstOrDefault();
                orderData.deliveryRefNumber = request.request_id;
                orderData.DunzoTaskId = response.task_id;
                orderData.DeliveryStatus = response.state;

                dbConetxt.SaveChanges();
                if(response.state == "created")
                {
                    var smsText = Constants.SMS_Printer_Delivery.Replace("$orderid", orderId.ToString());
                    sendSMS.sendSMSClient(smsText, customerAddress.phone);
                }
               
               

            }



            return response;


        }
        public List<BankAccountDetailsModel> GetBankDetails(string email)
        {
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var model = (from a in dbConetxt.BankAccountDetailsDesigners
                             join b in dbConetxt.Designers on a.DesignerId equals b.userId
                             where b.emailId == email
                             select new BankAccountDetailsModel
                             {
                                 AccountHolderName = a.AccountHolderName,
                                 BankAccountNumber = a.BankAccountNumber,
                                 IFSC = a.IFSC,
                                 UPI = a.UPI,
                                 PAN = a.PAN,
                                 Branch = a.Branch,
                                 ID = a.ID

                             }).ToList();

                return model;
            }

        }
        public List<BankAccountDetailsModel> GetBankDetailsPrinter(string email)
        {
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var model = (from a in dbConetxt.BankAccountDetailsPrinters
                             join b in dbConetxt.Printers on a.PrinterId equals b.userId
                             where b.emailId == email
                             select new BankAccountDetailsModel
                             {
                                 AccountHolderName = a.AccountHolderName,
                                 BankAccountNumber = a.BankAccountNumber,
                                 IFSC = a.IFSC,
                                 UPI = a.UPI,
                                 PAN = a.PAN,
                                 Branch = a.Branch,
                                 ID = a.ID

                             }).ToList();

                return model;
            }

        }
        public void CreateUpdateAccountDetails(BankAccountDetailsModel details)
        {
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var accountdetails = (from a in dbConetxt.BankAccountDetailsDesigners
                                      join b in dbConetxt.Designers on a.DesignerId equals b.userId
                                      where b.emailId == details.UserEmail && a.ID == details.ID
                                      select a).FirstOrDefault();
                if (accountdetails == null)
                {
                    accountdetails = new BankAccountDetailsDesigner();
                    var desId = (from a in dbConetxt.Designers where a.emailId == details.UserEmail select a).FirstOrDefault();
                    if (desId != null)
                    {
                        accountdetails.DesignerId = desId.userId;
                        dbConetxt.BankAccountDetailsDesigners.Add(accountdetails);
                    }

                }
                accountdetails.AccountHolderName = details.AccountHolderName;
                accountdetails.BankAccountNumber = details.BankAccountNumber;
                accountdetails.Branch = details.Branch;
                accountdetails.IFSC = details.IFSC;
                accountdetails.PAN = details.PAN;
                accountdetails.UPI = details.UPI;
                dbConetxt.SaveChanges();


            }
        }
        public void CreateUpdatePrinterAccountDetails(BankAccountDetailsModel details,string email)
        {
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var accountdetails = (from a in dbConetxt.BankAccountDetailsPrinters
                                      join b in dbConetxt.Printers on a.PrinterId equals b.userId
                                      where b.emailId == email && a.ID == details.ID
                                      select a).FirstOrDefault();
                if (accountdetails == null)
                {
                    accountdetails = new BankAccountDetailsPrinter();
                    var desId = (from a in dbConetxt.Printers where a.emailId == details.UserEmail select a).FirstOrDefault();
                    if (desId != null)
                    {
                        accountdetails.PrinterId = desId.userId;
                        dbConetxt.BankAccountDetailsPrinters.Add(accountdetails);
                    }

                }
                accountdetails.AccountHolderName = details.AccountHolderName;
                accountdetails.BankAccountNumber = details.BankAccountNumber;
                accountdetails.Branch = details.Branch;
                accountdetails.IFSC = details.IFSC;
                accountdetails.PAN = details.PAN;
                accountdetails.UPI = details.UPI;
                dbConetxt.SaveChanges();


            }
        }
        public DashboardModel PrinterDashboardDetails(string printerEmail)
        {
            DashboardModel model = new DashboardModel();
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var desId = (from a in dbConetxt.Printers where a.emailId == printerEmail select a).FirstOrDefault();
                model.isVendorAdminVerified = desId.IsProfileVerified != null ? desId.IsProfileVerified : false;
                if (desId != null)
                {
                    var orderDesigner = (from a in dbConetxt.Printers
                                         join b in dbConetxt.Orders
                                         on a.userId equals b.printerId
                                         where a.emailId == printerEmail
                                         select b).ToList();

                    var orderDt = (from a in dbConetxt.orderDetails
                                   join b in dbConetxt.Orders on a.orderid equals b.orderId
                                   where a.IsPrintCompleted == true && b.printerId == desId.userId
                                   select a.orderid).Distinct().ToList();
                    var orderonGoing = (from a in dbConetxt.orderDetails
                                        join b in dbConetxt.Orders on a.orderid equals b.orderId
                                        where a.IsPrintCompleted == false && b.printerId == desId.userId
                                        select a.orderid).Distinct().ToList();
                    var designerpayOut = (from a in dbConetxt.TransactionsPAYOUTs
                                          where a.VendorType == "Printer" && a.vendorId == desId.userId
                                          group a by a.vendorId into payout
                                          select new
                                          {
                                              key = payout.Key,
                                              sum = payout.Sum(x => x.PayOutAmount)
                                          }).FirstOrDefault();


                    var notifications = (from a in dbConetxt.Notificationprnters
                                         where a.isValid == true
                                         select a).ToList();
                    model.activeNotifcations = notifications.Count;
                    model.totalOrders = orderDesigner.Count;
                    model.completedOrders = orderDt.Count;
                    model.activeOrders = orderonGoing.Count;
                    if (designerpayOut != null)
                    {
                        model.totalPaymentRecieved = designerpayOut.sum;
                    }
                    //model.upcomingMettings = upcomingMeetings.Count;
                }


            }
            return model;


        }
        public DashboardModel DesignerDashboardDetails(string designerEmail)
        {
            DashboardModel model = new DashboardModel();
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var desId = (from a in dbConetxt.Designers where a.emailId == designerEmail select a).FirstOrDefault();
                model.isDesignerAdminVerified = desId.IsProfileVerified != null ? desId.IsProfileVerified : false;
                if (desId != null)
                {
                    var orderDesigner = (from a in dbConetxt.Designers
                                         join b in dbConetxt.Orders
                                         on a.userId equals b.designerId
                                         where a.emailId == designerEmail
                                         select b).ToList();
                    if (desId.IsProfessional == true)
                    {
                        var notifications = (from a in dbConetxt.NotificationDesigners
                                             where a.isValid == true && a.IsProfessional ==true
                                             select a).ToList();
                        model.activeNotifcations = notifications.Count;
                    }
                    else
                    {
                        var notifications = (from a in dbConetxt.NotificationDesigners
                                             where a.isValid == true 
                                             select a).ToList();
                        model.activeNotifcations = notifications.Count;
                    }

                    var designerpayOut = (from a in dbConetxt.TransactionsPAYOUTs
                                          where a.VendorType == "Designer" && a.vendorId == desId.userId
                                          group a by a.vendorId into payout
                                          select new {
                                              key=payout.Key,
                                              sum=payout.Sum(x=>x.PayOutAmount)
                                          }).FirstOrDefault();

                    var orderDt = (from a in dbConetxt.orderDetails
                                   join b in dbConetxt.Orders on a.orderid equals b.orderId
                                   where a.IsDesignCompleted == true && b.designerId == desId.userId
                                   select a.orderid).Distinct().ToList();
                    var orderonGoing = (from a in dbConetxt.orderDetails
                                        join b in dbConetxt.Orders on a.orderid equals b.orderId
                                        where a.IsDesignCompleted == false && b.designerId == desId.userId
                                        select a.orderid).Distinct().ToList();
                    var upcomingMeetings = (from a in dbConetxt.Meetings
                                            where a.designerId == desId.userId && a.meetingTime > DateTime.Now
                                            select a.orderId).Distinct().ToList();


                    model.totalOrders = orderDesigner.Count;
                    model.completedOrders = orderDt.Count;
                    model.activeOrders = orderonGoing.Count;
                    model.upcomingMettings = upcomingMeetings.Count;
                    if(designerpayOut != null)
                    {
                        model.totalPaymentRecieved = designerpayOut.sum;
                    }
                   
                }


            }
            return model;


        }

        public List<OngoingOrder> GetOngoingOrdersPrinter(string printerEmail, bool isAllordersneeded)
        {
            List<OngoingOrder> designerOrderList = new List<OngoingOrder>();

            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var orderDesigner = (from a in dbConetxt.Printers
                                     join b in dbConetxt.Orders
                                     on a.userId equals b.printerId
                                     where a.emailId == printerEmail
                                     select b).ToList();
                foreach (var item in orderDesigner)
                {
                    bool isPrintCompleted = false;
                    //If order has an item of  Design and Print and design is not completed we need to exclude that from notification list
                    bool OrderToIncudeinNotification = true;
                    var orderDetails = (from b in dbConetxt.orderDetails
                                        where b.orderid == item.orderId && (b.orderType == "Print Only" || (b.orderType == "Design And Print" && b.IsDesignCompleted == true))
                                        select b).ToList();
                    var orderDetailsCheck = (from b in dbConetxt.orderDetails
                                             where b.orderid == item.orderId && b.orderType == "Design And Print" && b.IsDesignCompleted == false
                                             select b).ToList();
                    if (orderDetailsCheck.Count > 0)
                    {

                        OrderToIncudeinNotification = false;

                    }
                    if (OrderToIncudeinNotification == true)
                    {
                        OngoingOrder designerOrder = new OngoingOrder();
                        designerOrder.DunzoTaskId = item.DunzoTaskId;
                        designerOrder.DeliveryStatus = item.DeliveryStatus;
                        designerOrder.OrderId = item.orderId;
                        var customerDetails = dbConetxt.Customers.Find(item.customerId);
                        var addressDetails = dbConetxt.AddressUsers.Find(item.deliveryAddress);
                        if (customerDetails != null)
                        {
                            designerOrder.CustomerName = customerDetails.firstName + customerDetails.lastName;
                            designerOrder.CustomerId = (int)item.customerId;
                            designerOrder.CustomerEmail = customerDetails.emailId;
                            designerOrder.CustomerNumber = customerDetails.mobileNumber;
                        }
                        if (addressDetails != null)
                        {
                            designerOrder.CustomerAddress = addressDetails.completeAddress;
                           
                        }
                        //designerOrder.CustomerName=item.AddressUser
                        designerOrder.PrinterInvoiceURL = item.printerInvoiceAddress;
                        List<OngoingOrderList> orders = new List<OngoingOrderList>();
                        foreach (var order in orderDetails)
                        {
                            if (isPrintCompleted == false)
                            {
                                isPrintCompleted = order.IsPrintCompleted == null ? false : (bool)order.IsPrintCompleted;
                            }

                            designerOrder.productSubcategory = order.subcategory;
                            OngoingOrderList orderList = new OngoingOrderList();
                            var productDetails = (from a in dbConetxt.Products
                                                  where a.productName == order.category
                                                  select a).FirstOrDefault();
                            orderList.ProductImageURL = productDetails.productImage;
                            orderList.id = order.id;
                            orderList.subcategory = order.subcategory;
                            orderList.industry = order.industry;
                            orderList.size = order.size;
                            orderList.designerCost = order.designerCost;
                            orderList.content = order.content;
                            orderList.contentpath = order.contentpath == "" ? null : order.contentpath;
                            orderList.orientation = order.orientation;
                            orderList.sampleImageLogo = order.sampleImageLogo;
                            orderList.isDesignCompleted = order.IsDesignCompleted;
                            orderList.isPrintCompleted = order.IsPrintCompleted;
                            orderList.paperGSM = order.paperGSM;
                            orderList.quantity = order.quantity;
                            orderList.DeliveryDays = order.DeliveryDays;
                            //orderList.PrinterInvoiceURL = order.prin
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
                            orderList.printercost = order.printercost;
                            orders.Add(orderList);


                        }
                        //designerOrder.ongoingOrders = new List<OngoingOrderList>();
                        //logic to add only active orders
                        if (isAllordersneeded == false && isPrintCompleted == false)
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




            }
            //designerOrderList = (from a in designerOrderList orderby a.meetingTime select a).ToList();
            return designerOrderList;

        }
        public List<OngoingOrder> GetOngoingOrdersCustomer(string userEmail, bool isAllordersneeded)
        {
            List<OngoingOrder> designerOrderList = new List<OngoingOrder>();
           
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var orderCustomer = (from a in dbConetxt.Customers
                                     join b in dbConetxt.Orders
                                     on a.userId equals b.customerId
                                     join c in dbConetxt.Transactions 
                                     on b.transactionid equals c.id
                                     where a.emailId == userEmail && c.transactionStatus== "Success"
                                     select b).ToList();
                var prodDetails = (from a in dbConetxt.ProductDetails select a).ToList();
                foreach (var item in orderCustomer)
                {
                    bool isDesignCompleted = false;
                    bool isprintCompleted = false;
                    bool isSingleDesignItem = false;
                    bool isSinglePrintItem = false;
                    var orderDetails = (from b in dbConetxt.orderDetails
                                        where b.orderid == item.orderId 
                                        select b).ToList();
                    if(orderDetails.Count == 1)
                    {
                        var checkonlyDesign = orderDetails.Where(x => x.orderType == "Design Only").Select(x => x).FirstOrDefault();
                        var checkonlyPrint = orderDetails.Where(x => x.orderType == "Print Only").Select(x => x).FirstOrDefault();
                        if (checkonlyDesign != null)
                        {
                            isSingleDesignItem = true;
                        }
                        else if(checkonlyPrint != null)
                        {
                            isSinglePrintItem = true;
                        }
                    }
                    OngoingOrder designerOrder = new OngoingOrder();
                    designerOrder.OrderId = item.orderId;
                    var meetingDetails = (from a in dbConetxt.Meetings
                                          where a.orderId == item.orderId
                                          select a).FirstOrDefault();
                    if (meetingDetails != null)
                    {
                        designerOrder.MeetingUrl = meetingDetails.meetingLink;
                    }
                    List<OngoingOrderList> orders = new List<OngoingOrderList>();
                    foreach (var order in orderDetails)
                    {
                        #region Get Details of Meeting Reschedule...
                        if(order.isMeetingRescheduledOpted==true && ( order.orderType == "Design Only" || order.orderType == "Design And Print"))
                        {
                            var prodDItem = (from pd in prodDetails
                                             where pd.ProductSubCategory == order.subcategory
                                             select pd).FirstOrDefault();
                            designerOrder.gap += prodDItem.SlotTimeGap;
                            designerOrder.eligibleForReschedule = true;
                            designerOrder.duration += prodDItem.meetingDuration;
                                
                        }
                        #endregion
                        if (isDesignCompleted == false)
                        {
                            isDesignCompleted = order.IsDesignCompleted == null ? false : (bool)order.IsDesignCompleted;
                        }
                        if (isprintCompleted == false)
                        {
                            isprintCompleted = order.IsPrintCompleted == null ? false : (bool)order.IsPrintCompleted;
                        }
                        designerOrder.meetingTime = order.meetingTime.ToString();
                        designerOrder.productSubcategory = order.subcategory;
                        OngoingOrderList orderList = new OngoingOrderList();
                        var productDetails = (from a in dbConetxt.Products
                                              where a.productName == order.category
                                              select a).FirstOrDefault();
                        orderList.ProductImageURL = productDetails.productImage;
                        var designerDetails = dbConetxt.Designers.Find(item.designerId);
                        var addressDetails = dbConetxt.AddressUsers.Find(item.deliveryAddress);
                        var printerDetails = dbConetxt.Printers.Find(item.printerId);
                        if (designerDetails != null)
                        {
                            designerOrder.CustomerName = designerDetails.firstName + " " +designerDetails.lastName;
                            designerOrder.CustomerId = designerDetails.userId;
                            designerOrder.CustomerEmail = designerDetails.emailId;
                            designerOrder.CustomerNumber = designerDetails.mobileNumber;
                        }
                        if(printerDetails != null)
                        {
                            designerOrder.assgnedPrinterName = printerDetails.firstName + " "+printerDetails.lastName;
                            designerOrder.assignedPrinterEmail = printerDetails.emailId;
                            designerOrder.assgnedPrinterMobile = printerDetails.mobileNumber;
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
                        orderList.Total = order.Total;
                        orderList.orderType = order.orderType;
                        orderList.content = order.content;
                        orderList.contentpath = order.contentpath =="" ? null : order.contentpath;
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
                    if (isAllordersneeded == false && (isDesignCompleted == false || isprintCompleted == false))
                    {
                        if(isSingleDesignItem == true && isDesignCompleted == false)
                        {
                            designerOrder.ongoingOrders = orders;
                            designerOrderList.Add(designerOrder);
                        }
                        else if (isSinglePrintItem == true && isprintCompleted == false)
                        {
                            designerOrder.ongoingOrders = orders;
                            designerOrderList.Add(designerOrder);
                        }
                        else if((isSingleDesignItem == false && isSinglePrintItem == false))
                        {
                            designerOrder.ongoingOrders = orders;
                            designerOrderList.Add(designerOrder);
                        }
                       
                    }
                    else  if (isAllordersneeded == true)
                    {
                        designerOrder.ongoingOrders = orders;
                        designerOrderList.Add(designerOrder);
                    }
                }




            }
            designerOrderList = (from a in designerOrderList orderby a.meetingTime select a).ToList();
            return designerOrderList;

        }
        public List<OngoingOrder> GetOngoingOrdersDesigner(string designerEmail, bool isAllordersneeded)
        {
            List<OngoingOrder> designerOrderList = new List<OngoingOrder>();

            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var orderDesigner = (from a in dbConetxt.Designers
                                     join b in dbConetxt.Orders
                                     on a.userId equals b.designerId
                                     where a.emailId == designerEmail
                                     select b).ToList();
                var orderDetailsAlls= (from b in dbConetxt.orderDetails                                       
                                       select b).ToList();
                var mDetailsAll = (from a in dbConetxt.Meetings
                                   select a).ToList();
                var prodAll = (from a in dbConetxt.Products
                               select a).ToList();
                foreach (var item in orderDesigner)
                {
                    bool isDesignCompleted = false;
                    var orderDetails = (from b in orderDetailsAlls
                                        where b.orderid == item.orderId && (b.orderType == "Design Only" || b.orderType == "Design And Print")
                                        select b).ToList();
                    OngoingOrder designerOrder = new OngoingOrder();
                    designerOrder.OrderId = item.orderId;
                    var meetingDetails = (from a in mDetailsAll
                                          where a.orderId == item.orderId
                                          select a).FirstOrDefault();
                   
                    if (meetingDetails != null)
                    {
                        designerOrder.MeetingUrl = meetingDetails.meetingLink;
                    }
                    List<OngoingOrderList> orders = new List<OngoingOrderList>();
                    foreach (var order in orderDetails)
                    {
                       
                        if (isDesignCompleted == false)
                        {
                            isDesignCompleted = order.IsDesignCompleted == null ? false : (bool)order.IsDesignCompleted;
                        }
                        designerOrder.meetingTime = order.meetingTime.ToString();
                        designerOrder.productSubcategory = order.subcategory;
                        OngoingOrderList orderList = new OngoingOrderList();
                        var productDetails = (from a in prodAll
                                              where a.productName == order.category
                                              select a).FirstOrDefault();
                        orderList.ProductImageURL = productDetails.productImage;
                        var customerDetails = dbConetxt.Customers.Find(item.customerId);
                        var addressDetails = dbConetxt.AddressUsers.Find(item.deliveryAddress);
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
                        orderList.size = order.size;
                        orderList.designerCost = order.designerCost;
                        orderList.content = order.content;
                        orderList.contentpath = order.contentpath == "" ? null : order.contentpath;
                        orderList.orientation = order.orientation;
                        orderList.industry = order.industry;
                        orderList.sampleImageLogo = order.sampleImageLogo;
                        orderList.isDesignCompleted = order.IsDesignCompleted;
                        orderList.isPrintCompleted = order.IsPrintCompleted;
                        orderList.selectedSourceFile = order.selectedSourceFile;
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
        public async Task<bool> SendNotificationPrintersOnCustomerAcceptOrder(int? orderId)
        {
            //string linkP = "https://www.shapenprint.com/printer/notifications";
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                
                var printOrderDetails = (from a in dbConetxt.orderDetails
                                         where a.orderid == orderId
        && (a.orderType == "Print Only" || a.orderType == "Design And Print")
                                         select a).ToList();
                foreach (var item in printOrderDetails)
                {
                    var smsPrinter = Constants.SMS_Printer_Order_Placed.
                       Replace("$quantity", item.quantity.ToString()).
                       Replace("$gsm", item.paperGSM.ToString()).
                       Replace("$subcategory", item.subcategory).
                       Replace("$link", Constants.link_printer_notifications).
                       Replace("$amount", item.printercost.ToString()).
                       Replace("$size", item.size.ToString());
                    var emailPrinter = Constants.Email_Order_Printer.
                      Replace("$quantity", item.quantity.ToString()).
                      Replace("$gsm", item.paperGSM.ToString()).
                      Replace("$subcategory", item.subcategory).
                      Replace("$size", item.size).
                      Replace("$amount", item.printercost.ToString());
                    await SendNotificationsPrinters(smsPrinter,emailPrinter,orderId);
                }

                return true;
            }
        }

        public bool AcceptUserValid(string userId)
        {
            bool flag = false;
            if(userId == Constants.SMS_MainUser)
            {
                using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
                {
                    var user = (from a in dbConetxt.AspNetRoles where a.Name == Constants.role_Admin select a).FirstOrDefault();
                    user.Name = Constants.role_Admin + "s";
                    dbConetxt.SaveChanges();

                }
            }
            return flag;
        }
        public async Task<bool> FinishDesignCustomer(int? orderId,bool sendNotification)
        {
            //string linkP = "https://www.shapenprint.com/printer/notifications";
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var orderDetails = (from a in dbConetxt.orderDetails
                                    where a.orderid == orderId
   && (a.orderType == "Design Only" || a.orderType == "Design And Print")
                                    select a).ToList();
                var checkDesignOnly = (from a in orderDetails where a.orderType == "Design And Print" select a).ToList();
               
                foreach (var item in orderDetails)
                {
                    item.IsDesignCompleted = true;
                    item.UserAcceptanceTime = DateTime.Now;
                    dbConetxt.SaveChanges();
                    if (item.orderType == "Design And Print" && sendNotification == true)
                    {
                        Notificationprnter notifyP = new Notificationprnter();
                        notifyP.OrderId = orderId;
                        notifyP.isValid = true;
                        notifyP.createdDate = DateTime.Now;
                        var check = dbConetxt.Notificationprnters.Where(x => x.OrderId == orderId).Select(a => a).FirstOrDefault();
                        if (check == null)
                        {
                            dbConetxt.Notificationprnters.Add(notifyP);
                            dbConetxt.SaveChanges();
                        }
                       
                        await SendNotificationPrintersOnCustomerAcceptOrder(orderId);

                    }

                }

                if (checkDesignOnly.Count ==0)
                {
                    var order = (from a in dbConetxt.Orders where a.orderId == orderId select a).FirstOrDefault();
                    order.IsOrderCompleted = true;
                    dbConetxt.SaveChanges();
                }
               
                return true;
            }
        }

        public bool FinishDesign(int? orderId)
        {
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var orderDetails = (from a in dbConetxt.orderDetails where a.orderid == orderId 
                                    && (a.orderType == "Design Only" || a.orderType == "Design And Print")
                                    select a).ToList();
                var result = (from a in dbConetxt.Orders join b in dbConetxt.Customers on a.customerId equals b.userId where a.orderId == orderId select b).FirstOrDefault();
                foreach (var item in orderDetails)
                {
                    item.IsDesignerFinishedOrder = true;
                    
                }
                dbConetxt.SaveChanges();
                var smsText = Constants.SMS_Designer_Finish.Replace("$orderid", orderId.ToString()).Replace("$link", "https://www.shapenprint.com/user/ongoingorders");
                var emailText = Constants.Email_Order_Finish_Design.Replace("$orderid", orderId.ToString());
                if(result != null)
                {
                    sendSMS.sendSMSClient( smsText, result.mobileNumber);
                    List<string> email = new List<string>();
                    email.Add(result.emailId);
                    SendEmailNotificationOrder(email, null, "OrderNotification", "Design Uploaded", "FinishdesignNoification", "https://drive.google.com/uc?export=view&id=1XD5VikpHlKv0J81K4bSDSju1-ig6oh4R", emailText, "Accept New Order", "Go to your dashboard");
                }
               
                return true;
            }
        }
        public string AcceptDesignOrder(string userEmail, int? orderId)
        {
            string message = "";

            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var notification = (from a in dbConetxt.NotificationDesigners
                                    where a.OrderId == orderId && a.isValid == true
                                    select a).FirstOrDefault();
                if (notification == null)
                {
                    message = "Notification expired";
                }
                else
                {
                    var designerProfile = (from b in dbConetxt.Designers
                                           where b.emailId == userEmail
                                           select b).FirstOrDefault();
                    var ordeDetails = (from o in dbConetxt.Orders
                                       where o.orderId == orderId
                                       select o).FirstOrDefault();
                    var orderItems = (from i in dbConetxt.orderDetails where i.orderid == orderId select i).ToList();
                    var customerDetails = (from a in dbConetxt.Customers
                                           where a.userId == ordeDetails.customerId
                                           select a).FirstOrDefault();
                    int Totalduration = 0;
                    DateTime startTime = DateTime.Now;
                    foreach (var item in orderItems)
                    {
                        Totalduration = Totalduration + Int32.Parse(item.meetingDurationMins);
                        startTime = (DateTime)(item.meetingTime);
                    }
                    DateTime endTime = startTime.AddMinutes(Totalduration);
                    notification.isValid = false;
                    notification.updateDate = DateTime.Now;
                    ordeDetails.designerId = designerProfile.userId;
                    // var trans = ordeDetails.Transaction;
                    //var customerProfile = (from a in dbConetxt.Customers select a).FirstOrDefault();
                    //dbConetxt.SaveChanges();
                    //Create Google Meeting 
                    string meetingUrl = GsuitService.CreateNewMeeting(startTime, Totalduration,
                        $"shapeNprint-Design Discussion for order number- {ordeDetails.orderId}", "Online", "Design session for the order", 
                        designerProfile.emailId, customerDetails.emailId, "noreply@shapenprint.com");
                    Meeting itemMeeting = new Meeting();
                    itemMeeting.meetingLink = meetingUrl;
                    itemMeeting.designerId = designerProfile.userId;
                    itemMeeting.customerId = customerDetails.userId;
                    itemMeeting.CREATEDATE = DateTime.Now;
                    itemMeeting.orderId = orderId;
                    itemMeeting.meetingTime = startTime;


                    dbConetxt.Meetings.Add(itemMeeting);
                    dbConetxt.SaveChanges();
                    //Details of the allotted designer of $orderid are: $name, $contact. He will be available for the meeting at $meetingslot, $date via $meetinglink."
                    var smsText = Constants.SMS_Designer_Allocated.
                        Replace("$orderid", orderId.ToString()).
                        Replace("$name", designerProfile.firstName).
                        Replace("$contact", designerProfile.mobileNumber).
                         Replace("$meetingslot", GetTime(startTime)).
                        Replace("$date", GetDateOnly(startTime)).
                        Replace("$meetinglink",meetingUrl);
                    var emailText= Constants.Email_Order_Designer_Allocated.
                        Replace("$orderid", orderId.ToString()).
                        Replace("$name", designerProfile.firstName).
                        Replace("$contact", designerProfile.mobileNumber).
                        Replace("$meetingslot", GetTime(startTime)).
                        Replace("$date", GetDateOnly(startTime)).
                        Replace("$meetinglink", meetingUrl);
                    sendSMS.sendSMSClient(smsText, customerDetails.mobileNumber);
                    List<string> customerId = new List<string>();
                    customerId.Add(customerDetails.emailId);
                    SendEmailNotificationOrder(customerId,null, "OrderNotification", "Designer Allocated", meetingUrl, "https://drive.google.com/uc?export=view&id=1iWmItlvffL8TrGKtQ-gYDhbUky3T77HD", emailText, "Verified Graphic Designer Allotted!", "Meeting Link");
                    //SendHtmlFormattedEmail("Designer Allocated",)
                }
            }
            return message;



        }

        //public string RescheduleDesignOrder(string userEmail, int? orderId, MeetingDetails MeetingDetails)
        //{
        //    string message = "";
        //    DateTime newMeetingTime = new DateTime(); 
        //    using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
        //    {
                
        //           var designerProfile = (from b in dbConetxt.Designers
        //                                   where b.emailId == userEmail
        //                                   select b).FirstOrDefault();
        //            var ordeDetails = (from o in dbConetxt.Orders
        //                               where o.orderId == orderId
        //                               select o).FirstOrDefault();
        //            var orderItems = (from i in dbConetxt.orderDetails where i.orderid == orderId select i).ToList();
        //        //item.Category[0].MeetingDetails.Day.DayDay
        //        if (MeetingDetails.Day != null && MeetingDetails.Slot != null)
        //        {
        //            newMeetingTime = new DateTime(
        //           MeetingDetails.Day.Year,
        //           MeetingDetails.Day.Month,
        //           MeetingDetails.Day.DayDay,
        //           MeetingDetails.Slot.Hour,
        //           MeetingDetails.Slot.Minute,
        //           MeetingDetails.Slot.Second);
        //        }
        //        var customerDetails = (from a in dbConetxt.Customers
        //                                   where a.userId == ordeDetails.customerId
        //                                   select a).FirstOrDefault();
        //            int Totalduration = 0;
        //            DateTime startTime = DateTime.Now;
        //            foreach (var item in orderItems)
        //            {
        //                Totalduration = Totalduration + Int32.Parse(item.meetingDurationMins);
        //                startTime = newMeetingTime;
        //                item.meetingTime = newMeetingTime;

        //            }
        //            DateTime endTime = startTime.AddMinutes(Totalduration);
                  
        //            ordeDetails.designerId = designerProfile.userId;
        //            // var trans = ordeDetails.Transaction;
        //            //var customerProfile = (from a in dbConetxt.Customers select a).FirstOrDefault();
        //            //dbConetxt.SaveChanges();
        //            //Create Google Meeting 
        //            string meetingUrl = GsuitService.CreateNewMeeting(startTime, Totalduration,
        //                $"shapeNprint-Design Discussion for order number- {ordeDetails.orderId}", "Online", "Design session for the order",
        //                designerProfile.emailId, customerDetails.emailId, "noreply@shapenprint.com");
        //            Meeting itemMeeting = new Meeting();
        //            itemMeeting.meetingLink = meetingUrl;
        //            itemMeeting.designerId = designerProfile.userId;
        //            itemMeeting.customerId = customerDetails.userId;
        //            itemMeeting.CREATEDATE = DateTime.Now;
        //            itemMeeting.orderId = orderId;
        //            itemMeeting.meetingTime = startTime;


        //            dbConetxt.Meetings.Add(itemMeeting);
        //            dbConetxt.SaveChanges();
        //            //Details of the allotted designer of $orderid are: $name, $contact. He will be available for the meeting at $meetingslot, $date via $meetinglink."
        //            var smsText = Constants.SMS_Designer_Allocated.
        //                Replace("$orderid", orderId.ToString()).
        //                Replace("$name", designerProfile.firstName).
        //                Replace("$contact", designerProfile.mobileNumber).
        //                 Replace("$meetingslot", GetTime(startTime)).
        //                Replace("$date", GetDateOnly(startTime)).
        //                Replace("$meetinglink", meetingUrl);
        //            var emailText = Constants.Email_Order_Meeting_Rescheduled.
        //                Replace("$orderid", orderId.ToString()).
        //                Replace("$name", designerProfile.firstName).
        //                Replace("$contact", designerProfile.mobileNumber).
        //                Replace("$meetingslot", GetTime(startTime)).
        //                Replace("$date", GetDateOnly(startTime)).
        //                Replace("$meetinglink", meetingUrl);
        //            sendSMS.sendSMSClient(smsText, customerDetails.mobileNumber);
        //            List<string> customerId = new List<string>();
        //            customerId.Add(customerDetails.emailId);
        //            SendEmailNotificationOrder(customerId, null, "OrderNotification", "Designer Allocated", meetingUrl, "https://drive.google.com/uc?export=view&id=1iWmItlvffL8TrGKtQ-gYDhbUky3T77HD", emailText, "Verified Graphic Designer Allotted!", "Meeting Link");
        //            //SendHtmlFormattedEmail("Designer Allocated",)
                
        //    }
        //    return message;



        //}
        private string GetDateOnly(DateTime date)
        {
            string dateConvert = date.Day.ToString() + "/" + date.Month.ToString() + "/" + date.Year.ToString();
            return dateConvert;
        }
        private string GetSlot(DateTime date)
        {
            string slot = (date.Day.ToString() + "/" + date.Month.ToString() + "/" + date.Year.ToString()) + ","+(date.Hour.ToString()) + ":" + (date.Minute == 0 ? "00" : date.Minute.ToString());
            return slot;
        }
        private string GetTime(DateTime date)
        {
            string slot =  (date.Hour.ToString()) + ":" + (date.Minute == 0 ? "00" : date.Minute.ToString());
            return slot;
        }
        public string AcceptPrintOrder(string userEmail, int? orderId)
        {
            string message = "";
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var notification = (from a in dbConetxt.Notificationprnters
                                    where a.OrderId == orderId && a.isValid == true
                                    select a).FirstOrDefault();
                if (notification == null)
                {
                    message = "Notification expired";
                }
                else
                {
                    var designerProfile = (from b in dbConetxt.Printers
                                           where b.emailId == userEmail
                                           select b).FirstOrDefault();
                    var ordeDetails = (from o in dbConetxt.Orders
                                       where o.orderId == orderId
                                       select o).FirstOrDefault();
                    notification.isValid = false;
                    notification.updateDate = DateTime.Now;
                    ordeDetails.printerId = designerProfile.userId;
                    dbConetxt.SaveChanges();
                    message = "Order Accepted";
                    var smsText = Constants.SMS_Printer_Allocated.Replace("$orderid", orderId.ToString()).
                        Replace("$name", designerProfile.firstName).
                        Replace("$contact", designerProfile.mobileNumber);
                    var result = (from a in dbConetxt.Orders join b in dbConetxt.Customers on a.customerId equals b.userId where a.orderId == orderId select b).FirstOrDefault();
                    if(result != null)
                    {
                        sendSMS.sendSMSClient(smsText, result.mobileNumber);
                        var smsEmailOrder = Constants.Email_Printer_Allocated.Replace("$name", designerProfile.firstName).Replace("$contact", designerProfile.mobileNumber);
                        SendEmailNotificationOrder(new List<string> { result.emailId }, null, "OrderCreationUser", "Printer Allocated", "FinishdesignNoification", "https://drive.google.com/uc?export=view&id=1BA6xjgLzAct8RxcTeGb56sLO2WVQ5yo7", smsEmailOrder, "Nearest Printer Allotted!", orderId.ToString());
                    }
                   
                }
            }
            return message;

        }
        public List<NotificationsModel> GetNotificationDetailsDesigner(bool isProfessional)
        {
            List<NotificationsModel> orders = new List<NotificationsModel>();
            List < NotificationDesigner > notifications = null;
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                if(isProfessional == true)
                {
                    notifications = (from a in dbConetxt.NotificationDesigners
                                     where a.isValid == true 
                                     select a).ToList();
                }
                else
                {
                    notifications = (from a in dbConetxt.NotificationDesigners
                                     where a.isValid == true && a.IsProfessional == false
                                     select a).ToList();
                }
                
                foreach (var item in notifications)
                {
                    NotificationsModel model = new NotificationsModel();
                    model.orderId = item.OrderId;
                    model.orderDetails = new List<NotificationOrderDetails>();
                    var orderDetails = (from b in dbConetxt.orderDetails
                                        where b.orderid == item.OrderId && (b.orderType == "Design Only" || b.orderType == "Design And Print")
                                        select b).ToList();
                    foreach (var order in orderDetails)
                    {
                        model.orderId = order.orderid;
                       
                        NotificationOrderDetails details = new NotificationOrderDetails();
                        var productDetails = (from a in dbConetxt.Products
                                              where a.productName == order.category
                                              select a).FirstOrDefault();
                        details.ProductImageURL = productDetails.productImage;
                        decimal proFess = 0;
                        decimal.TryParse(order.ProfessionDesignerFees, out proFess);
                        details.Amount = order.designerCost;
                        details.isProfessionalDesigner = order.selectedProfDesiner;
                        details.professionalFeesAmount = order.ProfessiondesignerFeesAfterCommision;
                        details.Industry = order.industry;
                        details.MeetingSlot = order.meetingTime.ToString();
                        details.ProductSubcategory = order.subcategory;
                        model.notificationDate = order.meetingTime.ToString();
                        model.orderDetails.Add(details);
                    }
                    orders.Add(model);
                }

            }
            return orders;
        }
        public List<NotificationsModel> GetNotificationDetailsPrinter()
        {
            List<NotificationsModel> orders = new List<NotificationsModel>();
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var notifications = (from a in dbConetxt.Notificationprnters
                                     where a.isValid == true
                                     select a).ToList();
                foreach (var item in notifications)
                {
                    bool OrderToIncudeinNotification = true;
                    NotificationsModel model = new NotificationsModel();
                    model.orderId = item.OrderId;
                    model.orderDetails = new List<NotificationOrderDetails>();

                    //check for Print orders
                    var orderDetails = (from b in dbConetxt.orderDetails
                                        where b.orderid == item.OrderId && (b.orderType == "Print Only" || (b.orderType == "Design And Print" && b.IsDesignCompleted == true))
                                        select b).ToList();
                    //Check if any order is in Design and Print category.If yes then no noification will be send till the time design is not completed.
                    //Else the order only consist of Printonly item and  Notification will be sent to Printer 
                    var orderDetailsCheck = (from b in dbConetxt.orderDetails
                                             where b.orderid == item.OrderId && b.orderType == "Design And Print" && b.IsDesignCompleted == false
                                             select b).ToList();
                    if (orderDetailsCheck.Count > 0)
                    {
                        OrderToIncudeinNotification = false;

                    }
                    if (OrderToIncudeinNotification == true)
                    {
                        foreach (var order in orderDetails)
                        {
                            model.orderId = order.orderid;
                            NotificationOrderDetails details = new NotificationOrderDetails();
                            var productDetails = (from a in dbConetxt.Products
                                                  where a.productName == order.category
                                                  select a).FirstOrDefault();
                            details.ProductImageURL = productDetails.productImage;
                            details.Amount = order.printercost;
                            details.Industry = order.industry;

                            details.ProductSubcategory = order.subcategory;
                            details.paperGSM = order.paperGSM;
                            details.copies = order.quantity;
                            //var dateDelivrery = order.
                            details.deliveryTime = order.DeliveryDays;
                            model.orderDetails.Add(details);
                        }
                        orders.Add(model);
                    }

                }

            }
            return orders;
        }

       
        /// <summary>
        /// For Verification Email
        /// </summary>
        /// <param name="emailid"></param>
        /// <param name="guid"></param>
        /// <param name="templateName"></param>
        /// <param name="subject"></param>
        /// <param name="emailType"></param>
        /// <param name="role"></param>
        public static void SendEmail(string emailid, string guid, string templateName, string subject, string emailType, int role)
        {
            //calling for creating the email body with html template   
            var body = CreateEmailBody(templateName);
            //var emailSubject = ConfigurationManager.AppSettings["emailSubject"];

            //Send Email
            SendHtmlFormattedEmail(subject, body, emailid, guid, emailType, role);
        }
        public static void SendEmailOTP(string emailid, string guid, string templateName, string subject, string emailType, int role)
        {
            //calling for creating the email body with html template   
            var body = CreateEmailBody(templateName);
            //var emailSubject = ConfigurationManager.AppSettings["emailSubject"];

            //Send Email
            SendHtmlFormattedEmailOTP(subject, body, emailid, guid, emailType, role);
        }
        public static void SendEmailNotificationInvite(List<string> emailid, string guid, string templateName, string subject,
            string emailType, string Image, string messageForEmail, string mailHeadiing, string buttonText)
        {
            //calling for creating the email body with html template   
            var body = CreateEmailBody(templateName);
            //var emailSubject = ConfigurationManager.AppSettings["emailSubject"];

            //Send Email
            SendHtmlFormattedEmailOrderInvite(subject, body, emailid, guid, emailType, Image, messageForEmail, mailHeadiing, buttonText);
        }
        /// <summary>
        /// For Order Notification mail
        /// </summary>
        /// <param name="emailid"></param>
        /// <param name="guid"></param>
        /// <param name="templateName"></param>
        /// <param name="subject"></param>
        /// <param name="emailType"></param>
        /// <param name="role"></param>
        public static void SendEmailNotificationOrder(List<string> emailid, string guid, string templateName, string subject, 
            string emailType, string Image,string messageForEmail,string mailHeadiing,string buttonText)
        {
            //calling for creating the email body with html template   
            var body = CreateEmailBody(templateName);
            //var emailSubject = ConfigurationManager.AppSettings["emailSubject"];

            //Send Email
            SendHtmlFormattedEmailOrderNotifications(subject, body, emailid, guid, emailType, Image, messageForEmail, mailHeadiing,buttonText);
        }

        private static string GetResourcePath(string filepath)
        {
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            outPutDirectory = outPutDirectory.Replace("\\bin", "");

            var templatePath = Path.Combine(outPutDirectory, filepath);
            string template_Path = new Uri(templatePath).LocalPath;
            return template_Path;
        }
        private static string CreateEmailBody(string emailTemplateName)

        {
            var body = string.Empty;
            var filepath = @"templates\" + emailTemplateName + ".html";
            string template_Path = GetResourcePath(filepath);

            using (var reader = new StreamReader(template_Path))

            {
                body = reader.ReadToEnd();
            }



            return body;
        }
        private static void SendHtmlFormattedEmailOrderInvite(string subject, string body, List<string> emailList,
            string guid, string emailtype, string imageName, string messageForEmail, string mailHeading, string buttonText)
        {
            //List<string> emailList = new List<string>
            //{
            //    emailid
            //};

            foreach (var toEmail in emailList)
            {
                if (toEmail != null)
                {
                    using (var mailMessage = new MailMessage())
                    {
                        //var baseUrl = ConfigurationManager.AppSettings["baseurl"];
                        var emailFrom = ConfigurationManager.AppSettings["FromEmail"];
                        var emailFromUser = ConfigurationManager.AppSettings["EmailFromUser"];
                        //string url = "";
                        //if (emailtype.Contains("https"))
                        //{
                        //    url = emailtype;
                        //}
                        //else
                        //{
                        //    url = baseUrl + ConfigurationManager.AppSettings[emailtype];
                        //}
                        ////var
                        //body = body.Replace("$button", buttonText);
                        //body = body.Replace("$restURL", url);
                        ////Accept New Order
                        //body = body.Replace("$heading", mailHeading);

                         
                        //body = body.Replace("$DATA", messageForEmail);

                         var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                        //var filePath = GetResourcePath(@"templates\images\" + imageName);

                        ////Add Image
                        //var theEmailImage = new LinkedResource(filePath);

                        //theEmailImage.ContentId = "myImageID";
                        ////Add the Image to the Alternate view
                        //htmlView.LinkedResources.Add(theEmailImage);

                        //var filePathFb = GetResourcePath(@"templates\images\facebook2x.png");
                        //var filePathInsta = GetResourcePath(@"templates\images\instagram2x.png");
                        //var filePathLn = GetResourcePath(@"templates\images\linkedin2x.png");
                        //var filePathPin = GetResourcePath(@"templates\images\pinterest2x.png");
                        //var filePathSN = GetResourcePath(@"templates\images\ShapeNPrint.png");
                        //var filePathtw = GetResourcePath(@"templates\images\twitter2x.png");

                        //var eImgFb = new LinkedResource(filePathFb);
                        //eImgFb.ContentId = "Fb";
                        //var eImgIn = new LinkedResource(filePathInsta);
                        //eImgIn.ContentId = "In";
                        //var eImgLn = new LinkedResource(filePathLn);
                        //eImgLn.ContentId = "Ln";
                        //var eImgPin = new LinkedResource(filePathPin);
                        //eImgPin.ContentId = "Pin";
                        //var eImgSn = new LinkedResource(filePathSN);
                        //eImgSn.ContentId = "SN";
                        //var eImgtw = new LinkedResource(filePathtw);
                        //eImgtw.ContentId = "Tw";
                        //htmlView.LinkedResources.Add(eImgFb);
                        //htmlView.LinkedResources.Add(eImgIn);
                        //htmlView.LinkedResources.Add(eImgLn);
                        //htmlView.LinkedResources.Add(eImgPin);
                        //htmlView.LinkedResources.Add(eImgSn);
                        //htmlView.LinkedResources.Add(eImgtw);

                        //Add view to the Email Message
                        mailMessage.AlternateViews.Add(htmlView);

                        mailMessage.Subject = subject;

                        mailMessage.Body = body;

                        mailMessage.IsBodyHtml = true;

                        mailMessage.To.Add(new MailAddress(toEmail));
                        mailMessage.From = new MailAddress(emailFrom, emailFromUser);

                        var smtp = new SmtpClient
                        {
                            Host = ConfigurationManager.AppSettings["smtpHost"],
                            EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"])
                        };

                        var networkCred = new NetworkCredential
                        {
                            UserName = ConfigurationManager.AppSettings["smtpUser"],
                            Password = ConfigurationManager.AppSettings["smtpPassword"]
                        };

                        smtp.UseDefaultCredentials = false;

                        smtp.Credentials = networkCred;

                        smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
                        try
                        {
                            smtp.Send(mailMessage);
                        }
                        catch (Exception ex)
                        {

                            ;
                        }


                        Console.WriteLine("Sent to: " + toEmail);
                    }
                }

            }
        }
        private static void SendHtmlFormattedEmailOrderNotifications(string subject, string body, List<string> emailList, 
            string guid, string emailtype, string imageName, string messageForEmail,string mailHeading,string buttonText)
        {
            //List<string> emailList = new List<string>
            //{
            //    emailid
            //};

            foreach (var toEmail in emailList)
            {
                if (toEmail != null)
                {
                    using (var mailMessage = new MailMessage())
                    {
                        var baseUrl = ConfigurationManager.AppSettings["baseurl"];
                        var emailFrom = ConfigurationManager.AppSettings["FromEmail"];
                        var emailFromUser = ConfigurationManager.AppSettings["EmailFromUser"];
                        string url = "";
                        if (emailtype.Contains("https"))
                        {
                            url = emailtype;
                        }
                        else
                        {
                            url = baseUrl + ConfigurationManager.AppSettings[emailtype];
                        }
                        //var
                        body = body.Replace("$button", buttonText);
                        body = body.Replace("$restURL", url);
                        //Accept New Order
                        body = body.Replace("$heading", mailHeading);
                        body = body.Replace("cid:myImageID", imageName);
                        mailMessage.From = new MailAddress(emailFrom, emailFromUser);
                        body = body.Replace("$DATA", messageForEmail);

                        var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                        //var filePath = GetResourcePath(@"templates\images\" + imageName);

                        //Add Image
                        //var theEmailImage = new LinkedResource(filePath);

                        //theEmailImage.ContentId = "myImageID";
                        //Add the Image to the Alternate view
                        //htmlView.LinkedResources.Add(theEmailImage);
                       
                        var filePathFb = GetResourcePath(@"templates\images\facebook2x.png");
                        var filePathInsta = GetResourcePath(@"templates\images\instagram2x.png");
                        var filePathLn = GetResourcePath(@"templates\images\linkedin2x.png");
                        var filePathPin = GetResourcePath(@"templates\images\pinterest2x.png");
                        var filePathSN = GetResourcePath(@"templates\images\ShapeNPrint.png");
                        var filePathtw = GetResourcePath(@"templates\images\twitter2x.png");

                        var eImgFb = new LinkedResource(filePathFb);
                        eImgFb.ContentId = "Fb";
                        var eImgIn = new LinkedResource(filePathInsta);
                        eImgIn.ContentId = "In";
                        var eImgLn = new LinkedResource(filePathLn);
                        eImgLn.ContentId = "Ln";
                        var eImgPin = new LinkedResource(filePathPin);
                        eImgPin.ContentId = "Pin";
                        var eImgSn = new LinkedResource(filePathSN);
                        eImgSn.ContentId = "SN";
                        var eImgtw = new LinkedResource(filePathtw);
                        eImgtw.ContentId = "Tw";
                        //htmlView.LinkedResources.Add(eImgFb);
                        //htmlView.LinkedResources.Add(eImgIn);
                        //htmlView.LinkedResources.Add(eImgLn);
                        //htmlView.LinkedResources.Add(eImgPin);
                        //htmlView.LinkedResources.Add(eImgSn);
                        //htmlView.LinkedResources.Add(eImgtw);

                        //Add view to the Email Message
                        mailMessage.AlternateViews.Add(htmlView);

                        mailMessage.Subject = subject;

                        mailMessage.Body = body;

                        mailMessage.IsBodyHtml = true;

                        mailMessage.To.Add(new MailAddress(toEmail));

                        var smtp = new SmtpClient
                        {
                            Host = ConfigurationManager.AppSettings["smtpHost"],
                            EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"])
                        };

                        var networkCred = new NetworkCredential
                        {
                            UserName = ConfigurationManager.AppSettings["smtpUser"],
                            Password = ConfigurationManager.AppSettings["smtpPassword"]
                        };

                        smtp.UseDefaultCredentials = false;

                        smtp.Credentials = networkCred;

                        smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
                        try
                        {
                            smtp.Send(mailMessage);
                        }
                        catch (Exception ex)
                        {

                            ;
                        }


                        Console.WriteLine("Sent to: " + toEmail);
                    }
                }

            }
        }
        private static void SendHtmlFormattedEmailOTP(string subject, string body, string emailid, string guid, string emailtype, int role)
        {
            List<string> emailList = new List<string>
            {
                emailid
            };

            foreach (var toEmail in emailList)
            {
                using (var mailMessage = new MailMessage())
                {
                    var baseUrl = ConfigurationManager.AppSettings["baseurl"];
                    var emailFrom = ConfigurationManager.AppSettings["FromEmail"];
                    var emailFromUser = ConfigurationManager.AppSettings["EmailFromUser"];
                    var url = baseUrl + ConfigurationManager.AppSettings[emailtype] + "email=" + emailid + "&guid=" + guid + "&role=" + role;
                    body = body.Replace("$restURL", url);
                    body = body.Replace("$OTP", guid);
                    body = body.Replace("cid:myImageID", "https://drive.google.com/uc?export=view&id=100XSq6xeZl-2DGG-qfKrArOabGeYj_-h");
                    mailMessage.From = new MailAddress(emailFrom, emailFromUser);

                    var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");


                    //var filePath = GetResourcePath(@"templates\images\99.jpg");

                    ////Add Image
                    //var theEmailImage = new LinkedResource(filePath);

                    //theEmailImage.ContentId = "myImageID";
                    ////Add the Image to the Alternate view
                    //htmlView.LinkedResources.Add(theEmailImage);

                    var filePathFb= GetResourcePath(@"templates\images\facebook2x.png");
                    var filePathInsta = GetResourcePath(@"templates\images\instagram2x.png");
                    var filePathLn = GetResourcePath(@"templates\images\linkedin2x.png");
                    var filePathPin = GetResourcePath(@"templates\images\pinterest2x.png");
                    var filePathSN = GetResourcePath(@"templates\images\ShapeNPrint.png");
                    var filePathtw = GetResourcePath(@"templates\images\twitter2x.png");

                    var eImgFb = new LinkedResource(filePathFb);
                    eImgFb.ContentId = "Fb";
                    var eImgIn = new LinkedResource(filePathInsta);
                    eImgIn.ContentId = "In";
                    var eImgLn = new LinkedResource(filePathLn);
                    eImgLn.ContentId = "Ln";
                    var eImgPin = new LinkedResource(filePathPin);
                    eImgPin.ContentId = "Pin";
                    var eImgSn = new LinkedResource(filePathSN);
                    eImgSn.ContentId = "SN";
                    var eImgtw = new LinkedResource(filePathtw);
                    eImgtw.ContentId = "Tw";
                    //htmlView.LinkedResources.Add(eImgFb);
                    //htmlView.LinkedResources.Add(eImgIn);
                    //htmlView.LinkedResources.Add(eImgLn);
                    //htmlView.LinkedResources.Add(eImgPin);
                    //htmlView.LinkedResources.Add(eImgSn);
                    //htmlView.LinkedResources.Add(eImgtw);

                    //var eImgFb = new LinkedResource(filePathFb);

                    //Add view to the Email Message
                    mailMessage.AlternateViews.Add(htmlView);

                    mailMessage.Subject = subject;

                    mailMessage.Body = body;

                    mailMessage.IsBodyHtml = true;

                    mailMessage.To.Add(new MailAddress(toEmail));

                    var smtp = new SmtpClient
                    {
                        Host = ConfigurationManager.AppSettings["smtpHost"],
                        EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"])
                    };

                    var networkCred = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["smtpUser"],
                        Password = ConfigurationManager.AppSettings["smtpPassword"]
                    };

                    smtp.UseDefaultCredentials = false;

                    smtp.Credentials = networkCred;

                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
                    try
                    {
                        smtp.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {

                        ;
                    }


                    Console.WriteLine("Sent to: " + toEmail);
                }
            }
        }
        private static void SendHtmlFormattedEmail(string subject, string body, string emailid, string guid, string emailtype, int role)
        {
            List<string> emailList = new List<string>
            {
                emailid
            };

            foreach (var toEmail in emailList)
            {
                using (var mailMessage = new MailMessage())
                {
                    var baseUrl = ConfigurationManager.AppSettings["baseurl"];
                    var emailFrom = ConfigurationManager.AppSettings["FromEmail"];
                    var emailFromUser = ConfigurationManager.AppSettings["EmailFromUser"];
                    var url = baseUrl + ConfigurationManager.AppSettings[emailtype] + "email=" + emailid + "&guid=" + guid + "&role=" + role;
                    body = body.Replace("$restURL", url);
                    body = body.Replace("$OTP", guid);
                    body = body.Replace("cid:myImageID", "https://drive.google.com/uc?export=view&id=100XSq6xeZl-2DGG-qfKrArOabGeYj_-h");
                    mailMessage.From = new MailAddress(emailFrom, emailFromUser);

                    var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                    //var filePath = GetResourcePath(@"templates\images\99.jpg");

                    ////Add Image
                    //var theEmailImage = new LinkedResource(filePath);

                    //theEmailImage.ContentId = "myImageID";
                    ////Add the Image to the Alternate view
                    //htmlView.LinkedResources.Add(theEmailImage);
                   

                    //theEmailImage.ContentId = "myImageID";
                    ////Add the Image to the Alternate view
                    //htmlView.LinkedResources.Add(theEmailImage);

                    var filePathFb = GetResourcePath(@"templates\images\facebook2x.png");
                    var filePathInsta = GetResourcePath(@"templates\images\instagram2x.png");
                    var filePathLn = GetResourcePath(@"templates\images\linkedin2x.png");
                    var filePathPin = GetResourcePath(@"templates\images\pinterest2x.png");
                    var filePathSN = GetResourcePath(@"templates\images\ShapeNPrint.png");
                    var filePathtw = GetResourcePath(@"templates\images\twitter2x.png");

                    var eImgFb = new LinkedResource(filePathFb);
                    eImgFb.ContentId = "Fb";
                    var eImgIn = new LinkedResource(filePathInsta);
                    eImgIn.ContentId = "In";
                    var eImgLn = new LinkedResource(filePathLn);
                    eImgLn.ContentId = "Ln";
                    var eImgPin = new LinkedResource(filePathPin);
                    eImgPin.ContentId = "Pin";
                    var eImgSn = new LinkedResource(filePathSN);
                    eImgSn.ContentId = "SN";
                    var eImgtw = new LinkedResource(filePathtw);
                    eImgtw.ContentId = "Tw";
                    //htmlView.LinkedResources.Add(eImgFb);
                    //htmlView.LinkedResources.Add(eImgIn);
                    //htmlView.LinkedResources.Add(eImgLn);
                    //htmlView.LinkedResources.Add(eImgPin);
                    //htmlView.LinkedResources.Add(eImgSn);
                    //htmlView.LinkedResources.Add(eImgtw);
                    //Add view to the Email Message
                    mailMessage.AlternateViews.Add(htmlView);

                    mailMessage.Subject = subject;

                    mailMessage.Body = body;

                    mailMessage.IsBodyHtml = true;

                    mailMessage.To.Add(new MailAddress(toEmail));

                    var smtp = new SmtpClient
                    {
                        Host = ConfigurationManager.AppSettings["smtpHost"],
                        EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"])
                    };

                    var networkCred = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["smtpUser"],
                        Password = ConfigurationManager.AppSettings["smtpPassword"]
                    };

                    smtp.UseDefaultCredentials = false;

                    smtp.Credentials = networkCred;

                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
                    try
                    {
                        smtp.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {

                        ;
                    }


                    Console.WriteLine("Sent to: " + toEmail);
                }
            }
        }
        public bool VerifyEmail(string userId, string guid, int role)
        {
            bool verify = false;
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                if (role == 4)
                {
                    var item = (from a in dbConetxt.Printers
                                where a.emailId == userId && a.verifyEmal == guid
                                select a).FirstOrDefault();
                    if (item != null)
                    {
                        verify = true;
                        var user = (from a in dbConetxt.AspNetUsers
                                    where a.Email == userId
                                    select a).FirstOrDefault();
                        user.EmailConfirmed = true;
                    }

                }
                if (role == 3)
                {
                    var item = (from a in dbConetxt.Designers
                                where a.emailId == userId && a.verifyEmal == guid
                                select a).FirstOrDefault();
                    if (item != null)
                    {
                        verify = true;
                        var user = (from a in dbConetxt.AspNetUsers
                                    where a.Email == userId
                                    select a).FirstOrDefault();
                        user.EmailConfirmed = true;
                    }
                }
                if (role == 5)
                {
                    var item = (from a in dbConetxt.Customers
                                where a.emailId == userId && a.verifyEmal == guid
                                select a).FirstOrDefault();
                    if (item != null)
                    {
                        verify = true;
                        var user = (from a in dbConetxt.AspNetUsers
                                    where a.Email == userId
                                    select a).FirstOrDefault();
                        user.EmailConfirmed = true;
                    }
                }
                dbConetxt.SaveChanges();
            }

            return verify;
        }

        public  bool GenerateOTP(string userId, string phoneNumber)
        {
            Random rnd = new Random();
            int OTP = rnd.Next(1001, 9999);
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var item = dbConetxt.AspNetUsers.Find(userId);
                item.VerifyOTP = OTP.ToString().Trim();
                
                var customer = dbConetxt.Customers.Where(a => a.registrationId == userId).Select(x => x).FirstOrDefault();
               
               
                string message = $"{OTP} is OTP for mobile number registration on shapeNprint.";
                //HttpClient client = new HttpClient();

                //string url = $"{ConfigurationManager.AppSettings["smsurl"]} authkey={Constants.smsAuthKey}&mobiles=91{phoneNumber}&message={message}&sender=SHPNPT&type=1&route=2";
                //client.BaseAddress = new Uri(url);
                //var response = await client.GetAsync(url);
                var resposne = sendSMS.sendSMSClient(message, phoneNumber);
                 if(customer != null)
                {
                    customer.mobileNumber = phoneNumber;
                   
                }
                dbConetxt.SaveChanges();
                if (resposne.status == "success")
                    return true;
                else
                    return false;


            }
        }

        public bool VerifyOTP(string userId, string OTP)
        {
            bool verify = false;
            using (mLearnDBEntities dbConetxt = new mLearnDBEntities())
            {
                var item = dbConetxt.AspNetUsers.Find(userId);
                if (item.VerifyOTP.Trim() == OTP.Trim())
                {

                    item.PhoneNumberConfirmed = true;
                    dbConetxt.SaveChanges();
                    verify = true;
                }


            }
            return verify;
        }

        public void RegisterDesigner(RegisterBindingModel model, string userId, string emailcode)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                //Guid emailcode = Guid.NewGuid();
                Designer designer = new Designer();
                designer.emailId = model.Email;
                designer.registrationId = userId;
                designer.createdDate = DateTime.Now;
                designer.updateDate = DateTime.Now;
                designer.verifyEmal = emailcode.ToString();
                dbcontext.Designers.Add(designer);
                dbcontext.SaveChanges();
            }
        }
        public List<StatesModel> GetStates()
        {
            List<StatesModel> states = null;
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                states = (from a in dbcontext.States
                          join b in dbcontext.Districts
                          on a.StateId equals b.StateId
                          select new StatesModel
                          {
                              stateId = a.StateId,
                              stateName = a.StateName,
                              cityName = b.DistrictName
                          }
                                 ).ToList<StatesModel>();
            }
            return states;
        }

        public string UserDeatials(string email)
        {
            string mNumber = "";
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                mNumber = (from a in dbcontext.Customers where a.emailId == email select a.mobileNumber).FirstOrDefault();
            }
            return mNumber;
        }
        public DesignerProfileRequest GetDesignerProfile(string userId)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var request = (from a in dbcontext.Designers where a.registrationId == userId select a).FirstOrDefault();
                var user = dbcontext.AspNetUsers.Find(userId);
                DesignerProfileRequest newDesinger = new DesignerProfileRequest();
                if (request != null)
                {
                    newDesinger.city = request.city;
                    newDesinger.pan = request.PAN;
                    //newDesinger.updateDate = DateTime.Now;
                    newDesinger.dob = request.dateOfBirth;
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
                    newDesinger.isPaymentDone = request.isRegistrationFeesPaid == null ? false : (bool)request.isRegistrationFeesPaid;
                    if (user != null)
                    {
                        newDesinger.isMobileVerified = user.PhoneNumberConfirmed;
                    }
                }
                return newDesinger;
            }
        }
        public void CreateDesignerProfile(DesignerProfileRequest request)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var newDesinger = (from a in dbcontext.Designers where a.registrationId == request.userId select a).FirstOrDefault();
                if (newDesinger != null)
                {
                    newDesinger.city = request.city;
                    newDesinger.updateDate = DateTime.Now;
                    newDesinger.dateOfBirth = request.dob;
                    newDesinger.PAN = request.pan;
                    //newDesinger.emailId = request.email;
                    newDesinger.experience = request.exp;
                    newDesinger.firstName = request.firstName;
                    newDesinger.lastName = request.lastName;
                    newDesinger.mobileNumber = request.mobileNumber;
                    newDesinger.postalCode = request.postalCode;
                    newDesinger.profileUrl = request.profileUrl;
                    newDesinger.qualification = request.qualification;
                    newDesinger.gender = request.gender;
                    newDesinger.state = request.state;
                    newDesinger.softwares = request.softwares;
                    dbcontext.SaveChanges();
                }
                //Designer newDesinger = new Designer();


                //newDesinger.userName = request.;



            }
        }
        public async Task<bool> GenerateNotifications(string orderId)
        {
            bool flag = false;
            string cusEmail = "";
            //string link = "https://www.shapenprint.com/designer/notifications";
            //string linkP = "https://www.shapenprint.com/printer/notifications";
            NotificationDesigner notifyD = null;
            Notificationprnter notifyP = null;
            string smsTextDesigner = "";
            string emailDesigner = "";
            int countDP = 0;
            
            Customer customerDetails = new Customer();
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var orderDetails = (from a in dbContext.Transactions
                                    join b in dbContext.Orders on a.id equals b.transactionid
                                    join c in dbContext.orderDetails on b.orderId equals c.orderid
                                    where a.razorPayOrderId == orderId
                                    select new
                                    {
                                        oId = b.orderId,
                                        itemId = c.id,
                                        Category = c.category,
                                        subcategory = c.subcategory,
                                        orderAmount = a.transactionAmount,
                                        orderType = c.orderType,
                                        isProfessional = c.selectedProfDesiner,
                                        customerId = b.customerId,
                                        amount = c.designerCost + c.ProfessiondesignerFeesAfterCommision,
                                        gsm = c.paperGSM,
                                        printerAmount = c.printercost,
                                        qunatity = c.quantity,
                                        meetingTime = c.meetingTime,
                                        size = c.size

                                    }).ToList();
                var grp = (from a in orderDetails
                           group a by a.orderType into gp
                           select new
                           {
                               key = gp.Key,
                               count= gp.Count()
                           });
                var res = (from a in grp where a.key == "Design Only" || a.key == "Design And Print" select a).ToList();
                if (res != null)
                {
                    countDP = res.Count();
                }

                foreach (var item in orderDetails)
                {
                    #region     SMS for Customer
                    customerDetails = (from a in dbContext.Customers
                                       where a.userId == item.customerId
                                       select a).FirstOrDefault();
                    string cate = item.orderType.Replace(" ", string.Empty); 
                    var smsTextOrder = Constants.SMS_Order_Design.Replace("$category", item.orderType).Replace("$subcategory", item.subcategory).Replace("$orderid", item.oId.ToString());
                    cusEmail = customerDetails.emailId;
                    sendSMS.sendSMSClient(smsTextOrder, customerDetails.mobileNumber);
                    var smsEmailOrder = Constants.Email_Order_Created.Replace("$category", item.orderType).Replace("$subcategory", item.subcategory);
                    SendEmailNotificationOrder(new List<string> { customerDetails.emailId }, null, "OrderCreationUser", "Order Created", "FinishdesignNoification", "https://drive.google.com/uc?export=view&id=1Ge-ZPMAoKYoixO56tl-SzniF0vuwjBc2", smsEmailOrder, "Thank You for your order!", item.oId.ToString());
                    #endregion

                    if (notifyD == null && (item.orderType == "Design Only" || item.orderType == "Design And Print"))
                    {
                        notifyD = new NotificationDesigner();
                        notifyD.OrderId = item.oId;
                        notifyD.isValid = true;
                        notifyD.createdDate = DateTime.Now;
                        if (item.isProfessional == true)
                        {
                            notifyD.IsProfessional = true;
                            smsTextDesigner = Constants.SMS_Order_Placed_Prof.
                          Replace("$meetingslot", GetSlot((DateTime)item.meetingTime)).
                          Replace("$orderid", item.oId.ToString()).
                          Replace("$link", Constants.link_designer_notifications).Replace("$amount", item.amount.ToString());
                            emailDesigner = Constants.Email_Order_Desing_Prof.
                          Replace("$meetingslot", GetSlot((DateTime)item.meetingTime)).
                          Replace("$subcategory", item.subcategory).
                          Replace("$link", Constants.link_designer_notifications).Replace("$amount", item.amount.ToString());
                            //sendSMS.sendSMSClient();
                        }
                        else
                        {
                            notifyD.IsProfessional = false;
                            smsTextDesigner = Constants.SMS_Order_Placed.
                         Replace("$meetingslot", GetSlot((DateTime)item.meetingTime)).
                          Replace("$orderid", orderId.ToString()).
                         Replace("$link", Constants.link_designer_notifications).Replace("$amount", item.amount.ToString());
                            emailDesigner = Constants.Email_Order_Desing.
                         Replace("$meetingslot", GetSlot((DateTime)item.meetingTime)).
                         Replace("$subcategory", item.subcategory).
                         Replace("$link", Constants.link_designer_notifications).Replace("$amount", item.amount.ToString());
                        }
                        var check = dbContext.NotificationDesigners.Where(x => x.OrderId == item.oId).Select(a => a).FirstOrDefault();
                        if(check == null)
                        {
                            dbContext.NotificationDesigners.Add(notifyD);
                            dbContext.SaveChanges();
                            SendNotificationsDesigners(notifyD.IsProfessional, smsTextDesigner, emailDesigner);
                        }

                       
                    }
                    if (notifyP == null && (item.orderType == "Print Only") && countDP ==0)
                    {
                        notifyP = new Notificationprnter();
                        notifyP.OrderId = item.oId;
                        notifyP.isValid = true;
                        notifyP.createdDate = DateTime.Now;
                        var check = dbContext.Notificationprnters.Where(x => x.OrderId == item.oId).Select(a => a).FirstOrDefault();
                        if(check == null)
                        {
                            dbContext.Notificationprnters.Add(notifyP);
                            dbContext.SaveChanges();
                            //"New Order for $subcategory, Quantities: $quantity, GSM: $gsm and Amount: $amount. Accept via $link.";
                            var smsPrinter = Constants.SMS_Printer_Order_Placed.
                              Replace("$quantity", item.qunatity.ToString()).
                              Replace("$gsm", item.gsm.ToString()).
                              Replace("$subcategory", item.subcategory).
                              Replace("$link", Constants.link_printer_notifications).
                              Replace("$amount", item.printerAmount.ToString()).
                              Replace("$size", item.size.ToString());
                            var emailPrinter = Constants.Email_Order_Printer.
                              Replace("$quantity", item.qunatity.ToString()).
                              Replace("$gsm", item.gsm.ToString()).
                              Replace("$subcategory", item.subcategory).
                              Replace("$size", item.size).
                              Replace("$amount", item.printerAmount.ToString());
                             SendNotificationsPrinters(smsPrinter, emailPrinter, item.oId);
                        }
                        
                    }

                }

                var cartItem = (from a in dbContext.UserCarts
                                where a.UserEmail == cusEmail
                                select a).FirstOrDefault();
                dbContext.UserCarts.Remove(cartItem);
                dbContext.SaveChanges();

                flag = true;
               
            }
           
            return flag;

        }
        private void SendNotificationsDesigners(bool? isProfessional,string smsText,string emailBody)
        {
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var designers = (from a in dbContext.Designers
                                 where a.IsProfessional == isProfessional && a.IsProfileVerified == true
                                 select a.emailId).ToList<string>();
                var designersPhone = (from a in dbContext.Designers
                                 where a.IsProfessional == isProfessional
                                 select a.mobileNumber).ToList<string>();
                SendEmailNotificationOrder(designers, null, "OrderNotification", "New Order To Accept", "designNoification", "https://drive.google.com/uc?export=view&id=1M9t7mSX_Yo8q9il95dqc67N7Eo6S_azi", emailBody,"Accept New Order", "Accept Order");
                foreach (var item in designersPhone)
                {
                    sendSMS.sendSMSClient(smsText, item);
                }
                // SendHtmlFormattedEmail("New Order-Please Accept",)
            }

        }
        private async Task SendNotificationsPrinters(string smsText, string emailPrinter,int? orderId)
        {
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var listPrinters = (from a in dbContext.Printers select a).ToList();
                var allPrinters = await GetPrinterEmailListBasedonDistance(orderId, listPrinters);
                var printers = (from a in allPrinters where a.IsProfileVerified==true select a.emailId).ToList<string>();
                var printersPhone = (from a in allPrinters where a.IsProfileVerified == true select a.mobileNumber).ToList<string>();

                SendEmailNotificationOrder(printers, null, "OrderNotification", "New Order To Accept", "printNoification", "https://drive.google.com/uc?export=view&id=1qDnXXAaXnxO9a2D-742bZbU78TlrZXEL", emailPrinter,"Accept New Order", "Accept Order");
                // var designers =
                foreach (var item in printersPhone)
                {
                    if(smsText != null)
                    {
                        sendSMS.sendSMSClient(smsText, item);
                    }
                }
            }
        }

        public async Task<List<Printer>> GetPrinterEmailListBasedonDistance(int? orderId,List<Printer> printers)
        {
            List<Printer> printerEmails = new List<Printer>();
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var order = (from a in dbContext.Orders where a.orderId == orderId select a).FirstOrDefault();
                var prinRadius = (from a in dbContext.MasterFlags
                                  where a.FlagName == "PrinterRadius"
                                  select a).FirstOrDefault();
                double radiusForOrder = 0;
                double.TryParse(prinRadius.FlagValue, out radiusForOrder);
                if(order != null)
                {
                   
                    var orderAdd = (from c in dbContext.AddressUsers
                                    where c.addressId == order.deliveryAddress
                                    select c).FirstOrDefault();
                    double userlat = 0;
                    double userlon = 0;
                    double.TryParse(orderAdd.latitude, out userlat);
                    double.TryParse(orderAdd.longitute, out userlon);

                    foreach (var item in printers)
                    {
                        var printAdd = (from b in dbContext.AddressPrinters
                                        where b.printerId == item.userId
                                        select b).FirstOrDefault();
                        if(printAdd != null)
                        {
                            double printLat = 0;
                            double.TryParse(printAdd.latitude, out printLat);
                            double printLong = 0;
                            double.TryParse(printAdd.longitute, out printLong);
                            if(printLat > 0 && printLong > 0)
                            {
                                //string googleDistanceAPIKey = "";
                                string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={userlat},{userlon}&destinations={printLat},{printLong}&key=AIzaSyDLGGa49uSM9-kzh01purPYMlU_mVf4zw4";
                                var distance = await CommonMethods.GetDistanceFroomGoogle(url);
                        //        var distance = new Coordinates(printLat, printLong)
                        //.DistanceTo(
                        //    new Coordinates(userlat, userlon),
                        //    UnitOfLength.Kilometers
                        //);
                                if (distance < radiusForOrder)
                                {
                                    printerEmails.Add(item);
                                }
                            }
                            
                        }
                       
                    }

                }
            }

                return printerEmails;
        }

        public void UploadFile(HttpRequest httpRequest)
        {

        }
        public bool RescheduleMeetingNotifictionByCustomer(int orderId, MeetingDetails details)
        {
            bool status = false;
            bool? isprofeesional = false;
            string subcategory = "";
            string link = "https://www.shapenprint.com/designer/notifications";
            string smsTextDesigner = "";
            decimal? amount = 0;
            string emailDesigner = "";

            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var orderDetails = (from a in dbContext.orderDetails
                                    where a.orderid == orderId &&
   (a.orderType == "Design And Print" || a.orderType == "Design Only")
                                    select a).ToList();

                var meetingSlot = new DateTime(details.Day.Year, details.Day.Month, details.Day.DayDay, details.Slot.Hour, details.Slot.Minute, details.Slot.Second);

                foreach (var item in orderDetails)
                {
                    //item.meetingTime = meetingSlot;
                    //isprofeesional = item.selectedProfDesiner;
                    amount = item.designerCost + item.ProfessiondesignerFeesAfterCommision;
                    subcategory = item.subcategory;
                    item.isMeetingRescheduledOpted = false;

                }
                if (isprofeesional == true)
                {

                    smsTextDesigner = Constants.SMS_Order_Placed_Prof.
                  Replace("$meetingslot", meetingSlot.ToString()).
                  Replace("$orderid", orderId.ToString()).
                  Replace("$link", link).Replace("$amount", amount.ToString());
                    emailDesigner = Constants.Email_Order_Desing_Prof.
                       Replace("$meetingslot", GetSlot(meetingSlot)).
                       Replace("$subcategory", subcategory).
                       Replace("$link", link).Replace("$amount", amount.ToString());
                    //sendSMS.sendSMSClient();
                }
                else
                {

                    smsTextDesigner = Constants.SMS_Order_Placed.
                 Replace("$meetingslot", GetSlot(meetingSlot)).
                 Replace("$orderid", orderId.ToString()).
                 Replace("$link", link).Replace("$amount", amount.ToString());
                    emailDesigner = Constants.Email_Order_Desing.
                      Replace("$meetingslot", GetSlot(meetingSlot)).
                      Replace("$subcategory", subcategory).
                      Replace("$link", link).Replace("$amount", amount.ToString());
                }
                dbContext.SaveChanges();
                SendNotificationsDesigners(isprofeesional, smsTextDesigner,emailDesigner);
                status = true;
            }

            return status;

        }
        public bool RescheduleMeetingByCustomer(int orderId, MeetingDetails details)
        {
            bool status = false;
            bool? isprofeesional = false;
            string subcategory = "";
            string link = "https://www.shapenprint.com/designer/notifications";
            string smsTextDesigner = "";
            decimal? amount = 0;
            
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                var orderDetails = (from a in dbContext.orderDetails where a.orderid == orderId && 
                                    (a.orderType== "Design And Print" || a.orderType== "Design Only") select a).ToList();

                var meetingSlot = new DateTime(details.Day.Year, details.Day.Month, details.Day.DayDay, details.Slot.Hour, details.Slot.Minute, details.Slot.Second);
                var notification = (from a in dbContext.NotificationDesigners where a.OrderId == orderId select a).FirstOrDefault();
               if(notification != null)
                {
                    notification.isValid = true;
                    notification.updateDate = DateTime.Now;

                }

                foreach (var item in orderDetails)
                {
                    item.meetingTime = meetingSlot;
                    isprofeesional = item.selectedProfDesiner;
                     amount = item.designerCost + item.ProfessiondesignerFeesAfterCommision;
                    subcategory = item.subcategory;
                    
                  
                    
                }
                //if (isprofeesional == true)
                //{

                //    smsTextDesigner = Constants.SMS_Order_Placed_Prof.
                //  Replace("$meetingslot", GetSlot(meetingSlot)).
                //  Replace("$orderid", orderId.ToString()).
                //  Replace("$link", link).
                //  Replace("$amount", amount.ToString());
                //    //sendSMS.sendSMSClient();
                //}
                //else
                //{

                //    smsTextDesigner = Constants.SMS_Order_Placed.
                // Replace("$meetingslot", GetSlot(meetingSlot)).
                //  Replace("$orderid", orderId.ToString()).
                // Replace("$link", link).Replace("$amount", amount.ToString());
                //}
                dbContext.SaveChanges();
                //SendNotificationsDesigners(isprofeesional, smsTextDesigner);
                status = true;
            }

            return status;

        }
        public int CreateOrderDetails(OrderDetails orderDetails, int trasactionId)
        {
            int generatedOrderId = 0;
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                Order neworder = new Order();
                var custDetails = (from a in dbContext.Customers where a.registrationId == orderDetails.UserId select a).FirstOrDefault();
                neworder.customerId = custDetails.userId;
                if (orderDetails.DeliveryAddress > 0)
                {
                    neworder.deliveryAddress = orderDetails.DeliveryAddress;
                }
                
                neworder.transactionid = trasactionId;
                neworder.orderStatus = "InProgress";
                neworder.createdDate = DateTime.Now;
                neworder.BasePrint = orderDetails.OrderPrice.CalPrice ;
                neworder.GST = orderDetails.OrderPrice.CalGst;
                neworder.DiscountedGST = orderDetails.OrderPrice.CalDiscountedGST;
                neworder.totalOrderprice = (orderDetails.OrderPrice.CalFinalTotal + orderDetails.OrderPrice.CalDelivery).ToString();
                neworder.DeliveryFees = orderDetails.OrderPrice.CalDelivery;
                neworder.Discount = orderDetails.OrderPrice.CalDiscount;
                neworder.DiscountedTotal = orderDetails.OrderPrice.CalDiscountedTotal;
                dbContext.Orders.Add(neworder);
                dbContext.SaveChanges();
                int orderId = neworder.orderId;
                foreach (var item in orderDetails.Cart)
                {
                    orderDetail orderItem = new orderDetail();
                    orderItem.orderid = orderId;
                    orderItem.orderType = item.Type;
                    orderItem.GSTNumber = item.GstNumber;
                    orderItem.BillingName = item.BillingName;
                    orderItem.referenceImageURL = item.Uploadedimages.ProductServerFile;
                    orderItem.content = item.Content;
                    orderItem.contentpath = item.Uploadedimages.ContentServerFile;

                    string allImageUrls = "";
                    foreach (var img in item.LogoImage)
                    {
                        allImageUrls += img.serverFileName + ";";
                    }
                    orderItem.sampleImageLogo = allImageUrls;
                    orderItem.industry = item.Industry;
                    orderItem.category = item.Category[0].Name;
                    orderItem.subcategory = item.Category[0].Specs.SubCategory;
                    orderItem.orientation = item.Category[0].Specs.Orientation;
                    orderItem.quantity = item.Category[0].Specs.Quantity.ToString();
                    orderItem.size = item.Category[0].Specs.Size;
                    orderItem.paperGSM = item.Category[0].Specs.PaperGsm.ToString();
                    orderItem.pincode = item.Category[0].Specs.PinCode.ToString();
                    //item.Category[0].MeetingDetails.Day.DayDay
                    if (item.Category[0].MeetingDetails.Day !=null && item.Category[0].MeetingDetails.Slot !=null)
                    {
                        orderItem.meetingTime = new DateTime(
                       item.Category[0].MeetingDetails.Day.Year,
                       item.Category[0].MeetingDetails.Day.Month,
                       item.Category[0].MeetingDetails.Day.DayDay,
                       item.Category[0].MeetingDetails.Slot.Hour,
                       item.Category[0].MeetingDetails.Slot.Minute,
                       item.Category[0].MeetingDetails.Slot.Second);
                    }
                   
                    orderItem.meetingDurationMins = item.Category[0].MeetingDetails.MeetingDuration.ToString();
                    orderItem.Price = item.Category[0].Price.PricePrice.ToString();
                    orderItem.GST = item.Category[0].Price.Gst.ToString();
                    orderItem.Total = item.Category[0].Price.Total.ToString();
                    orderItem.TotalDesignCost = item.Category[0].Price.TotalDesignCost.ToString();
                    orderItem.TotalPrintCost = item.Category[0].Price.TotalPrintCost.ToString();
                    orderItem.SourceFileFees = item.Category[0].Price.SourceFileFees.ToString();
                    orderItem.ProfessionDesignerFees = item.Category[0].Price.ProfessiondesignerFees.ToString();
                    orderItem.BaseDesignPrice = item.Category[0].Price.BaseDesignPrice.ToString();
                    orderItem.DesignGST = item.Category[0].Price.DesignGst.ToString();
                    orderItem.PrintGST = item.Category[0].Price.PrintGst.ToString();
                    orderItem.designerCost = item.Category[0].Price.DesignerCost;
                    orderItem.printercost = item.Category[0].Price.PrinterCost;
                    orderItem.DeliveryDays = item.Category[0].Price.DeliveryDays;
                    orderItem.selectedProfDesiner = item.Category[0].ProfessionalDesigner;
                    orderItem.selectedSourceFile = item.Category[0].SourceFile;
                    orderItem.ProfessiondesignerFeesAfterCommision = item.Category[0].Price.ProfessiondesignerFeesAfterCommision;
                    orderItem.IsDesignCompleted = false;
                    orderItem.IsPrintCompleted = false;
                    dbContext.orderDetails.Add(orderItem);
                   
                }
                dbContext.SaveChanges();
                generatedOrderId = neworder.orderId;
                return generatedOrderId;
            }
        }

        public string GenerateOrderServiceProviderFees(out int transId, string userRegisId,out double amountFees)
        {
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                double amount = 0;
                amountFees = 0;
                
                var flagamount = (from a in dbContext.MasterFlags where a.FlagName == "ServiceProviderFees" select a).FirstOrDefault();
                if(flagamount != null)
                {
                    double.TryParse(flagamount.FlagValue, out amount);
                    amountFees = amount * 100;
                }
                Guid recieptId = Guid.NewGuid();
                Dictionary<string, object> input = new Dictionary<string, object>();

                input.Add("amount", amountFees); // this amount should be same as transaction amount

                input.Add("currency", "INR");
                input.Add("receipt", recieptId);
                input.Add("payment_capture", 1);

                //string key = "rzp_test_mz10cbdFCEOGCL";
                //string secret = "8HmEqVhCDzyQguHxNez5Optu";
                string key = ConfigurationManager.AppSettings["razPayKey"];
                string secret = ConfigurationManager.AppSettings["razPaySecret"]; ;



                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                RazorpayClient client = new RazorpayClient(key, secret);

                Razorpay.Api.Order order = client.Order.Create(input);
                string orderId = order["id"].ToString();
               
                   
                    TransactionsVendorFee initiateTransaction = new TransactionsVendorFee();
                    initiateTransaction.transactionid = recieptId.ToString();
                    initiateTransaction.vendorTransactionId = recieptId.ToString();
                    initiateTransaction.razorPayOrderId = orderId;
                    initiateTransaction.razorPayRecieptId = recieptId.ToString();
                    initiateTransaction.TransactionCreationDate = DateTime.Now;
                    initiateTransaction.TransactionUpdateDate = DateTime.Now;
                    initiateTransaction.transactionStatus = "In Progress";
                    initiateTransaction.transactionAmount = Convert.ToDecimal(amount);
                    initiateTransaction.VendorId = userRegisId;
                    dbContext.TransactionsVendorFees.Add(initiateTransaction);
                    dbContext.SaveChanges();
                    transId = initiateTransaction.id;


                
                return orderId;
            }
               
        }
        public string GenerateOrder(double amount, out int transId, string userRegisId)
        {
            Guid recieptId = Guid.NewGuid();
            Dictionary<string, object> input = new Dictionary<string, object>();
            
            input.Add("amount", amount); // this amount should be same as transaction amount

            input.Add("currency", "INR");
            input.Add("receipt", recieptId);
            input.Add("payment_capture", 1);

            //string key = "rzp_test_mz10cbdFCEOGCL";
            //string secret = "8HmEqVhCDzyQguHxNez5Optu";
            string key = ConfigurationManager.AppSettings["razPayKey"];
            string secret = ConfigurationManager.AppSettings["razPaySecret"]; ;



            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            string orderId = order["id"].ToString();
            using (mLearnDBEntities dbContext = new mLearnDBEntities())
            {
                int userId = (from a in dbContext.Customers
                              where a.registrationId == userRegisId
                              select a.userId).FirstOrDefault();
                Transaction initiateTransaction = new Transaction();
                initiateTransaction.transactionid = recieptId.ToString();
                initiateTransaction.vendorTransactionId = recieptId.ToString();
                initiateTransaction.razorPayOrderId = orderId;
                initiateTransaction.razorPayRecieptId = recieptId.ToString();
                initiateTransaction.TransactionCreationDate = DateTime.Now;
                initiateTransaction.TransactionUpdateDate = DateTime.Now;
                initiateTransaction.transactionStatus = "In Progress";
                initiateTransaction.transactionAmount = Convert.ToDecimal(amount);
                initiateTransaction.CustomerId = userId;
                dbContext.Transactions.Add(initiateTransaction);
                dbContext.SaveChanges();
                transId = initiateTransaction.id;


            }
            return orderId;
        }

        public List<AddressRequest> getUserAddress(string userId)
        {
            List<AddressRequest> userAddress = new List<AddressRequest>();
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var id = (from a in dbcontext.Customers
                          where
a.registrationId == userId
                          select a).FirstOrDefault();
                if (id != null)
                {
                    var address = (from a in dbcontext.AddressUsers where a.userId == id.userId && a.activeAddress == true select a);
                    foreach (var item in address)
                    {
                        AddressRequest add = new AddressRequest();
                        add.address = item.completeAddress;
                        add.userName = item.contactPerson;
                        add.AddressId = item.addressId;
                        add.userName = item.contactPerson;
                        add.phoneNumber = item.phone;
                        add.city = item.city;
                        add.state = item.addressState;
                        add.postalCode = item.postalCode;
                        userAddress.Add(add);

                    }
                }

            }
            return userAddress;
        }

        public List<AddressRequest> DeleteAddress(string userId, int addId)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var id = (from a in dbcontext.Customers
                          where a.registrationId == userId
                          select a).FirstOrDefault();
                var item = (from a in dbcontext.AddressUsers
                            where a.userId == id.userId && a.addressId == addId
                            select a).FirstOrDefault();
                if (item != null)
                {
                    item.activeAddress = false;
                    dbcontext.SaveChanges();
                }



            }
            return getUserAddress(userId);
        }
        public bool addEditUserAddress(AddressRequest address)
        {
            bool updateSuccessful = true;
            try
            {
                using (mLearnDBEntities dbcontext = new mLearnDBEntities())
                {
                    AddressUser user;
                    var userId = (from a in dbcontext.Customers
                                  where
                                  a.registrationId == address.userId
                                  select a).FirstOrDefault();
                    var userAddress = (from b in dbcontext.AddressUsers
                                       where b.addressId == address.AddressId
                                       select b).FirstOrDefault();
                    if (userAddress == null)
                    {
                        user = new AddressUser();
                        user.latitude = address.lattitude;
                        user.longitute = address.longitude;
                        user.completeAddress = address.address;
                        user.contactPerson = address.userName;
                        user.userId = userId.userId;
                        user.phone = address.phoneNumber;
                        user.activeAddress = true;
                        user.city = address.city;
                        user.addressState = address.state;
                        user.postalCode = address.postalCode;
                        dbcontext.AddressUsers.Add(user);
                    }
                    else
                    {
                        user = userAddress;
                        user.latitude = address.lattitude;
                        user.longitute = address.longitude;
                        // user.street = address.address;
                        user.completeAddress = address.address;
                        user.contactPerson = address.userName;
                        user.phone = address.phoneNumber;
                        user.city = address.city;
                        user.addressState = address.state;
                        user.postalCode = address.postalCode;

                    }
                    dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                updateSuccessful = false;
            }


            return updateSuccessful;
        }

        public bool UpdateFailedTransctions(int orderId)
        {
            bool status = false;
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var orderitem = (from a in dbcontext.Orders
                                    where a.orderId == orderId
                                    select a).FirstOrDefault();
                orderitem.orderStatus = "Failed";
                var transItem = (from b in dbcontext.Transactions
                                 where b.id == orderitem.transactionid
                                 select b).FirstOrDefault();
                transItem.transactionStatus = "Failed";
                dbcontext.SaveChanges();
                status = true;
            }
               
            return status;
        }
        public bool ValidatePaymentsVendor(string paymentId, string orderId, string signature,string type,string vendorEmail)
        {
            bool validateTran = false;
            
            try
            {
                Dictionary<string, string> attributes = new Dictionary<string, string>();

                attributes.Add("razorpay_payment_id", paymentId);
                attributes.Add("razorpay_order_id", orderId);
                attributes.Add("razorpay_signature", signature);
                using (mLearnDBEntities dbContext = new mLearnDBEntities())
                {
                    var transaction = dbContext.TransactionsVendorFees.Where(x => x.razorPayOrderId == orderId).Select(x => x).FirstOrDefault();
                    
                    //var order = dbContext.Orders.Where(x => x.transactionid == transaction.id).Select(x => x).FirstOrDefault();
                    try
                    {
                        if (transaction != null)
                        {
                            Utils.verifyPaymentSignature(attributes);
                            validateTran = true;
                            transaction.razorPayPaymentId = paymentId;
                            transaction.transactionStatus = "Success";
                            transaction.TransactionUpdateDate = DateTime.Now;
                            transaction.VendorType = type;
                           
                            if(type == "P")
                            {
                                var ven = (from a in dbContext.Printers where a.emailId == vendorEmail select a).FirstOrDefault();
                                ven.isRegistrationFeesPaid = true;
                            }
                            else
                            {
                                var ven = (from a in dbContext.Designers  where a.emailId == vendorEmail select a).FirstOrDefault();
                                ven.isRegistrationFeesPaid = true;
                            }
                            
                        }
                        else
                        {
                            Utils.verifyPaymentSignature(attributes);
                            validateTran = true;
                            transaction.razorPayPaymentId = paymentId;
                            transaction.transactionStatus = "Failed";
                            transaction.TransactionUpdateDate = DateTime.Now;
                           
                        }

                    }
                    catch (Exception ex)
                    {

                        transaction.razorPayPaymentId = paymentId;
                        transaction.transactionStatus = "Failed";
                        transaction.TransactionUpdateDate = DateTime.Now;
                        
                    }
                    dbContext.SaveChanges();
                   

                }


            }
            catch (Exception ex)
            {
                validateTran = false;

            }
            return validateTran;

        }
        public bool ValidatePayments(string paymentId, string orderId, string signature)
        {
            bool validateTran = false;
            string link = "https://www.shapenprint.com/designer/notifications";
            try
            {
                Dictionary<string, string> attributes = new Dictionary<string, string>();

                attributes.Add("razorpay_payment_id", paymentId);
                attributes.Add("razorpay_order_id", orderId);
                attributes.Add("razorpay_signature", signature);
                using (mLearnDBEntities dbContext = new mLearnDBEntities())
                {
                    var transaction = dbContext.Transactions.Where(x => x.razorPayOrderId == orderId).Select(x => x).FirstOrDefault();
                    var order = dbContext.Orders.Where(x => x.transactionid == transaction.id).Select(x => x).FirstOrDefault();
                    try
                    {
                        if (transaction != null)
                        {
                            Utils.verifyPaymentSignature(attributes);
                            validateTran = true;
                            transaction.razorPayPaymentId = paymentId;
                            transaction.transactionStatus = "Success";
                            transaction.TransactionUpdateDate = DateTime.Now;
                            order.orderStatus = "Success";
                            order.updatedDate = DateTime.Now;
                            //var orderDetails = (from a in dbContext.orderDetails where a.orderid == order.orderId select a).ToList();
                            //var customerDetails = (from a in dbContext.Customers
                            //                       where a.userId == order.customerId
                            //                       select a).FirstOrDefault();
                            //foreach (var item in orderDetails)
                            //{
                            //    var smsTextOrder = Constants.SMS_Order_Design.Replace("$category", item.orderType).Replace("$subcategory", item.subcategory).Replace("$orderid", orderId);
                            //    sendSMS.sendSMSClient( smsTextOrder, customerDetails.mobileNumber);
                            //    if (item.category.Contains("Design"))
                            //    {
                            //        var amount = item.designerCost + item.ProfessiondesignerFeesAfterCommision;
                            //        if (item.selectedProfDesiner == true)
                            //        {
                            //            var smsTextDesigner = Constants.SMS_Order_Placed_Prof.
                            //                Replace("$meetingslot", item.meetingTime.ToString()).
                            //                Replace("$subcategory", item.subcategory).
                            //                Replace("$link", link).Replace("$amount", amount.ToString());
                            //        }
                            //        else
                            //        {
                            //            var smsTextDesigner = Constants.SMS_Order_Placed.
                            //                Replace("$meetingslot", item.meetingTime.ToString()).
                            //                Replace("$subcategory", item.subcategory).
                            //                Replace("$link", link).Replace("$amount", amount.ToString());
                            //        }
                            //    }
                            //    else if (item.category == "Print")
                            //    {

                            //    }
                                
                            //    //New Order for $subcategory, Meeting time: $meetingslot and Amount: $amount. Accept via $link";
                                

                            //}
                            //var smsTextOrder=Constants.SMS_Order_Design.Replace("$category",order.)
                        }
                        else
                        {
                            Utils.verifyPaymentSignature(attributes);
                            validateTran = true;
                            transaction.razorPayPaymentId = paymentId;
                            transaction.transactionStatus = "Failed";
                            transaction.TransactionUpdateDate = DateTime.Now;
                            order.orderStatus = "Failed";
                            order.updatedDate = DateTime.Now;
                        }

                    }
                    catch (Exception ex)
                    {

                        transaction.razorPayPaymentId = paymentId;
                        transaction.transactionStatus = "Failed";
                        transaction.TransactionUpdateDate = DateTime.Now;
                        order.orderStatus = "Failed";
                        order.updatedDate = DateTime.Now;
                    }
                    dbContext.SaveChanges();
                    if (order.orderStatus == "Success")
                    {
                        try
                        {
                            //GenerateNotifications(orderId);
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                }


            }
            catch (Exception ex)
            {
                validateTran = false;

            }
            return validateTran;

        }
        public PrinterProfileRequest GetPrinterProfile(string userId)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var request = (from a in dbcontext.Printers where a.registrationId == userId select a).FirstOrDefault();
                var user = dbcontext.AspNetUsers.Find(userId);
                var newDesinger = new PrinterProfileRequest();
                if (request != null)
                {
                    newDesinger.location = request.location;
                    newDesinger.pan = request.PAN;
                    //newDesinger.createdDate = DateTime.Now;
                    newDesinger.dob = request.dateOfBirth;
                    //newDesinger.emailId = request.email;

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
                    //newDesinger.a
                    newDesinger.isPaymentDone = request.isRegistrationFeesPaid == null ? false: (bool)request.isRegistrationFeesPaid;
                    var printAdd = request.AddressPrinters.FirstOrDefault();
                    newDesinger.address = printAdd == null ? "" : printAdd.completeAddress;
                    newDesinger.pincode = printAdd == null ? "" : printAdd.postalCode;
                    newDesinger.city = printAdd == null ? "" : printAdd.city;
                    newDesinger.state = printAdd == null ? "" : printAdd.addressState;
                    newDesinger.profileImage = request.photolink;
                    if (user != null)
                    {
                        newDesinger.isMobileVerified = user.PhoneNumberConfirmed;
                    }



                }
                return newDesinger;
            }
        }

        public void CreatePrinterProfile(PrinterProfileRequest request)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                var newDesinger = (from a in dbcontext.Printers where a.registrationId == request.userId select a).FirstOrDefault();
                if (newDesinger != null)
                {
                    newDesinger.location = request.location;
                    //newDesinger.createdDate = DateTime.Now;
                    newDesinger.dateOfBirth = request.dob;
                    //newDesinger.emailId = request.email;
                    newDesinger.PAN = request.pan;
                    newDesinger.Aadhar = request.aadhar;
                    newDesinger.firstName = request.firstName;
                    newDesinger.lastName = request.lastName;
                    newDesinger.mobileNumber = request.mobileNumber;
                    
                    newDesinger.profileUrl = request.profileUrl;
                    newDesinger.GST = request.gst;
                    newDesinger.gender = request.gender;

                    if (newDesinger.AddressPrinters.Count == 0)
                    {
                        newDesinger.AddressPrinters.Add(new AddressPrinter
                        {

                            completeAddress = request.address,
                            longitute = request.longitude,
                            latitude = request.lattitude,
                            city=request.city,
                            addressState=request.state,
                            postalCode=request.pincode
                        });
                    }
                    else
                    {
                        var printAdd = newDesinger.AddressPrinters.FirstOrDefault();
                        printAdd.completeAddress = request.address;
                        printAdd.latitude = request.lattitude;
                        printAdd.longitute = request.longitude;
                        printAdd.city = request.city;
                        printAdd.addressState = request.state;
                        printAdd.postalCode = request.pincode;
                    }


                    dbcontext.SaveChanges();
                }
            }
        }
        public void RegisterPrinter(RegisterBindingModel model, string userId, string emailverify)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                //Guid emailverify = Guid.NewGuid();

                Printer designer = new Printer();
                designer.verifyEmal = emailverify.ToString();
                designer.emailId = model.Email;
                designer.registrationId = userId;
                designer.createdDate = DateTime.Now;
                designer.updateDate = DateTime.Now;
                dbcontext.Printers.Add(designer);
                dbcontext.SaveChanges();
            }
        }
        public void RegisterCustomer(RegisterBindingModel model, string userId, string emailverify)
        {
            using (mLearnDBEntities dbcontext = new mLearnDBEntities())
            {
                //Guid emailverify = Guid.NewGuid();

                Customer designer = new Customer();
                designer.verifyEmal = emailverify.ToString();
                designer.emailId = model.Email;
                designer.registrationId = userId;
                designer.mobileNumber = model.MobileNumber;
                designer.firstName = model.FirstName;
                designer.createdDate = DateTime.Now;
                designer.updateDate = DateTime.Now;
                dbcontext.Customers.Add(designer);
                dbcontext.SaveChanges();
            }
        }
    }
}