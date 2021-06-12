using mLearnBackend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;

namespace mLearnBackend.Helper
{
    public static class sendSMS
    {
        public static SMSResponse sendSMSClient(string smsMessage,string number)
        {
            //Documents
            string path = @"C:\logs\SMSLog.txt";

            var key = ConfigurationManager.AppSettings["smsapikey"];
            String message = HttpUtility.UrlEncode(smsMessage);
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                {
                {"apikey" , key},
                {"numbers" , number},
                {"message" , message},
                {"sender" , "SHPNPT"}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                var resposne = JsonConvert.DeserializeObject<SMSResponse>(result);
                //if (!File.Exists(path))
                //{
                //    // Create a file to write to.
                //    using (StreamWriter sw = File.CreateText(path))
                //    {
                //        sw.WriteLine("SMS text->");
                //        sw.WriteLine(smsMessage);
                //        sw.WriteLine("Phone Number->" + number.ToString());
                //        sw.WriteLine("Response->" + result);
                //    }
                //}

                //// This text is always added, making the file longer over time
                //// if it is not deleted.
                //using (StreamWriter sw = File.AppendText(path))
                //{
                //    sw.WriteLine("SMS text->");
                //    sw.WriteLine(smsMessage);
                //    sw.WriteLine("Phone Number->" + number.ToString());
                //    sw.WriteLine("Response->" + result);
                //}
                return resposne;
            }
        }
    }
}