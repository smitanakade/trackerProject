using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using POTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using POTracker.CustomFilters;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace POTracker.Reports
{
    public partial class commentsViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                testText.Text = Request.QueryString["q"];
                var result = GetUserRoles();
                var roleUserGroupsDR = result.AsEnumerable().Where(r=>r.Field<string>("RoleName")!= "PurchasingAdmin").GroupBy(r => r.Field<String>("RoleName")).ToList();
                var roleUserGroupList = new List<RoleUserGroup>();
                roleUserGroupsDR.ForEach(r =>
                {
                    var roleUserGroup = new RoleUserGroup
                    {
                        RoleUser = r.Key,
                        Users = r.Select(u => new RoleUser { Username = u.Field<string>("UserName") }).ToList()
                    };
                    roleUserGroupList.Add(roleUserGroup);
                }
                );
                Repeater1.DataSource = roleUserGroupList;
                Repeater1.DataBind();
            }


        }
        public DataTable GetUserRoles()
        {

            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            var sql = "SELECT u.Id ,u.Email,u.UserName,r.Name as RoleName FROM AspNetUsers as u Inner join AspNetUserRoles as ur on ur.UserId = u.Id Inner join AspNetRoles as r on r.Id= ur.RoleId";
            try
            {
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
                    string sql = "SELECT  DISTINCT CommentsId as id,Convert(varchar(50), OrderNumber) as OrderNumber,UserName,Comment,CommentDate, FROM AUOrderTrackerComments WHERE ordernumber = " + po_order_no + " ORDER BY date ASC ";
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

        private static string GetEmailAddress(string username)
        {
            DataTable table = new DataTable();
            SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
            if (SqlConn != null)
            {
                try
                {
                    SqlConn.Open();
                    string sql = "SELECT DISTINCT Email FROM aspnet_Membership, aspnet_Users WHERE aspnet_Membership.UserId = aspnet_Users.UserId AND aspnet_Users.UserName = '" + username + "'";
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
                values += table.Rows[0][i].ToString() + ",";
            }
            values = values.TrimEnd(',');
            return values;
        }
        public static bool sendEmailMaster(string ordernumber, string username, string EmailNames)
        {
            DataTable commentsTable = GetComments(ordernumber);
            string SendFrom = "ordertracker@arlec.com.au";
            string[] words = EmailNames.Split(',');
            string SendTo = "";
            foreach (string word in words)
            {
                SendTo += GetEmailAddress(word) + ",";
            }
            SendTo = SendTo.TrimEnd(',') + "," + GetEmailAddress(username);
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            string username = User.Identity.Name.ToString();
            
            if (testText.Text != "" && username != "" && TextBox2.Text != "")
            {
                DateTime now = DateTime.Now;
                bool isInsertNote;
                SqlConnection SqlConn = ProntoConnectionAgent.getConnectionObject();
                if (SqlConn != null)
                {
                    try
                    {
                        SqlConn.Open();

                        string sql = "INSERT INTO AUOrderTrackerComments (OrderNumber, UserName, Comment, CommentDate) VALUES(@ordernumber, @username, @note, @date)";
                        SqlCommand command = new SqlCommand(sql, SqlConn);
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@ordernumber", testText.Text);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@note", TextBox2.Text);
                        command.Parameters.AddWithValue("@date", now);
                        command.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        isInsertNote = false;
                    }
                    finally
                    {
                        isInsertNote = true;
                        ProntoConnectionAgent.Close(SqlConn);
                    }


                    if (isInsertNote)
                    {
                        List<string> selectedUser = new List<string>();
                        for (var j = 0; j < Repeater1.Items.Count; j++)
                        {
                            CheckBoxList chklst = (CheckBoxList)Repeater1.Items[j].FindControl("CheckBoxListUsers");
                            for (int i = 0; i < chklst.Items.Count; i++)
                            {
                                if (chklst.Items[i].Selected)
                                {
                                    selectedUser.Add(chklst.Items[i].Text);
                                }

                               
                            }

                        }

                        string ordernumber;
                        string Name;
                        string sendTo;

                        bool isSendEmail = false;
                        if (selectedUser.Count !=0)
                        {
                             isSendEmail = ProntoDatabase.sendEmailMaster(testText.Text, username);
                            isSendEmail = true;
                        }
                        else
                        {

                            isSendEmail = true;
                        }

                        if (isSendEmail == true)
                        {
                            //refresh parent page and close current window.
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "keyname", "window.opener.location.reload(true);self.close()", true);
                            //this.Page.ClientScript.RegisterStartupScript(this.GetType(), "keyname", "window.close();", true);
                        }

                    }
                    else
                    {
                        error.Text = "The message has not been successfully sent! Please check your message...";
                    }
                }
                else
                {
                    error.Text = "The message has not been successfully sent! Please check your message...";
                }
            }
        }
    }
}