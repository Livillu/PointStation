using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools
{
    public partial class UserOtherConfigs : UserControl
    {
        public UserOtherConfigs()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql = $"IF(SELECT COUNT(*) FROM [OtherConfigs] where [FSno]=3)=0 INSERT INTO [OtherConfigs]([FSno],[FName],[F1]) VALUES(3,'庫存不足警示','{Convert.ToUInt16(checkBox1.Checked)}')";
            sql += $" ELSE UPDATE [OtherConfigs] SET [F1]='{Convert.ToUInt16(checkBox1.Checked)}' where [FSno]=3";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sql, conn1);
            cmd1.Connection.Open();
            if (cmd1.ExecuteNonQuery() < 1)
            {
                MessageBox.Show("設定失敗!!! 請重試....");
            }
            else
            {
                MainForm.QutyNoError = checkBox1.Checked;
                MessageBox.Show("設定成功....");
            }
        }

        private void UserOtherConfigs_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = MainForm.QutyNoError;
            checkBox2.Checked = MainForm.CheckAccounts;
            checkBox3.Checked = MainForm.CheckMig;
            string sql = $"SELECT ISNULL([F2],'') [F2],ISNULL([F3],'') [F3] FROM [OtherConfigs] where [FSno]=5";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sql, conn1);
            cmd1.Connection.Open();
            SqlDataReader reader = cmd1.ExecuteReader();
            if (reader.Read())
            {
                textBox1.Text = reader.GetString(0);
                textBox2.Text = reader.GetString(1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = $"IF(SELECT COUNT(*) FROM [OtherConfigs] where [FSno]=4)=0 INSERT INTO [OtherConfigs]([FSno],[FName],[F1]) VALUES(4,'啟用折扣檢查','{Convert.ToUInt16(checkBox2.Checked)}')";
            sql += $" ELSE UPDATE [OtherConfigs] SET [F1]='{Convert.ToUInt16(checkBox2.Checked)}' where [FSno]=4";

            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sql, conn1);
            cmd1.Connection.Open();
            if (cmd1.ExecuteNonQuery() < 1)
            {
                MessageBox.Show("設定失敗!!! 請重試....");
            }
            else
            {
                MainForm.CheckAccounts = checkBox2.Checked;
                MessageBox.Show("設定成功....");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sql = $"IF(SELECT COUNT(*) FROM [OtherConfigs] where [FSno]=5)=0 INSERT INTO [OtherConfigs]([FSno],[FName],[F1],[F2],[F3]) VALUES(5,'啟用電子發票','{Convert.ToUInt16(checkBox3.Checked)}','{textBox1.Text}','{textBox2.Text}')";
            sql += $" ELSE UPDATE [OtherConfigs] SET [F1]='{Convert.ToUInt16(checkBox3.Checked)}',[F2]='{textBox1.Text}',[F3]='{textBox2.Text}' where [FSno]=5";

            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sql, conn1);
            cmd1.Connection.Open();
            if (cmd1.ExecuteNonQuery() < 1)
            {
                MessageBox.Show("設定失敗!!! 請重試....");
            }
            else
            {
                MainForm.CheckMig = checkBox3.Checked;
                MessageBox.Show("設定成功....");
            }
        }
    }
}
