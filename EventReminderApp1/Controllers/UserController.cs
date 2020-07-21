using EventReminderApp1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Mvc;

namespace EventReminderApp1.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        EventRepository eventRepository = new EventRepository();
        //const string Connectionstring = @"Data Source= LENOVO\SQLSERVER; Initial Catalog = dbwebapp; Integrated Security = True";
        // GET: User
        string resetid;
        public UserController()
        {
            //Timer myTimer = new Timer();
            //myTimer.Interval = 60000;
            //myTimer.AutoReset = true;
            //myTimer.Elapsed += new ElapsedEventHandler(SendMailToUser);
            //myTimer.Enabled = true;
            //SendEmail("amithaunnikrishnan415@gmail.com", "Reminder", "hi");
        }

        public ActionResult Home()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.UserId = Session["UserID"];
                ViewBag.Emaild = Session["EmailId"];
                ViewBag.UserName = Session["UserName"];
                return View();
            }
            return View();
        }

        [HttpPost]
        public JsonResult Register(Registration register)
        {
            string userid;
            string mail;
            string uname;

            var status = false;
            var dob = register.DOB.ToString("yyyy-MM-dd");
            string query = "insert into tblRegister(UserName,DOB,Phone,EmailId,Password)" +
                    " values('" + register.Username + "','" + dob + "','" + register.Phone + "','" + register.Email + "','" + register.Password + "')";
            eventRepository.AddUpdateDeleteSQL(query);

            List<string> variables = eventRepository.UserRegister(register);
            if (variables.Count != 0)
            {
                userid = variables[0];
                mail = variables[1];
                uname = variables[2];


                Session["UserID"] = userid;
                Session["EmailId"] = mail;
                Session["UserName"] = uname;
                status = true;

                return new JsonResult { Data = new { status = status, Username = uname } };
            }
            else
            {
                status = false;
                return new JsonResult { Data = new { status = status } };
            }
        }
        [HttpPost]
        public JsonResult Login(Registration login)
        {
            string userid;
            string mail;
            string uname;

            var status = false;

            List<string> variables = eventRepository.LoginDetails(login);
            if (variables.Count != 0)
            {
                userid = variables[0];
                mail = variables[1];
                uname = variables[2];

            
                Session["UserID"] = userid;
                Session["EmailId"] = mail;
                Session["UserName"] = uname;
                status = true;
               
                return new JsonResult { Data = new { status = status, Username = uname } };
            }
            else
            {
                status = false;
                return new JsonResult { Data = new { status = status } };
            }
            
        }
        [HttpPost]
        public JsonResult GoogleLogin(string email, string name, string gender, string lastname, string location)
        {
            /* var status = false;
             using (SqlConnection con = new SqlConnection(Connectionstring))
             {
                 string qry;
                 con.Open();
                 string query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
                 SqlCommand cmd = new SqlCommand(query, con);
                 cmd.CommandType = CommandType.Text;

                 SqlDataAdapter sda = new SqlDataAdapter(cmd);
                 DataTable datatable = new DataTable();
                 sda.Fill(datatable);
                 if (datatable.Rows.Count == 1)
                 {
                     DataRow row = datatable.Rows[0];
                     Session["UserID"] = row["UserID"].ToString();
                     Session["EmailId"] = row["EmailId"].ToString();

                     status = true;
                 }
                 else
                 {
                     qry = "insert into tblRegister(UserName,EmailId)" +
                     " values('" + name + "','" + email + "')";
                     eventRepository.AddUpdateDeleteSQL(qry);
                     status = true;
                 }
             }
             return new JsonResult { Data = new { status = status } };*/

            var status = false;
            string qry;
            string query;
            List<string> variables;
            query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
            variables = eventRepository.GoogleLoginDetails(query);
            if(variables.Count != 0)
            {
                string userid = variables[0];
                string mail = variables[1];

                Session["UserID"] = userid;
                Session["EmailId"] = mail;
                status = true;
            }
             
            else
            {
               qry = "insert into tblRegister(UserName,EmailId)" +
                                     " values('" + name + "','" + email + "')";
               eventRepository.AddUpdateDeleteSQL(qry);
               query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
               variables = eventRepository.GoogleLoginDetails(query);
               if (variables.Count != 0)
               {
                   string userid = variables[0];
                   string mail = variables[1];

                   Session["UserID"] = userid;
                   Session["EmailId"] = mail;
               }

                status = true;
            }

            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult FacebookLogin(string email, string first_name)
        {
            /*  var status = false;
              using (SqlConnection con = new SqlConnection(Connectionstring))
              {
                  string qry;
                  con.Open();
                  string query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
                  SqlCommand cmd = new SqlCommand(query, con);
                  cmd.CommandType = CommandType.Text;

                  SqlDataAdapter sda = new SqlDataAdapter(cmd);
                  DataTable datatable = new DataTable();
                  sda.Fill(datatable);
                  if (datatable.Rows.Count == 1)
                  {
                      DataRow row = datatable.Rows[0];
                      Session["UserID"] = row["UserID"].ToString();
                      Session["EmailId"] = row["EmailId"].ToString();
                      //Session["userid"] = uid;
                      // Session["email"] = mail;

                      status = true;
                  }
                  else
                  {
                      qry = "insert into tblRegister(UserName,EmailId)" +
                      " values('" + first_name + "','" + email + "')";
                      eventRepository.AddUpdateDeleteSQL(qry);
                      status = true;
                  }
              }
              return new JsonResult { Data = new { status = status } };*/

            var status = false;
            string qry;
            string query;
            List<string> variables;
            query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
            variables = eventRepository.GoogleLoginDetails(query);
            if (variables.Count != 0)
            {
                string userid = variables[0];
                string mail = variables[1];

                Session["UserID"] = userid;
                Session["EmailId"] = mail;
                status = true;
            }
            else
            {
                qry = "insert into tblRegister(UserName,EmailId)" +
                                     " values('" + first_name + "','" + email + "')";
                eventRepository.AddUpdateDeleteSQL(qry);
                query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
                variables = eventRepository.GoogleLoginDetails(query);
                if (variables.Count != 0)
                {
                    string userid = variables[0];
                    string mail = variables[1];

                    Session["UserID"] = userid;
                    Session["EmailId"] = mail;
                }
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }
        public JsonResult UserDetails()
        {
            string userid = Session["UserID"].ToString();
            string qry = "select * from tblRegister where UserID=" + userid;
            Registration register = eventRepository.GetUserDetails(qry);
            return new JsonResult { Data = register, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        [HttpPost]
        public JsonResult SaveUserDetails(Registration register)
        {
            var status = false;
            string userid = Session["UserID"].ToString();
            var dob = register.DOB.ToString("yyyy-MM-dd ");
            string qry = string.Empty;
            qry = "Update tblRegister set UserName = '" + register.Username + "', DOB = '" + dob +
                    "', Phone= '" + register.Phone + "',EmailId= '" + register.Email + "' where UserID= " + userid;
            int count = eventRepository.AddUpdateDeleteSQL(qry);
            if(count == 1)
            {
               status = true;
                Session["EmailId"] = register.Email;
                Session["UserName"] = register.Username;
            }
            return new JsonResult { Data = new { status = status, Username = register.Username } };
        }
        public JsonResult GetEvents()
        {
            string userid = null;
            if (Session["UserID"] != null)
            {
                userid = Session["UserID"].ToString();
            }
            List<Events> eventlist = eventRepository.EventsList(userid);
            return new JsonResult { Data = eventlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult Edit(int id)
        {
            Events events = eventRepository.GetEventById(id);
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        [HttpPost]
        public JsonResult SaveEvent(Events events)
        {
            string userid = Session["UserID"].ToString();
            eventRepository.EditEvent(events, userid);
            var status = true;
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            Events events = new Events();
            eventRepository.Delete(eventID);
            var status = true;
            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult CreateEvent(Events events)
        {
            string userid = Session["UserID"].ToString();
            eventRepository.AddEvent(events, userid);
            var status = true;
            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult Logout()
        {
            Session.Clear();
            var status = true;
            return new JsonResult { Data = new { status = status }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public void SendMailToUser(object sender, EventArgs e)
        {
            bool status = false;

            var currentDate = DateTime.Now;
            var eventDate = currentDate.AddMinutes(+5).ToString("yyyy-MM-dd HH:mm");
            string qry = $"Select EmailId,StartDate,Subject,Description from tblRegister join tblEvents on (tblRegister.UserID=tblEvents.UserID) where StartDate='{eventDate}' ";
            List<Events> mailDetails = eventRepository.GetMailDetails(qry);
            foreach (Events item in mailDetails)
            {
                string ebody = "<p>Hi,<br />This is a reminder of the following event-<br />Event:" + item.Subject + "<br />" + "Description:" + item.Description + "<br />" + "Time:" + item.StartDate + "</ p > ";
                status = SendEmail(item.Email, "EventReminderApp1", ebody);
            }

        }

        public bool SendEmail(string toEmail, string subject, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString(); ;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                MailMessage mailMessage = new MailMessage(senderEmail, toEmail, subject, emailBody);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                Exception e = ex;
                return false;
            }

        }
        public void SendResetPasswordLinkEmail(string toEmail, string activationCode)
        {
            var verifyUrl = "/User/ResetPassword/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var subject = "Reset Password";
            string emailBody = "<p>Hi,<br /><br />A request is been sent to reset your account password. Please click the link below to reset your password </ p >" +
                "<br /><br /><a href=" + link + ">Reset Password link</a>";

            string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
            string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString(); ;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Timeout = 100000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            MailMessage mailMessage = new MailMessage(senderEmail, toEmail, subject, emailBody);
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = UTF8Encoding.UTF8;
            client.Send(mailMessage);

            return;
        }


        [HttpPost]
        public JsonResult ForgetPassword(string email)
        {
            string qry;
            var status = false;
            string query = $"Select EmailId from tblRegister where EmailId='{email}' ";
            bool verify = eventRepository.VerifyEmail(query);
            if (verify)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendResetPasswordLinkEmail(email, resetCode);
                qry = $"Update tblRegister set ResetPasswordCode='{resetCode}' where EmailId='{email}'";
                eventRepository.AddUpdateDeleteSQL(qry);
                status = true;
                return new JsonResult { Data = new { status = status } };
            }
            else
            {
                return new JsonResult { Data = new { status = status } };
            }
        }
        public ActionResult ResetPassword(string id)
        {
            //resetid = id;
            string qry = $"Select * from tblRegister where ResetPasswordCode= '{id}' ";
            var user = eventRepository.GetUser(qry);
            if (user != null)
            {
                ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
                resetPasswordModel.ResetCode = id;
                return View(resetPasswordModel);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            string qry = $"Select * from tblRegister where ResetPasswordCode= '{resetPasswordModel.ResetCode}' ";
            var user = eventRepository.GetUser(qry);
            if(user != null)
            {
                string query = "update tblRegister set Password = '" + resetPasswordModel.NewPassword + "' where ResetPasswordCode ='" + resetPasswordModel.ResetCode + "'";
                eventRepository.AddUpdateDeleteSQL(query);
                string query2 = "update tblRegister set ResetPasswordCode = '" + "" + "' where ResetPasswordCode ='" + resetPasswordModel.ResetCode + "'";
                ViewBag.Message = "Password updated successfully";
            }
            else
            {
                ViewBag.Message = "Failed";
            }
            return View(resetPasswordModel);
        }
    }
}