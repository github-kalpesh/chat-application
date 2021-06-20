using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ChatApplicationSystem.App_Code
{
    public static class ChatHelper
    {

        /// <summary>
        /// Insert user chat into table 
        /// </summary>
        /// <param name="sendByID"></param>
        /// <param name="receiveByID"></param>
        /// <param name="sendTime"></param>
        /// <param name="message"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        public static int InsertUserChat(int sendByID,int receiveByID, DateTime sendTime, string message, bool isRead = false)
        {
            try
            {
                UserChatBean ucBean = new UserChatBean();
                Chatting uc = new Chatting();
                uc.SendByID = sendByID;
                uc.ReceiveByID = receiveByID;
                uc.Message = message;
                uc.SendTime = sendTime;
                uc.IsRead = isRead;
                ucBean.UserChatObject = uc;
                return ucBean.InsertRecord();
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public static int UpdateUserChat(int ID,int sendByID, int receiveByID, DateTime sendTime, string message, bool isRead = false)
        {
            try
            {
                UserChatBean ucBean = new UserChatBean();
                Chatting uc = new Chatting();
                uc.ID = ID; 
                uc.SendByID = sendByID;
                uc.ReceiveByID = receiveByID;
                uc.Message = message;
                uc.SendTime = sendTime;
                uc.IsRead = isRead;
                ucBean.UserChatObject = uc;
                return ucBean.UpdateRecord();
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public static int DeleteUserChat(int ID)
        {
            try
            {
                UserChatBean uc = new UserChatBean();
                return uc.DeleteRecord(ID);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        /// <summary>
        /// Get user by  ID
        /// </summary>
        /// <returns></returns>
        public static ChatUser getUser(int userID)
        {
            var rtnUser = new ChatUser();
            try
            {
                UserChatBean uc = new UserChatBean();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("@ID", userID));
                Params.Add(new SqlParameter("@OPERATIONID", 7));
                DataTable dt = uc.usr_UserChat(Params); ;
                rtnUser = CreateObjectFromTable<ChatUser>(dt);
            }
            catch (Exception ex)
            {
                rtnUser = null;
            }
            return rtnUser;
        }
        /// <summary>
        /// Get All active user
        /// </summary>
        /// <returns></returns>
        public static List<ChatUser> getAllUser(int loginUserID)
        {
            var rtnUsers = new List<ChatUser>();
            try
            {
                UserChatBean uc = new UserChatBean();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("@ID", loginUserID));
                Params.Add(new SqlParameter("@OPERATIONID", 8));
                DataTable dt = uc.usr_UserChat(Params);
                rtnUsers = CreateListFromTable<ChatUser>(dt);
            }
            catch (Exception ex)
            {
            }
            return rtnUsers;
        }

        /// <summary>
        /// get last chat data, for recent chat list
        /// </summary>
        /// <returns>List of RecentChatUser</returns>
        public static Tuple<List<ChatUser>, List<RecentChatUser>> getRecentChatUser(int loginUserID)
        {
            var users = new List<ChatUser>();
            var recentChat = new List<RecentChatUser>();
            var rtnRecentUsers = Tuple.Create(users, recentChat);
            try
            {
                UserChatBean uc = new UserChatBean();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("@SendByID", loginUserID));
                Params.Add(new SqlParameter("@OPERATIONID", 9));
                DataSet ds = uc.usr_getRecentChatUser(Params);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {

                    recentChat = CreateListFromTable<RecentChatUser>(ds.Tables[0]);
                    users = CreateListFromTable<ChatUser>(ds.Tables[1]);
                    rtnRecentUsers = Tuple.Create(users, recentChat);
                }
            }
            catch (Exception ex)
            {
            }
            return rtnRecentUsers;
        }

        /// <summary>
        /// get last chat data for specific user, for recent chat list
        /// </summary>
        /// <param name="loginUserID">Current login user</param>
        /// <param name="receiveByID">recent with user</param>
        /// <returns></returns>
        public static RecentChatUser getRecentChatUser(int loginUserID, int receiveByID)
        {
            var rtnRecentUsers = new RecentChatUser();
            try
            {
                UserChatBean uc = new UserChatBean();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("@SendByID", loginUserID));
                Params.Add(new SqlParameter("@ReceiveByID", receiveByID));
                Params.Add(new SqlParameter("@OPERATIONID", 10));
                DataTable dt = uc.usr_UserChat(Params);
                rtnRecentUsers = CreateObjectFromTable<RecentChatUser>(dt);
            }
            catch (Exception ex)
            {
            }
            return rtnRecentUsers;
        }

        /// <summary>
        /// Get Unread messages
        /// </summary>
        /// <param name="loginUserID">Current login user</param>
        /// <param name="receiveByID">Message send by user</param>
        /// <returns></returns>
        public static List<Chatting> getPendingMessages(int loginUserID, int receiveByID, int currentPage = 1, int recordPerPage = 50)
        {
            var rtnUserChat = new List<Chatting>();
            try
            {
                UserChatBean uc = new UserChatBean();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("@SendByID", loginUserID));
                Params.Add(new SqlParameter("@ReceiveByID", receiveByID));

                Params.Add(new SqlParameter("@CurrentPage", currentPage));
                Params.Add(new SqlParameter("@RecordPerPage", recordPerPage));

                Params.Add(new SqlParameter("@OPERATIONID", 5));
                DataTable dt = uc.usr_UserChat(Params);
                rtnUserChat = CreateListFromTable<Chatting>(dt);
            }
            catch (Exception ex)
            {
                
            }
            return rtnUserChat;
        }

        /// <summary>
        /// Get  messages
        /// </summary>
        /// <param name="id">user chat id</param>
        /// <returns></returns>
        public static Chatting getUserChat(int id)
        {
            var rtnUserChat = new Chatting();
            try
            {
                UserChatBean uc = new UserChatBean();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("@ID", id));
                Params.Add(new SqlParameter("@OPERATIONID", 13));
                DataTable dt = uc.usr_UserChat(Params);
                rtnUserChat = CreateObjectFromTable<Chatting>(dt);
            }
            catch (Exception ex)
            {

            }
            return rtnUserChat;
        }

        /// <summary>
        /// set mark as read messages
        /// </summary>
        /// <param name="id">user chat id</param>
        /// <returns></returns>
        public static int setMarkAsRead(int id)
        {
            var rtn = 0;
            try
            {
                UserChatBean uc = new UserChatBean();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("@ID", id));
                Params.Add(new SqlParameter("@IsRead", 1));
                Params.Add(new SqlParameter("@OPERATIONID", 12));
                DataTable dt = uc.usr_UserChat(Params);
                rtn = 1;
            }
            catch (Exception ex)
            {
                rtn = 0;
            }
            return rtn;
        }

        /// <summary>
        /// set mark as read messages
        /// </summary>
        /// <param name="id">user chat id</param>
        /// <returns></returns>
        public static int setMarkAsRead(int loginUserID, int receiveByID)
        {
            var rtn = 0;
            try
            {
                UserChatBean uc = new UserChatBean();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("@SendByID", receiveByID));
                Params.Add(new SqlParameter("@ReceiveByID", loginUserID));
                Params.Add(new SqlParameter("@IsRead", 1));
                Params.Add(new SqlParameter("@OPERATIONID", 14));
                uc.usr_UserChat(Params);
                rtn = 1;
            }
            catch (Exception ex)
            {
                rtn = 0;
            }
            return rtn;
        }

        #region Common function
        // function that creates an object from the given data table row
        public static T CreateObjectFromTable<T>(DataTable tbl) where T : new()
        {
            // define return Obj
            T obj = new T();

            // go through each row
            if(tbl != null && tbl.Rows.Count > 0)
            {
                DataRow row = tbl.Rows[0];
                obj = CreateItemFromRow<T>(row);
            }
            // return the Obj
            return obj;
        }
        // function that creates a list of an object from the given data table
        public static List<T> CreateListFromTable<T>(DataTable tbl) where T : new()
        {
            // define return list
            List<T> lst = new List<T>();

            // go through each row
            foreach (DataRow r in tbl.Rows)
            {
                // add to the list
                lst.Add(CreateItemFromRow<T>(r));
            }

            // return the list
            return lst;
        }

        // function that creates an object from the given data row
        private static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            // create a new object
            T item = new T();

            // set the item
            SetItemFromRow(item, row);

            // return 
            return item;
        }

        private static void SetItemFromRow<T>(T item, DataRow row) where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }
        #endregion
    }
}