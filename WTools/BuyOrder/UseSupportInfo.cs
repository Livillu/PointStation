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

namespace WTools.BuyOrder
{
    public partial class UseSupportInfo : UserControl
    {
        DataTable TTD;
        public UseSupportInfo()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            TTD = new DataTable();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("", conn1);
            cmd1.Connection.Open();
            string sql = "SELECT [SupId] ,[SupName] ,[SupCname] ,[SupTel] ,[SupAddr] ,[SupSno] ,[Boss] ,[CTel] ,[SupEmail] ,[SupWeb] FROM [Support] where 1=1";
            if (textBox10.Text.Trim() != "") sql += $" and SupName like '%{textBox10.Text}%'";
            if (textBox9.Text.Trim() != "") sql += $" and SupSno like '%{textBox9.Text}%'";
            if (textBox11.Text.Trim() != "") sql += $" and SupId like '%{textBox11.Text}%'";
            cmd1.CommandText = sql;
            SqlDataReader sdr = cmd1.ExecuteReader();
            TTD.Load(sdr);
            dataGridView1.DataSource = TTD;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabPage2.Text = "新增";
            ButtonChange(1);
            tbBoss.Text = "";
            tbCTel.Text = "";
            tbSupAddr.Text = "";
            tbSupCname.Text = "";
            tbSupEmail.Text = "";
            tbSupId.Text = "";
            tbSupName.Text = "";
            tbSupSno.Text = "";
            tbSupTel.Text = "";
            tbSupWeb.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                DataRow dr = TTD.NewRow();
                dr = TTD.Rows[index];
                tbBoss.Text = dr["Boss"].ToString();
                tbCTel.Text = dr["CTel"].ToString();
                tbSupAddr.Text = dr["SupAddr"].ToString();
                tbSupCname.Text = dr["SupCname"].ToString();
                tbSupEmail.Text = dr["SupEmail"].ToString();
                tbSupId.Text = dr["SupId"].ToString();
                tbSupName.Text = dr["SupName"].ToString();
                tbSupSno.Text = dr["SupSno"].ToString();
                tbSupTel.Text = dr["SupTel"].ToString();
                tbSupWeb.Text = dr["SupWeb"].ToString();
                tabPage2.Text = "編輯";
                ButtonChange(1);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("", conn1);
            cmd1.Connection.Open();
            if (tabPage2.Text == "編輯")
            {
                cmd1.CommandText = $"UPDATE [Support] SET [SupName] ='{tbSupName.Text}' ,[SupCname] ='{tbSupCname.Text}' ,[SupTel] ='{tbSupTel.Text}' ,[SupAddr] ='{tbSupAddr.Text}' ,[SupSno] ='{tbSupSno.Text}' ,[Boss] ='{tbBoss.Text}' ,[CTel] ='{tbCTel.Text}' ,[SupEmail] ='{tbSupEmail.Text}' ,[SupWeb] ='{tbSupWeb.Text}' WHERE [SupId] = '{tbSupId.Text}'";
            }
            else if (tabPage2.Text == "新增")
            {
                cmd1.CommandText = $"INSERT INTO [Support]([SupId],[SupName],[SupCname],[SupTel],[SupAddr],[SupSno],[Boss],[CTel],[SupEmail],[SupWeb]) VALUES('{tbSupId.Text}','{tbSupName.Text}','{tbSupCname.Text}','{tbSupTel.Text}','{tbSupAddr.Text}','{tbSupSno.Text}','{tbBoss.Text}','{tbCTel.Text}','{tbSupEmail.Text}','{tbSupWeb.Text}')";
            }
            if (cmd1.ExecuteNonQuery() > 0)
            {
                button1.PerformClick();
                MessageBox.Show("存檔完成....");
            }
            else
            {
                MessageBox.Show("存檔失敗!!!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
           
        }
        private void ButtonChange(int index)
        {
            button2.Enabled = true;
            button3.Enabled = true;
            tabControl1.TabPages.Clear();
            switch (index)
            {
                case 1://新增狀態
                    button2.Enabled = false;
                    button3.Enabled = false;
                    tabControl1.TabPages.Add(tabPage2);
                    break;
                default:
                    tabControl1.TabPages.Add(tabPage1);
                    break;
            }
        }
        private void UseSupportInfo_Load(object sender, EventArgs e)
        {
            ButtonChange(0);
        }
    }
}
