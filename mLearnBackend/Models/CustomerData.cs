using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class CustomerData
    {
        public string firstName { get; set; }
        public string lastName { get; set; }

        public string emailId { get; set; }
        public string mobileNumber { get; set; }

        public string cretaedDate { get; set; }

        public bool isMobileVerified { get; set; }

        public bool isEmailVerified { get; set; }

    }
}