using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class DunzoDeliveryStatus
    {
        public string task_id { get; set; }
        public string state { get; set; }
        public long event_timestamp { get; set; }
        public EtaTask eta { get; set; }
        public long request_timestamp { get; set; }
        public object contact { get; set; }
        public object dzid { get; set; }
        public object cod_amount_collected { get; set; }
        public object refund_amount { get; set; }
        public object tracking_url { get; set; }
    }
    public class EtaTask
    {
        public double pickup { get; set; }
        public double dropoff { get; set; }
    }
}