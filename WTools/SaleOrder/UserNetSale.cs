using OfficeOpenXml;
using ScottPlot.MultiplotLayouts;
using ScottPlot.TickGenerators.TimeUnits;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OfficeOpenXml.ExcelErrorValue;
using static WTools.MainForm;

namespace WTools.SaleOrder
{
    public partial class UserNetSale : UserControl
    {
        DataTable dt;
        public class datainfo
        {
            public string od_id { get; set; }
            public string od_state { get; set; }
            public string cus_name { get; set; }
            public string od_date { get; set; }
            public string pt_id { get; set; }
            public int pt_quty { get; set; }
            public int pt_quty_bk { get; set; }
            public decimal pt_price { get; set; }
            public decimal pt_total_price { get; set; }
            
        }
        public class dataM
        {
            public string sno { get; set; }//統一發票
            public string Scode {  get; set; }//買方統編
            public string od_id { get; set; }//單號
            public string cus_name { get; set; }//買家
            public string od_date { get; set; }//訂單日
            public decimal pt_total_price { get; set; }//銷貨額
            public List<dataT> items { get; set; }//銷貨明細

        }
        public class dataT
        {
            public string od_id { get; set; }//單號
            public string pt_id { get; set; }//品號
            public int pt_quty { get; set; }//數量
            public decimal pt_price { get; set; }//單價
            public string unit { get; set; }//單位
            public string pt_name { get; set; }//品名
        }
        public UserNetSale()
        {
            InitializeComponent();
        }

