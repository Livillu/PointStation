using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools
{
    public partial class UserControl4 : UserControl
    {
        DataTable DT = new DataTable();
        public UserControl4()
        {
            InitializeComponent();
            dataGridView1.DataSource = DT;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(DT !=null && DT.Rows.Count > 0) DT.Rows.Clear();
            string sqlparam = "";
            if (textBox2.Text != "") sqlparam += " WHERE MB001 LIKE '%" + textBox2.Text + "%' OR  MB002 LIKE '%" + textBox2.Text + "%'";                      
            string sqlstring = "SELECT [MB001],[MB002],[MB003],[MB051],[MB064],[MB004],[GpSno],[SupId],(SELECT TOP (1) [SupName] FROM [Support] where [SupId]=a.SupId) [SupName],[CostPrice] FROM [Products] a " + sqlparam;

            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sqlstring, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DT.Load(sdr);
            label3.Text=DT.Rows.Count.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {           
            bool isok = true;
            List<int> listDel = new List<int>();
            int i = 0;
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(item.Cells[0].Value))
                {
                    try
                    {
                        listDel.Add(i);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception: " + ex.Message);
                        return;
                    }
                }
                i++;
            }
            if (listDel.Count > 0 && DialogResult.Yes == MessageBox.Show("確定刪除勾選品項", "刪除商品", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                SqlTransaction sqlTransaction = null;
                sqlTransaction = conn1.BeginTransaction();
                cmd1.Transaction = sqlTransaction;
                
                for (int j1 = listDel.Count; j1>0; j1--)
                {
                    int K1 = listDel[j1 - 1];
                    cmd1.CommandText = "DELETE FROM [Products] WHERE [MB001]='" + DT.Rows[K1]["MB001"].ToString() + "'";
                    cmd1.ExecuteNonQuery();
                }
                try
                {
                    sqlTransaction.Commit();
                }
                catch
                {
                    isok = false;
                    sqlTransaction.Rollback();
                    MessageBox.Show("刪除失敗!!!!");
                }
                if (isok)
                {
                    for (int j = listDel.Count; j > 0; j--)
                    {
                        DT.Rows.RemoveAt(listDel[j-1]);
                    }
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataRow row = DT.Rows[e.RowIndex];
            EditProduct editProduct = new EditProduct(row);
            editProduct.Text = "編輯商品";
            DialogResult dr = editProduct.ShowDialog();
            if (dr == DialogResult.OK)
            {
                row=editProduct.ResultProduct();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataRow row = DT.NewRow();
            EditProduct editProduct = new EditProduct(row);
            editProduct.Text = "新增商品";
            DialogResult dr = editProduct.ShowDialog();
            if (dr == DialogResult.OK)
            {
                row = editProduct.ResultProduct();
                DT.Rows.Add(row);
            }
            
        }
        private void UserControl4_Load(object sender, EventArgs e)
        {
            DataColumn dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB001";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB002";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = true;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB003";
            dataColumn1.DefaultValue = "";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = true;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB004";
            dataColumn1.DefaultValue = "";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "MB051";
            dataColumn1.DefaultValue = 0;
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.DefaultValue = 0;
            dataColumn1.ColumnName = "MB064";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = true;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "GpSno";
            dataColumn1.DefaultValue = "";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = true;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "SupName";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = true;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "SupId";
            dataColumn1.DefaultValue = "";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "CostPrice";
            dataColumn1.DefaultValue = 0;
            DT.Columns.Add(dataColumn1);

        }
}
}
