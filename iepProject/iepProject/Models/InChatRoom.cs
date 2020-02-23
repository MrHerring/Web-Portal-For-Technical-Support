using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iepProject.Models
{
    public class InChatRoom
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public Guid ChatRoomId { get; set; }

        public virtual ChatRoom ChatRoom { get; set; }

    }
}