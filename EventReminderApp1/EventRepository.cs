using EventReminderApp1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EventReminderApp1
{
    public class EventRepository
    {
        const string Connectionstring = @"Data Source= LENOVO\SQLSERVER; Initial Catalog = dbwebapp; Integrated Security = True";

        SqlConnection con = new SqlConnection(Connectionstring);

        public void UserRegister(Registration register)
        {
            con.Open();
            string query = "Insert Into tblRegister Values(@UserName,@EmailId,@Password)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserName", register.Username);
            cmd.Parameters.AddWithValue("@EmailId", register.Email);
            cmd.Parameters.AddWithValue("@Password", register.Password);
            cmd.ExecuteNonQuery();
        }
        public List<string> LoginDetails(Registration login)
        {
            List<string> variables = new List<string>();
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
                string userid = row["UserID"].ToString();
                string mail = row["EmailId"].ToString();
                variables.Add(userid);
                variables.Add(mail);
            }
            con.Close();
            return variables;

        }

        public List<string> GoogleLoginDetails(string query)
        {
            List<string> variables = new List<string>();
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable datatable = new DataTable();
            sda.Fill(datatable);

            if (datatable.Rows.Count == 1)
            {
                DataRow row = datatable.Rows[0];
                string userid = row["UserID"].ToString();
                string mail = row["EmailId"].ToString();
                variables.Add(userid);
                variables.Add(mail);
            }
            con.Close();
            return variables;

        }

        public void AddEvent(Events events, string userid)
        {
            string qry = string.Empty;
           // var startdate = Convert.ToDateTime(events.StartDate);
          //  var enddate = Convert.ToDateTime(events.EndDate);
            var startdate = events.StartDate.ToString("yyyy-MM-dd HH:mm");
            var enddate = events.EndDate.ToString("yyyy-MM-dd HH:mm");

            qry = "insert into tblEvents(UserID,Subject,Description,StartDate,EndDate)" +
                    " values('" + userid + "','" + events.Subject + "','" + events.Description + "','" + startdate + "','" + enddate + "')";
            this.AddUpdateDeleteSQL(qry);
        }
        public void EditEvent(Events events, string userid)
        {
            string qry = string.Empty;
            qry = "Update tblEvents set Subject = '" + events.Subject + "', Description = '" + events.Description +
                    "', StartDate= '" + events.StartDate + "',EndDate= '" + events.EndDate + "' where EventID= '" + events.EventID + "' and UserID= " + userid;
            this.AddUpdateDeleteSQL(qry);
        }

        public int AddUpdateDeleteSQL(string qry)
        {
            con.Open();
            int count = new SqlCommand(qry, con).ExecuteNonQuery();
            con.Close();
            return count;
        }

        public Events GetEventById(int eventId)
        {
            string qry = "select * from tblEvents where EventId=" + eventId;
            DataRow row = GetSQLList(qry).Rows[0];

            return new Events
            {
                EventID = Convert.ToInt32(row.ItemArray[0]),
                UserID = Convert.ToInt32(row.ItemArray[1]),
                Subject = row.ItemArray[2].ToString(),
                Description = row.ItemArray[3].ToString(),
                StartDate = Convert.ToDateTime(row.ItemArray[4]),
                EndDate = Convert.ToDateTime(row.ItemArray[5]),
            };
        }

        private DataTable GetSQLList(string qry)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader sdr = cmd.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(sdr);
            con.Close();
            return datatable;
        }

        public int Delete(int eventId)
        {
            string qry = "Delete from tblEvents where EventId=" + eventId;
            return this.AddUpdateDeleteSQL(qry);
        }

        public List<Events> EventsList(string userid)
        {
            string query = "Select * from tblEvents where UserID=" + userid;
            DataTable datatable = GetSQLList(query);

            List<Events> eventList = new List<Events>();

            foreach (DataRow row in datatable.Rows)
            {
                Events eventModel = new Events();
                eventModel.EventID = Convert.ToInt32(row.ItemArray[0]);
                eventModel.UserID = Convert.ToInt32(row.ItemArray[1]);
                eventModel.Subject = row.ItemArray[2].ToString();
                eventModel.Description = row.ItemArray[3].ToString();
                eventModel.StartDate = Convert.ToDateTime(row.ItemArray[4]);
                eventModel.EndDate = Convert.ToDateTime(row.ItemArray[5]);
                eventList.Add(eventModel);
            }
            return eventList;
        }

        public List<Events> GetMailDetails(string qry)
        {
           
            List<Events> mailDetails = new List<Events>();
           // var startdate = Convert.ToDateTime(events.StartDate);
            con.Open();
            SqlCommand cmd = new SqlCommand(qry, con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable datatable = new DataTable();
            sda.Fill(datatable);

            if (datatable.Rows.Count != 0)
            {
                foreach (DataRow row in datatable.Rows)
                {
                    Events events = new Events();
                    events.Email = row["EmailId"].ToString();
                    events.StartDate = Convert.ToDateTime(row["StartDate"]);
                    events.Subject = row["Subject"].ToString();
                    events.Description = row["Description"].ToString();
                    mailDetails.Add(events);
                }
            }
            con.Close();
            return mailDetails;
        }

    }
}