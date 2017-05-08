using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using SMSservice.Helper;

namespace SMSservice
{
    /// <summary>
    /// Summary description for SMS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class SMS : System.Web.Services.WebService
    {
        [WebMethod]
        public string SendTo(string user, string pass,string ToAddress, string Body)
        {

        
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SMSDb"].ConnectionString);

            //controll auth
            var en = Encryptor.MD5Hash(pass);
            var userId =  ValidateUser(user, en);
            //me store procedure

            if (userId > 0)
            {
                using (SqlConnection con = new SqlConnection(conn.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertToSMSDb2", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = user;
                        cmd.Parameters.Add("@Body", SqlDbType.VarChar).Value = Body;
                        cmd.Parameters.Add("@ToAddress", SqlDbType.VarChar).Value = ToAddress;
                        cmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = 1;


                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return "Good";
            }
            else
            {
                return "Kah po dilni";
            }

           
        }


        public int ValidateUser(string username, string pass)
        {
            var userId = 0;
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SMSDb"].ConnectionString);
            using (SqlConnection con = new SqlConnection(conn.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("prc_ControllUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
                    cmd.Parameters.Add("@PWD", SqlDbType.VarChar).Value = pass;
                    con.Open();
                    //userId = cmd.ExecuteNonQuery();
                    userId = (int) cmd.ExecuteScalar();
                }
            }

            return userId;
        }

        public Boolean VerifyHashPassword(string thisPassword, string thisHash)
        {
            var isValid = false;
            var tmpHash = Encryptor.MD5Hash(thisPassword); // Call the routine on user input
            if (tmpHash == thisHash) isValid = true;  // Compare to previously generated hash
            return isValid;
        }


        [WebMethod]
        public string SendToMany(string user, string pass, string senderNr, string[] msg)
        {

            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SMSDb"].ConnectionString);

            //controll auth

            //
            //me store procedure
            using (SqlConnection con = new SqlConnection(conn.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertToSMSDb", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var mesazhi  in msg)
                    {
                        cmd.Parameters.Add("@User", SqlDbType.VarChar).Value = user;
                        cmd.Parameters.Add("@Msg", SqlDbType.VarChar).Value = msg;
                        cmd.Parameters.Add("@Sender", SqlDbType.VarChar).Value = "Sistemi";
                        cmd.Parameters.Add("@ToSender", SqlDbType.VarChar).Value = senderNr;
                    }



                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return "msn";
        }
    }
}
