using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class DesignerProfileRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string dob { get; set; } 

        public string qualification { get; set; }
        public bool isProfessional { get; set; }
        public bool isVerified { get; set; }
        public string exp { get; set; }
        public string profileUrl { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string pan { get; set; }
        public string mobileNumber { get; set; }
        public string userId { get; set; }
        public string emailId { get; set; }
        public bool isMobileVerified { get; set; }
        public string softwares { get; set; }
        public int? Registrationnumber { get; set; }
        public string profileImage { get; set; }
        public bool isPaymentDone { get; set; }
    }
}