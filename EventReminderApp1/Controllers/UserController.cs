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
        string Connectionstring = @"Data Source= LENOVO\SQLSERVER; Initial Catalog = dbwebapp; Integrated Security = True";
        EventRepository eventRepository = new EventRepository();
        // GET: User

      /*  public UserController()
        {
            Timer myTimer = new Timer();
            myTimer.Interval = 60000;
            myTimer.AutoReset = true;
            myTimer.Elapsed += new ElapsedEventHandler(SendMailToUser);
            myTimer.Enabled = true;
        }*/

        public ActionResult Home()
        {
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
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                con.Open();
                string query = "Select UserID,EmailId,Password From tblRegister Where EmailId=@EmailId and Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@EmailId", login.Email);
                cmd.Parameters.AddWithValue("@Password", login.Password);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable datatable = new DataTable();
                sda.Fill(datatable);
                if (datatable.Rows.Count == 1)
                {
                    DataRow row = datatable.Rows[0];
                    login.UserID = Convert.ToInt32(row["UserID"]);
                    Session["UserID"] = row["UserID"].ToString();
                    Session["EmailId"] = login.Email.ToString();

                    status = true;
                }
                else
                {

                }
                return new JsonResult { Data = new { status = status } };
            }
        }
        [HttpPost]
        public JsonResult GoogleLogin(string email, string name, string gender, string lastname, string location)
        {
            var status = false;
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
                    string uid = row["UserID"].ToString();
                    string mail = row["EmailId"].ToString();
                    Session["UserID"] = uid;
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
            }
            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult FacebookLogin(string email, string name)
        {
            var status = false;
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
                    string uid = row["UserID"].ToString();
                    string mail = row["EmailId"].ToString();
                    Session["UserID"] = uid;
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
            eventRepository.AddEditEvent(events,userid);
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
            eventRepository.AddEditEvent(events, userid);
            var status = true;
            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult Logout()
        {
            Session.Clear();
            var status = true;
            return new JsonResult { Data = new { status = status },JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

       /* public void SendMailToUser(object sender, EventArgs e)
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

        }*/


    }
}