using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WTools.MigPrint;
using static WTools.MigPrint.MIG4.F0501;

namespace WTools
{
    public partial class invoice : Form
    {
        public string msg = "";
        public invoice()
        {
            InitializeComponent();
        }
        List<string> list = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length > 0 && textBox1.Text.Trim().Length == 10)
            {
                //msg = textBox1.Text.Trim();
                //刪除交易資料
                DataTable dt = new DataTable();

                string sql = "SELECT [MB001],[Quty],[Cdate] FROM [TSales] inner join [MSales] on [MSales].Sno=[TSales].Sno WHERE [MSales].Sno='" + textBox1.Text.Trim() + "' and [MSales].Isok='1'";
                SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Connection.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                    SqlCommand cmd1 = new SqlCommand("", conn1);
                    cmd1.Connection.Open();
                    SqlTransaction sqlTransaction=null;
                    sqlTransaction = conn1.BeginTransaction();
                    cmd1.Transaction = sqlTransaction;
                    dt.Load(sdr);
                    try
                    {
                        //還原庫存量
                        foreach (DataRow item in dt.Rows)
                        {
                            cmd1.CommandText = "Update [Products] SET [MB064]=[MB064]+" + item[1].ToString() + " where [MB001]='" + item[0].ToString() + "'";
                            cmd1.CommandText += "INSERT INTO [PDList]([userid],[MB001],[Quty]) values('CancelBack','" + item[0].ToString() + "'," + item[1].ToString() + ");";
                            if (cmd1.ExecuteNonQuery() > 0)
                            {
                                list.Add($"{item[0].ToString().Trim()}:{item[1].ToString().Trim()}");
                            }
                        }
                        //廢除發票
                        cmd1.CommandText = "Update MSales SET [Isok]='0' WHERE Sno='" + textBox1.Text.Trim() + "'";
                        cmd1.ExecuteNonQuery();
                        sqlTransaction.Commit();
                        /*
                         * 電子發票XML上傳檔
                        MigXml.ERP_F0501ToXmls("MT43487100", "TEST...");
                        
                        CancelInvoice data = new CancelInvoice();
                        data.CancelInvoiceNumber = textBox1.Text.Trim();
                        data.InvoiceDate =Convert.ToDateTime(dt.Rows[0]["Cdate"]).ToString("yyyyMMdd");
                        data.BuyerID = "0000000000";
                        data.SellerID = MigCommon.SellerId;
                        data.CancelDate = DateTime.Today.ToString("yyyyMMdd");
                        data.CancelTime = DateTime.Now.ToString("HH:mm:ss");
                        data.CancelReason = "發票號碼";
                        MigXml.POS_F0501ToXmls(data);
                        */
                        this.Close();

                    } catch { 
                        if(sqlTransaction != null)
                        {
                            sqlTransaction.Rollback();
                            msg = "發票作廢失敗!!!!";
                        }
                    }
                }
                else
                {
                    msg = "查無發票號碼交易紀錄!!!";
                }               
            }
            else
            {
                msg = "輸入發票號碼錯誤!!!";
            }
            if (msg != "")
            {
                MessageBox.Show(msg);
            }
        }
        public List<string> GetMsg()
        {
            return list;
        }
    }
}
