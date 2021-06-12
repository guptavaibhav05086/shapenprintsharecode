using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class DiscountsModel
    {
        public int DiscountId { get; set; }
        public string CartAmount { get; set; }
        public string DiscountPercentage { get; set; }
    }
}