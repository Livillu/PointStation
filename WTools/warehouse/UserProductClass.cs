using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTools.warehouse
{
    public partial class UserProductClass : UserControl
    {
        DataTable dt;
        public UserProductClass()
        {
            InitializeComponent();
        }

        private void UserProductClass_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand("SELECT [ClassId],[ClassName] FROM [PtClass]", conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            tbClassId.Text= dt.Rows[e.RowIndex]["ClassId"].ToString();
            tbClassName.Text = dt.Rows[e.RowIndex]["ClassName"].ToString();
            prgstatus(5);
        }
        private void prgstatus(int index)
        {
            tbClassId.Enabled = false;
            tbClassName.Enabled = false;
            if (index == 0)
            {
                tbClassName.Enabled = true;
                tbClassId.Enabled = true;
            }
            if (index == 1)
            {
                tbClassName.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Setbutton("A");
            tbClassId.Text = "";
            tbClassName.Text = "";
            prgstatus(0);
        }

        private void Setbutton(string tp)
        {
            if (tp == "A")
            {
                button2.Enabled = false;
                button3.Enabled = false;
                button5.Enabled = true;
                button4.Enabled = true;
            }
            if (tp == "S")
            {
                prgstatus(5);
                button2.Enabled = true;
                button3.Enabled = true;
                button5.Enabled = false;
                button4.Enabled = false;
                tbClassId.Text = "";
                tbClassName.Text = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tbClassId.Text.Trim() != "")
            {
                Setbutton("A");
                prgstatus(1);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Setbutton("S");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Setbutton("S");
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT count(*) FROM [PtClass] WHERE [ClassId]='{tbClassId.Text.Trim()}'", conn1);
            cmd1.Connection.Open();
            string sql;
            if (Convert.ToInt16(cmd1.ExecuteScalar()) > 0)
            {
                sql = $"UPDATE [PtClass] SET [ClassName] ='{tbClassName.Text}' WHERE [ClassId]='{tbClassId.Text.Trim()}'";
                cmd1.CommandText = sql;
            }
            else
            {
                sql = $"INSERT INTO [PtClass]([ClassId],[ClassName]) VALUES('{tbClassId.Text}','{tbClassName.Text}')";
                cmd1.CommandText = sql;
            }
            cmd1.ExecuteNonQuery();
            prgstatus(2);
            UserProductClass_Load(null,null);
        }
    }
}
