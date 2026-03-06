using QRCoder;
using ScottPlot.Colormaps;
using ScottPlot.Hatches;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using WTools.MigPrint;
using WTools.SaleOrder;
using WTools.warehouse;
using static QRCoder.PayloadGenerator.SwissQrCode;
using static WTools.MigPrint.MIG4.F0401;
using static WTools.SaleOrder.InvoicePrint;

namespace WTools.PostDesk
{
    public partial class UserClientDesk : UserControl
    {
        public static DataTable Discounts,CheckDt,RealDiscount;
        Button[] TmpButton = new Button[3];
        public UserClientDesk()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            invoiceClear();
        }

        private void invoiceClear()
        {
            if (MainForm.DTsale != null && MainForm.DTsale.Rows.Count > 0) MainForm.DTsale.Rows.Clear();
            textBox4.Text = "0";
            textBox3.Text = "0";
            textBox6.Text = "0";
            textBox7.Text = "0";
            textBox1.Text = "";
            textBox5.Text = "";
            numericUpDown1.Text = "0";
            button5.Enabled = false;
            listBox1.Items.Clear();
            textBox2.Focus();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            invoice inv = new invoice();
            inv.ShowDialog();
            List<string> msg = inv.GetMsg();
            if (msg.Count > 0)
            {
                invoiceClear();
            }
        }
       
