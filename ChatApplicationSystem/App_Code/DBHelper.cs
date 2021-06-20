using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Web;
using System.IO;

namespace ChatApplicationSystem.App_Code
{
    public class DBHelper
    {
        #region Constructors

        public DBHelper()
        {
        }

        #endregion

        #region Variables

        public static int Commandtimeout = 10000;

        #endregion

        #region Properties

        public bool IsErrorFound
        { get; set; }

        string _ErrorMessage;
        public string ErrorMessage
        {
            get
            {
                return "<span style='color: Red'>" + _ErrorMessage + "</span>";
            }
            set
            {
                _ErrorMessage = value;
            }
        }

        #endregion

        #region Mothods
        /// <summary>
        /// To Audit SQL operation 
        /// </summary>
        /// <authoer>Maulik A. Dusara</authoer>
        /// <date>27-05-2015</date>
        /// <returns>To Log SQL command to Log File</returns>
        public static void InsertIntoAuditTrail(string SPName, string MethodName, bool Status, SqlParameterCollection Data, string Query)
        {
            SqlParameter[] objPara = new SqlParameter[Data.Count];

            Data.CopyTo(objPara, 0);

            InsertIntoAuditTrail(SPName, MethodName, Status, objPara.ToList(), Query);
        }
        //Author : Kinjan Shah
        //Usage  : Gets Required Information to Store in Audit Trail
        //Created Date : 06-02-2015
        public static void InsertIntoAuditTrail(string SPName, string MethodName, bool Status, List<SqlParameter> Data, string Query)
        {
            string FileName = DateTime.Now.ToShortDateString().Replace("/", "-");
            string LogFilePath = HttpContext.Current.Server.MapPath("~/Logs");
            string FullFileName = LogFilePath + "\\" + FileName + ".txt";
            string UserName = "";
            string PageURL = "";
            string Parameters = "";
            string CurrentPageCookieName = "PageURL";// ConfigurationManager.AppSettings["PageURLCookie"].ToString();
            StreamWriter SR = null;

            try
            {
                if (!File.Exists(FullFileName))
                {
                    SR = File.CreateText(FullFileName);
                }
                else
                {
                    SR = File.AppendText(FullFileName);
                    SR.WriteLine("----------------------------------------------------------------------------------");
                }


                if (HttpContext.Current.Request.Cookies[ConfigurationManager.AppSettings["ApplicationCookie"].ToString() + "UserName"] != null)
                {
                    UserName = HttpContext.Current.Request.Cookies[ConfigurationManager.AppSettings["ApplicationCookie"].ToString() + "UserName"].Value.ToString();
                }


                HttpCookie myCookieValue = HttpContext.Current.Request.Cookies[ConfigurationManager.AppSettings["ApplicationCookie"].ToString() + CurrentPageCookieName];
                if (myCookieValue != null && !string.IsNullOrEmpty(myCookieValue.Value))
                {
                    PageURL = myCookieValue.Value;
                }

                if (Data != null)
                {
                    for (int i = 0; i < Data.Count; i++)
                    {
                        if (i != 0)
                            Parameters = Parameters + "\t\t\t\t\t";

                        Parameters += Data[i].ParameterName + "\t:\t" + Convert.ToString(Data[i].Value);

                        if (i < Data.Count - 1)
                            Parameters = Parameters + "\n";
                    }
                }
                SR.Write("User Name\t\t:\t" + UserName + "\n");
                SR.Write("Created Date\t:\t" + DateTime.Now.ToString() + "\n");
                SR.Write("Machine Name\t:\t" + HttpContext.Current.Request.UserHostName + "\n");
                SR.Write("Page URL\t\t:\t" + PageURL + "\n");
                SR.Write("SP Name\t\t\t:\t" + SPName + "\n");
                SR.Write("Query\t\t\t:\t" + Query + "\n");
                SR.Write("Method Name\t\t:\t" + MethodName + "\n");
                SR.Write("Data\t\t\t:\t" + Parameters + "\n");
                SR.Write("Status\t\t\t:\t" + (Status ? "Success" : "Failed") + "\n");
                SR.Write("IP Address\t\t:\t" + HttpContext.Current.Request.UserHostAddress + "\n");
            }
            catch (Exception ex)
            {
                try
                {
                    SR.Write(ex.StackTrace.ToString());
                }
                catch (Exception exx)
                {

                }

            }
            finally
            {
                if (SR != null)
                    SR.Close();
            }
        }


