using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iepProject.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public String Status { get; set; }

        public String Amount { get; set; }

        public String Package { get; set; }

        public float Paid { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

    }
}