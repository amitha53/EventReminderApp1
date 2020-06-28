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
        // GET: User

       public UserController()
        {
            Timer myTimer = new Timer();
            myTimer.Interval = 60000;
            myTimer.AutoReset = true;
            myTimer.Elapsed += new ElapsedEventHandler(SendMailToUser);
            myTimer.Enabled = true;
            //SendEmail("amithaunnikrishnan415@gmail.com", "Reminder", "hi");
        }

        public ActionResult Home()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.UserId = Session["UserID"];
                ViewBag.Emaild = Session["EmailId"];
                return View();
            }
            return View();
        }

        [HttpPost]
        public JsonResult Register(Registration register)
        {
            eventRepository.UserRegister(register);
            var status = true;
            return new JsonResult { Data = new { status = status } };
        }
        [HttpPost]
        public JsonResult Login(Registration login)
        {
            var status = false;
              
            List<string> variables = eventRepository.LoginDetails(login);

            string userid = variables[0];
            string mail = variables[1];
            
            if (variables != null)
            {
                Session["UserID"] = userid;
                Session["EmailId"] = mail;
                status = true;
            }
            else
            {

            }
            return new JsonResult { Data = new { status = status } };
        }
        [HttpPost]
        public JsonResult GoogleLogin(string email, string name, string gender, string lastname, string location)
        {
            var status = false;
            string qry;
            string query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
            List<string> variables = eventRepository.GoogleLoginDetails(query);
            string userid = variables[0];
            string mail = variables[1];

            if (variables != null)
            {
                Session["UserID"] = userid;
                Session["EmailId"] = mail;
                status = true;
            }
            else
            {
                qry = "insert into tblRegister(UserName,EmailId)" +
                                    " values('" + name + "','" + email + "')";
                eventRepository.AddUpdateDeleteSQL(qry);
                status = true;
            }
               
            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult FacebookLogin(string email, string name)
        {
            var status = false;
            string qry;
            string query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
            List<string> variables = eventRepository.GoogleLoginDetails(query);
            string userid = variables[0];
            string mail = variables[1];

            if (variables != null)
            {
                Session["UserID"] = userid;
                Session["EmailId"] = mail;
                status = true;
            }
            else
            {
                qry = "insert into tblRegister(UserName,EmailId)" +
                                    " values('" + name + "','" + email + "')";
                eventRepository.AddUpdateDeleteSQL(qry);
                status = true;
            }

            return new JsonResult { Data = new { status = status } };
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
        [HttpPost]
        public JsonResult SaveEvent(Events events)
        {
            string userid = Session["UserID"].ToString();  
            eventRepository.EditEvent(events,userid);
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
            return new JsonResult { Data = new { status = status },JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

       public void SendMailToUser(object sender, EventArgs e)
        {
            bool status = false;

            var currentDate = DateTime.Now;
            var eventDate = currentDate.AddMinutes(+5).ToString("MM-DD-YYYY HH:mm a");
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


    }
}