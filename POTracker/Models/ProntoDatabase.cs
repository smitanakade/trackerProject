using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Security;

namespace POTracker.Models
{
    public class ProntoConnectionAgent
    {
        public static SqlConnection getConnectionObject()
        {
            SqlConnection conn = null;
            string dbConnString = ConfigurationManager.ConnectionStrings["PRONTOETLConnectionString"].ConnectionString;
            try
            {
                conn = new SqlConnection(dbConnString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return conn;
        }
        public static void Close(SqlConnection SqlConn)
        {
            SqlConn.Close();
            SqlConn.Dispose();
        }
    }

    public static class ProntoDatabase
    {

        public static DataTable GetOrderswithLines(string filterValue, string sortValue)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "";
                    var Type = "";
                   
                        //                        sql = @"SELECT lf.po_order_no,o.cre_accountcode, o.po_order_date, o.po_notes, o.po_revision_no,lf.stock_code, lf.po_order_qty, lf.pol_user_only_date1, 
                        //lf.po_l_seq, lf.po_backorder_flag,os.type_stock AS SupplierStatus,o.cre_accountcode ,o.po_whse_code
                        //FROM purchase_order_line_flag as lf  
                        //INNER JOIN purchase_order  as o on o.po_order_no = lf.po_order_no   
                        //INNER JOIN purchase_order_flag  as pf on pf.po_order_no = o.po_order_no   AND lf.po_backorder_flag = o.po_backorder_flag
                        //INNER JOIN cre_master m on o.cre_accountcode = m.cre_accountcode
                        //LEFT JOIN Order_Status as os on os.po_order_no=lf.po_order_no and os.stock_code=lf.stock_code and os.cre_accountcode= o.cre_accountcode";
                        sql = @"SELECT lf.po_order_no,o.cre_accountcode, o.po_order_date, o.po_notes, o.po_revision_no,lf.stock_code, lf.po_order_qty, lf.pol_user_only_date1, 
lf.po_l_seq, lf.po_backorder_flag,os.type_stock AS SupplierStatus,o.cre_accountcode ,o.po_whse_code,s.otherIssues,s.specialOrders
FROM purchase_order_line_flag as lf  
INNER JOIN purchase_order  as o on o.po_order_no = lf.po_order_no   
INNER JOIN purchase_order_flag  as pf on pf.po_order_no = o.po_order_no   AND lf.po_backorder_flag = o.po_backorder_flag
INNER JOIN cre_master m on o.cre_accountcode = m.cre_accountcode
INNER JOIN status s on s.po_order_no= lf.po_order_no
LEFT JOIN Order_Status as os on os.po_order_no=lf.po_order_no and os.stock_code=lf.stock_code and os.cre_accountcode= o.cre_accountcode";
                        if (Type != "")
                        {
                            if (Type == "A")
                            {
                                //  sql = sql + "  where fl.cre_accountcode NOT IN (select DISTINCT cre_accountcode from SupplerType where Type='B') ";
                            }
                            if (Type == "B")
                            {
                                sql = sql + "  where pf.cre_accountcode  IN (select DISTINCT cre_accountcode from SupplerType where Type='B') ";

                            }

                        }
                    

                    

                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;

        }
        public static DataTable GetOrders(string filterValue, string sortValue)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "";
                    var Type = "";
                    /* Smita has changed the following code for showing particular suppler baised on roles*/

                    //if (Roles.IsUserInRole("ShenzhenOffice") || Roles.IsUserInRole("NingboOffice"))
                    //{
                    //    Type = "A";

                    //}
                    //if (Roles.IsUserInRole("Tosun"))
                    //{
                    //    Type = "B";
                    //}
                    /*juny has been changed the below to add warehouse code in the main grid*/
                    if (filterValue == null)
                    {
                        //   sql = @"SELECT DISTINCT po_order_no, cre_accountcode, po_order_date, po_notes, po_revision_no , po_whse_code = (select po_whse_code FROM purchase_order  where po_order_no = fl.po_order_no) 
                        //             FROM purchase_order_flag fl";
                        sql = @"SELECT DISTINCT po_order_no,  fl.cre_accountcode, po_order_date, po_notes, po_revision_no
                                , po_whse_code = (select DISTINCT po_whse_code FROM purchase_order  where po_order_no = fl.po_order_no) 
                                FROM purchase_order_flag fl INNER JOIN cre_master m on fl.cre_accountcode = m.cre_accountcode ";
                        /* Smita has changed the following code for showing particular suppler baised on roles*/

                        if (Type != "")
                        {
                            if (Type == "A")
                            {
                                //  sql = sql + "  where fl.cre_accountcode NOT IN (select DISTINCT cre_accountcode from SupplerType where Type='B') ";
                            }
                            if (Type == "B")
                            {
                                sql = sql + "  where fl.cre_accountcode  IN (select DISTINCT cre_accountcode from SupplerType where Type='B') ";

                            }
                            // sql = sql + "  where fl.cre_accountcode  NOT IN (select DISTINCT cre_accountcode from SupplerType where Type='" + Type + "') ";

                        }
                    }
                    else
                    {
                        //  sql = @"SELECT DISTINCT purchase_order_flag.po_order_no, purchase_order_flag.cre_accountcode, purchase_order_flag.po_order_date, purchase_order_flag.po_notes,purchase_order_flag.po_revision_no 
                        //  , po_whse_code = (select po_whse_code FROM purchase_order  where po_order_no = purchase_order_flag.po_order_no) 
                        //      FROM purchase_order_flag, status WHERE status." + filterValue + " = 1 AND purchase_order_flag.po_order_no = status.po_order_no ";

                        sql = @"SELECT DISTINCT fl.po_order_no, fl.cre_accountcode, fl.po_order_date, fl.po_notes,fl.po_revision_no 
                    , po_whse_code = (select po_whse_code FROM purchase_order  where po_order_no = fl.po_order_no) 
                        FROM purchase_order_flag fl INNER JOIN status s on s.po_order_no=fl.po_order_no INNER JOIN cre_master on  fl.cre_accountcode=cre_master.cre_accountcode WHERE
                        s." + filterValue + "= 1 AND fl.po_order_no = s.po_order_no ";
                        if (Type != "")
                        {
                            if (Type == "A")
                            {
                                // sql = sql + "  and fl.cre_accountcode IN (select DISTINCT cre_accountcode from SupplerType where Type='" + Type + "') ";
                            }
                            if (Type == "B")
                            {
                                //sql = sql + "  and fl.cre_accountcode NOT IN (select DISTINCT cre_accountcode from SupplerType where Type='A') ";
                                sql = sql + "  and fl.cre_accountcode IN (select DISTINCT cre_accountcode from SupplerType where Type='B') ";
                            }

                        }
                    }

                    if (sortValue != null)
                    {
                        if (sortValue.Equals("po_notes"))
                            sql = sql + " ORDER BY " + sortValue + " DESC";
                        else
                            sql = sql + " ORDER BY " + sortValue + " ASC";
                    }

                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;


        }
        public static String GetPOByCommentsID(int CommandID)
        {
            var poNumber = "";
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    if (CommandID > 0)
                    {
                        SqlConn.Open();
                        var sql = "SELECT ordernumber from ordertrackernotes where Id = @CommandID";
                        SqlCommand command = new SqlCommand(sql, SqlConn);
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@CommandID", CommandID);
                        poNumber = command.ExecuteScalar().ToString();

                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    return poNumber;
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return poNumber;
        }
        public static Boolean DeleteNoteByID(int CommandID)
        {
            var result = false;
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    if (CommandID > 0)
                    {

                        SqlConn.Open();
                        string sql2 = "delete from ordertrackernotes where Id = @CommandID";
                        SqlCommand command2 = new SqlCommand(sql2, SqlConn);
                        command2.Parameters.Clear();
                        command2.Parameters.AddWithValue("@CommandID", CommandID);
                        command2.ExecuteNonQuery();
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return result;
        }
        public static DataTable GetStockStatus()
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "";
                    var Type = "";
                    /* Smita has changed the following code for showing particular suppler baised on roles*/

                    //if (Roles.IsUserInRole("ShenzhenOffice") || Roles.IsUserInRole("NingboOffice"))
                    //{
                    //    Type = "A";

                    //}
                    //if (Roles.IsUserInRole("Tosun"))
                    //{
                    //    Type = "B";
                    //}
                    sql = "Select cre_accountcode as Supplier,po_order_no,stock_code, Case when price_issues_YES = 1 and price_issues_NO= 0 then 'Done'when price_issues_YES=0 and price_issues_NO=1 then  'Not Done' When price_issues_NO=0 and price_issues_YES=0 then '' end As 'Price issues solved', Case when product_testing_YES = 1 and product_testing_NO= 0 then 'Done' when product_testing_YES=0 and product_testing_NO=1 then  'Not Done' When product_testing_NO=0 and product_testing_YES=0 then '' end As 'Product testing completed & approved'  ,Case when test_report_YES = 1 and test_report_NO= 0 then 'Done' when test_report_YES=0 and product_testing_NO=1 then 'Not Done' When test_report_NO=0 and test_report_YES=0 then '' end As 'Test report & approval cert. finished',Case when specification_YES = 1 and specification_NO= 0 then 'Done' when specification_YES=0 and specification_NO=1 then 'Not Done' When specification_NO=0 and specification_YES=0 then '' end As 'Specification released',Case when artwork_YES = 1 and artwork_NO= 0 then 'Done' when artwork_YES=0 and artwork_NO=1 then 'Not Done' When artwork_NO=0 and artwork_YES=0 then '' end As 'Artwork released',Case when labels_YES = 1 and labels_NO= 0 then 'Done' when labels_NO=0 and artwork_NO=1 then 'Not Done' When labels_NO=0 and labels_YES=0 then '' end As 'Labels released',Comment,Country from Order_Status where Country='AU' ";
                    if (Type != "")
                    {
                        if (Type == "A")
                        {
                            //  sql = sql + "  Where  type_stock='N' and cre_accountcode NOT IN (select DISTINCT cre_accountcode from SupplerType where Type='B') ORDER BY po_order_no ";
                        }
                        if (Type == "B")
                        {
                            sql = sql + "  And  type_stock='N' and cre_accountcode  IN (select DISTINCT cre_accountcode from SupplerType where Type='B') ORDER BY po_order_no";
                        }

                    }
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }

            }

