using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTools.warehouse
{
    public partial class UserProductMan : UserControl
    {
        DataTable DT = new DataTable();
        public UserProductMan()
        {
            InitializeComponent();
            dataGridView1.DataSource = DT;
        }
        public class EProduct
        {
            public string MB001 { get; set; }
            public string MB002 { get; set; }
            public string MB003 { get; set; }
            public string MB004 { get; set; }
            public decimal MB051 { get; set; } = 0;
            public int MB064 { get; set; }  
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(DT !=null && DT.Rows.Count > 0) DT.Rows.Clear();
            string sqlparam = "";
            if (textBox2.Text != "") sqlparam += " WHERE MB001 LIKE '%" + textBox2.Text + "%' OR  MB002 LIKE '%" + textBox2.Text + "%'";                      
            string sqlstring = "SELECT [MB001],[MB002],[MB003],[MB051],[MB064],[MB004],[GpSno],[SupId],(SELECT TOP (1) [SupName] FROM [Support] where [SupId]=a.SupId) [SupName] FROM [Products] a " + sqlparam;

            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sqlstring, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DT.Load(sdr);           
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
            if (listDel.Count > 0 && DialogResult.Yes == MessageBox.Show("確定刪除", "刪除商品", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                SqlTransaction sqlTransaction = null;
                sqlTransaction = conn1.BeginTransaction();
                cmd1.Transaction = sqlTransaction;
                
                for (int j1 = 0;j1< listDel.Count; j1 ++)
                {
                    try
                    {
                        cmd1.CommandText = "DELETE FROM [Products] WHERE [MB001]='" + DT.Rows[j1][0].ToString() +"'";
                        cmd1.ExecuteNonQuery();
                        sqlTransaction.Commit();
                    }
                    catch
                    {
                        if (sqlTransaction != null)
                        {
                            isok=false;
                            sqlTransaction.Rollback();
                            MessageBox.Show("刪除失敗!!!!");
                        }
                    }
                }
                if (isok)
                {
                    for (int j = listDel.Count - 1; j > -1; j--)
                    {
                        DT.Rows.RemoveAt(listDel[j]);
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
            DataRow row = null;
            EditProduct editProduct = new EditProduct(row);
            editProduct.Text = "新增商品";
            DialogResult dr = editProduct.ShowDialog();
            if (dr == DialogResult.OK)
            {
                row = editProduct.ResultProduct();
            }
        }
    }
}
