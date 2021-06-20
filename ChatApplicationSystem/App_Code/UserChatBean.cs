using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ChatApplicationSystem.App_Code
{
    public class UserChatBean
    {
        #region Constructor
        public UserChatBean()
        { }
        #endregion

        #region  Properties Object
        public Chatting UserChatObject;
        #endregion

        #region ICommonOperations Members
        public int InsertRecord()
        {
            List<SqlParameter> Params = new List<SqlParameter>();
            Params.Add(new SqlParameter("@SendByID", UserChatObject.SendByID));
            Params.Add(new SqlParameter("@ReceiveByID", UserChatObject.ReceiveByID));
            Params.Add(new SqlParameter("@SendTime", UserChatObject.SendTime));
            Params.Add(new SqlParameter("@Message", UserChatObject.Message));
            Params.Add(new SqlParameter("@IsRead", UserChatObject.IsRead));
            Params.Add(new SqlParameter("@OPERATIONID", 1));
            SqlParameter param = new SqlParameter("@UserChatID", UserChatObject.ID);
            param.Direction = ParameterDirection.Output;
            Params.Add(param);
            int Id = DBHelper.ExecuteNonQuery("Sp_Usr_UserChat", Params, "@UserChatID");
            return Id;
        }
        public int UpdateRecord()
        {
            List<SqlParameter> Params = new List<SqlParameter>();
            Params.Add(new SqlParameter("@ID", UserChatObject.ID));
            Params.Add(new SqlParameter("@SendByID", UserChatObject.SendByID));
            Params.Add(new SqlParameter("@ReceiveByID", UserChatObject.ReceiveByID));
            Params.Add(new SqlParameter("@SendTime", UserChatObject.SendTime));
            Params.Add(new SqlParameter("@Message", UserChatObject.Message));
            Params.Add(new SqlParameter("@IsRead", UserChatObject.IsRead));
            Params.Add(new SqlParameter("@OPERATIONID", 2));
            int Id = DBHelper.ExecuteNonQuery("Sp_Usr_UserChat", Params);
            return Id;
        }
        public int DeleteRecord(int ID)
        {
            return DBHelper.ExecuteNonQueryWithoutProcedure("Exec Sp_Usr_UserChat @OPERATIONID = 3, @ID = " + ID.ToString());
        }
        public DataTable usr_UserChat(List<SqlParameter> Params)
        {
            return DBHelper.FillTable("Sp_Usr_UserChat", Params);
        }

        public DataSet usr_getRecentChatUser(List<SqlParameter> Params)
        {
            return DBHelper.GetDataSet("Sp_Usr_UserChat", Params);
        }

        #endregion
    }
}