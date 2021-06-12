using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models.Response
{
    public class RazorPayoutResponse
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string fund_account_id { get; set; }
        public FundAccount fund_account { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public Dictionary<string, string> notes { get; set; }
        public int fees { get; set; }
        public int tax { get; set; }
        public string status { get; set; }
        public string purpose { get; set; }
        public object utr { get; set; }
        public string mode { get; set; }
        public string reference_id { get; set; }
        public string narration { get; set; }
        public object batch_id { get; set; }
        public object failure_reason { get; set; }
        public int created_at { get; set; }

        
    }
    public class Contact
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string email { get; set; }
        public string type { get; set; }
        public string reference_id { get; set; }
        public object batch_id { get; set; }
        public bool active { get; set; }
        public Dictionary<string,string> notes { get; set; }
        public int created_at { get; set; }
    }

    public class BankAccount
    {
        public string ifsc { get; set; }
        public string bank_name { get; set; }
        public string name { get; set; }
        public List<object> notes { get; set; }
        public string account_number { get; set; }
    }
    public class VPAResponse
    {
        public string address { get; set; }
        public string handle { get; set; }
        public string username { get; set; }

    }

    public class FundAccount
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string contact_id { get; set; }
        public Contact contact { get; set; }
        public string account_type { get; set; }
        public BankAccount bank_account { get; set; }
        public VPAResponse vpa { get; set; }
        public object batch_id { get; set; }
        public bool active { get; set; }
        public int created_at { get; set; }
    }
}