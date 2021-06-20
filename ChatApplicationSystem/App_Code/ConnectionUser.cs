using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApplicationSystem
{
    /// <summary>
    /// Summary description for SalesOrderNotification
    /// </summary>
    [Serializable]
    public class ChatUserConnection
    {
        private int _id;

        public int UserID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _connectionID;

        public string ConnectionId
        {
            get { return _connectionID; }
            set { _connectionID = value; }
        }
    }

}