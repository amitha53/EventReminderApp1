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
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}