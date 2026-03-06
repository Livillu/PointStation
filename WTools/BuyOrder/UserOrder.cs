using ScottPlot.Colormaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools.BuyOrder
{
    public partial class UserOrder : UserControl
    {
        public DataTable DT;//
        public DataTable DT1;//
        public DataTable DTsale;
        public UserOrder()
        {
            InitializeComponent();
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
        private void button1_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            DT = new DataTable();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("", conn1);
            cmd1.Connection.Open();
            string sql = "SELECT a.[SupId],[Od_No],[TotalPrice],[Cdate] FROM [OrderProductM] a inner join [Support] b on a.SupId=b.SupId where 1=1";
            if (textBox10.Text.Trim() != "") sql += $" and SupName like '%{textBox10.Text}%'";
            if (textBox9.Text.Trim() != "") sql += $" and SupSno like '%{textBox9.Text}%'";
            if (textBox11.Text.Trim() != "") sql += $" and b.SupId like '%{textBox11.Text}%'";
            cmd1.CommandText = sql;
            SqlDataReader sdr = cmd1.ExecuteReader();
            DT.Load(sdr);
            dataGridView1.DataSource = DT;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //產生單號
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT max([Od_No]) FROM [OrderProductM] where [Od_No] like (FORMAT(GETDATE(), 'yyyyMMdd')+'%')", conn1);
            cmd1.Connection.Open();
            string tmp = cmd1.ExecuteScalar().ToString();
            if (tmp != "")
            {
                textBox3.Text = tmp.Substring(0, 8) + string.Format("{0:00}", Convert.ToInt16(tmp.Substring(8, 2)) + 1);
            }
            else
            {
                cmd1.CommandText = "SELECT FORMAT(GETDATE(), 'yyyyMMdd')+'01'";
                textBox3.Text = cmd1.ExecuteScalar().ToString();
            }
            ButtonChange(1);
            tabPage2.Text = "新增";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && DT1 !=null && DT1.Rows.Count>0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                DataRow dr = DT.Rows[index];
                textBox3.Text = dr["Od_No"].ToString();
                comboBox1.SelectedValue= dr["SupId"].ToString();
                tabPage2.Text = "編輯";
                DTsale.Rows.Clear();
                foreach (DataRow dr1 in DT1.Rows)
                {
                    DataRow dr2 = DTsale.NewRow();
                    dr2.ItemArray=dr1.ItemArray;
                    DTsale.Rows.Add(dr2);
                }
                ButtonChange(1);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "") return;
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT TOP (1) [MB001],[MB002],[MB003],[MB004],[CostPrice] FROM [Products] where MB001='{textBox2.Text.Trim()}'", conn1);
            //SqlCommand cmd1 = new SqlCommand($"SELECT TOP (1) [MB001],[MB002],[MB003],[MB004],[CostPrice] FROM [Products] where MB001='{textBox2.Text.Trim()}' and SupId ='{comboBox1.SelectedValue.ToString()}'", conn1);//
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            if (sdr.Read())
            {
                textBox2.Text = "";
                DataRow dr2 = DTsale.NewRow();
                dr2[0] = sdr[0];
                dr2[1] = sdr[1];
                dr2[2] = sdr[2];
                dr2[3] = sdr[3];
                dr2[4] = sdr[4];
                dialogform dl = new dialogform(sdr[1].ToString());
                dl.Text = "採購量";
                DialogResult dr = dl.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    dr2[5] = Convert.ToDecimal(dl.GetMsg());
                    dr2[6] = Convert.ToDecimal(dl.GetMsg()) * Convert.ToDecimal(sdr[4]);
                    DTsale.Rows.Add(dr2);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataTable dtSale = DTsale;
            if (dtSale.Rows.Count > 0 && comboBox1.SelectedIndex > -1)
            {
                decimal TotalPrice = 0.00M;
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                SqlTransaction sqlTransaction = null;
                sqlTransaction = conn1.BeginTransaction();
                cmd1.Transaction = sqlTransaction;
                if (tabPage2.Text == "新增")
                {
                    //單頭
                    string UsId = "";
                    foreach (DataRow row in dtSale.Rows)
                    {
                        TotalPrice += Convert.ToDecimal(row[6]);
                        cmd1.CommandText = $"INSERT INTO [OrderProuductT] ([Od_No],[PtNo],[Quty],[Price]) VALUES('{textBox3.Text}','{row[0].ToString()}',{row[5].ToString()},{row[4].ToString()})";
                        cmd1.ExecuteNonQuery();
                    }
                    cmd1.CommandText = $"INSERT INTO [OrderProductM] ([SupId],[Od_No],[TotalPrice],[UsId]) VALUES('{comboBox1.SelectedValue.ToString()}','{textBox3.Text}',{TotalPrice},'{UsId}')";
                    cmd1.ExecuteNonQuery();
                    try
                    {
                        sqlTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        MessageBox.Show($"{ex.Message}新增異常!!!! 請重試...");
                    }
                } 
                else if(tabPage2.Text == "編輯")
                {
                    cmd1.CommandText = $"DELETE FROM [OrderProuductT] where [Od_No] ='{textBox3.Text}'";
                    cmd1.ExecuteNonQuery();
                    //msg = "編輯完成....";
                    foreach (DataRow row in dtSale.Rows)
                    {
                        TotalPrice += Convert.ToDecimal(row[6]);
                        cmd1.CommandText = $"INSERT INTO [OrderProuductT] ([Od_No],[PtNo],[Quty],[Price]) VALUES('{textBox3.Text}','{row[0].ToString()}',{row[5].ToString()},{row[4].ToString()})";
                        cmd1.ExecuteNonQuery();
                    }
                    cmd1.CommandText = $"UPDATE [OrderProductM] SET [SupId] = '{comboBox1.SelectedValue.ToString()}',[TotalPrice] ={TotalPrice}  ,[Cdate] = getdate() ,[UsId] = {MainForm.UserId} WHERE [Od_No] ='{textBox3.Text}'";
                    cmd1.ExecuteNonQuery();
                    try
                    {
                        sqlTransaction.Commit();
                        button1.PerformClick();
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        MessageBox.Show($"{ex.Message}編輯異常!!!! 請重試...");
                    }
                }
            }

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "delete")
            { 
                DTsale.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            
            DataRow udr = DT.Rows[e.RowIndex];
            string sql = $"SELECT MB001,MB002,MB003,MB004,CostPrice,MB064,CostPrice * [Quty] as Total  FROM [OrderProuductT] a inner join Products b on a.PtNo=b.MB001 WHERE [Od_No]='{udr["Od_No"]}'";
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DT1 = new DataTable();
            DT1 = DTsale.Clone();
            while (sdr.Read())
            {
                DataRow dr2 = DT1.NewRow();
                dr2["MB002"] = sdr["MB002"];
                dr2["MB001"] = sdr["MB001"];
                dr2["MB003"] = sdr["MB003"];
                dr2["MB004"] = sdr["MB004"];
                dr2["CostPrice"] = sdr["CostPrice"];
                dr2["MB064"] = sdr["MB064"];
                dr2["Total"] = sdr["Total"];
                DT1.Rows.Add(dr2);
            }
            dataGridView3.DataSource = DT1;
        }

        private void UserOrder_Load(object sender, EventArgs e)
        {
            DTsale = new DataTable();
            DataColumn dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB001";
            dataColumn.AllowDBNull = false;
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB002";
            dataColumn.AllowDBNull = false;
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB003";
            dataColumn.AllowDBNull = false;
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB004";
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(decimal);
            dataColumn.ColumnName = "CostPrice";
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(decimal);
            dataColumn.ColumnName = "MB064";
            dataColumn.ReadOnly = false;
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(decimal);
            dataColumn.ColumnName = "Total";
            dataColumn.ReadOnly = false;
            DTsale.Columns.Add(dataColumn);
            dataGridView2.DataSource = DTsale;

            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [SupId],[SupName] FROM [Support]", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            DataTable DSupport = new DataTable();
            DSupport.Load(sdr);
            comboBox1.DataSource = DSupport;
            comboBox1.DisplayMember = "SupName";
            comboBox1.ValueMember = "SupId";
            ButtonChange(0);
        }
    }
}
