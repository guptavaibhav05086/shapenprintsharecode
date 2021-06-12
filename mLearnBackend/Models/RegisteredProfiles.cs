using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class RegisteredProfiles
    {
        public List<DesignerProfileRequest> dprofile { get; set; }

        public List<PrinterProfileRequest> pprofile { get; set; }

        public List<CustomerData> customerProfile { get; set; }

    }
}