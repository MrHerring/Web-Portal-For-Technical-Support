using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace iepProject.Models
{
    public class Answer
    {
        public Answer()
        {
            this.Replies = new HashSet<Reply>();
            this.PositiveRatingCnt = 0;
            this.NegativeRatingCnt = 0;
        }


        public int Id { get; set; }
        public String Text { get; set; }

        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }

        public DateTime TimeCreated { get; set; }

        public int PositiveRatingCnt { get; set; }
        public int NegativeRatingCnt { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }
    }
}