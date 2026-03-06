using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static WTools.InvoicePrint;

namespace WTools
{
    public partial class UserControl2 : UserControl
    {
        public static DataTable DT, DTsale,Discounts;
        public UserControl2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (TB001.Text.Trim().Length > 0 && DT != null && DT.Rows.Count > 0)
            {
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    if (DT.Rows[i][1].ToString().Contains(TB001.Text.Trim()) || DT.Rows[i][0].ToString().Contains(TB001.Text.Trim()))
                    {
                        dataGridView1.CurrentCell = dataGridView1[1, i];
                        //TB001.Text = "";
                        break;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            invoiceClear();
        }

        private void invoiceClear()
        {
            if (DTsale != null && DTsale.Rows.Count>0) DTsale.Rows.Clear();
            textBox4.Text = "0";
            textBox3.Text = "0";
            textBox6.Text = "0";
            textBox7.Text = "0";
            textBox1.Text = "";
            textBox5.Text = "";
            numericUpDown1.Text = "0";
            button5.Enabled = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
           invoice inv = new invoice();
            DialogResult result = inv.ShowDialog();
            if (result == DialogResult.OK)
            {
                List<string> msg = inv.GetMsg();
                if (msg.Count > 0)
                {
                    invoiceClear();
                    foreach (string s in msg)
                    {
                        string[] tmp=s.Split(':');
                        if (tmp.Length == 2)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                if (DT.Rows[i]["MB001"].ToString().Trim() == tmp[0])
                                {
                                    DT.Rows[i]["MB064"] = Convert.ToInt32(DT.Rows[i]["MB064"]) - Convert.ToInt32(tmp[1]);
                                    break;
                                }
                            }
                        }

                    }
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataRow dr1=DT.Rows[e.RowIndex];
            DataRow dr2 = DTsale.NewRow();
            dr2[0]= dr1[1];
            dr2[4]= dr1[0];
            dr2[1]= dr1[4];
            dialogform dl = new dialogform(dr1[1].ToString());
            dl.productName= dr2[0].ToString();
            dl.Text = "出售量";
            DialogResult dr = dl.ShowDialog();
            if (dr == DialogResult.OK)
            {
                dr2[2] = dl.GetMsg();
                dr2[3] = dl.GetMsg()* Convert.ToInt32(dr1[4]);
                DTsale.Rows.Add(dr2);
                RowsSun();
            }
        }

        private int ItemsDiscount(DataTable dt)
        {
            int discount = 0,Quty=0,Price=0;
            listBox1.Items.Clear();
            foreach (DataRow dr in Discounts.Rows)
            {
                string gpid=dr["GpSno"].ToString();
                int tmpdiscount = 0;
                Quty = 0; Price = 0;
                foreach (DataRow dr1 in dt.Rows)
                {
                    if (gpid == dr1["GpSno"].ToString())
                    {
                        Quty += Convert.ToInt16(dr1["MB064"]);
                        Price += Convert.ToInt16(dr1["MB064"]) * Convert.ToInt16(dr1["MB051"]);
                    }
                }
                //判定量Quty
                if (Convert.ToInt16(dr[1]) > 0 && Convert.ToInt16(dr[2])==0)
                {
                    //折扣數量
                    int LimitQuty=Convert.ToInt16(dr[1]);                    
                    int counts=Quty / LimitQuty;

                    //判斷現金折扣折數折扣
                    if (counts > 0)
                    {
                        //現金折扣
                        if (Convert.ToInt16(dr["SubMoney"]) > 0)
                        {
                            tmpdiscount= Convert.ToInt16(dr["SubMoney"]) * counts;
                            discount += tmpdiscount;
                        }
                        else
                        {
                            //折數折扣
                            if (Convert.ToInt16(dr["SubDiscount"]) > 0)
                            {
                                tmpdiscount= Convert.ToInt32(Convert.ToDecimal(dr["SubDiscount"]) % 100 * Convert.ToDecimal(Price));
                                discount += tmpdiscount;
                            }
                        }
                    }
                }
                //判定金額
                else if (Convert.ToInt16(dr[2]) > 0 && Convert.ToInt16(dr[1]) == 0)
                {
                    //折扣金額
                    int LimitPrice = Convert.ToInt16(dr[2]);
                   
                    //判斷現金折扣折數折扣
                    if (Price >= LimitPrice)
                    {
                        //現金折扣
                        if (Convert.ToInt16(dr["SubMoney"]) > 0)
                        {
                            tmpdiscount = Convert.ToInt16(dr["SubMoney"]);
                            discount += tmpdiscount;
                        }
                        else
                        {
                            //折數折扣
                            if (Convert.ToInt16(dr["SubDiscount"]) > 0)
                            {
                                tmpdiscount = Convert.ToInt32(Convert.ToDecimal(dr["SubDiscount"]) % 100 * Convert.ToDecimal(Price));
                                discount += tmpdiscount;
                            }
                        }
                    }
                }
                if (tmpdiscount > 0)
                {
                    listBox1.Items.Add($"{gpid}折扣優惠:{tmpdiscount}");
                }
            }
            return discount;
        }
        private void RowsSun()
        {
            int result = 0;
            int discount = 0;
            foreach (DataRow dr in DTsale.Rows)
            {
                //合計
                result +=Convert.ToInt32(dr[3]);
            }
            //折扣
            discount=ItemsDiscount(DTsale);
            textBox6.Text = discount.ToString();
            textBox4.Text = result.ToString();
            textBox7.Text = (result - discount).ToString();
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                DataRow dr = DTsale.Rows[e.RowIndex];
                dr[3] = (int)dr[1] * (int)dr[2];
                RowsSun();
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DTsale.Rows.RemoveAt(e.RowIndex);
            RowsSun();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (textBox7.Text.Length > 0 && textBox7.Text.Trim() != "0" && numericUpDown1.Text.Length > 0)
            {
                var tmp= Convert.ToInt32(numericUpDown1.Text)-Convert.ToInt32(textBox7.Text);
                textBox3.Text = tmp.ToString();
                if (tmp < 0) textBox3.ForeColor = Color.Red;
                else textBox3.ForeColor = Color.DeepSkyBlue;
                if (tmp >= 0 && textBox1.Text.Trim().Length == 10) button5.Enabled = true;
                else button5.Enabled = false;
            }
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox3.Text) > -1 && textBox1.Text.Trim().Length==10)
            {
                if (textBox5.Text.Trim().Length > 0 && textBox5.Text.Trim().Length != 8)
                {
                    MessageBox.Show("公司統編開立錯誤!!!!");
                    return;
                }
                //存檔
                if (DTsale.Rows.Count>0)
                {
                    SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                    SqlCommand cmd1 = new SqlCommand("", conn1);
                    cmd1.Connection.Open();
                    SqlTransaction sqlTransaction = null;
                    sqlTransaction = conn1.BeginTransaction();
                    cmd1.Transaction = sqlTransaction;
                    try
                    {
                        foreach (DataRow item in DTsale.Rows)
                        {
                            //銷貨明細檔
                            cmd1.CommandText = "INSERT INTO [TSales] ([Sno],[MB001],[Quty],[Price],[Tprice],[Gp]) VALUES('" +textBox1.Text.Trim()+"','" + item[4]+"',"+ item[2].ToString() + "," + item[1].ToString() + "," + item[3].ToString() + "," + item[5].ToString() + ");";
                            //交易進出紀錄檔
                            cmd1.CommandText += "INSERT INTO [PDList]([userid],[MB001],[Quty],[InOut]) values('" + textBox1.Text.Trim() + "','" + item[4] + "',"+ item[2].ToString() + ",0);";
                            //產品檔
                            cmd1.CommandText += "  UPDATE [Products] set MB064=MB064-"+ item[2].ToString()+" where MB001='" + item[4] + "';";

                            if (cmd1.ExecuteNonQuery() > 0)
                            {
                                for(int i = 0; i < DT.Rows.Count; i++)
                                {
                                    if (DT.Rows[i]["MB001"]== item[4])
                                    {
                                        var t1 = DT.Rows[i]["MB064"];
                                        var t2 = item[2];
                                        DT.Rows[i]["MB064"] =Convert.ToInt32(DT.Rows[i]["MB064"]) - Convert.ToInt32(item[2]);
                                        break;
                                    }
                                }
                            }
                        }
                        //銷貨主檔
                        cmd1.CommandText = "INSERT INTO [MSales] ([Sno],[Price],[Scode],[Discount]) VALUES('" + textBox1.Text.Trim() + "',"+ textBox4.Text.Trim() + ",'" + textBox5.Text.Trim() + "',"+ textBox6.Text.Trim()+")";
                        cmd1.ExecuteNonQuery();                    
                        //變更當前發票檔
                        cmd1.CommandText = $"UPDATE [OtherConfigs] SET [F4]='{textBox1.Text.Substring(0,2)+(Convert.ToInt32(textBox1.Text.Substring(2, 8))+1).ToString()}' where [FSno]=1";
                        cmd1.ExecuteNonQuery();
                        sqlTransaction.Commit();
                        //清檔
                        invoiceClear();
                        button5.Enabled = false;
                    }
                    catch
                    {
                        if (sqlTransaction != null)
                        {
                            sqlTransaction.Rollback();
                            MessageBox.Show("交易失敗!!!!");
                            return;
                        }
                    }
                    //廢除發票
                }
                else
                {
                    MessageBox.Show("查無發票號碼交易紀錄!!!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("輸入發票號碼錯誤!!!");
                return;
            }      
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(textBox4.Text.Trim() !="" && textBox4.Text.Trim() != "0")
            {
                if (numericUpDown1.Text == "") numericUpDown1.Text = "0";
                numericUpDown1.Text = (Convert.ToInt32(numericUpDown1.Text) + Convert.ToDecimal(((Button)sender).Text)).ToString();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //寫入資料
            InvoicePrint oPrinter=new InvoicePrint();

            //送出列印
            InvoiceData data = new InvoiceData();
            int year= DateTime.Today.Year-1911;
            string month= DateTime.Today.ToString("MM");
            string day = DateTime.Today.ToString("dd");
            if (Convert.ToInt16(month) % 2 == 0)
            {
                data.InvoiceDruin = $"{year}{month}";
            }
            else
            {                
                data.InvoiceDruin = $"{year}{Convert.ToInt16(month) + 1}";
            }


            data.InvoiceNumber = textBox1.Text;
            data.InvoiceDate = DateTime.Today.ToString($"{year}-MM-dd");
            data.InvoiceTime = DateTime.Now.ToString("HH:MM:ss");
            data.TotalAmount = Convert.ToInt16(textBox4.Text);
            data.BuyerIdentifier = textBox5.Text;
            data.BarCode = oPrinter.BarCodeINV(data.InvoiceDruin, data.InvoiceNumber, data.RandomNumber);
            data.QRCode2 = new List<Product>();
            foreach (DataRow dr in DTsale.Rows)
            {
                Product product = new Product();
                product.Name = dr[0].ToString();
                product.Price = Convert.ToInt16(dr[1]);
                product.Qutys = Convert.ToInt16(dr[2]);
                data.QRCode2.Add(product);
            }
            oPrinter.MastQrcode = data;
            oPrinter.Print();
        }

        private void TB001_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                int isok = -1;
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    string tmp= DT.Rows[i][0].ToString().Trim();
                    if (tmp==textBox2.Text.Trim())
                    {
                        dataGridView1.CurrentCell = dataGridView1[1, i];
                        isok = i;
                        break;
                    }
                }
                if (isok > -1)
                {
                    textBox2.Text = "";
                    DataRow dr1 = DT.Rows[isok];
                    DataRow dr2 = DTsale.NewRow();
                    dr2[0] = dr1[1];
                    dr2[4] = dr1[0];
                    dr2[1] = dr1[4];
                    dr2[5] = dr1[5];
                    dr2[6] = dr1[6];
                    dialogform dl = new dialogform(dr1[1].ToString());
                    dl.Text = "出售量";
                    DialogResult dr = dl.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        if(DTsale.Rows.Count == 0){
                            //取得發票編號
                            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                            SqlCommand cmd1 = new SqlCommand("SELECT [F4] FROM [OtherConfigs] where [FSno]=1", conn1);
                            cmd1.Connection.Open();
                            textBox1.Text=cmd1.ExecuteScalar().ToString();
                            //string NO2=NO1.Substring(6,4);
                        }
                        dr2[2] = dl.GetMsg();
                        dr2[3] = dl.GetMsg() * Convert.ToInt32(dr1[4]);
                        DTsale.Rows.Add(dr2);
                        RowsSun();
                    }
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            if (textBox4.Text.Length > 0 && textBox4.Text.Trim() != "0")
            {
                if (numericUpDown1.Text == "0") { numericUpDown1.Text = ""; }
                numericUpDown1.Text = numericUpDown1.Text + (sender as Button).Text;
            }
        }

