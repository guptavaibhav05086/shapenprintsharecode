using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class DashboardModel
    {
        public int activeNotifcations { get; set; }
        public int activeOrders { get; set; }

        public int completedOrders { get; set; }

        public int totalOrders { get; set; }

        public int upcomingMettings { get; set; }

        public decimal? totalPaymentRecieved { get; set; }

        public bool? isDesignerAdminVerified { get; set; }

        public bool? isVendorAdminVerified { get; set; }
    }
}