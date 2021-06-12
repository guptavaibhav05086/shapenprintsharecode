using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class BankAccountDetailsModel
    {
        public int ID { get; set; }
        public string AccountHolderName { get; set; }
        public string BankAccountNumber { get; set; }
        public string Branch { get; set; }
        public string IFSC { get; set; }
        public string UPI { get; set; }
        public string PAN { get; set; }
        public string UserEmail { get; set; }
    }
}