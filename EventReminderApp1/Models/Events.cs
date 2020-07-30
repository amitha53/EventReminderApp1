using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventReminderApp1.Models
{
    public class Events
    {
        public int EventID { get; set; }
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateStr { get { return StartDate.ToString(); } }
        public DateTime EndDate { get; set; }
        public string EndDateStr { get { return EndDate.ToString(); } }
        public string MailSend { get; set; }
    }
}