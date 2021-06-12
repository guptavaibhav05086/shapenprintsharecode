using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class PrinterProfileRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string pan { get; set; }
        public string location { get; set; }
        public string city { get; set; }

        public string state { get; set; }
        public string pincode { get; set; }
        public string exp { get; set; }
        public string profileUrl { get; set; }
        public string aadhar { get; set; }
      
        public string email { get; set; }
        public string mobileNumber { get; set; }

        public bool isMobileVerified { get; set; }
        public string userId { get; set; }
        public string lattitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
        public string profileImage { get; set; }
        public int? Registrationnumber { get; set; }
        public string gst { get; set; }
        public string gstFileName { get; set; }
        public bool isPaymentDone { get; set; }
        public bool isProfessional { get; set; }
        public bool isVerified { get; set; }
    }
}