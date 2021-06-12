using Microsoft.ApplicationInsights.Extensibility.Implementation;
using mLearnBackend.Models;
using Newtonsoft.Json;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace mLearnBackend.Helper
{
    public class CommonMethods
    {
        public async static Task<DunzoDeliveryStatus> HTTPPostCalls(string url, StringContent data)
        {
            string result = "";
            DunzoDeliveryStatus status = null;
            using (HttpClient client = new HttpClient())
            {
                string DunzobaseUrl = ConfigurationManager.AppSettings["DunzobaseUrl"];
                string clientid = ConfigurationManager.AppSettings["DunzoClientId"];
                string token = ConfigurationManager.AppSettings["DunzoToken"];
               
                client.DefaultRequestHeaders.Add("client-id", clientid);
                client.DefaultRequestHeaders.Add("Authorization", token);
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("Accept-Language", "en_US");

                var response = await client.PostAsync(DunzobaseUrl, data);

                result = response.Content.ReadAsStringAsync().Result;
                try
                {
                    status = JsonConvert.DeserializeObject<DunzoDeliveryStatus>(result);
                    if (status.eta ==null)
                    {
                        status.state = result;
                    }
                }
                catch (Exception ex)
                {
                    status = new DunzoDeliveryStatus();
                    status.state = result;
                   
                }
               

            }
            return status;
           
           
        }
        public async static Task<DunzoDeliveryStatus> HTTPGetCallDunzoTask(string taskId)
        {
            string result = "";
            DunzoDeliveryStatus status = null;
            using (HttpClient client = new HttpClient())
            {
                string DunzobaseUrl = ConfigurationManager.AppSettings["DunzoTaskStatusUrl"].Replace("{taskId}", taskId);

                string clientid = ConfigurationManager.AppSettings["DunzoClientId"];
                string token = ConfigurationManager.AppSettings["DunzoToken"];

                client.DefaultRequestHeaders.Add("client-id", clientid);
                client.DefaultRequestHeaders.Add("Authorization", token);
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("Accept-Language", "en_US");

                var response = await client.GetAsync(DunzobaseUrl);

                result = response.Content.ReadAsStringAsync().Result;
                status = JsonConvert.DeserializeObject<DunzoDeliveryStatus>(result);
               

            }
            return status;


        }
        public async static Task<string> HTTPPostCallsRazorVendorPayout(string url, StringContent data)
        {
           

            string result = "";
            using (HttpClient client = new HttpClient())
            {
                //string DunzobaseUrl = ConfigurationManager.AppSettings["DunzobaseUrl"];
                string clientid = ConfigurationManager.AppSettings["razPayKey"];
                string token = ConfigurationManager.AppSettings["razPaySecret"];

                //client.DefaultRequestHeaders.Add("client-id", clientid);
                client.DefaultRequestHeaders.Add($"Authorization", $"Basic {Base64Encode($"{clientid}:{token}")}");
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                //client.DefaultRequestHeaders.Add("Accept-Language", "en_US");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.PostAsync(url, data);

                result = response.Content.ReadAsStringAsync().Result;
            }
            return result;


        }

        public static string Base64Encode(string textToEncode)
        {
            byte[] textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
            return Convert.ToBase64String(textAsBytes);
        }

        public static async Task<double> GetDistanceFroomGoogle(string url)
        {
            double distance = 0;
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var result = response.Content.ReadAsStringAsync().Result;
                var status = JsonConvert.DeserializeObject<GoogleDistanceMatrixResponse>(result);
                distance = status.rows[0].elements[0].distance.value == 0 ? 0 : status.rows[0].elements[0].distance.value/1000;

            }
            return distance;
        }
    }
}