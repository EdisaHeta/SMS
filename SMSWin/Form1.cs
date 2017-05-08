using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SMSWin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private static string ExecutePost(string user, string pass, string ToAddress, string Body)
        {

            WebClient client = new WebClient();

            string url = "http://localhost:51748/SMS.asmx/SendTo?user=" + user + "&pass=" + pass + "&ToAddress=" + ToAddress + "&Body=" + Body;
           
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
           
            return s;

        }

        private void button1_Click(object sender, EventArgs e)
        {
           label1.Text =    ExecutePost(userTxt.Text, passTxt.Text, toAddressTxt.Text, bodyTxt.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
