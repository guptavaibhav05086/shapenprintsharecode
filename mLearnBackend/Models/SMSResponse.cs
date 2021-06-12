using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mLearnBackend.Models
{
    public class SMSResponse
    {
        public int balance { get; set; }
        public int batch_id { get; set; }
        public int cost { get; set; }
        public int num_messages { get; set; }
        public Message message { get; set; }
        public List<Message2> messages { get; set; }
        public string status { get; set; }
    }
    public class Message
    {
        public int num_parts { get; set; }
        public string sender { get; set; }
        public string content { get; set; }

    }

    public class Message2
    {
        public string id { get; set; }
        public object recipient { get; set; }

    }
}