            return table;

        }
        public static DataTable GetOrdersLine()
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    
                    string sql = "SELECT DISTINCT lf.po_order_no, lf.stock_code, lf.po_order_qty, lf.pol_user_only_date1, lf.po_l_seq, lf.po_backorder_flag,os.type_stock AS SupplierStatus,o.cre_accountcode FROM purchase_order_line_flag as lf  INNER JOIN purchase_order  as o on o.po_order_no = lf.po_order_no  left JOIN Order_Status as os on os.po_order_no=lf.po_order_no and os.stock_code=lf.stock_code and os.cre_accountcode= o.cre_accountcode  where  lf.po_backorder_flag = o.po_backorder_flag order by lf.po_order_no ";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;
        }
        public static DataTable GetOrderLineFromPO(string po)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                   
                    string sql = "SELECT DISTINCT lf.po_order_no, lf.stock_code, lf.po_order_qty, lf.pol_user_only_date1, lf.po_l_seq, lf.po_backorder_flag,os.type_stock AS SupplierStatus,o.cre_accountcode FROM purchase_order_line_flag as lf  INNER JOIN purchase_order  as o on o.po_order_no = lf.po_order_no  left JOIN Order_Status as os on os.po_order_no=lf.po_order_no and os.stock_code=lf.stock_code and os.cre_accountcode= o.cre_accountcode  where  lf.po_backorder_flag = o.po_backorder_flag and lf.po_order_no="+po+" order by lf.po_order_no ";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;
        }

        public static DataTable GetPdfUrlName(string orderNumber)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "SELECT DISTINCT po_backorder_flag, po_revision_no, po_whse_code FROM purchase_order WHERE po_order_no = " + orderNumber;
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;
        }


        public static DataTable GetPdfUrlNameNZ(string orderNumber)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "SELECT DISTINCT po_backorder_flag, po_revision_no FROM UK_purchase_order WHERE po_order_no = " + orderNumber;
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;
        }

        public static DataTable GetRoles()
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            try
            {
                SqlConn.Open();
                string sql = "SELECT Name From AspNetRoles";
                SqlCommand command = new SqlCommand(sql, SqlConn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                command.CommandType = CommandType.Text;
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                ProntoConnectionAgent.Close(SqlConn);
            }
            return table;
        }
        public static DataTable ShowRegisterUser()
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            try
            {
                SqlConn.Open();
                string sql = "SELECT Name From AspNetRoles";
                SqlCommand command = new SqlCommand(sql, SqlConn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                command.CommandType = CommandType.Text;
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                ProntoConnectionAgent.Close(SqlConn);
            }
            return table;

        }
        public static DataTable GetComments(string po_order_no)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    int ordernumber = Convert.ToInt32(po_order_no);
                    string sql = "SELECT  DISTINCT UserName, Comment, CommentDate,Convert(varchar(50),OrderNumber) as ordernumber ,CommentsId FROM AUOrderTrackerComments WHERE ordernumber = " + po_order_no + " ORDER BY CommentDate ASC ";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;

        }
        public static DataTable FetchComments()
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "SELECT DISTINCT username, note, date,Convert(varchar(50),ordernumber) as ordernumber ,id FROM ordertrackernotes ORDER BY date ASC ";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;
        }

        public static DataTable GetProductStatus(string supplier, string stockcode, int orderno)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    //int ordernumber = Convert.ToInt32(po_order_no);
                    string sql = "SELECT Id,cre_accountcode,po_order_no,stock_code,price_issues_Yes,price_issues_NO,product_testing_YES,product_testing_NO,test_report_YES,test_report_NO,specification_YES,specification_NO,artwork_YES,artwork_NO,labels_YES,labels_NO,Comment FROM Order_Status where stock_code='" + stockcode + "' and cre_accountcode ='" + supplier + "'";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;

        }
        public static DataTable GetStatus()
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "SELECT po_order_no, otherIssues, specialOrders FROM status ";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;
        }


        public static DataTable GetStatusNZ()
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "SELECT po_order_no, otherIssues, specialOrders FROM UK_status ";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            return table;
        }


        public static void changeOtherIssues(string q, string c)
        {
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "UPDATE status SET otherIssues = @c WHERE po_order_no = '" + q + "'";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    command.Parameters.AddWithValue("@c", Convert.ToInt32(c));
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
        }

        public static void changeOtherIssuesNZ(string q, string c)
        {
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "UPDATE UK_status SET otherIssues = @c WHERE po_order_no = '" + q + "'";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    command.Parameters.AddWithValue("@c", Convert.ToInt32(c));
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
        }

        public static void changeSpecialOrders(string q, string c)
        {
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "UPDATE status SET specialOrders = @c WHERE po_order_no = '" + q + "'";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    command.Parameters.AddWithValue("@c", Convert.ToInt32(c));
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
        }

        public static void changeSpecialOrdersNZ(string q, string c)
        {
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "UPDATE UK_status SET specialOrders = @c WHERE po_order_no = '" + q + "'";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    command.Parameters.AddWithValue("@c", Convert.ToInt32(c));
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
        }


        public static bool insertNote(string ordernumber, string username, string note, DateTime date)
        {
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                   
        string sql = "INSERT INTO AUOrderTrackerComments (OrderNumber, UserName, Comment, CommentDate) VALUES(@ordernumber, @username, @note, @date)";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ordernumber", ordernumber);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@note", note);
                    command.Parameters.AddWithValue("@date", date);
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    return false;
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
                return true;
            }
            return false;
        }
        public static string[] GetPurchaseOrderPDFs()
        {
            var purchaseOrderPDFFiles = Directory.GetFiles(@"C:\Inetpub\wwwroot\orderTracker\PURCHASEORDERS\", "*.pdf", SearchOption.AllDirectories);
            return purchaseOrderPDFFiles;


        }
        public static bool checkPDF(string po_order_no, string po_revision_no)
        {
            string PDFname = "Arlec-PURCHASE ORDER-" + po_order_no + "-*" + "*.pdf";
            string[] files1 = Directory.GetFiles(@"C:\Inetpub\wwwroot\orderTracker\PURCHASEORDERS\", PDFname, SearchOption.AllDirectories);
            if (files1.Count() == 0)
                return false;
            else
                return true;
        }


        public static string[] PDFlist(string PDFname)
        {
            string[] files1 = Directory.GetFiles(@"C:\Inetpub\wwwroot\orderTracker\PURCHASEORDERS\", PDFname, SearchOption.AllDirectories);
            return files1;
        }


        private static string GetProductCode(string POnumber)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "SELECT DISTINCT stock_code FROM purchase_order_line_flag WHERE po_order_no = " + POnumber;
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            String values = "";
            for (int i = 0; i < table.Rows.Count; i++)
            {
                values += table.Rows[i][0].ToString() + ",";
            }


            ///Add New Zealand Product Codes
            ///
            DataTable tableNZ = new DataTable();
            SqlConnection SqlConn2 = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn2 != null)
            {
                try
                {
                    SqlConn2.Open();
                    string sql2 = "SELECT DISTINCT stock_code FROM UK_purchase_order_line_flag WHERE po_order_no = " + POnumber;
                    SqlCommand command2 = new SqlCommand(sql2, SqlConn2);
                    SqlDataAdapter adapter2 = new SqlDataAdapter(command2);
                    command2.CommandType = CommandType.Text;
                    adapter2.Fill(tableNZ);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn2);
                }
            }

            for (int i = 0; i < tableNZ.Rows.Count; i++)
            {
                values += tableNZ.Rows[i][0].ToString() + ",";
            }

            values = values.TrimEnd(',');
            return values;
        }


        public static bool sendEmailMaster(string ordernumber, string username)
        {
            DataTable commentsTable = GetComments(ordernumber);
            string SendTo = GetEmailAddress(username);
            string SendFrom = "ordertracker@arlec.com.au";
           // string[] words = EmailNames.Split(',');
            //string SendTo = "";
            //foreach (string word in words)
            //{
            //    SendTo += GetEmailAddress(word) + ",";
            //}
           // SendTo = SendTo + "," + GetEmailAddress(username);
            string sendSubject = "Arlec Order Tracking - PO#" + ordernumber + " - From " + username;
            string body = "";

            body += "<h1 align='center'>Amended Order</h1><p></p>";
            body += "<h2>Order Number: " + ordernumber + "</h2>";
            body += "<h2>Name: " + username + "</h2>";
            body += "<h2>Product Codes: " + GetProductCode(ordernumber) + "</h2>";
            body += "<h2>Comments:</h2>";

            body += "<table style='border: thin dashed #808080; width: 100%; font-size: medium; font-weight: normal; font-style: normal; color: #800000;'>";
            for (int i = 0; i < commentsTable.Rows.Count; i++)
            {
                body += "<tr>";
                for (int j = 0; j < commentsTable.Columns.Count; j++)
                {
                    if (j == 0)
                    {
                        body += "<td style='font-size: large;'>" + commentsTable.Rows[i][j].ToString() + "</td>";
                    }
                    else
                    {
                        body += "<td>" + commentsTable.Rows[i][j].ToString() + "</td>";
                    }
                }
                body += "</tr>";
            }
            body += "</table>";
            body += "<p></p><p></p><p></p><h2>(This email is an automatic email which sent from Arlec Australia. Do not reply.) </h2>";
            bool isSend = sendEmail(SendFrom, SendTo, sendSubject, body);
            return isSend;

        }

        private static string GetEmailAddress(string username)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    // string sql = "SELECT DISTINCT Email FROM aspnet_Membership, aspnet_Users WHERE aspnet_Membership.UserId = aspnet_Users.UserId AND aspnet_Users.UserName = '" + username + "'";

                    string sql = "SELECT Email From AspNetUsers where UserName = '" + username + "'";
                    SqlCommand command = new SqlCommand(sql, SqlConn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    command.CommandType = CommandType.Text;
                    adapter.Fill(table);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    ProntoConnectionAgent.Close(SqlConn);
                }
            }
            String values = "";
            //for (int i = 0; i < table.Rows.Count; i++)
            //{
            //    values += table.Rows[0][i].ToString() + ",";
            //}

            values = table.Rows[0][0].ToString();
            return values;
        }

        public static bool sendEmail(string sendFrom, string sendTo, string sendSubject, string body)
        {
            bool sent = false;
            try
            {
                MailMessage message = new MailMessage(sendFrom, sendTo, sendSubject, body);
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("aaws-nav01");
                client.UseDefaultCredentials = true;
                client.Send(message);
                sent = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return sent;
        }
    }
}
