using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools
{
    public partial class UserAssember : UserControl
    {
        DataTable DT,DT1, DTsale;
        public UserAssember()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DTsale.Rows.RemoveAt(e.RowIndex);
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                DataRow dr = DTsale.Rows[e.RowIndex];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DTsale.Rows.Count > 0 && textBox1.Text != "" && textBox2.Text != "" && numericUpDown1.Value >0)
            {
                SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd = new SqlCommand($"INSERT INTO [GpProductM] ([GpId], [GpName], [GpPrice]) VALUES('{textBox1.Text}','{textBox2.Text}',{numericUpDown1.Value})", conn);
                cmd.Connection.Open();
                if (tabPage2.Text == "新增")
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        foreach (DataRow dr in DTsale.Rows)
                        {
                            string sql = $"INSERT INTO [GpProductT] ([GpId],[MB001],[MB064]) VALUES('{textBox1.Text}','{dr[0]}',{dr[2]})";
                            cmd.CommandText = sql;
                            if (cmd.ExecuteNonQuery() > 0) MessageBox.Show("新增完成....");
                            else MessageBox.Show("新增失敗!!!!");
                        }
                    }
                }
                else if (tabPage2.Text == "編輯")
                {
                    string msg = "編輯失敗!!!!";
                    string sql = $"UPDATE [GpProductM] SET [GpName]='{textBox2.Text}',[GpPrice]={numericUpDown1.Value} where [GpId]='{textBox1.Text}'";
                    cmd.CommandText= sql;
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        sql = $"DELETE FROM [GpProductT] where [GpId]='{textBox1.Text}'";
                        cmd.CommandText = sql;
                        if (cmd.ExecuteNonQuery() > 0) {
                            msg = "編輯完成....";
                            foreach (DataRow dr in DTsale.Rows)
                            {
                                sql = "INSERT INTO [GpProductT] ([GpId],[MB001],[MB064]) VALUES(";
                                sql += $"'{textBox1.Text}','{dr[0]}',{dr[2]})";
                                cmd.CommandText = sql;
                                if(cmd.ExecuteNonQuery()<1) msg = "編輯失敗...."; 
                            }
                        }
                    }
                    button10.PerformClick();
                    MessageBox.Show(msg);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql = $"SELECT TOP(1) [MB001],[MB002] FROM [Products] WHERE [MB001]='{textBox4.Text}'";
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {    
                /*dialogform dl = new dialogform(sdr[1].ToString());
                dl.productName = sdr[1].ToString();
                dl.Text = "數量";
                DialogResult dr = dl.ShowDialog();*/
                //if (dr == DialogResult.OK)
                //{
                DataRow dr2 = DTsale.NewRow();
                dr2[0] = sdr[0];
                dr2[1] = sdr[1];
                //dr2[2] = dl.GetMsg();
                dr2[2] = 1;
                DTsale.Rows.Add(dr2);
                textBox4.Text = "";
                //}
            }
        }

        private void TableClear()
        {
            DTsale.Rows.Clear();
            textBox1.Text = "";
            textBox2.Text = "";
            numericUpDown1.Value = 0;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            TableClear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DT = new DataTable();
            string sql = "SELECT [GpId] [MB001],[GpName],[GpPrice],1 [MB064] FROM [GpProductM] WHERE 1=1";
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.HasRows)
            {
                DT.Load(sdr);
                dataGridView1.DataSource = DT;
            }         
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
            button7.Enabled = true;
            switch (index)
            {
                case 1://新增狀態
                    button9.Enabled = false;
                    button8.Enabled = false;
                    button7.Enabled = false;
                    break;
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            tabPage2.Text = "新增";
            textBox1.Enabled = true;
            DTsale.Rows.Clear();
            TableClear();
            ButtonChange(1);
            Change(2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            Change(1);
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataRow udr = DT.Rows[e.RowIndex];
            string sql = $"SELECT [MB001],(SELECT TOP (1) [MB002] FROM [Products] WHERE [MB001]=a.[MB001]) [MB002],[MB064] FROM [GpProductT] a WHERE [GpId]='{udr["MB001"].ToString()}'";
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
                    dr2["MB064"] = sdr["MB064"];
                    DT1.Rows.Add(dr2);
            }
            dataGridView3.DataSource = DT1;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView2.Columns[e.ColumnIndex].Name== "Delete")
            {
                SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd = new SqlCommand($"DELETE FROM [GpProductT] WHERE  [GpId]='{textBox1.Text.Trim()}' and [MB001]='{DTsale.Rows[e.RowIndex]["MB001"]}'", conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                DTsale.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count>0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                if (DialogResult.Yes == MessageBox.Show($"{DT.Rows[index]["MB001"]}{DT.Rows[index]["GpName"]} 確定刪除???", "刪除資料", MessageBoxButtons.YesNo))
                {
                    SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
                    SqlCommand cmd = new SqlCommand($"DELETE FROM [GpProductM] WHERE  [GpId]='{DT.Rows[index]["MB001"]}';DELETE FROM [GpProductT] WHERE  [GpId]='{DT.Rows[index]["MB001"]}';", conn);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    DT.Rows.RemoveAt(index);
                    DT1.Rows.Clear();
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            { 
                DataRow mdr = DT.Rows[dataGridView1.SelectedRows[0].Index];
                DTsale.Rows.Clear();
                foreach (DataRow dr in DT1.Rows) {
                    DataRow idr = DTsale.NewRow();
                    idr[0] = dr[0];
                    idr[1] = dr[1];
                    idr[2] = dr[2];
                    DTsale.Rows.Add(idr);
                }
                //[GpId] [MB001],[GpName],[GpPrice],1 [MB064]
                textBox1.Text = mdr["MB001"].ToString();
                textBox2.Text = mdr["GpName"].ToString();
                numericUpDown1.Value = Convert.ToDecimal(mdr["GpPrice"]);
                tabPage2.Text = "編輯";
                textBox1.Enabled = false;
                Change(2);
                ButtonChange(1);
            }
        }

        private void UserAssember_Load(object sender, EventArgs e)
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
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "MB064";
            dataColumn.ReadOnly = false;
            DTsale.Columns.Add(dataColumn);
            dataGridView2.DataSource = DTsale;

            dataGridView3.DataSource = DT1;
        }
    }
}
