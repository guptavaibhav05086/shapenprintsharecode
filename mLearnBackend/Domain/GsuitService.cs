using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace mLearnBackend.Domain
{
    public class GsuitService
    {
        public static UserCredential GetUserTokenGoogle(string[] scopes)
        {
            AppFlowMetadata objFlow = new AppFlowMetadata();
            string userId = null;
            var flow = objFlow.Flow;
            TokenResponse token = null;

            using (mLearnDBEntities dbConetx = new mLearnDBEntities())
            {
                var item = dbConetx.StoreAPITokens.Select(x => x).OrderByDescending(x=>x.id).FirstOrDefault();
                if(item != null)
                {
                    token = JsonConvert.DeserializeObject<TokenResponse>(item.tokenValue);
                    userId = item.tokenKey;
                }

            }


            
           

            return new UserCredential(flow,userId, token);
        }
        public static string CreateNewMeeting(DateTime startTime,int duration,
            string mSummary,string location,string description,
            string designerAddress,string customerAddress,string moderatorAddress)
        {
            string[] Scopes = { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.CalendarEvents };
            string ApplicationName = "Google Calendar API .NET Quickstart";
            UserCredential credential = GetUserTokenGoogle(Scopes);
          
            DateTime startDate = startTime.AddHours(-13).AddMinutes(-30);
            DateTime endDate = startDate.AddMinutes(duration);
            //DateTime endDate = startTime.AddMinutes(duration);

            DateTime sTime = TimeZoneInfo.ConvertTime(startTime, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            DateTime eTime= TimeZoneInfo.ConvertTime(endDate, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            Event newEvent = new Event()
            {
                Summary = mSummary,
                Location = location,
                Description = description,
                Start = new EventDateTime()
                {
                    DateTime = startDate,
                    TimeZone = "Asia/Kolkata",
                },
                End = new EventDateTime()
                {
                    DateTime = endDate,
                    TimeZone = "Asia/Kolkata",
                },
               // Recurrence = new String[] { "RRULE:FREQ=DAILY;COUNT=2" },
                Attendees = new EventAttendee[] {
                new EventAttendee() { Email = designerAddress},
                new EventAttendee() { Email = customerAddress },
                new EventAttendee() { Email = moderatorAddress },
            },
               ConferenceData=new ConferenceData()
               {
                   CreateRequest=new CreateConferenceRequest ()
                   {
                       RequestId = Guid.NewGuid().ToString()
                   },
                   ConferenceSolution=new ConferenceSolution ()
                   {
                       Key = new ConferenceSolutionKey()
                       {
                           Type= "eventHangout"
                       }
                   }
               },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {
                new EventReminder() { Method = "email", Minutes = 24 * 60 },
                new EventReminder() { Method = "sms", Minutes = 10 },
                }
                }
            };

            String calendarId = "primary";
            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
            request.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All;
            request.ConferenceDataVersion = 1;
            Event createdEvent = request.Execute();
            string link = createdEvent.HangoutLink;
            return link;

        }
        public static void CreateEvents(CalendarService service)
        {
            Event newEvent = new Event()
            {
                Summary = "Test Events",
                Location = "800 Howard St., San Francisco, CA 94103",
                Description = "A chance to hear more about Google's developer products.",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2020-06-21T09:00:00-07:00"),
                    TimeZone = "Asia/Kolkata",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2020-06-23T17:00:00-07:30"),
                    TimeZone = "Asia/Kolkata",
                },
                Recurrence = new String[] { "RRULE:FREQ=DAILY;COUNT=2" },
                Attendees = new EventAttendee[] {
                new EventAttendee() { Email = "guptavaibhav.05086@gmail.com" },
                new EventAttendee() { Email = "aucsalejobs@gmail.com" },
            },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {
                new EventReminder() { Method = "email", Minutes = 24 * 60 },
                new EventReminder() { Method = "sms", Minutes = 10 },
                }
                }
            };

            String calendarId = "primary";
            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
            Event createdEvent = request.Execute();
        }
        public void getEvents()
        {
             string[] Scopes = { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.CalendarEvents };
             string ApplicationName = "Google Calendar API .NET Quickstart";
            UserCredential credential = GetUserTokenGoogle(Scopes);

            //using (var stream =
            //    new FileStream(@"C:\Users\Praveen\Documents\visual studio 2015\Projects\ShareandPrint-Backend\mLearnBackend\credentials.json", FileMode.Open, FileAccess.Read))
            //{
            //    // The file token.json stores the user's access and refresh tokens, and is created
            //    // automatically when the authorization flow completes for the first time.
            //    string credPath = @"C:\Users\Praveen\Documents\visual studio 2015\Projects\ShareandPrint-Backend\mLearnBackend\token.json";
            //    var cred = GoogleClientSecrets.Load(stream).Secrets;
            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        cred, 
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(credPath, true)).Result;
            //    Console.WriteLine("Credential file saved to: " + credPath);
            //}

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            CreateEvents(service);
            // List events.
            Events events = request.Execute();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }
            //Console.Read();
        }
    }
}