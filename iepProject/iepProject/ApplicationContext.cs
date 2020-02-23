using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using iepProject.Models;

namespace iepProject
{
    public class ApplicationContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public ApplicationContext() : base("iep_project_db")
        {
            
        }

        public System.Data.Entity.DbSet<iepProject.Models.Answer> Answers { get; set; }

        public System.Data.Entity.DbSet<iepProject.Models.ChatRoom> ChatRooms { get; set; }

        public System.Data.Entity.DbSet<iepProject.Models.InChatRoom> InChatRooms { get; set; }

        public System.Data.Entity.DbSet<iepProject.Models.Message> Messages { get; set; }

        public System.Data.Entity.DbSet<iepProject.Models.Reply> Replies { get; set; }
    }
}


