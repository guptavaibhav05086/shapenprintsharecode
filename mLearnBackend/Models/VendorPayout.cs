using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class VendorPayout
    {
        public int orderId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public decimal? TotalPayout { get; set; }
        public decimal? AmountTransfered { get; set;}
        public decimal? PayoutTaxes { get; set; }
        public decimal? PayoutFees { get; set; }

        public string VendorType { get; set; }

    }
}