        private int ItemsDiscount(DataTable dt)
        {
            int discount = 0;
            listBox1.Items.Clear();
            RealDiscount.Rows.Clear();
            foreach (DataRow dr in Discounts.Rows)
            {
                string gpid = dr["GpSno"].ToString();
                string GpName = dr["GpName"].ToString();
                int tmpdiscount = 0;
                int counts=0,Quty = 0, Price = 0;
                string ptname = "";
                foreach (DataRow dr1 in dt.Rows)
                {
                    if (gpid == dr1["GpSno"].ToString())
                    {
                        Quty += Convert.ToInt16(dr1["MB064"]);
                        Price += Convert.ToInt16(dr1["MB064"]) * Convert.ToInt16(dr1["MB051"]);
                        ptname = dr1["MB002"].ToString();
                    }
                }
                //判定量Quty
                if (Convert.ToInt16(dr[1]) > 0 && Convert.ToInt16(dr[2]) == 0)
                {
                    //折扣數量
                    int LimitQuty = Convert.ToInt16(dr[1]);
                    counts = Quty / LimitQuty;

                    //判斷現金折扣折數折扣
                    if (counts > 0)
                    {
                        //現金折扣
                        if (Convert.ToInt16(dr["SubMoney"]) > 0)
                        {
                            tmpdiscount = Convert.ToInt16(dr["SubMoney"]) * counts;
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
                    DataRow rdr=RealDiscount.NewRow();
                    rdr["ID"] = gpid;//折扣代碼
                    rdr["SubMoney"] = dr["SubMoney"];//折扣金額
                    rdr["Quty"] = Convert.ToInt16(dr[1]);//折扣數量
                    rdr["Counts"] = counts;//則扣組數
                    rdr["RelCounts"] = Convert.ToInt16(dr[1])* counts;//需處理筆數，每處理一筆減1直到歸零
                    RealDiscount.Rows.Add(rdr);
                    listBox1.Items.Add($"{ptname} ({GpName})");
                    if (Convert.ToInt16(dr[1]) > 0 && Convert.ToInt16(dr[2]) == 0)
                    {
                        listBox1.Items.Add($"優惠:{dr["SubMoney"]} * {counts}={Convert.ToInt32(dr["SubMoney"]) * counts}");
                    }
                    else if (Convert.ToInt16(dr[2]) > 0 && Convert.ToInt16(dr[1]) == 0)
                    {
                        listBox1.Items.Add($"優惠{dr["SubDiscount"]}:{tmpdiscount}");
                    }
                }
            }
            return discount;
        }
        private void RowsSun()
        {
            int result = 0;
            int discount = 0;
            foreach (DataRow dr in MainForm.DTsale.Rows)
            {
                //合計
                result += Convert.ToInt32(dr[3]);
            }
            //折扣
            discount = ItemsDiscount(MainForm.DTsale);
            textBox6.Text = discount.ToString();
            textBox4.Text = result.ToString();
            textBox7.Text = (result - discount).ToString();
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                DataRow dr = MainForm.DTsale.Rows[e.RowIndex];
                dr[3] = (int)dr[1] * (int)dr[2];
                RowsSun();
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            MainForm.DTsale.Rows.RemoveAt(e.RowIndex);
            RowsSun();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (textBox7.Text.Length > 0 && textBox7.Text.Trim() != "0" && numericUpDown1.Text.Length > 0)
            {
                var tmp = Convert.ToInt32(numericUpDown1.Text) - Convert.ToInt32(textBox7.Text);
                textBox3.Text = tmp.ToString();
                if (tmp < 0) textBox3.ForeColor = Color.Red;
                else textBox3.ForeColor = Color.DeepSkyBlue;
                if (tmp >= 0 && textBox1.Text.Trim().Length == 10) button5.Enabled = true;
                else button5.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool start = true,isok=false;
            //折扣檢查
            if (MainForm.CheckAccounts && CheckOrder() > 0)
            {
                FShowLost showLost = new FShowLost(CheckDt);
                showLost.ShowDialog(this);
                start=showLost.GetResult();
            }
            //是否繼續結帳
            if (start == false) return;
            if (Convert.ToInt32(textBox3.Text) > -1 && textBox1.Text.Trim().Length == 10)
            {
                if (textBox5.Text.Trim().Length > 0 && textBox5.Text.Trim().Length != 8)
                {
                    MessageBox.Show("公司統編開立錯誤!!!!");
                    return;
                }
                //存檔
                if (MainForm.DTsale.Rows.Count > 0)
                {
                    List<SaleOrder.InvoicePrint.Product> products = new List<SaleOrder.InvoicePrint.Product>();
                    SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                    SqlCommand cmd1 = new SqlCommand("", conn1);
                    cmd1.Connection.Open();
                    SqlTransaction sqlTransaction = null;
                    sqlTransaction = conn1.BeginTransaction();
                    cmd1.Transaction = sqlTransaction;
                    int trtype = 0, index = 0;
                    List<ProductItem> details = new List<ProductItem>();
                    if (radioButton1.Checked)
                    {
                        trtype = 0;
                    }
                    if (radioButton2.Checked)
                    {
                        trtype = 1;
                    }
                    foreach (DataRow item in MainForm.DTsale.Rows)
                    {
                        index++;
                        //銷貨明細檔
                        //折扣金額
                        decimal tmpdiscount = 0;
                        if (RealDiscount != null && RealDiscount.Rows.Count > 0)
                        {
                            for (int k = 0; k < RealDiscount.Rows.Count; k++)
                            {
                                if (RealDiscount.Rows[k]["ID"].ToString() == item["GpSno"].ToString())
                                {
                                    //折扣金額
                                    if (Convert.ToInt16(RealDiscount.Rows[k]["RelCounts"]) - Convert.ToInt16(item["MB064"]) > -1)
                                    {
                                        for (int j = 0; j < Convert.ToInt16(item["MB064"]); j++)
                                        {
                                            tmpdiscount += Convert.ToDecimal(RealDiscount.Rows[k]["SubMoney"]) / Convert.ToDecimal(RealDiscount.Rows[k]["Quty"]);
                                            RealDiscount.Rows[k]["RelCounts"] = Convert.ToInt16(RealDiscount.Rows[k]["RelCounts"]) - 1;
                                        }
                                    }
                                }
                            }
                        }
                        /*電子發票XML上傳檔*/
                        if (MainForm.CheckMig)
                        {
                            ProductItem tmp = new ProductItem();
                            tmp.Description = item["MB002"].ToString();
                            tmp.Quantity = Convert.ToDecimal(item["MB064"]);
                            tmp.UnitPrice = Convert.ToDecimal(item["MB051"]) - tmpdiscount;
                            tmp.TaxType = "1";
                            tmp.Amount = Convert.ToDecimal(item["Total"]) - tmpdiscount;
                            tmp.SequenceNumber = index.ToString();
                            details.Add(tmp);
                        }
                        //銷貨紀錄檔
                        cmd1.CommandText = $"INSERT INTO [TSales] ([Sno],[MB001],[Quty],[Price],[Tprice],[Gp],[Discount]) VALUES('{textBox1.Text.Trim()}','{item["MB001"]}',{item["MB064"]},{item["MB051"]},{item["Total"]},{item["Gp"]},{tmpdiscount});";
                        //交易進出紀錄檔
                        cmd1.CommandText += $"INSERT INTO [PDList]([userid],[MB001],[Quty],[InOut]) values('{textBox1.Text.Trim()}','{item["MB001"]}',{item["MB064"]},0);";
                        //產品庫存檔
                        cmd1.CommandText += $"UPDATE [Products] set MB064=MB064-{item["MB064"]} where MB001='{item["MB001"]}';";
                        cmd1.ExecuteNonQuery();
                    }
                    //銷貨主檔
                    cmd1.CommandText = $"INSERT INTO [MSales] ([Sno],[Price],[Scode],[Discount],[TrType]) VALUES('{textBox1.Text.Trim()}',{textBox4.Text.Trim()},'{textBox5.Text.Trim()}',{textBox6.Text.Trim()},{trtype})";
                    cmd1.ExecuteNonQuery();
                    //變更當前發票檔
                    cmd1.CommandText = $"UPDATE [OtherConfigs] SET [F4]='{textBox1.Text.Substring(0, 2) + (Convert.ToInt32(textBox1.Text.Substring(2, 8)) + 1).ToString()}' where [FSno]=1";
                    cmd1.ExecuteNonQuery();
                    try
                    {
                        sqlTransaction.Commit();
                        isok = true;                       
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
                    if (isok)
                    {
                        //列印發票
                        if (MainForm.CheckMig)
                        {
                            //列印發票
                            InvoicePrinting(textBox1.Text.Trim());
                            //電子發票XML上傳檔

                            Main tmpmain=new Main();
                            tmpmain.InvoiceNumber = textBox1.Text.Trim();
                            tmpmain.InvoiceDate = DateTime.Today.ToString("yyyy-MM-dd");
                            tmpmain.InvoiceTime = DateTime.Now.ToString("HH:mm:ss");
                            tmpmain.Seller = MigCommon.MyBan();
                            tmpmain.Buyer = MigCommon.NoOneBan();
                            tmpmain.InvoiceType = "07";
                            tmpmain.DonateMark = "0";
                            tmpmain.PrintMark = "Y";

                            Amount amount = new Amount();
                            amount.SalesAmount = Convert.ToDecimal(textBox7.Text.Trim());
                            amount.FreeTaxSalesAmount = 0;
                            amount.ZeroTaxSalesAmount = 0;
                            amount.TaxAmount = 0;
                            amount.TaxType = "1";
                            amount.TaxRate = Convert.ToDecimal(0.05);
                            amount.TotalAmount = Convert.ToDecimal(textBox7.Text.Trim());

                            MsgResult result = new MsgResult();
                            result.data = new Invoice();
                            result.data.Details = details;
                            result.data.Amount = amount;
                            result.data.Main=tmpmain;
                            MigXml.F0401ToXmls(result.data);
                            
                        }
                        //清檔
                        invoiceClear();
                        button5.Enabled = false;
                        //檢查庫存量
                        if (MainForm.QutyNoError)
                        {
                            DataTable temdt = MainForm.CheckProduct();
                            if (temdt != null && temdt.Rows.Count > 0)
                            {
                                FCheckProduct fCheckProduct = new FCheckProduct(temdt);
                                fCheckProduct.ShowDialog();
                            }
                        }
                    }
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
            if (textBox4.Text.Trim() != "" && textBox4.Text.Trim() != "0")
            {
                if (numericUpDown1.Text == "") numericUpDown1.Text = "0";
                numericUpDown1.Text = (Convert.ToInt32(numericUpDown1.Text) + Convert.ToDecimal(((Button)sender).Text)).ToString();
            }
        }
        private void TmpButtonConfig(int i)
        {
            
            if (TmpButton[i].BackColor == Color.LightGreen)
            {
                if (MainForm.DTsale.Rows.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt = MainForm.DTsale.Copy();
                    MainForm.TmpsTable[0] = dt;
                    TmpButton[i].BackColor = Color.OrangeRed;
                    invoiceClear();
                }
            }
            else if (TmpButton[i].BackColor == Color.OrangeRed)
            {
                if (MainForm.TmpsTable[0].Rows.Count > 0)
                {
                    MainForm.DTsale.Rows.Clear();
                    foreach (DataRow row in MainForm.TmpsTable[0].Rows)
                    {
                        DataRow dr = MainForm.DTsale.NewRow();
                        for (int j = 0; j < 7; j++)
                        {
                            dr[j] = row[j];
                        }
                        MainForm.DTsale.Rows.Add(dr);
                    }
                    MainForm.TmpsTable[0].Rows.Clear();
                    RowsSun();
                    //取得發票編號
                    SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                    SqlCommand cmd1 = new SqlCommand("SELECT [F4] FROM [OtherConfigs] where [FSno]=1", conn1);
                    cmd1.Connection.Open();
                    textBox1.Text = cmd1.ExecuteScalar().ToString();
                    cmd1.Connection.Close();
                }
                TmpButton[i].BackColor = Color.LightGreen;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            TmpButtonConfig(0);
        }

        private DataRow GetProductRow(string ptid)
        {
            DataRow dr=null;
            string sql = $"SELECT [MB001],[MB002],[MB004],[MB064],[MB051],Gp,[GpSno] from(SELECT [MB001],[MB002],[MB004],[MB064],[MB051],0 Gp,[GpSno] FROM [Products] union SELECT  GpId [MB001],[GpName],'',99,[GpPrice],1,'' FROM [GpProductM]) c WHERE [MB001]='{ptid}'";
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            if (dt != null && dt.Rows.Count > 0)
            {
                dr = dt.Rows[0];
            }
            return dr;
        }
        private void TB001_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                DataRow dr1 = GetProductRow(textBox2.Text.Trim());
           
                if (dr1 !=null)
                {
                    textBox2.Text = "";
                    textBox2.Focus();
                    DataRow dr2 = MainForm.DTsale.NewRow();
                    //MB002,MB051,MB064,Total,MB001,Gp,GpSno
                    dr2[0] = dr1[1];
                    dr2[4] = dr1[0];
                    dr2[1] = dr1[4];
                    dr2[5] = dr1[5];
                    dr2[6] = dr1[6];
                    if (MainForm.DTsale.Rows.Count == 0)
                    {
                        //取得發票編號
                        SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                        SqlCommand cmd1 = new SqlCommand("SELECT [F4] FROM [OtherConfigs] where [FSno]=1", conn1);
                        cmd1.Connection.Open();
                        try
                        {
                            textBox1.Text = cmd1.ExecuteScalar().ToString();
                        }
                        catch
                        {
                            MessageBox.Show("發票設定異常!!!!");
                            return;
                        }
                        //string NO2=NO1.Substring(6,4);
                    }
                    dr2[2] = 1;
                    dr2[3] = 1 * Convert.ToInt32(dr1[4]);
                    MainForm.DTsale.Rows.Add(dr2);
                    RowsSun();
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
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
            TmpButton[0] = button11;
            TmpButton[1] = button22;
            TmpButton[2] = button23;
            button1.Visible = MainForm.CheckAccounts;
            Discounts = new DataTable();
            for (int i = 0; i < 3; i++)
            {
                if (MainForm.TmpsTable[i] == null || MainForm.TmpsTable[i].Rows.Count == 0) TmpButton[i].BackColor = Color.LightGreen;
                else TmpButton[i].BackColor = Color.OrangeRed;
            }

            textBox2.Focus();
            
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand("SELECT [GpSno],[Quty],[Price],[SubMoney],[SubDiscount],[GpName] FROM [DiscountRule] where GETDATE() between [StDate] and [EdDate]", conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.HasRows)
            { 
                Discounts.Load(sdr);
                sdr.Close();
            }
            dataGridView2.DataSource = MainForm.DTsale;

            //CheckDt 檢查不足優惠折扣方案
            CheckDt=new DataTable();
            DataColumn dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB001";
            CheckDt.Columns.Add(dataColumn1);

            dataColumn1 =new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "MB002";
            CheckDt.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "MB064";
            dataColumn1.ReadOnly = false;
            CheckDt.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "MB051";
            CheckDt.Columns.Add(dataColumn1);

            //RealDiscount 實際優惠折扣
            RealDiscount = new DataTable();
            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(string);
            dataColumn1.ColumnName = "ID";
            RealDiscount.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "SubMoney";
            RealDiscount.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "Quty";
            RealDiscount.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "Counts";
            RealDiscount.Columns.Add(dataColumn1);

            dataColumn1 = new DataColumn();
            dataColumn1.AllowDBNull = false;
            dataColumn1.DataType = typeof(int);
            dataColumn1.ColumnName = "RelCounts";
            RealDiscount.Columns.Add(dataColumn1);

            //切換功能重計金額
            if (MainForm.DTsale.Rows.Count > 0)
            {
                RowsSun();
            }
            //檢查庫存量
            if (MainForm.QutyNoError)
            {
                DataTable temdt = new DataTable();
                temdt = MainForm.CheckProduct();
                if (temdt != null && temdt.Rows.Count > 0)
                {
                    FCheckProduct fCheckProduct = new FCheckProduct(temdt);
                    fCheckProduct.ShowDialog();
                }
            }
        }

        private int CheckOrder()
        {
            CheckDt.Rows.Clear();
            foreach (DataRow dr in Discounts.Rows)
            {
                string gpid = dr["GpSno"].ToString();
                string GpName = dr["GpName"].ToString();
                string Ptname = "";//產品名稱
                int tmpdiscount = 0;
                int counts = 0, Quty = 0, Price = 0;
                foreach (DataRow dr1 in MainForm.DTsale.Rows)
                {
                    if (gpid == dr1["GpSno"].ToString())
                    {
                        Quty += Convert.ToInt16(dr1["MB064"]);
                        Price += Convert.ToInt16(dr1["MB064"]) * Convert.ToInt16(dr1["MB051"]);
                        Ptname = dr1["MB002"].ToString();
                    }
                }
                //判定量Quty
                if (Convert.ToInt16(dr[1]) > 0 && Convert.ToInt16(dr[2]) == 0)
                {
                    //折扣數量
                    int LimitQuty = Convert.ToInt16(dr[1]);
                    counts = Quty % LimitQuty;

                    //判斷現金折扣折數折扣
                    if (counts > 0)
                    {
                        //現金折扣
                        if (Convert.ToInt16(dr["SubMoney"]) > 0)
                        {
                            DataRow tmpdr = CheckDt.NewRow();
                            tmpdr[0] = Ptname;//產品名稱
                            tmpdr[1] = GpName;//專案名稱
                            tmpdr[2] = LimitQuty - counts;//缺少數量
                            tmpdr[3] = Convert.ToInt16(dr["SubMoney"]);//優惠金額
                            CheckDt.Rows.Add(tmpdr);
                        }
                        else
                        {
                            //折數折扣
                            if (Convert.ToInt16(dr["SubDiscount"]) > 0)
                            {
                                DataRow tmpdr = CheckDt.NewRow();
                                tmpdr[0] = Ptname;//產品名稱
                                tmpdr[1] = GpName;
                                tmpdr[2] = LimitQuty - counts;//缺少數量
                                tmpdr[3] = Convert.ToInt32(Convert.ToDecimal(dr["SubDiscount"]) % 100 * Convert.ToDecimal(Price));//優惠折扣
                                CheckDt.Rows.Add(tmpdr);
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
                    if (Price < LimitPrice)
                    {
                        //現金折扣
                        if (Convert.ToInt16(dr["SubMoney"]) > 0)
                        {
                            tmpdiscount = Convert.ToInt16(dr["SubMoney"]);
                            DataRow tmpdr = CheckDt.NewRow();
                            tmpdr[0] = Ptname;
                            tmpdr[1] = GpName;
                            tmpdr[2] = LimitPrice - Price;//缺少金額
                            tmpdr[3] = tmpdiscount;
                            CheckDt.Rows.Add(tmpdr);
                        }
                        else
                        {
                            //折數折扣
                            if (Convert.ToInt16(dr["SubDiscount"]) > 0)
                            {
                                DataRow tmpdr = CheckDt.NewRow();
                                tmpdr[0] = Ptname;
                                tmpdr[1] = GpName;
                                tmpdr[2] = LimitPrice - Price;//缺少金額
                                tmpdr[3] = Convert.ToInt32(Convert.ToDecimal(dr["SubDiscount"]) % 100 * Convert.ToDecimal(Price));
                                CheckDt.Rows.Add(tmpdr);
                            }
                        }
                    }
                }
            }
            return CheckDt.Rows.Count;
        }

        private void button1_Click(object sender, EventArgs e)
        {           
            if (MainForm.CheckAccounts && CheckOrder() > 0 )
            {
                FShowLost showLost = new FShowLost(CheckDt);
                showLost.ShowDialog(this);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            TmpButtonConfig(1);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            TmpButtonConfig(2);
        }
        /*
         * 電子發票上傳
        private void button24_Click(object sender, EventArgs e)
        {
            MigXml.F0401XML("MT43487100");
            MigXml.F0501ToXmls("MT43487100","TEST...");
        }
        */
        //發票列印
        private void InvoicePrint2(string InviceNo)
        {
            int Y = DateTime.Now.Year - 1911;
            int M = DateTime.Now.Month;
            int D = DateTime.Now.Day;
            List<SaleOrder.InvoicePrint.Product> products = new List<SaleOrder.InvoicePrint.Product>();
            SaleOrder.InvoicePrint invoice = new SaleOrder.InvoicePrint();
            invoice.MastQrcode = new InvoiceData();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT (SELECT TOP (1) [MB002] FROM [Products] where [MB001]=a.[MB001]) [Name],sum([Quty]) [Quty],sum([Tprice]-[Discount]) [Price] FROM [TSales] a where [Sno]='{InviceNo}' group by [MB001]", conn1);
            cmd1.Connection.Open();
            SqlDataReader item = cmd1.ExecuteReader();
            while (item.Read())
            {
                SaleOrder.InvoicePrint.Product product = new SaleOrder.InvoicePrint.Product();
                product.Name = item["Name"].ToString();
                product.Qutys = Convert.ToInt16(item["Quty"]);
                product.Price = Convert.ToInt16(item["Price"]);
                products.Add(product);
            }
            invoice.MastQrcode.QRCode2 = products;

            if (Convert.ToInt16(M) % 2 == 0)
            {
                invoice.MastQrcode.InvoiceDruin = $"{Y}{M:00}";
            }
            else
            {
                invoice.MastQrcode.InvoiceDruin = $"{Y}{(M + 1):00}";
            }
            invoice.MastQrcode.CopanyName = MainForm.CpInfo[0];
            invoice.MastQrcode.SellerIdentifier = MainForm.CpInfo[1];
            invoice.MastQrcode.BusinessIdentifier = MainForm.CpInfo[1];
            invoice.MastQrcode.InvoiceNumber = textBox1.Text.Trim();
            invoice.MastQrcode.InvoiceDate = Y.ToString() + DateTime.Today.ToString("MMdd");
            invoice.MastQrcode.InvoiceTime = DateTime.Now.ToString("HH:mm:ss");
            invoice.MastQrcode.TotalAmount = Convert.ToDecimal(textBox7.Text.Trim());
            if (textBox5.Text.Trim().Length == 8) { invoice.MastQrcode.BuyerIdentifier = textBox5.Text.Trim(); }
            invoice.MastQrcode.BarCode = invoice.BarCodeINV();
            invoice.MastQrcode.SalesAmount = 0;
            invoice.MastQrcode.TaxAmount = 0;
            invoice.MastQrcode.QRCode1 = invoice.QRCodeINV();
            invoice.MastQrcode.PrintName = MainForm.CpInfo[2];
            invoice.Print();
        }

        private void InvoicePrinting(string InviceNo)
        {
            int Y = DateTime.Now.Year - 1911;
            int M = DateTime.Now.Month;
            int D = DateTime.Now.Day;
            List<WTools.InvoicePrint.Product> products=new List<WTools.InvoicePrint.Product>();
            WTools.InvoicePrint invoice = new WTools.InvoicePrint();
            invoice.MastQrcode = new WTools.InvoicePrint.InvoiceData();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"SELECT (SELECT TOP (1) [MB002] FROM [Products] where [MB001]=a.[MB001]) [Name],sum([Quty]) [Quty],sum([Tprice]-[Discount]) [Price] FROM [TSales] a where [Sno]='{InviceNo}' group by [MB001]", conn1);
            cmd1.Connection.Open();
            SqlDataReader item = cmd1.ExecuteReader();
            while (item.Read())
            {
                WTools.InvoicePrint.Product product = new WTools.InvoicePrint.Product();
                product.Name = item["Name"].ToString();
                product.Qutys = Convert.ToInt16(item["Quty"]);
                product.Price = Convert.ToInt16(item["Price"]);
                products.Add(product);
            }
            if (Convert.ToInt16(M) % 2 == 0) invoice.MastQrcode.InvoiceDruin = $"{Y}{M:00}";
            else invoice.MastQrcode.InvoiceDruin = $"{Y}{(M + 1):00}";
            invoice.MastQrcode.QRCode2 = products;
            invoice.MastQrcode.CopanyName = MainForm.CpInfo[0];
            invoice.MastQrcode.SellerIdentifier = MainForm.CpInfo[1];
            invoice.MastQrcode.BusinessIdentifier=MainForm.CpInfo[1];
            invoice.MastQrcode.InvoiceNumber = textBox1.Text.Trim();
            invoice.MastQrcode.InvoiceDate = Y.ToString()+DateTime.Today.ToString("MMdd");
            invoice.MastQrcode.InvoiceTime = DateTime.Now.ToString("HH:mm:ss");
            invoice.MastQrcode.TotalAmount = Convert.ToDecimal(textBox7.Text.Trim());
            if (textBox5.Text.Trim().Length==8) { invoice.MastQrcode.BuyerIdentifier = textBox5.Text.Trim(); }
            invoice.MastQrcode.BarCode = invoice.BarCodeINV(invoice.MastQrcode.InvoiceDruin, invoice.MastQrcode.InvoiceNumber, invoice.MastQrcode.RandomNumber);
            invoice.MastQrcode.SalesAmount = 0;
            invoice.MastQrcode.TaxAmount = 0;
            invoice.MastQrcode.QRCode1 = invoice.QRCodeINV();         
            invoice.MastQrcode.PrintName=MainForm.CpInfo[2];           
            invoice.Print();
        }
    }
}
