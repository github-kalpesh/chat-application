using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using ChatApplicationSystem;
using ChatApplicationSystem.App_Code;

namespace ChatApplicationSystem
{
    /// <summary>
    /// Chat application main hub
    /// </summary>
    public class ChatApplicationHub : Hub
    {
        //public static List<User> userlist = new List<User>();

        private int loginUserID;
        private static List<ChatUserConnection> onlineUserList = new List<ChatUserConnection>();

        private List<ChatUser> _availableUsers = new List<ChatUser>();
        /// <summary>
        /// When new or old user connect
        /// </summary>
        /// <param name="userID">login user id</param>
        public void UserConnect(int userID)
        {
            //Temp 
            loginUserID = userID;
            var user = ChatHelper.getUser(userID);
            if (user != null)
            {
                if (onlineUserList != null && onlineUserList.Any(x => x.UserID == userID))
                {
                    onlineUserList.Remove(onlineUserList.FirstOrDefault(x => x.UserID == userID));
                    onlineUserList.Add(new ChatUserConnection()
                    {
                        UserID = userID,
                        Name = user.Name,
                        ConnectionId = Context.ConnectionId
                    });
                }
                else
                {
                    onlineUserList.Add(new ChatUserConnection()
                    {
                        UserID = userID,
                        Name = user.Name,
                        ConnectionId = Context.ConnectionId
                    });
                }
                broadcastNewOnlineUser();
            }
            else
            {
                Clients.Caller.userNotAvailable();
            }
        }

        public void broadcastNewOnlineUser()
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var onlineUserJson = jsonSerialiser.Serialize(onlineUserList);
            var availableUsersWithRecentChatJson = GetAllUsersWithRecentChat();
            Clients.AllExcept(Context.ConnectionId).receiveOnlineUsers(onlineUserJson);
            Clients.Caller.NewOnlineUsers(onlineUserJson, availableUsersWithRecentChatJson);
        }

        public void broadcastOnlineUser()
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var onlineUserJson = jsonSerialiser.Serialize(onlineUserList);
            Clients.All.receiveOnlineUsers(onlineUserJson);
        }

        public void OfflineUser(string connectionID)
        {
            ChatUserConnection objRemove = onlineUserList.Find(t => t.ConnectionId == connectionID);
            if (objRemove != null)
            {
                onlineUserList.Remove(objRemove);
                broadcastOnlineUser();
            }
        }

        public string SendMessageToUser(int userId,int receiveByID, string message)
        {
            var rtn = "";
            var chatID = ChatHelper.InsertUserChat(userId, receiveByID, DateTime.Now, message, false);
            if (chatID > 0)
            {
                var userChat = ChatHelper.getUserChat(chatID);
                if(userChat != null)
                {
                    var jsonSerialiser = new JavaScriptSerializer();
                    rtn = jsonSerialiser.Serialize(userChat);
                    if (IsConnected(receiveByID))
                    {
                        var onlineUser = onlineUserList.FirstOrDefault(x => x.UserID == receiveByID);
                        Clients.Client(onlineUser.ConnectionId).reciveNewMessage(userId, rtn);
                    }
                }
            }
            return rtn;
        }

        public string GetUserChats(int loginID, int receiverID)
        {
            var rtn = new List<Chatting>();
            var jsonSerialiser = new JavaScriptSerializer();
            rtn = ChatHelper.getPendingMessages(loginID, receiverID);
            ChatHelper.setMarkAsRead(loginID, receiverID);
            return jsonSerialiser.Serialize(rtn);
        }

        public int GetLoginUserId()
        {
            //Get Login user id
            return loginUserID;
        }



        public override Task OnConnected()
        {
            loginUserID = 1;
            //UserConnect(loginUserID);
            //OnlineUser(Context.ConnectionId)
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            OfflineUser(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        #region private function

        private bool IsConnected(int userId)
        {
            bool isConnected = false;
            if(onlineUserList != null && onlineUserList.Count > 0)
            {
                if(onlineUserList.Any(x=>x.UserID == userId))
                {
                    isConnected = true;
                }
            }
            return isConnected;
        }

        private string GetAllUsers()
        {
            var jsonSerialiser = new JavaScriptSerializer();
            if (_availableUsers != null && _availableUsers.Count > 0)
            {
                var json = jsonSerialiser.Serialize(_availableUsers);
                return json.ToString();
            }
            else
            {
                var users = ChatHelper.getAllUser(loginUserID);
                if (users != null)
                {
                    var json = jsonSerialiser.Serialize(users);
                    return json.ToString();
                }
            }
            return jsonSerialiser.Serialize(new List<ChatUser>());
        }

        private string GetAllUsersWithRecentChat()
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var recentChatUser = ChatHelper.getRecentChatUser(loginUserID);
            if (recentChatUser != null)
            {
                var json = jsonSerialiser.Serialize(recentChatUser);
                return json.ToString();
            }
            return jsonSerialiser.Serialize(new List<ChatUser>());
        }

        #endregion private function
    }
}