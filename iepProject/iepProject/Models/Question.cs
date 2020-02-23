using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace iepProject.Models
{
    public class Question
    {

        public Question()
        {
            this.Answers = new HashSet<Answer>();
        }


        public int Id { get; set; }

        public String Title { get; set; }
        public String Text { get; set; }

        [DisplayName("Upload File")]
        public string ImagePath { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }


        public bool IsLocked { get; set; }

        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }


        public virtual ICollection<Answer> Answers { get; set; }
    }
}