        //Author : Payal
        //Usage  : Get DataSet from executing stored procedure.
        //Created Date : 11-09-2014
        //Last Modified By : Rashmi
        //Last Modified Date : 29-10-2014 (Removed exception)
        public static DataSet GetDataSet(string strQuery, List<SqlParameter> para)
        {

            DataSet ds = new DataSet();
            bool IsSuccess = true;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                try
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    //con.Open();
                    using (SqlCommand cmd = new SqlCommand(strQuery, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 10000;
                        cmd.Connection.Open();
                        for (int iCount = 0; iCount < para.Count; iCount++)
                        {
                            cmd.Parameters.Add(para[iCount]);
                        }
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    throw ex;
                }
                finally
                {
                    InsertIntoAuditTrail(strQuery, "GetDataSet", IsSuccess, para, "");
                }
            }
            return ds;
        }

        //Author : Payal
        //Usage  : Get Data table from executing stored procedure.
        //Created Date : 11-09-2014
        //Last Modified By : Rashmi
        //Last Modified Date : 29-10-2014 (Removed exception)
        public static DataTable FillTable(string strQuery, List<SqlParameter> para)
        {
            DataTable dt = new DataTable();
            bool IsSuccess = true;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                try
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    //con.Open();
                    using (SqlCommand cmd = new SqlCommand(strQuery, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 10000;
                        cmd.Connection.Open();
                        for (int iCount = 0; iCount < para.Count; iCount++)
                        {
                            cmd.Parameters.Add(para[iCount]);
                        }
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    throw ex;
                }
                finally
                {
                    InsertIntoAuditTrail(strQuery, "FillTable", IsSuccess, para, "");
                }
            }
            return dt;
        }
        //Author : Payal
        //Usage  : Get single value from executing stored procedure with sql parameters.
        //Created Date : 11-09-2014
        //Last Modified By : Rashmi
        //Last Modified Date : 29-10-2014 (Removed exception)
        public static int ExecuteNonQuery(string strQuery, List<SqlParameter> para)
        {
            bool IsSuccess = true;
            int iResult = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (cmd.Connection.State == ConnectionState.Open)
                        {
                            cmd.Connection.Close();
                        }
                        cmd.CommandTimeout = 10000;
                        cmd.Connection.Open();
                        for (int iCount = 0; iCount < para.Count; iCount++)
                        {
                            cmd.Parameters.Add(para[iCount]);
                        }
                        iResult = cmd.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {
                        IsSuccess = false;
                        throw ex;
                    }
                    finally
                    {
                        if (cmd.Connection.State != ConnectionState.Closed)
                        {
                            cmd.Connection.Close();
                            cmd.Dispose();
                        }
                        con.Close();

                        InsertIntoAuditTrail(strQuery, "ExecuteNonQuery", IsSuccess, para, "");
                    }
                }
            }
            return iResult;
        }



        //Author : Rashmi
        //Usage  : Return ID from Database
        public static int ExecuteNonQuery(string strQuery, List<SqlParameter> para, string ID)
        {
            bool IsSuccess = true;
            int iResult = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (cmd.Connection.State == ConnectionState.Open)
                        {
                            cmd.Connection.Close();
                        }
                        cmd.CommandTimeout = 10000;
                        cmd.Connection.Open();
                        for (int iCount = 0; iCount < para.Count; iCount++)
                        {
                            cmd.Parameters.Add(para[iCount]);
                        }
                        cmd.ExecuteNonQuery();
                        iResult = Convert.ToInt32(cmd.Parameters[ID].Value);

                    }
                    catch (Exception ex)
                    {
                        IsSuccess = false;
                        throw ex;
                    }
                    finally
                    {
                        if (cmd.Connection.State != ConnectionState.Closed)
                        {
                            cmd.Connection.Close();
                            cmd.Dispose();
                        }
                        con.Close();

                        para.Add(new SqlParameter("@ID", ID));
                        InsertIntoAuditTrail(strQuery, "ExecuteNonQuery", IsSuccess, para, "");
                    }
                }
            }
            return iResult;
        }

        //Author : Rashmi
        //Usage  : Return Out Parameter from Database
        public static string ExecuteNonQuery(string strQuery, List<SqlParameter> para, string Message, string OutPut)
        {
            bool IsSuccess = true;
            string message = "";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (cmd.Connection.State == ConnectionState.Open)
                        {
                            cmd.Connection.Close();
                        }
                        cmd.CommandTimeout = 10000;
                        cmd.Connection.Open();
                        for (int iCount = 0; iCount < para.Count; iCount++)
                        {
                            cmd.Parameters.Add(para[iCount]);
                        }
                        cmd.ExecuteNonQuery();
                        message = (cmd.Parameters[Message].Value).ToString();

                    }
                    catch (Exception ex)
                    {
                        IsSuccess = false;
                        throw ex;
                    }
                    finally
                    {
                        if (cmd.Connection.State != ConnectionState.Closed)
                        {
                            cmd.Connection.Close();
                            cmd.Dispose();
                        }
                        con.Close();

                        //para.Add(new SqlParameter("@ID", ID));
                        InsertIntoAuditTrail(strQuery, "ExecuteNonQuery", IsSuccess, para, "");
                    }
                }
            }
            return message;
        }


        //Author : Payal
        //Usage  : Get single value from executing stored procedure without sql parameters.
        //Created Date : 11-09-2014
        //Last Modified By : Rashmi
        //Last Modified Date : 29-10-2014 (Removed exception)
        public static int ExecuteNonQuery(string strQuery)
        {
            bool IsSuccess = true;
            int iResult = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = Commandtimeout;
                        if (cmd.Connection.State == ConnectionState.Open)
                        {
                            cmd.Connection.Close();
                        }
                        cmd.CommandTimeout = 10000;
                        cmd.Connection.Open();
                        iResult = cmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        IsSuccess = false;
                        throw ex;
                    }
                    finally
                    {
                        if (cmd.Connection.State != ConnectionState.Closed)
                        {
                            cmd.Connection.Close();
                            cmd.Dispose();
                        }
                        con.Close();

                        List<SqlParameter> para = new List<SqlParameter>();
                        InsertIntoAuditTrail("", "ExecuteNonQuery", IsSuccess, para, strQuery);
                    }
                }
            }
            return iResult;
        }
        //Author : Payal
        //Usage  : Get single value from executing sql string.
        //Created Date : 11-09-2014
        //Last Modified By :
        //Last Modified Date : 
        public static int ExecuteNonQueryWithoutProcedure(string strQuery)
        {
            bool IsSuccess = true;
            int iResult = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = Commandtimeout;
                        if (cmd.Connection.State == ConnectionState.Open)
                        {
                            cmd.Connection.Close();
                        }
                        cmd.CommandTimeout = 10000;
                        cmd.Connection.Open();
                        iResult = cmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        IsSuccess = false;
                        throw ex;
                    }
                    finally
                    {
                        if (cmd.Connection.State != ConnectionState.Closed)
                        {
                            cmd.Connection.Close();
                            cmd.Dispose();
                        }
                        con.Close();

                        List<SqlParameter> para = new List<SqlParameter>();
                        InsertIntoAuditTrail("", "ExecuteNonQueryWithoutProcedure", IsSuccess, para, strQuery);
                    }
                }
            }
            return iResult;
        }
        //Author : Payal
        //Usage  : Get single object using ExecuteScalar.
        //Created Date : 11-09-2014
        //Last Modified By : Rashmi
        //Last Modified Date : 29-10-2014 (Removed exception)
        public static object ExecuteScalar(string strQuery, List<SqlParameter> para)
        {
            bool IsSuccess = true;
            object oResult = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    try
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = Commandtimeout;
                        if (cmd.Connection.State == ConnectionState.Open)
                        {
                            cmd.Connection.Close();
                        }
                        cmd.Connection.Open();

                        for (int iCount = 0; iCount < para.Count; iCount++)
                        {
                            cmd.Parameters.Add(para[iCount]);
                        }

                        oResult = cmd.ExecuteScalar();

                    }
                    catch (Exception ex)
                    {
                        IsSuccess = false;
                        throw ex;
                    }
                    finally
                    {
                        if (cmd.Connection.State != ConnectionState.Closed)
                        {
                            cmd.Connection.Close();
                            cmd.Dispose();
                        }
                        con.Close();

                        InsertIntoAuditTrail(strQuery, "ExecuteScalar", IsSuccess, para, "");
                    }
                }
            }
            return oResult;
        }

        public static object ExecuteScalarText(string strQuery, List<SqlParameter> para)
        {
            bool IsSuccess = true;
            object oResult = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    try
                    {
                        con.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = Commandtimeout;
                        if (cmd.Connection.State == ConnectionState.Open)
                        {
                            cmd.Connection.Close();
                        }
                        cmd.Connection.Open();

                        for (int iCount = 0; iCount < para.Count; iCount++)
                        {
                            cmd.Parameters.Add(para[iCount]);
                        }

                        oResult = cmd.ExecuteScalar();

                    }
                    catch (Exception ex)
                    {
                        IsSuccess = false;
                        throw ex;
                    }
                    finally
                    {
                        if (cmd.Connection.State != ConnectionState.Closed)
                        {
                            cmd.Connection.Close();
                            cmd.Dispose();
                        }
                        con.Close();

                        InsertIntoAuditTrail(strQuery, "ExecuteScalar", IsSuccess, para, "");
                    }
                }
            }
            return oResult;
        }
        //Author : Payal
        //Usage  : Get single DataRow using Stored Procedure with sql parameters.
        //Created Date : 11-09-2014
        //Last Modified By :
        //Last Modified Date : 
        public static DataRow GetDataRow(string strQuery, List<SqlParameter> para)
        {
            bool IsSuccess = true;
            DataTable dtTemp = FillTable(strQuery, para);
            try
            {
                if (dtTemp.Rows.Count > 0)
                {
                    return dtTemp.Rows[0];
                }
                else
                {
                    DataRow drTemp = dtTemp.NewRow();
                    return drTemp;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                throw ex;
            }
            finally
            {
                InsertIntoAuditTrail(strQuery, "GetDataRow", IsSuccess, para, "");
            }
        }
        //Author : Payal
        //Usage  : Get single DataRow using Stored Procedure with sql parameters & row index.
        //Created Date : 11-09-2014
        //Last Modified By :
        //Last Modified Date : 
        public static DataRow GetDataRow(string strQuery, List<SqlParameter> para, int RowIndex)
        {
            bool IsSuccess = true;
            try
            {
                if (RowIndex >= 0)
                {
                    DataTable dt = FillTable(strQuery, para);
                    if (RowIndex <= dt.Rows.Count - 1)
                        return dt.Rows[RowIndex];
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                throw ex;
            }
            finally
            {
                para.Add(new SqlParameter("@RowIndex", RowIndex));
                InsertIntoAuditTrail(strQuery, "GetDataRow", IsSuccess, para, "");
            }
            return null;
        }
        #endregion

        //Author : Payal
        //Usage  : Get single object using ExecuteScalar.
        //Created Date : 15-06-2015    
        public static object ExecuteScalarWithOutCommandType(string strQuery, List<SqlParameter> para)
        {
            bool IsSuccess = true;
            object oResult = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    try
                    {
                        con.Open();
                        cmd.CommandTimeout = Commandtimeout;
                        if (cmd.Connection.State == ConnectionState.Open)
                        {
                            cmd.Connection.Close();
                        }
                        cmd.CommandTimeout = 10000;
                        cmd.Connection.Open();

                        for (int iCount = 0; iCount < para.Count; iCount++)
                        {
                            cmd.Parameters.Add(para[iCount]);
                        }

                        oResult = cmd.ExecuteScalar();

                    }
                    catch (Exception ex)
                    {
                        IsSuccess = false;
                        throw ex;
                    }
                    finally
                    {
                        if (cmd.Connection.State != ConnectionState.Closed)
                        {
                            cmd.Connection.Close();
                            cmd.Dispose();
                        }
                        con.Close();

                        InsertIntoAuditTrail(strQuery, "ExecuteScalar", IsSuccess, para, "");
                    }
                }
            }
            return oResult;
        }
        public static int ExecuteNonQueryWithoutProcedure(string strQuery, SqlConnection cn, SqlTransaction objTrans)
        {
            bool IsSuccess = true;
            int iResult = 0;

            using (SqlCommand cmd = new SqlCommand(strQuery, cn))
            {
                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = Commandtimeout;
                    cmd.Transaction = objTrans;
                    cmd.CommandTimeout = 10000;
                    //cmd.Connection.Open();
                    iResult = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    throw ex;
                }
                finally
                {
                    List<SqlParameter> para = new List<SqlParameter>();
                    InsertIntoAuditTrail("", "ExecuteNonQueryWithoutProcedure", IsSuccess, para, strQuery);
                }
            }

            return iResult;
        }
        //
        public static object ExecuteScalarWithOutCommandType(string strQuery, List<SqlParameter> para, SqlConnection cn, SqlTransaction objTrans)
        {
            bool IsSuccess = true;
            object oResult = null;
            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EliteERP"].ConnectionString))
            //{
            using (SqlCommand cmd = new SqlCommand(strQuery, cn))
            {
                try
                {
                    //con.Open();
                    cmd.Transaction = objTrans;
                    cmd.CommandTimeout = Commandtimeout;
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        cmd.Connection.Close();
                    }
                    cmd.CommandTimeout = 10000;
                    //cmd.Connection.Open();

                    for (int iCount = 0; iCount < para.Count; iCount++)
                    {
                        cmd.Parameters.Add(para[iCount]);
                    }

                    oResult = cmd.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    throw ex;
                }
                finally
                {
                    if (cmd.Connection.State != ConnectionState.Closed)
                    {
                        cmd.Connection.Close();
                        cmd.Dispose();
                    }
                    //con.Close();

                    InsertIntoAuditTrail(strQuery, "ExecuteScalar", IsSuccess, para, "");
                }
            }
            //}
            return oResult;
        }
        #region  "Sql Transction"
        //Author : Rashmi
        //Usage  : Return ID from Database with also use Sql Transction and Sql Connection
        public static int ExecuteNonQuery(string strQuery, List<SqlParameter> para, string ID, SqlConnection con, SqlTransaction objTrans)
        {
            bool IsSuccess = true;
            int iResult = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = objTrans;
                    cmd.CommandTimeout = 10000;
                    for (int iCount = 0; iCount < para.Count; iCount++)
                    {
                        cmd.Parameters.Add(para[iCount]);
                    }
                    cmd.ExecuteNonQuery();
                    iResult = Convert.ToInt32(cmd.Parameters[ID].Value);
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                throw ex;
            }
            finally
            {
                para.Add(new SqlParameter("@ID", ID));
                InsertIntoAuditTrail(strQuery, "ExecuteNonQuery", IsSuccess, para, "");
            }
            return iResult;
        }

        // Author Rashmi
        // used for multiple transaction in one Page
        public static int ExecuteNonQuery(string strQuery, List<SqlParameter> para, SqlConnection con, SqlTransaction objTrans)
        {
            bool IsSuccess = true;
            int iResult = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = objTrans;
                    cmd.CommandTimeout = 10000;
                    for (int iCount = 0; iCount < para.Count; iCount++)
                    {
                        cmd.Parameters.Add(para[iCount]);
                    }
                    iResult = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                throw ex;
            }
            finally
            {
                InsertIntoAuditTrail(strQuery, "ExecuteNonQuery", IsSuccess, para, "");
            }
            return iResult;
        }

        public static DataTable FillTable(string strQuery, List<SqlParameter> para, SqlConnection cn, SqlTransaction objTrans)
        {
            bool IsSuccess = true;
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand(strQuery, cn))
                {
                    cmd.Transaction = objTrans;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 10000;
                    for (int iCount = 0; iCount < para.Count; iCount++)
                    {
                        cmd.Parameters.Add(para[iCount]);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                throw ex;
            }
            finally
            {
                InsertIntoAuditTrail(strQuery, "FillTable", IsSuccess, para, "");
            }
            return dt;
        }
        #endregion
    }
}

