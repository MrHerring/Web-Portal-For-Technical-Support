using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iepProject.Models
{
    public class Message
    {
        public int Id { get; set; }

        public String Text { get; set; }

        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        public Guid ChatRoomId { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }
    }
}