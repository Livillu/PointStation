using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTools
{
    public partial class UserOtherIO : UserControl
    {
        DataTable TTD;
        int tbInOut,Sid=0;
        public UserOtherIO()
        {
            InitializeComponent();
        }
        private void Change(int index)
        {
            tabControl1.TabPages.Clear();
            if (index == 2)
            {
                tabControl1.TabPages.Add(tabPage2);
            }
            else
            {
                tabControl1.TabPages.Add(tabPage1);
            }
        }
        private void ButtonChange(int index)
        {
            button9.Enabled = true;
            button8.Enabled = true;
            switch (index)
            {
                case 1://新增狀態
                    button9.Enabled = false;
                    button8.Enabled = false;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            Change(1);
            TTD = new DataTable();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("", conn1);
            cmd1.Connection.Open();
            string sql = "SELECT [Id],[Sno],[MB001],[Quty],[Price],[InOut],[Cdate],[Mark],[Quty]*[Price]*[InOut] Total FROM [OtherCost] where 1=1";
            if (textBox10.Text.Trim() != "") sql += $" and MB001 like '%{textBox10.Text}%'";
            if (textBox11.Text.Trim() != "") sql += $" and Sno like '%{textBox11.Text}%'";
            cmd1.CommandText = sql;
            SqlDataReader sdr = cmd1.ExecuteReader();
            TTD.Load(sdr);
            dataGridView1.DataSource = TTD;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
            ButtonChange(0);
            Change(1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) tbInOut = -1;
            else tbInOut = 1;
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT count(*) FROM [OtherCost] where [Id]={Sid}", conn1);
            cmd1.Connection.Open();
            string sql;
            string SNO = DateTime.Now.ToString("MMddHHmmss");
            if(tbSno.Text=="") tbSno.Text = SNO;
            if (tabPage2.Text == "編輯")
            {
                sql = $"UPDATE [OtherCost] SET [Sno] = '{tbSno.Text}',[MB001] ='{tbMB001.Text}' ,[Quty] ='{tbQuty.Value}' ,[Price] ='{tbPrice.Value}' ,[InOut] ='{tbInOut}' ,[Mark] ='{tbMark.Text}',[Cdate]='{dateTimePicker1.Value.ToString("yyyy-MM-dd hh:mm:ss")}' WHERE [Id] = {Sid}";
                cmd1.CommandText = sql;
            }
            else if (tabPage2.Text == "新增")
            {
                sql = $"INSERT INTO [OtherCost]([Sno],[MB001],[Quty],[Price],[InOut],[Mark],[Cdate]) VALUES('{tbSno.Text}','{tbMB001.Text}',{tbQuty.Value},{tbPrice.Value},{tbInOut},'{tbMark.Text}','{dateTimePicker1.Value.ToString("yyyy-MM-dd hh:mm:ss")}')";
                cmd1.CommandText = sql;
            }
            if (cmd1.ExecuteNonQuery() > 0)
            {
                tbSno.Text = "";
                tbMark.Text = "";
                tbMB001.Text = "";
                tbPrice.Value = 0;
                tbQuty.Value = 0;
            }
            else
            {
                MessageBox.Show("存檔失敗!!!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Sid = 0;
            tbMark.Text = "";
            tbSno.Text = "";
            tbQuty.Value = 0;
            tbPrice.Value = 0;
            tbMB001.Text = "";
            dateTimePicker1.Value = DateTime.Now;          
            tbSno.Enabled = true;
            tabPage2.Text = "新增";
            ButtonChange(1);
            Change(2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow dgr = dataGridView1.SelectedRows[0];
                tbQuty.Value = Convert.ToDecimal(dgr.Cells["Quty"].Value);
                tbPrice.Value = Convert.ToDecimal(dgr.Cells["Price"].Value);
                tbMB001.Text = dgr.Cells["MB001"].Value.ToString().Trim();
                tbSno.Text = dgr.Cells["Sno"].Value.ToString().Trim();
                Sid =Convert.ToInt32(dgr.Cells["Id"].Value);
                if (Convert.ToInt16(dgr.Cells["InOut"].Value) > 0)
                {
                    radioButton2.Checked=true;
                }
                dateTimePicker1.Value = Convert.ToDateTime(dgr.Cells["Cdate"].Value);
                tbMark.Text = dgr.Cells["Mark"].Value.ToString().Trim();
                tbSno.Enabled = false;
                tabPage2.Text = "編輯";
                Change(2);
                ButtonChange(1);
            }
        }

        private void UserOtherIO_Load(object sender, EventArgs e)
        {
            Change(1);
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataRow dr = TTD.NewRow();
            dr = TTD.Rows[e.RowIndex];
            Sid=Convert.ToInt32(dr["Id"]);
            tbSno.Text = dr["Sno"].ToString();
            tbMB001.Text = dr["MB001"].ToString();
            tbQuty.Text = dr["Quty"].ToString();
            tbPrice.Text = dr["Price"].ToString();
            tbMark.Text = dr["Mark"].ToString();
            dateTimePicker1.Value = Convert.ToDateTime(dr["Cdate"]);
            if (Convert.ToInt16(dr["InOut"])>0 )radioButton2.Checked=true;
            else radioButton1.Checked = true;
        }
    }
}
