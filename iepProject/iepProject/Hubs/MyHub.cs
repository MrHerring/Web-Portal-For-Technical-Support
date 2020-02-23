using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace iepProject.Hubs
{
    public class MyHub : Hub
    {

        private ApplicationContext db = new ApplicationContext();

        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string username, string message, string room, Guid roomId, int creatorId)
        {
            iepProject.Models.Message msg = new Models.Message();
            msg.Text = message;
            msg.ChatRoomId = roomId;
            msg.CreatorId = creatorId;

            db.Messages.Add(msg);

            db.SaveChanges();

            Clients.Group(room).displayMessage(username, message);
        }

        //room <--> ChatRoom Id
        public void JoinGroup(string room)
        {
            Groups.Add(Context.ConnectionId, room);
        }

    }
}