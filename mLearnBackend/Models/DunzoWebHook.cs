using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class DunzoWebHook
    {
        public string event_type { get; set; }
        public string event_id { get; set; }
        public string task_id { get; set; }
        public string reference_id { get; set; }
        public string state { get; set; }
        public long event_timestamp { get; set; }
        public Eta eta { get; set; }
        public Runner runner { get; set; }
        public long request_timestamp { get; set; }
        public string price { get; set; }
        public string total_time { get; set; }
        public string cancelled_by { get; set; }
        public string cancellation_reason { get; set; }
        public string cod_amount_collected { get; set; }
    }
    //public class Eta
    //{
    //    public int pickup { get; set; }
    //    public int dropoff { get; set; }
    //}

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Runner
    {
        public string name { get; set; }
        public string phone_number { get; set; }
        public Location location { get; set; }
    }
}