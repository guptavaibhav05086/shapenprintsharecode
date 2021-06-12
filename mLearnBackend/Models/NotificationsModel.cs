using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class NotificationsModel
    {
       public int? orderId { get; set; }
        public string notificationDate { get; set; }

        public List<NotificationOrderDetails> orderDetails {get;set;}
   

    }
    public class NotificationOrderDetails
    {
        public string ProductSubcategory { get; set; }
        public string Industry { get; set; }

        public string MeetingSlot { get; set; }

        public decimal? Amount { get; set; }

        public string copies { get; set; }

        public string paperGSM { get; set; }

        public string deliveryTime { get; set; }

        public bool? isProfessionalDesigner { get; set; }

        public decimal? professionalFeesAmount { get; set; }

        public string ProductImageURL { get; set; }


    }
}