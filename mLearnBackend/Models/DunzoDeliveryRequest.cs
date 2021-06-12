using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class Eta
    {
        public int pickup { get; set; }
        public int dropoff { get; set; }
    }

    public class DunzoDeliveryResponse
    {
        public string task_id { get; set; }
        public string state { get; set; }
        public double estimated_price { get; set; }
        public Eta eta { get; set; }
    }
    public class DunzoDeliveryRequest
    {
        public string request_id { get; set; }
        public PickupDetails pickup_details { get; set; }
        public DropDetails drop_details { get; set; }
        public SenderDetails sender_details { get; set; }
        public ReceiverDetails receiver_details { get; set; }
        public string[] package_content { get; set; }
        public int package_approx_value { get; set; }
        public string special_instructions { get; set; }
        public string payment_method { get; set; }
        public PaymentData payment_data { get; set; }
        public string delivery_type { get; set; }
        public long schedule_time { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Address
    {
        public string apartment_address { get; set; }
        public string street_address_1 { get; set; }
        public string street_address_2 { get; set; }
        public string landmark { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string country { get; set; }
    }

    public class PickupDetails
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public Address address { get; set; }
    }

    //public class Address2
    //{
    //    public string apartment_address { get; set; }
    //    public string street_address_1 { get; set; }
    //    public string street_address_2 { get; set; }
    //    public string landmark { get; set; }
    //    public string city { get; set; }
    //    public string state { get; set; }
    //    public string pincode { get; set; }
    //    public string country { get; set; }
    //}

    public class DropDetails
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public Address address { get; set; }
    }

    public class SenderDetails
    {
        public string name { get; set; }
        public string phone_number { get; set; }
    }

    public class ReceiverDetails
    {
        public string name { get; set; }
        public string phone_number { get; set; }
    }

    public class PaymentData
    {
        public double amount { get; set; }
        public bool collect_delivery_charge { get; set; }
    }


}