using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iepProject.Models
{
    public class User
    {

        public User()
        {
            this.Questions = new HashSet<Question>();
            this.Answers = new HashSet<Answer>();
            this.Grades = new HashSet<Grade>();
        }

        public int Id { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public String LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        public int TokenAmount { get; set; }
        // true: active, false: blocked
        public bool Status { get; set; }

        // 0: admin, 1: user, 2: techSupport
        public int Role { get; set; }


        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }

    }
}