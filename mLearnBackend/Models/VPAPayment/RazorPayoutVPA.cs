using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models.VPAPayment
{
    public class RazorPayoutVPA
    {
        public string account_number { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string mode { get; set; }
        public string purpose { get; set; }
        public FundAccount fund_account { get; set; }
        public bool queue_if_low_balance { get; set; }
        public string reference_id { get; set; }
        public string narration { get; set; }
        public Dictionary<string, string> notes { get; set; }
    }
   
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class VPA
    {
        public string address { get; set; }

    }


    public class Contact
    {
        public string name { get; set; }
        public string email { get; set; }
        public string contact { get; set; }
        public string type { get; set; }
        public string reference_id { get; set; }
        public Dictionary<string, string> notes { get; set; }
    }

    public class FundAccount
    {
        public string account_type { get; set; }
        public VPA vpa { get; set; }
        public Contact contact { get; set; }
    }
}