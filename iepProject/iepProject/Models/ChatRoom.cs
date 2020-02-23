using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iepProject.Models
{
    public class ChatRoom
    {
        public ChatRoom()
        {
            this.Id = Guid.NewGuid();
            this.Messages = new HashSet<Message>();
            this.InChatRooms = new HashSet<InChatRoom>();
        }

        public Guid Id { get; set; }

        
        public DateTime TimeCreated { get; set; }

        //Creator of a ChatRoom
        public int CreatorID { get; set; }
        public virtual User Creator { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<InChatRoom> InChatRooms { get; set; }


    }
}