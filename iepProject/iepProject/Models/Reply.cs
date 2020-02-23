using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iepProject.Models
{
    public class Reply
    {

        public int Id { get; set; }

        public String Text { get; set; }

        public int AuthorId { get; set; }
        public virtual User Author { get; set; }


        public int AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public DateTime TimeCreated { get; set; }

    }
}