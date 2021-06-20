using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApplicationSystem
{
    public class RecentChatUser
    {
        public int ID { get; set; }
        public int SendByID { get; set; }
        public int ReceiveByID { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        /// <summary>
        /// UnReadMessageCount for show statistic for message count
        /// </summary>
        public int UnreadMessageCount { get; set; }

    }
}