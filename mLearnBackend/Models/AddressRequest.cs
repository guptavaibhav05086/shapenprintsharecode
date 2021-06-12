using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class AddressRequest
    {
        public string userName { get; set; }
        public string userId { get; set; }
        public string lattitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
        public int AddressId { get; set; }
        public int? printer { get; set;}

        public string state { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
    }

    public class AddressResponse
    {
        public List<AddressRequest> printerAdd { get; set; }

        public AddressRequest userAdd { get; set; }
    }
}