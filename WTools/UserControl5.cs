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
    public partial class UserControl5 : UserControl
    {
        DataTable DTsale, DSupport;
        public UserControl5()
        {
            InitializeComponent();
        }

        private void Getdata()
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT TOP (1) [MB001],[MB002],[MB003],[MB004],[CostPrice] FROM [Products] where MB001='{textBox2.Text.Trim()}' and SupId ='{comboBox1.SelectedValue.ToString()}'", conn1);//
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

        private void TB001_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (e.KeyChar == (char)13)
            {
                Getdata();
            }
        }

        private void UserControl5_Load(object sender, EventArgs e)
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
            dataGridView1.DataSource = DTsale;

            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [SupId],[SupName] FROM [Support]", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            DataTable DSupport = new DataTable();
            DSupport.Load(sdr);
            comboBox1.DataSource = DSupport;
            comboBox1.DisplayMember = "SupName";
            comboBox1.ValueMember = "SupId";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Getdata();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = false;
            //產生單號
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT max([Od_No]) FROM [OrderProductM] where [Od_No] like (FORMAT(GETDATE(), 'yyyyMMdd')+'%')", conn1);
            cmd1.Connection.Open();
            string tmp=cmd1.ExecuteScalar().ToString();
            if (tmp !="")
            {
                textBox3.Text = tmp.Substring(0, 8)+string.Format("{0:00}",Convert.ToInt16(tmp.Substring(8,2))+1);
            }
            else
            {
                cmd1.CommandText = "SELECT FORMAT(GETDATE(), 'yyyyMMdd')+'01'";
                textBox3.Text = cmd1.ExecuteScalar().ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dtSale = DTsale;
            if(dtSale.Rows.Count > 0 && comboBox1.SelectedIndex>-1)
            {
                decimal TotalPrice=0.00M;
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand("",conn1);
                cmd1.Connection.Open();
                SqlTransaction sqlTransaction = null;
                sqlTransaction = conn1.BeginTransaction();
                cmd1.Transaction = sqlTransaction;
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
                    button1.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = true;
                }
                catch (Exception ex)
                {
                    sqlTransaction.Rollback();
                    MessageBox.Show($"{ex.Message}新增異常!!!! 請重試...");
                }
            }
        }
    }
}
