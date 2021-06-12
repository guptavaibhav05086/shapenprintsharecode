using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Helper
{
    public static class Constants
    {
        public static string link_user_orders = "https://www.shapenprint.com/user/ongoingorders";
        public static string link_designer_notifications = "https://www.shapenprint.com/designer/notifications";
        public static string link_printer_notifications = "https://www.shapenprint.com/printers/notifications";

        public static string role_Designer = "Designer";
        public static string role_Printer = "Printer";
        public static string role_Customer = "Customer";
        public static string role_Admin = "Admin";
        public static string smsAuthKey = "ODdkOGIxZjk1ZTl";
        public static string Email_Order_Desing = "New Order for $subcategory, Meeting time: $meetingslot and Amount: $amount";
        public static string Email_Order_Desing_Prof = "New Order for $subcategory <strong>(PROFESSIONAL DESIGN)</strong>, Meeting time: $meetingslot and Amount: INR $amount";
        public static string Email_Order_Printer = "New Order for $subcategory,Quantities: $quantity,GSM: $subcategory, Size: $size and,Amount: INR $amount";
        public static string Email_Order_Finish_Design = "Yay! Your design of Order $orderid is successfully uploaded on your dashboard. Please accept the design to download it.";
        public static string Email_Order_Designer_Allocated = "Details of the allotted designer of  Order ID $orderid are: $name, $contact. He will be available for the meeting at $meetingslot, $date";
        public static string Email_Order_Meeting_Rescheduled = "Meeting rescheduled with the allotted designer  $name, $contact for  Order ID $orderid. He will be available for the meeting at $meetingslot, $date";
        public static string Email_Order_Designer_NotAvaibale = "Sorry, the allotted designer is unavailable for the meeting. Another designer will be allotted shortly. Please choose another meeting slot from your dashboard.";
        public static string Email_Printer_Allocated = "Details of the allotted printer of are: $name, $contact.";
        public static string Email_Order_Created = "Your Order for $category of $subcategory is successfully placed";
        public static string Email_Order_Completed = "Glad to see that you liked the creative.";


        public static string SMS_Order_Design = "Your Order for $category of $subcategory is successfully placed. Your Order ID is $orderid";
        //Your Order for $category of $subcategory is successfully placed. Your Order ID is $orderid
        public static string SMS_Designer_Allocated = "$name, $contact is the allotted designer for $orderid. He will be available for the meeting at $meetingslot $date via $meetinglink.";
        //$name, $contact is the allotted designer for $orderid. He will be available for the meeting at $meetingslot $date via $meetinglink.
        public static string SMS_Designer_Finish = "Yay! Your design of $orderid is successfully uploaded on your dashboard. You can check via $link.";
        //Yay! Your design of $orderid is successfully uploaded on your dashboard. You can check via %%|orderPopUpLink^{"inputtype" : "text", "maxlength" : "50"}%%.
        public static string SMS_Printer_Allocated = "$name, $contact is the allotted printer for $orderid";
        //$name is the allotted printer for $orderid
        public static string SMS_Printer_Delivery = "Yay! Your print with $orderid is out for delivery.";
        //Yay! Your print of $orderid is out for delivery.
        public static string SMS_Design_Completed = "Awesome! Your design is completed.Please accept it to proceed for printing from your dashboard, $link";
        //Awesome! Your design is completed.Please accept it to proceed for printing from your dashboard, $link
        public static string SMS_Order_Placed = "New Order for $orderid, Meeting time: $meetingslot and Amount: $amount. Accept via $link";
        //New Order for $orderid, Meeting time: $meetingslot and Amount: $amount. Accept via $link
        public static string SMS_Order_Placed_Prof = "New Order for $orderid, (PROFESSIONAL DESIGN) Meeting time: $meetingslot and Amount: $amount. Accept via $link";
        //New Order for $orderid, (PROFESSIONAL DESIGN) Meeting time: $meetingslot and Amount: $amount. Accept via $link
        public static string SMS_Printer_Order_Placed = "New Order for $subcategory, Quantities: $quantity, Size: $size, GSM: $gsm and Amount: $amount. Accept via $link.";
        //New Order for $subcategory, Quantities: $quantity, Size: $size, GSM: $gsm and Amount: $amount. Accept via $link.
        public static string SMS_Customer_MeetingCancelled = "Sorry, the allotted designer is unavailable for the meeting. Another designer will be allotted shortly. Please choose another meeting slot to proceed via $link";
        //Sorry, the allotted designer is unavailable for the meeting. Another designer will be allotted shortly. Please choose another meeting slot to proceed via $link
        public static string SMS_Printer_Order_Completed = "Glad to see that you liked the creative.Please rate us on $link. Your feedback is valuable to us.";
        //Glad to see that you liked the creative.Please rate us on %%|rating^{ "inputtype" : "text", "maxlength" : "67"}%%. Your feedback is valuable to us.
        public static string SMS_MainUser = "Glad to serve you";


    }
}