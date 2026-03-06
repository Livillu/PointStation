using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools.warehouse
{
    public partial class UserGetStock : UserControl
    {
        DataTable PtDt;
        string Sno1;
        int UpdateQuty = 0;
        public UserGetStock()
        {
            InitializeComponent();
        }

        private void UserGetStock_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            tabControl1.TabPages.Add(tabPage1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            Change(1);
            PtDt =new DataTable();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT [Sno],[userid] ,a.[MB001], [MB002],[MB003],[MB004],[Quty], case [InOut] when 1 then '退料' else '領料' end [InOut],[Cdate],[Memo] FROM [GetBackPt] a inner join [Products] b on a.MB001=b.MB001 ", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            PtDt.Load(sdr);
            dataGridView1.DataSource = PtDt;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            txmb001.Text= "";
            txmb001.Enabled = true;
            numericUpDown1.Value = 1;
            lbProductName.Text = "";
            lbM004.Text ="";
            lbM003.Text = "";
            tabPage2.Text = "新增";
            ButtonChange(1);
            Change(2);
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                DataRow dr=PtDt.Rows[index];
                Sno1 =dr["Sno"].ToString();
                UpdateQuty = Convert.ToInt32(dr["Quty"]);
                numericUpDown1.Value = Convert.ToDecimal(UpdateQuty);
                txmb001.Text = dr["MB001"].ToString();
                lbProductName.Text = dr["MB002"].ToString();
                lbM004.Text = dr["MB004"].ToString();
                lbM003.Text = dr["MB003"].ToString();
                textBox1.Text = dr["Memo"].ToString();
                txmb001.Enabled = false;
                tabPage2.Text = "編輯";
                Change(2);
                ButtonChange(2);
            }
        }

        private void txmb001_Leave(object sender, EventArgs e)
        {
            if (txmb001.Text.Trim().Length > 3)
            {
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand($"SELECT [MB001], [MB002],[MB003],[MB004] FROM [Products] where [MB001]='{txmb001.Text.Trim()}' ", conn1);
                cmd1.Connection.Open();
                SqlDataReader sdr = cmd1.ExecuteReader();
                if (sdr.Read())
                {
                    lbProductName.Text = sdr["MB002"].ToString().Trim();
                    lbM004.Text = sdr["MB004"].ToString().Trim();
                    lbM003.Text = sdr["MB003"].ToString().Trim();
                }
                cmd1.Connection.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Sno1 = "";
            UpdateQuty = 0;
            button1.PerformClick();
            ButtonChange(0);
            Change(1);
        }

        private void ButtonChange(int index)
        {
            button2.Enabled = true;
            button6.Enabled = true;
            switch (index) {
                case 1://新增狀態
                    button2.Enabled =false;
                    break;
                case 2://編輯狀態
                    button6.Enabled =false;
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (txmb001.Text.Trim().Length > 2)
            {
                WarehouseTools wt = new WarehouseTools();
                wt.MB001 = txmb001.Text.Trim();
                wt.Quty = Convert.ToInt32(numericUpDown1.Value);
                wt.Userid = MainForm.UserId;
                //UserType=0.進貨 1.進貨退回 2.銷貨 3.銷退 4.領料 5.領料退回
                wt.UserType = radioButton1.Checked ? "4" : "5";
                string InOut = radioButton1.Checked ? "-1" : "1";
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                if (tabPage2.Text == "新增")
                {
                    int code = 1;
                    SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
                    SqlCommand cmd = new SqlCommand($"SELECT ISNULL(max(Od_No),'') FROM [OrderProductM] where Od_No like CONVERT(char(8),getdate(),112)+'%'", conn);
                    cmd.Connection.Open();
                    string pcode = cmd.ExecuteScalar().ToString().Trim();
                    if (pcode.Length == 11)
                    {
                        code = Convert.ToInt16(pcode.Substring(8, 3)) + 1;
                    }
                    cmd.CommandText = $"SELECT CONVERT(char(8),getdate(),112)+'{String.Format("{0:D3}", code)}'";
                    wt.Od_No = cmd.ExecuteScalar().ToString();

                    cmd1.CommandText = $"INSERT INTO [GetBackPt]([userid],[MB001],[Quty],[InOut],[UserType],[Memo]) VALUES('{MainForm.UserId}','{txmb001.Text}',{numericUpDown1.Value},{InOut},'{wt.UserType}','{textBox1.Text}')";
                    if (cmd1.ExecuteNonQuery() > 0)
                    {
                        if (wt.SetWarehouse())
                        {
                            textBox1.Text = "";
                            txmb001.Text = "";
                            numericUpDown1.Value = 1;
                            lbProductName.Text = "";
                            lbM004.Text = "";
                            lbM003.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("新增失敗!!!!");
                        }
                    }
                }
                else if (tabPage2.Text == "編輯")
                {
                    cmd1.CommandText = $"Update [GetBackPt] SET [userid]='{MainForm.UserId}',[MB001]='{txmb001.Text}',[Quty]={numericUpDown1.Value},[InOut]={InOut},[UserType]='{wt.UserType}',[Memo]='{textBox1.Text}' WHERE Sno='{Sno1}'";
                    if (cmd1.ExecuteNonQuery() > 0)
                    {
                        if (wt.SetWarehouse())
                        {
                            MessageBox.Show("更新完成....");
                        }
                    }
                }
                
            }
            else { MessageBox.Show("部分欄位未填寫!!!!"); }
        }

    }
}
