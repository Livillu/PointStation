using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace WTools
{
    public partial class UserInvConfig : UserControl
    {
        public UserInvConfig()
        {
            InitializeComponent();
        }

        private void UserInvConfig_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(printerName);
            }
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [FSno],[F1],[F2],[F3],[F4] FROM [OtherConfigs] where FSno=1", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr=cmd1.ExecuteReader();
            if (sdr.Read())
            {
                comboBox1.Text = sdr[1].ToString();
                textBox1.Text = sdr[2].ToString();
                textBox2.Text = sdr[3].ToString();
                textBox3.Text = sdr[4].ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text=="" || textBox2.Text=="" || textBox3.Text == "")
            {
                MessageBox.Show("所有欄位必填 !!!");
                return;
            }
            string sql = $"IF(SELECT COUNT(*) FROM [OtherConfigs] where [FSno]=1)=0 INSERT INTO [OtherConfigs]([FSno],[FName],[F1],[F2],[F3],[F4]) VALUES(1,'發票設定','{comboBox1.Text}' ,'{textBox1.Text}','{textBox2.Text}','{textBox3.Text}')";
            sql += $" ELSE UPDATE [OtherConfigs] SET [F1] ='{comboBox1.Text}' ,[F2] = '{textBox1.Text}',[F3] = '{textBox2.Text}',[F4] ='{textBox3.Text}' WHERE [FSno]=1";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sql, conn1);
            cmd1.Connection.Open();
            if (cmd1.ExecuteNonQuery() < 1)
            {
                MessageBox.Show("發票設定失敗!!!請重設....");
            }
            else
            {
                MessageBox.Show("發票設定成功....");
            }
        }
    }
}
