using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using static WTools.SaleOrder.UserNetSale;

namespace WTools.warehouse
{
    public partial class UserOrderList : UserControl
    {
        DataTable DSupport; 
        public SqlDataAdapter sqlDataAdapter;
        public DataTable LDT;
        public UserOrderList()
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
            button6.Enabled = true;
            button7.Enabled = true;
            switch (index)
            {
                case 1://新增狀態
                    button6.Enabled = false;
                    button7.Enabled = false;
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [Od_No],[Cdate] ,case [UserType] when '0' then '進貨' when '1' then '進貨退回' when '2' then '銷貨' when '3' then '銷退' when '4' then '領料' when '5' then '退料' else '' end [UserType] FROM [OrderProductM]", conn1);
            cmd1.Connection.Open();
            if (textBox2.Text.Length > 0)
            {
                cmd1.CommandText = $"SELECT [Od_No],[Cdate] ,case [UserType] when '0' then '進貨' when '1' then '進貨退回' when '2' then '銷貨' when '3' then '銷退' when '4' then '領料' when '5' then '退料' else '' end [UserType] FROM [OrderProductM] where [Od_No]='{textBox2.Text}'";
            }
            SqlDataReader sdr = cmd1.ExecuteReader();
            DataTable tmpdt = new DataTable();
            tmpdt.Load(sdr);
            dataGridView2.DataSource = tmpdt;
            if (tmpdt.Rows.Count > 0)
            {
                textBox3.Text = tmpdt.Rows[0]["Od_No"].ToString();
            }
            else
            {
                textBox3.Text = "";
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                string quty = DSupport.Rows[e.RowIndex][e.ColumnIndex].ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count>0 && DSupport.Rows.Count > 0)
            {
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                foreach (DataRow dr in DSupport.Rows)
                {
                    WarehouseTools wt = new WarehouseTools();
                    wt.UserType = "0";
                    wt.Userid = MainForm.UserId;
                    wt.MB001 = dr["MB001"].ToString();
                    wt.Quty = Convert.ToInt32(dr["Quty"]);
                    wt.Pt_Name = dr["Pt_Name"].ToString();
                    wt.Pt_type = dr["Pt_type"].ToString();
                    wt.Pt_Unit = dr["Pt_Unit"].ToString();
                    wt.CheckQuty = Convert.ToInt32(dr["CheckQuty"]);
                    wt.CheckBack = Convert.ToInt32(dr["CheckBack"]);
                    if (wt.SetWarehouse())
                    {
                        cmd1.CommandText = $"UPDATE [OrderProuductT] SET [CheckQuty] ={ Convert.ToInt32(dr["CheckQuty"])},[CheckBack] = {Convert.ToInt32(dr["CheckBack"])} WHERE [Od_No]='{Convert.ToInt32(dr["Od_No"])}' and [PtNo]='{dr["MB001"]}'";
                        cmd1.ExecuteNonQuery();
                    }
                }
            }
        }

        private void UserOrderList_Load(object sender, EventArgs e)
        {
            DSupport = new DataTable();

            DataColumn dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "Od_No";
            dataColumn.AllowDBNull = false;
            DSupport.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB001";
            dataColumn.AllowDBNull = false;
            DSupport.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB002";
            dataColumn.AllowDBNull = false;
            DSupport.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB003";
            dataColumn.AllowDBNull = false;
            DSupport.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB004";
            DSupport.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(decimal);
            dataColumn.ColumnName = "Quty";
            DSupport.Columns.Add(dataColumn);
            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(decimal);
            dataColumn.ColumnName = "CheckQuty";
            DSupport.Columns.Add(dataColumn);
            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(decimal);
            dataColumn.ColumnName = "CheckBack";
            DSupport.Columns.Add(dataColumn);
            bindingSource1.DataSource = DSupport;
            dataGridView1.DataSource = bindingSource1;
            Change(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "" && textBox3.Text != "" && lbProductName.Text != "" && numericUpDown1.Value > 0 )
            {
                WarehouseTools wt = new WarehouseTools();
                wt.Od_No = textBox3.Text;
                wt.MB001 = textBox1.Text.Trim();
                wt.Quty = Convert.ToInt32(numericUpDown1.Value);
                wt.Userid = MainForm.UserId;
                wt.Pt_Name =lbProductName.Text;
                wt.Pt_type =lbM004.Text;
                wt.Pt_Unit =lbM003.Text;
                wt.CheckQuty = Convert.ToInt32(numericUpDown1.Value);
                wt.CheckBack = Convert.ToInt32(numericUpDown1.Value);
         
                //UserType=0.進貨 1.進貨退回 2.銷貨 3.銷退 4.領料 5.領料退回
                wt.UserType = "0";
                if (wt.SetWarehouse())
                {
                    lbM003.Text = "";
                    lbM004.Text = "";
                    lbProductName.Text = "";
                    textBox1.Text = "";
                    numericUpDown1.Value = 0;
                }
                else { MessageBox.Show("存檔失敗!!!!"); }
            }
            else MessageBox.Show("部分欄位未填寫!!!!");
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "")
            {
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand($"SELECT [MB001],[MB002],[MB003],[MB004] FROM [Products] where MB001='{textBox1.Text.Trim()}'", conn1);
                cmd1.Connection.Open();
                SqlDataReader sdr1 = cmd1.ExecuteReader();
                if (sdr1.Read())
                {
                    lbProductName.Text = sdr1["MB002"].ToString();
                    lbM003.Text = sdr1["MB003"].ToString();
                    lbM004.Text = sdr1["MB004"].ToString();
                }
                else
                {
                    lbProductName.Text = "";
                    lbM003.Text = "";
                    lbM004.Text = "";
                }
            }
        }
        private string GetNumCode()
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
            return cmd.ExecuteScalar().ToString();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            lbM003.Text = "";
            lbM004.Text = "";
            lbProductName.Text = "";
           