        private void UserNetSale_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 3;
        }
        private bool TmpSqlTable(List<datainfo> info)
        {
            SqlConnection conn = new SqlConnection(MainForm.PosErp);
            SqlCommand cmd = new SqlCommand("truncate table [TmpCusOrder]", conn);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            int index = 1;
            bool isok = true;
            foreach (datainfo item in info)
            {
                cmd.CommandText = "INSERT INTO [TmpCusOrder] ([od_id], [od_state], [cus_name], [od_date], [pt_id], [pt_price], [pt_quty], [pt_quty_bk], [pt_total_price]) VALUES";
                cmd.CommandText += $"('{item.od_id}','{item.od_state}','{item.cus_name}','{item.od_date}','{item.pt_id}',{item.pt_price},{item.pt_quty},{item.pt_quty_bk},{item.pt_total_price})";
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        index++;
                    }
                }
                catch
                {
                    isok = false;
                    MessageBox.Show($"第{index}行資料異常錯誤!!!{ cmd.CommandText }");
                    break;
                }
            }
            cmd.Connection.Close();
            return isok;
        }
        
        //扣除批號數量
        private DataTable subkindnu(string pt_id)
        {
            DataTable dt = new DataTable();
            string cl = "SELECT MF002,sum(MF010*MF008) as MF0108 FROM [INVMF] where MF001 = '"+ pt_id + "'  group by  MF002 having sum(MF010 * MF008) > 0 order by MF002";

            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand(cl, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.HasRows)
            {
                dt.Load(sdr);
            }
            return dt;
        }

        private string GetOrderNumber(string COMPANY)
        {
            int code = 1;
            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand($"SELECT ISNULL(max([TC002]),'') FROM [{COMPANY}].[dbo].[COPTC] where [TC002] like CONVERT(char(8),getdate(),112)+'%'", conn);
            cmd.Connection.Open();
            string pcode = cmd.ExecuteScalar().ToString().Trim();
            if (pcode.Length == 11)
            {
                code = Convert.ToInt16(pcode.Substring(8, 3)) + 1;
            }
            cmd.CommandText = $"SELECT CONVERT(char(8),getdate(),112)+'{String.Format("{0:D3}", code)}'";
            pcode = cmd.ExecuteScalar().ToString();
            cmd.Connection.Close();
            return pcode;
        }
        private string GetSaleNumber(string COMPANY)
        {
            int code = 1;
            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand($"SELECT ISNULL(max([TG002]),'') FROM [{COMPANY}].[dbo].[COPTG] where [TG002] like CONVERT(char(8),getdate(),112)+'%'", conn);
            cmd.Connection.Open();
            string pcode = cmd.ExecuteScalar().ToString().Trim();
            if (pcode.Length == 11)
            {
                code = Convert.ToInt16(pcode.Substring(8, 3)) + 1;
            }
            cmd.CommandText = $"SELECT CONVERT(char(8),getdate(),112)+'{String.Format("{0:D3}", code)}'";
            pcode = cmd.ExecuteScalar().ToString();
            cmd.Connection.Close();
            return pcode;
        }
        private string GetTranscatNumber(string COMPANY)
        {
            int code = 1;
            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand($"SELECT ISNULL(max([LA007]),'') FROM [{COMPANY}].[dbo].[INVLA] where [LA007] like CONVERT(char(8),getdate(),112)+'%'", conn);
            cmd.Connection.Open();
            string pcode = cmd.ExecuteScalar().ToString().Trim();
            if (pcode.Length == 11)
            {
                code = Convert.ToInt16(pcode.Substring(8, 3)) + 1;
            }
            cmd.CommandText = $"SELECT CONVERT(char(8),getdate(),112)+'{String.Format("{0:D3}", code)}'";
            pcode = cmd.ExecuteScalar().ToString();
            cmd.Connection.Close();
            return pcode;
        }
        //檢查商品存在
        private int CheckProductExnd(string pt_id, string COMPANY)
        {
            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand($"SELECT count(*) FROM [{COMPANY}].[dbo].[INVMB] where [MB001]='{pt_id}'", conn);
            cmd.Connection.Open();
            int i= Convert.ToInt16(cmd.ExecuteScalar());
            return i;
        }
        //檢查發票重覆存在
        private int CheckInvoicExnd(string order_id, string COMPANY)
        {
            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand($"SELECT count(*) FROM [{COMPANY}].[dbo].[COPTG] where TG014='{order_id}'", conn);
            cmd.Connection.Open();
            int i = Convert.ToInt16(cmd.ExecuteScalar());
            return i;
        }
        //交易產生ERP銷貨單
        private void exportToerp(dataM row)
        {
            //寫入訂單主檔
            string msg = "";
            List<string> listSno = new List<string>();
            string COMPANY = "", CREATOR = "NetUser", USR_GROUP = "NET", MODIFIER = "NetUser";
            string TH014 = "221", TG004 = "01-0001", TG007 = "零售客戶";
            string TG013 = row.pt_total_price.ToString();
            DateTime dt = Convert.ToDateTime(row.od_date);
            string TG003 = dt.ToString("yyyyMMdd");
            string TG025 = Math.Round(row.pt_total_price * 0.05M).ToString();
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    COMPANY = "LH01";
                    break;
                case 1:
                    COMPANY = "YS";
                    break;
                case 2:
                    COMPANY = "WP01";
                    break;
                case 3:
                    COMPANY = "TEST";
                    break;
            }
            SqlConnection conn1 = new SqlConnection(YScon);
            SqlCommand cmd1 = new SqlCommand();
            //string TH015 = GetOrderNumber(COMPANY);
            string TH002 = GetSaleNumber(COMPANY);
            string LA007 =GetTranscatNumber(COMPANY);
            string TH001 = "236";
            string TG014 = row.od_id;
            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand("", conn);
            cmd.Connection.Open();
            SqlTransaction sqlTransaction = null;
            sqlTransaction = conn.BeginTransaction();
            cmd.Transaction = sqlTransaction;
            int index = 0;
            int qutyCount = 0;
            foreach (dataT item in row.items)
            {
                //--------------訂單
                string TH005 = "", TH009 = "";
                if(conn1.State==ConnectionState.Open) conn1.Close();
                cmd1 = new SqlCommand($"SELECT isnull([MB002],0) [MB002],isnull([MB004],0) [MB004] FROM [{COMPANY}].[dbo].[INVMB] where [MB001]='{item.pt_id.Trim()}'", conn1);
                cmd1.Connection.Open();
                SqlDataReader reader = cmd1.ExecuteReader();
                if (reader.Read())
                {
                    TH005 = reader["MB002"].ToString();
                    TH009 = reader["MB004"].ToString();
                }   
                string TH004 = item.pt_id.Trim(), TH008 = item.pt_quty.ToString(), TH012 = item.pt_price.ToString();
                
                string TH013 = (item.pt_quty * item.pt_price).ToString();
                //取得實際品名

                qutyCount += item.pt_quty;
                //檢查商品存在
               index++;
                string TH003 = String.Format("{0:D3}", index);

                //--------------銷貨單
                TH014 = "";
                string TH017 = "", TH016="", TH015="";
                string TH036 = Convert.ToInt32(Convert.ToDecimal(TH013) * 0.05M).ToString();
                string TH038 = TH036;
                string TH035 =(Convert.ToInt32(Convert.ToDecimal(TH013)) - Convert.ToInt32(TH036)).ToString();
                string TH037 = TH035;
                string sql = $"INSERT INTO [{COMPANY}].[dbo].[COPTH] ([COMPANY], [CREATOR], [USR_GROUP], [CREATE_DATE], [MODIFIER], [MODI_DATE], [CREATE_TIME], [MODI_TIME]";
                sql += ", [TH001], [TH002], [TH003], [TH004], [TH005],[TH008], [TH009],[TH012]";
                sql += ",[TH013],[TH014],[TH015],[TH016],[TH017],[TH035], [TH036], [TH037], [TH038]";
                sql += ", [CREATE_AP], [CREATE_PRID], [MODI_AP], [MODI_PRID], [FLAG]";
                sql += ", [TH007],[TH010],[TH020],[TH021],[TH024],[TH025],[TH026],[TH031] ,[TH042],[TH043],[TH057],[TH099],[TH079])";
                sql += $" VALUES('{COMPANY}','{CREATOR}','{USR_GROUP}', convert(char(8),GETDATE(),112),'{MODIFIER}', convert(char(8),GETDATE(),112),convert(varchar(10), GETDATE(), 108),convert(varchar(10), GETDATE(), 108)";
                sql += $",'{TH001}','{TH002}','{TH003}','{TH004}','{TH005}',{TH008},'{TH009}',{TH012}";
                sql += $",{TH013},'{TH014}','{TH015}','{TH016}','{TH017}',{TH035},{TH036},{TH037},{TH038}";
                sql += $",'WISDOM22','COPI08','WISDOM22','COPI08',2";
                sql += $",'Z',0,'Y','N',0,1,'Y','2',0,0,0,1,0)";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                //----更新庫存
                sql = $"UPDATE [{COMPANY}].[dbo].[INVMB] SET [MB064]=[MB064]-{TH008} where [MB001]='{item.pt_id.Trim()}'";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                //新增交易資料
                string LA001 = TH004, LA004 = TG003, LA008= String.Format("{0:D4}", index), LA011= TH008;
                sql = $"INSERT INTO [{COMPANY}].[dbo].[INVLA]([COMPANY],[CREATOR],[USR_GROUP],[CREATE_DATE],[CREATE_TIME],[CREATE_AP],[CREATE_PRID]";
                sql += ",[LA001],[LA004],[LA007],[LA008],[LA011]";
                sql += ",[FLAG],[LA005],[LA006],[LA012],[LA013],[LA014],[LA015]";
                sql += ",[LA017],[LA018],[LA019],[LA020],[LA021],[LA009]) VALUES(";
                sql += $"'{COMPANY}','{CREATOR}','{USR_GROUP}', convert(char(8),GETDATE(),112),convert(varchar(10), GETDATE(), 108),'WISDOM22','COPI08'";
                sql += $",{LA001},{LA004},{LA007},{LA008},{LA011}";
                sql += ",1,-1,'231',0,0,2,'N'";
                sql += ",0,0,0,0,0,'Z')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            //--------------訂單
            if (index > 0)
            {
                string TG033 = qutyCount.ToString();
               
                //--------------銷貨單
                string TG015="", TG038= TG003.Substring(0, 6), TG045= TG013, TG046= TG025, TG049 = TG007;
                string sqlstring = $"INSERT INTO [{COMPANY}].[dbo].[COPTG]([COMPANY],[CREATOR],[USR_GROUP],[CREATE_DATE],[MODIFIER],[MODI_DATE],[CREATE_TIME],[MODI_TIME]";
                sqlstring += ",[TG001],[TG002],[TG003],[TG004],[TG007],[TG013],[TG014]";
                sqlstring += ",[TG015],[TG025],[TG033],[TG038],[TG042],[TG045],[TG046],[TG049]";
                sqlstring += ",[FLAG],[CREATE_AP],[CREATE_PRID],[MODI_AP],[MODI_PRID]";
                sqlstring += ",[TG016],[TG017],[TG022],[TG023],[TG024],[TG030]";
                sqlstring += ",[TG031],[TG032],[TG034],[TG036],[TG037],[TG041],[TG043]";
                sqlstring += ",[TG044],[TG047],[TG055],[TG056],[TG057],[TG058]";
                sqlstring += ",[TG063],[TG064],[TG068],[TG070],[TG071],[TG072]";
                sqlstring += ",[TG091],[TG092],[TG093],[TG094],[TG010],[TG011],[TG012]) VALUES";
                sqlstring += $"('{COMPANY}','{CREATOR}','{USR_GROUP}', convert(char(8),GETDATE(),112),'{MODIFIER}', convert(char(8),GETDATE(),112),convert(varchar(10), GETDATE(), 108),convert(varchar(10), GETDATE(), 108)";
                sqlstring += $",'{TH001}','{TH002}','{TG003}','{TG004}','{TG007}',{TG013},'{TG014}'";
                sqlstring +=$",'{TG015}',{TG025},{TG033},'{TG038}',convert(char(8),GETDATE(),112),{TG045},{TG046},'{TG049}'"; 
                sqlstring += ",2,'WISDOM22','COPI08','WISDOM22','COPI08'";
                sqlstring += ",1,1,0,'Y','N','N'";
                sqlstring += $",1,0,'Y','N','N',0,'{CREATOR}'";
                sqlstring += ",0.05,'N',0,1,0,0";
                sqlstring += ",0,0,'1',0,0,'N'";
                sqlstring += ",0,0,0,0,'001','NTD',1)";
                cmd.CommandText = sqlstring;
                cmd.ExecuteNonQuery();
                try
                {
                    sqlTransaction.Commit();
                }
                catch
                {
                    if (sqlTransaction != null)
                    {
                        sqlTransaction.Rollback();
                        msg = row.sno;
                        MessageBox.Show($"{msg}匯入失敗!!!!");
                    }
                }
            }
        }

        //交易產生ERP訂單
        private void exportToerpOrder(dataM row)
        {
            //寫入訂單主檔
            string msg = "";
            List<string> listSno = new List<string>();
            string TC016 = "1";// --1.內含,2.外加,\r\n
            string COMPANY = "", CREATOR = "NetUser", USR_GROUP = "NET", MODIFIER = "NetUser";
            string TC001 = "221", TC004 = "01-0001", TC031 = "", TC032 = "01-0001", TC040 = "NetUser";
            string TC043 = "零售客戶", TC046 = "零售客戶";
            string TC029 = row.pt_total_price.ToString();
            DateTime dt = Convert.ToDateTime(row.od_date);
            string TC003 = dt.ToString("yyyyMMdd");
            string TD038 = dt.ToString("yyyyMMddHHmmss");
            string TC030 = (row.pt_total_price * 0.05M).ToString();
            string TC039 = TC003;
            string TC012 = row.od_id.ToString();
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    COMPANY = "LH01";
                    break;
                case 1:
                    COMPANY = "YS";
                    break;
                case 2:
                    COMPANY = "WP01";
                    break;
                case 3:
                    COMPANY = "TEST";
                    break;
            }
            //-----訂單
            SqlConnection conn1 = new SqlConnection(YScon);
            SqlCommand cmd1 = new SqlCommand();
            string TC002 = GetOrderNumber(COMPANY);
            string TH001 = "236";
            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand("", conn);
            cmd.Connection.Open();
            SqlTransaction sqlTransaction = null;
            sqlTransaction = conn.BeginTransaction();
            cmd.Transaction = sqlTransaction;
            int index = 0;
            int qutyCount = 0;
            foreach (dataT item in row.items)
            {
                //--------------訂單
                string TD005 = "", TD010 = "";
                if (conn1.State == ConnectionState.Open) conn1.Close();
                cmd1 = new SqlCommand($"SELECT [MB001],[MB002],[MB004] FROM [{COMPANY}].[dbo].[INVMB] where [MB001]='{item.pt_id.Trim()}'", conn1);
                cmd1.Connection.Open();
                SqlDataReader reader = cmd1.ExecuteReader();
                if (reader.Read())
                {
                    TD005 = reader["MB002"].ToString();
                    TD010 = reader["MB004"].ToString();
                }
                string TD004 = item.pt_id.Trim(), TD008 = item.pt_quty.ToString(), TD011 = item.pt_price.ToString();

                string TD012 = (item.pt_quty * item.pt_price).ToString();
                //取得實際品名

                qutyCount += item.pt_quty;
                //檢查商品存在
                index++;
                string TD003 = String.Format("{0:D3}", index);
                string sqlstring1 = $"INSERT INTO [{COMPANY}].[dbo].[COPTD]([COMPANY],[CREATOR],[USR_GROUP],[CREATE_DATE],[MODIFIER],[MODI_DATE],[CREATE_TIME],[MODI_TIME]";
                sqlstring1 += ",[TD001],[TD002],[TD003],[TD004],[TD005],[TD008],[TD010],[TD011],[TD012],[TD014],[TD038]";
                sqlstring1 += ",[FLAG],[CREATE_AP],[CREATE_PRID],[MODI_AP],[MODI_PRID],[TD013],[TD007],[TD009],[TD016],[TD021],[TD022],[TD024]";
                sqlstring1 += ",[TD025],[TD026],[TD030],[TD031],[TD032],[TD033],[TD034],[TD035],[TD036],[TD099],[TD040]) values";
                sqlstring1 += $"('{COMPANY}','{CREATOR}','{USR_GROUP}', convert(char(8),GETDATE(),112),'{MODIFIER}', convert(char(8),GETDATE(),112),convert(varchar(10), GETDATE(), 108),convert(varchar(10), GETDATE(), 108)";
                sqlstring1 += $",'{TC001}','{TC002}','{TD003}','{TD004}','{TD005}',{TD008},'{TD010}',{TD011},{TD012},'{TC012}','{TD038}'";
                sqlstring1 += ",2,'WISDOM22','COPI06','WISDOM22','COPI06',convert(char(8),GETDATE(),112),'Z',0,'N','Y',0,0";
                sqlstring1 += ",0,1,0,0,'1',0,0,0,0,1,0)";
                cmd.CommandText = sqlstring1;
                cmd.ExecuteNonQuery();
            }
            //--------------訂單
            if (index > 0)
            {
                TC031 = qutyCount.ToString();
                string sqlstring = $"INSERT INTO [{COMPANY}].[dbo].[COPTC]([COMPANY],[CREATOR],[USR_GROUP],[CREATE_DATE],[MODIFIER],[MODI_DATE],[CREATE_TIME],[MODI_TIME]";
                sqlstring += ",[TC001],[TC002],[TC003],[TC004],[TC012],[TC016],[TC029],[TC030],[TC031],[TC032],[TC039],[TC040]";
                sqlstring += ",[TC043],[TC046]";
                sqlstring += ",[FLAG],[CREATE_AP],[CREATE_PRID],[MODI_AP],[MODI_PRID],[TC007],[TC008],[TC009],[TC019],[TC026],[TC027],[TC028]";
                sqlstring += ",[TC041],[TC042],[TC049],[TC050],[TC051],[TC052],[TC055],[TC056],[TC057],[TC058],[TC064],[TC066],[TC067]";
                sqlstring += ",[TC074],[TC075],[TC091],[TC092]) values";
                sqlstring += $"('{COMPANY}','{CREATOR}','{USR_GROUP}', convert(char(8),GETDATE(),112),'{MODIFIER}', convert(char(8),GETDATE(),112),convert(varchar(10), GETDATE(), 108),convert(varchar(10), GETDATE(), 108)";
                sqlstring += $",'{TH001}','{TC002}','{TC003}','{TC004}','{TC012}','{TC016}',{TC029},{TC030},{TC031},'{TC032}','{TC039}','{TC040}'";
                sqlstring += $",'{TC043}','{TC046}'";
                sqlstring += ",2,'WISDOM22','COPI06','WISDOM22','COPI06','001','NTD',1,1,0,'Y',0";
                sqlstring += ",0.05,'N',0,'1',0,0,'1',0,0,0,'1',0,0";
                sqlstring += ",'N',0,0,0)";
                cmd.CommandText = sqlstring;
                cmd.ExecuteNonQuery();
                //----------------------------------------
                try
                {
                    sqlTransaction.Commit();
                }
                catch
                {
                    if (sqlTransaction != null)
                    {
                        sqlTransaction.Rollback();
                        msg = row.sno;
                        MessageBox.Show($"{msg}匯入失敗!!!!");
                    }
                }
            }
            if (listSno.Count > 0)
            {
                foreach (string cc in listSno)
                {
                    msg += cc;
                }
                msg += "查無品項!!!";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand($"SELECT a.[Sno] [od_id],[MB001] [pt_id],[PtName] [pt_name],[PtUnit] [unit],[Quty] [pt_quty],[Tprice][pt_price],[Cdate] [od_date],b.[Price] [pt_total_price] FROM [TSales] a inner join [OutPos].[dbo].[MSales] b on a.Sno=b.Sno where [Cdate] between '{dateTimePicker1.Value.ToString("yyyy-MM-dd")}' and '{dateTimePicker2.Value.ToString("yyyy-MM-dd")}' order by a.[Sno]", conn);
            cmd.Connection.Open();
            SqlDataReader sdrs = cmd.ExecuteReader();
            dt = new DataTable();
            dt.Load(sdrs);
            dataGridView1.DataSource = dt;
            if (dt != null && dt.Rows.Count > 0)
            {
                button2.Enabled = true;
                label2.Text = dt.Rows.Count.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if (dt == null || dt.Rows.Count == 0) return;
            List<string> errmsg1 = new List<string>();
            foreach (DataRow sdr in dt.Rows)
            {
                if (CheckProductExnd(sdr["pt_id"].ToString(), comboBox2.Text) == 0) listBox1.Items.Add(sdr["pt_id"].ToString() + "品號不存在");
                if (CheckInvoicExnd(sdr["od_id"].ToString(), comboBox2.Text)>0) listBox1.Items.Add(sdr["od_id"].ToString() + "重複匯入");
            }
            if (listBox1.Items.Count==0)
            {
                string od_id = "";
                List<dataT> items = new List<dataT>();
                dataM data = new dataM();
                List<dataM> lsdata = new List<dataM>();//主檔
                foreach (DataRow sdr in dt.Rows)
                {
                    if (od_id != "" && sdr["od_id"].ToString() != od_id)
                    {
                        //add dataM List
                        data.items = items;
                        lsdata.Add(data);
                        //clean dataM
                        items = new List<dataT>();
                        data = new dataM();
                    }
                    od_id = sdr["od_id"].ToString();
                    data.pt_total_price = Convert.ToDecimal(sdr["pt_total_price"]);
                    data.od_id = od_id;
                    //data.cus_name = sdr["cus_name"].ToString();
                    data.od_date = sdr["od_date"].ToString();
                    dataT item = new dataT();
                    item.pt_price = Convert.ToDecimal(sdr["pt_price"]);
                    item.od_id = od_id;
                    item.pt_id = sdr["pt_id"].ToString();
                    item.pt_quty = Convert.ToInt16(sdr["pt_quty"]);
                    item.unit = sdr["unit"].ToString();
                    item.pt_name = sdr["pt_name"].ToString();
                    items.Add(item);
                }
                if (items != null && items.Count > 0)
                {
                    data.items = items;
                    lsdata.Add(data);
                }

                if (lsdata != null && lsdata.Count > 0)
                {
                    //加入ERP
                    foreach (dataM item in lsdata) exportToerp(item);
                    //異常顯示
                    if (listBox1.Items.Count == 0)
                    {
                        MessageBox.Show($"設定完成總筆數{dt.Rows.Count} ...");
                    }
                }
                else
                {
                    MessageBox.Show($"查無資料可匯入 !!!!");
                }
            }
        }
    }
}
