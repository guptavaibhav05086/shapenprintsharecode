using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class UserCartModel
    {
        public int CartId { get; set; }
        public string UserEmail { get; set; }

        public string UserCart { get; set; }
        public bool IsPrintonly { get; set; } = false;

        public ProductList prodList { get; set; }
    }
}