            textBox1.Text = "";
            numericUpDown1.Value = 0;
            tabPage2.Text = "新增入庫";
            textBox3.Text = GetNumCode();
            ButtonChange(1);
            Change(2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            Change(1);
        }

        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView2==null || dataGridView2.Rows.Count<1 || button7.Enabled==false) { return; }
            DataGridViewRow dgv = dataGridView2.Rows[e.RowIndex];
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT [Od_No],[PtNo] MB001,[Pt_Name] MB002,[Pt_type] MB004,[Pt_Unit] MB003,[Quty], [CheckQuty], [CheckBack] FROM [OrderProuductT] where [Od_No]='{dgv.Cells["Od_No1"].Value.ToString()}'", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            DSupport = new DataTable();
            DSupport.Load(sdr);
            dataGridView1.DataSource = DSupport;
            if (DSupport.Rows.Count > 0)
            {
                textBox3.Text = DSupport.Rows[0]["Od_No"].ToString();
            }
            
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand com1 = new SqlCommand($"UPDATE [OrderProuductT] SET [CheckQuty] ={DSupport.Rows[e.RowIndex]["CheckQuty"]},[CheckBack] ={DSupport.Rows[e.RowIndex]["CheckBack"]} WHERE [Od_No]='{textBox3.Text}' AND [PtNo]='{DSupport.Rows[e.RowIndex]["MB001"]}'",conn1);
            com1.Connection.Open();
            com1.ExecuteNonQuery();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string msg = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");//指明非商业应用
                string filePath = openFileDialog1.FileName;
                FileInfo existingFile = new FileInfo(filePath);
                if (!existingFile.Exists)
                {
                    MessageBox.Show("檔案異常!!!");
                    return;
                }

                List<int> errorlist = new List<int>();
                try
                {
                    using (ExcelPackage package = new ExcelPackage(existingFile))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.End.Row;
                        int colCount = worksheet.Dimension.End.Column;
                        List<WarehouseTools> info = new List<WarehouseTools>();
                        for (int row = 2; row <= rowCount; row++)
                        {
                            string Od_No="", PtNo = "", Pt_Name = "", Pt_type = "", Pt_Unit = "";
                            int Quty=0;
                            /*for (int col = 1; col <= colCount; col++)
                            {
                                object cellValue = worksheet.Cells[row, col].Value;
                                string displayValue = cellValue?.ToString().Trim() ?? string.Empty;
                            }*/
                            try
                            {
                                object cellValue = worksheet.Cells[row, 1].Value;
                                PtNo = cellValue?.ToString().Trim() ?? string.Empty;
                                cellValue = worksheet.Cells[row, 2].Value;
                                Pt_Name = cellValue?.ToString().Trim() ?? string.Empty;
                                cellValue = worksheet.Cells[row, 3].Value;
                                Pt_Unit = cellValue?.ToString().Trim() ?? string.Empty;
                                cellValue = worksheet.Cells[row, 4].Value;
                                Pt_type = cellValue?.ToString().Trim() ?? string.Empty;
                                cellValue = worksheet.Cells[row, 5].Value;
                                Quty = Convert.ToInt32(cellValue);
                                Od_No = GetNumCode();
                               
                                WarehouseTools wt = new WarehouseTools();
                                wt.Od_No = Od_No;
                                wt.MB001 = PtNo;
                                wt.Quty = Quty;
                                wt.Userid = MainForm.UserId;
                                wt.Pt_Name = Pt_Name;
                                wt.Pt_type = Pt_type;
                                wt.Pt_Unit = Pt_Unit;
                                wt.CheckQuty = wt.Quty;
                                wt.CheckBack = wt.Quty;
                                //UserType=0.進貨 1.進貨退回 2.銷貨 3.銷退 4.領料 5.領料退回
                                wt.UserType = "0";
                                info.Add(wt);
                            }
                            catch
                            {
                                errorlist.Add(row);
                                break;
                            }
                        }
                        if (errorlist.Count > 0)
                        {
                            for (int i = 0; i < errorlist.Count; i++)
                            {
                                msg += $"第{errorlist[i]}行資料異常錯誤!!!;{Environment.NewLine}";
                            }
                            MessageBox.Show(msg);
                            return;
                        }
                        else
                        {
                            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                            SqlCommand cmd1 = new SqlCommand("", conn1);
                            cmd1.Connection.Open();
                            foreach (WarehouseTools wt in info)
                            {
                                cmd1.CommandText = $"if (SELECT count([MB001]) FROM [Products] where [MB001]='{wt.MB001}')=0 INSERT INTO Products([MB001],[MB002],[MB003],[MB004],[MB051],[MB064],[CostPrice],[GpSno]) ";
                                cmd1.CommandText += $"VALUES('{wt.MB001}','{wt.Pt_Name}','{wt.Pt_Unit}','{wt.Pt_type}',0,0,0,'')";
                                cmd1.ExecuteNonQuery();
                               
                                if (wt.SetWarehouse())
                                {

                                }                               
                            }
                            cmd1.Connection.Close();
                        }
                    }
                }
                catch { }
            }
        }

    }

}
