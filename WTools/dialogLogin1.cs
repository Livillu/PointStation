using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTools
{
    public partial class dialogLogin1 : Form
    {
        string userid,userPrivate;
        public dialogLogin1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT [usId],[usPrivat] FROM [UsLogin] where [usId] = '{textBox1.Text}' and [usPw] = '{textBox2.Text}'", conn1);
            cmd1.Connection.Open();
            SqlDataReader tmp=cmd1.ExecuteReader();
            if (tmp.Read())
            {
                userid = tmp[0].ToString();
                userPrivate = tmp[1].ToString();
            }
        }
        public string Getmsg()
        {
            return userid;
        }
        public string Getprivate()
        {
            return userPrivate;
        }
    }
}
