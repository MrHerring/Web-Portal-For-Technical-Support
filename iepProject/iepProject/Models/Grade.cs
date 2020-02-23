using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iepProject.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public bool IsPositive { get; set; }

        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        public int AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
    }
}