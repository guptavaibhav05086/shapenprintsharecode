using mLearnBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace mLearnBackend.Domain
{
    public class mLearnServices
    {
        mLearnDBEntities _dbConetxt;

        public int RegisterUserProfile(RegisterBindingModel model, string registrationId)
        {
            UserProfile registerProfile = new UserProfile();
            using (_dbConetxt = new mLearnDBEntities())
            {
               
                registerProfile.emailId = model.Email;
                registerProfile.registrationId = registrationId;
                registerProfile.createdDate = DateTime.Now;
                registerProfile.updateDate = DateTime.Now;
                _dbConetxt.UserProfiles.Add(registerProfile);
                _dbConetxt.SaveChanges();

            }
            return registerProfile.userId;
        }
        public void TrainerCourseProfile(RegisterBindingModel model,int trainerId)
        {
            using (_dbConetxt = new mLearnDBEntities())
            {
                foreach (var item in model.TrainerTechnologies)
                {
                    TrainerCourseMapping registerCourse= new TrainerCourseMapping();
                    registerCourse.courseId = item;
                    registerCourse.trainerId = trainerId;
                    _dbConetxt.TrainerCourseMappings.Add(registerCourse);
                    _dbConetxt.SaveChanges();
                }
                
                

            }
        }

        public string ResetPassword(string email)
        {
            string message = "";
            string userType = "Vendor";
            //SendEmail();
            using (_dbConetxt = new mLearnDBEntities())
            {
                var result = _dbConetxt.AspNetUsers.Where(x => x.Email == email).ToList();
                
                if (result.Count() >= 0)
                {
                    var id = result[0].Id;
                    var resetRequest = (from a in _dbConetxt.PasswordResets where a.userEmail == email select a).FirstOrDefault();
                    var customer = (from a in _dbConetxt.Customers where a.registrationId == id  select a).FirstOrDefault();
                    if(customer != null)
                    {
                        userType = "Customer";
                    }
                    if(resetRequest != null)
                    {
                        resetRequest.GuidKey = result[0].Id;
                    }
                    else
                    {
                        PasswordReset reset = new PasswordReset();
                        //var id = Guid.NewGuid();
                        //reset.GuidKey = id.ToString();
                        reset.GuidKey = result[0].Id;
                        reset.userEmail = email;
                        reset.createdDate = new DateTime();
                        
                        _dbConetxt.PasswordResets.Add(reset);
                       
                        message = "Request generated.Please check u r mail box ";
                    }
                    _dbConetxt.SaveChanges();
                    SendEmail(email, result[0].Id, "PasswordReset", "ShapeNPrint-Password Reset",userType);
                }
                else
                {
                    message = "No User found with this email";
                }


            }
            return message;
                
        }
        private static void SendEmail(string emailid,string guid,string templateName,string subject,string userType)
        {
            //calling for creating the email body with html template   
            var body = CreateEmailBody(templateName);
            //var emailSubject = ConfigurationManager.AppSettings["emailSubject"];

            //Send Email
            SendHtmlFormattedEmail(subject, body,emailid,guid,userType);
        }
        private static string GetResourcePath(string filepath)
        {
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            outPutDirectory = outPutDirectory.Replace("\\bin", "");
            
            var templatePath = Path.Combine(outPutDirectory, filepath);
            string template_Path = new Uri(templatePath).LocalPath;
            return template_Path;
        }
        private static string CreateEmailBody(string emailTemplateName)

        {
            var body = string.Empty;
            //var templateName = ConfigurationManager.AppSettings[emailTemplateName];
            //using streamreader for reading my htmltemplate   
            //var filepath = "C:\\Users\\DELL\\source\\repos\\SendEmail\\SendEmail\\" +
            //              "\\templates\\" + templateName + ".html";

            var filepath = @"templates\" + emailTemplateName + ".html";
            string template_Path = GetResourcePath(filepath);

            // My path C:\Users\DELL\Downloads\SendEmail\SendEmail\SendEmail\templates
            using (var reader = new StreamReader(template_Path))

            {
                body = reader.ReadToEnd();
            }

          

            return body;
        }

        private static void SendHtmlFormattedEmail(string subject, string body,string emailid, string guid, string userType)
        {
            List<string> emailList = new List<string>
            {
               emailid
            };

            foreach (var toEmail in emailList)
            {
                using (var mailMessage = new MailMessage())
                {
                    var baseUrl = ConfigurationManager.AppSettings["baseurl"];
                    var emailFrom = ConfigurationManager.AppSettings["FromEmail"];
                    var emailFromUser = ConfigurationManager.AppSettings["EmailFromUser"];
                    var url = baseUrl+ ConfigurationManager.AppSettings["resetUrl"] + "email=" + emailid + "&guid=" + guid + "&type=" + userType;
                    body = body.Replace("$restURL", url);
                    body = body.Replace("cid:myImageID", "https://drive.google.com/uc?export=view&id=12RZQFztV-xMPzbmEfm07CtKRCg3gcuv3");
                    mailMessage.From = new MailAddress(emailFrom, emailFromUser);

                    var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                    var filePath = GetResourcePath(@"templates\images\3275434.jpg");

                    //Add Image
                    var theEmailImage = new LinkedResource(filePath);

                    theEmailImage.ContentId = "myImageID";
                    //Add the Image to the Alternate view
                    //htmlView.LinkedResources.Add(theEmailImage);


                    theEmailImage.ContentId = "myImageID";
                    //Add the Image to the Alternate view
                    //htmlView.LinkedResources.Add(theEmailImage);

                    var filePathFb = GetResourcePath(@"templates\images\facebook2x.png");
                    var filePathInsta = GetResourcePath(@"templates\images\instagram2x.png");
                    var filePathLn = GetResourcePath(@"templates\images\linkedin2x.png");
                    var filePathPin = GetResourcePath(@"templates\images\pinterest2x.png");
                    var filePathSN = GetResourcePath(@"templates\images\ShapeNPrint.png");
                    var filePathtw = GetResourcePath(@"templates\images\twitter2x.png");

                    var eImgFb = new LinkedResource(filePathFb);
                    eImgFb.ContentId = "Fb";
                    var eImgIn = new LinkedResource(filePathInsta);
                    eImgIn.ContentId = "In";
                    var eImgLn = new LinkedResource(filePathLn);
                    eImgLn.ContentId = "Ln";
                    var eImgPin = new LinkedResource(filePathPin);
                    eImgPin.ContentId = "Pin";
                    var eImgSn = new LinkedResource(filePathSN);
                    eImgSn.ContentId = "SN";
                    var eImgtw = new LinkedResource(filePathtw);
                    eImgtw.ContentId = "Tw";
                    //htmlView.LinkedResources.Add(eImgFb);
                    //htmlView.LinkedResources.Add(eImgIn);
                    //htmlView.LinkedResources.Add(eImgLn);
                    //htmlView.LinkedResources.Add(eImgPin);
                    //htmlView.LinkedResources.Add(eImgSn);
                    //htmlView.LinkedResources.Add(eImgtw);

                    //var filePath = GetResourcePath(@"templates\favicon.png");

                    ////Add Image
                    //var theEmailImage = new LinkedResource(filePath);

                    //theEmailImage.ContentId = "myImageID";
                    ////Add the Image to the Alternate view
                    //htmlView.LinkedResources.Add(theEmailImage);

                    //Add view to the Email Message
                    mailMessage.AlternateViews.Add(htmlView);

                    mailMessage.Subject = subject;

                    mailMessage.Body = body;

                    mailMessage.IsBodyHtml = true;

                    mailMessage.To.Add(new MailAddress(toEmail));

                    var smtp = new SmtpClient
                    {
                        Host = ConfigurationManager.AppSettings["smtpHost"],
                        EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"])
                    };

                    var networkCred = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["smtpUser"],
                        Password = ConfigurationManager.AppSettings["smtpPassword"]
                    };

                    smtp.UseDefaultCredentials = false;

                    smtp.Credentials = networkCred;

                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);

                    smtp.Send(mailMessage);

                    Console.WriteLine("Sent to: " + toEmail);
                }
            }
        }
        public List<FeedbackModel> GetUserFeedback(int courseId)
        {
            List<FeedbackModel> resposne = null;
            using (mLearnDBEntities dbConetx = new mLearnDBEntities())
            {

                resposne = (from a in dbConetx.UserFeedbacks
                            where a.courseId == courseId
                            orderby a.Id descending
                            select new FeedbackModel
                            {
                                userId = a.UserProfile.userId,
                                userName = a.UserProfile.userName,
                                feedback = a.feedback,
                                starRating = (int)a.starRating

                            }).ToList<FeedbackModel>();


            }
            return resposne;
        }

    }
}