using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApplicationSystem
{
    public class Chatting
    {
        public int ID { get; set; }
        public DateTime SendTime { get; set; }
        public string Message { get; set; }
        public int SendByID { get; set; }
        public int ReceiveByID { get; set; }
        public bool IsRead { get; set; }
    }
}