        private void UserControl2_Load_1(object sender, EventArgs e)
        {
            DT = new DataTable();
            DataColumn dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB001";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB002";
            dataColumn1.AllowDBNull = false;
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB004";
            dataColumn1.AllowDBNull = false;
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "MB064";
            dataColumn1.ReadOnly = false;
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "MB051";
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "Gp";
            dataColumn1.AllowDBNull = false;
            DT.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "GpSno";
            dataColumn1.AllowDBNull = true;
            DT.Columns.Add(dataColumn1);           

            //sale
            DTsale =new DataTable();
            DataColumn dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB002";           
            DTsale.Columns.Add(dataColumn);
            
            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "MB051";
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "MB064";
            dataColumn.ReadOnly = false;
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "Total";
            dataColumn.ReadOnly = false;
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB001";
            dataColumn.AllowDBNull = false;
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "Gp";
            dataColumn.AllowDBNull = false;
            DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "GpSno";
            dataColumn.AllowDBNull = true;
            DTsale.Columns.Add(dataColumn);

            string sql = "SELECT [MB001],[MB002],[MB004],[MB064],[MB051],0 Gp,[GpSno] FROM [Products] union SELECT [GpId],[GpName],'',99,[GpPrice],1,'' FROM [GpProductM] WHERE 1=1";
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            sql += " order by MB002";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.HasRows)
            {
                DT.Load(sdr);
                sdr.Close();
                cmd.CommandText = "SELECT [GpSno],[Quty],[Price],[SubMoney],[SubDiscount] FROM [DiscountRule] where GETDATE() between [StDate] and [EdDate]";
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    Discounts=new DataTable();
                    Discounts.Load(sdr);
                    sdr.Close ();
                }
            }
            else
            {
                DT = null;
            }
            dataGridView1.DataSource = DT;
            dataGridView2.DataSource = DTsale;

        }
    }
}
