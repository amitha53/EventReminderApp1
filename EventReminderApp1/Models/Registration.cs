using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventReminderApp1.Models
{
    public class Registration
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public DateTime DOB { get; set; }
        public string DOBStr { get { return DOB.ToString("dd-MM-yyyy"); } }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ResetPasswordCode { get; set; }
    }
}