using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Windows.Forms;
using WTools.BuyOrder;
using WTools.warehouse;

namespace WTools
{
    public partial class MainForm : Form
    {
        //public static string YScon = "Server =192.168.1.252;Database=TEST;User ID=sa;Password=dsc@53290529;encrypt=false;";
        public static string YScon = "Server =192.168.1.252;Database=YS;User ID=sa;Password=dsc@53290529;encrypt=false;";
        public static string PosErp = "Server=(localdb)\\MSSQLLocalDB;Database=PosToErp;Integrated Security=true";
        public static string WP01 = "Server =192.168.1.252;Database=WP01;User ID=sa;Password=dsc@53290529;encrypt=false;";
        public static string OutPoscon = "Server=(localdb)\\MSSQLLocalDB;Database=OutPos;Integrated Security=true";
        public static string UserId = "System";
        public static string UserPrivat = "0";//0:一般使用者 9:管理者
        public static string PrintName = "";//發票印表機名稱
        public static bool QutyNoError = true; //檢查庫存量警示
        public static bool CheckAccounts = true; //折扣檢查
        public static bool CheckMig = true; //折扣檢查
        //public static DataTable PDT;//折扣表
        public static DataTable DTsale;//當前交易表
        public static DataTable[] TmpsTable= { null, null, null };//暫存交易表
        public static string[] CpInfo = { "", "", "" };//發票列印公司、統編、印表機
        public static ProLocal proLocal= new ProLocal();//庫存儲位
        public MainForm()
        {
            InitializeComponent();
        }

        public static DataTable CheckProduct()
        {
            DataTable dt = new DataTable();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [MB001],[MB002] FROM [Products] where MB064<1", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            if (sdr.HasRows)
            {
                dt.Load(sdr);
            }
            return dt;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as Button).Text;
            UserControl1 userControl1 = new UserControl1();
            panel2.Controls.Clear();
            panel2.Controls.Add(userControl1);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as Button).Text;
            UserControl3 userControl3 = new UserControl3();
            panel2.Controls.Clear();
            panel2.Controls.Add(userControl3);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as Button).Text;
            PostDesk.UserClientDesk clientDesk=new PostDesk.UserClientDesk();
            panel2.Controls.Clear();
            panel2.Controls.Add(clientDesk);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dialogLogin dw=new dialogLogin();
            dw.ShowDialog();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            //其他設定
            string sql = "select (SELECT [F1] FROM [OtherConfigs] where FSno=2) F02,(SELECT [F1] FROM [OtherConfigs] where FSno=3) F03,(SELECT [F1] FROM [OtherConfigs] where FSno=4) F04,(SELECT [F1] FROM [OtherConfigs] where FSno=5) F05";
            SqlCommand cmd1 = new SqlCommand(sql, conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            if(sdr.Read()){
                QutyNoError = Convert.ToBoolean(Convert.ToUInt16(sdr["F03"])); //檢查庫存量警示
                CheckAccounts = Convert.ToBoolean(Convert.ToUInt16(sdr["F04"])); //折扣檢查
                CheckMig = Convert.ToBoolean(Convert.ToUInt16(sdr["F05"]));//電子發票
            }
            //庫存位置
            proLocal.master = new List<ProLocalM>();
            cmd1 = new SqlCommand("SELECT [sno],[Upitem],[Name] FROM [Stockhouse] where Upitem=0;", conn1);
            sdr.Close();
            sdr = cmd1.ExecuteReader();
            while (sdr.Read())
            {
                ProLocalM pm=new ProLocalM();
                pm.upid =Convert.ToInt32(sdr["Upitem"]);
                pm.id = Convert.ToInt32(sdr["sno"]);
                pm.name=sdr["Name"].ToString();
                proLocal.master.Add(pm);
            }
            //儲存位置
            proLocal.detail = new List<ProLocalM>();
            cmd1 = new SqlCommand("SELECT [sno],[Upitem],[Name] FROM [Stockhouse] where Upitem>0;", conn1);
            sdr.Close();
            sdr = cmd1.ExecuteReader();
            while (sdr.Read())
            {
                ProLocalM pm = new ProLocalM();
                pm.upid = Convert.ToInt32(sdr["Upitem"]);
                pm.id = Convert.ToInt32(sdr["sno"]);
                pm.name = sdr["Name"].ToString();
                proLocal.detail.Add(pm);
            }
            //公司基本資料
            cmd1 = new SqlCommand("SELECT TOP (1) [SupName],[SupSno],(SELECT TOP (1) [F1] FROM [OtherConfigs] where [FSno]=1) PrintName FROM [Company]", conn1);
            sdr.Close();
            sdr = cmd1.ExecuteReader();
            if (sdr.Read())
            {
                CpInfo[0] = sdr["SupName"].ToString();
                CpInfo[1] = sdr["SupSno"].ToString();
                CpInfo[2] = sdr["PrintName"].ToString();
            }
            //使用者權限
            if (MainForm.UserPrivat == "0")
            {
                button10.Visible = false;
                button12.Visible = false;
            }
            //銷貨明細
            MainForm.DTsale = new DataTable();
            DataColumn dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB002";
            MainForm.DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "MB051";
            MainForm.DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "MB064";
            dataColumn.ReadOnly = false;
            MainForm.DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "Total";
            dataColumn.ReadOnly = false;
            MainForm.DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "MB001";
            MainForm.DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = false;
            dataColumn.DataType = typeof(int);
            dataColumn.ColumnName = "Gp";
            MainForm.DTsale.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.AllowDBNull = true;
            dataColumn.DataType = typeof(string);
            dataColumn.ColumnName = "GpSno";
            MainForm.DTsale.Columns.Add(dataColumn);
            button2.PerformClick();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as System.Windows.Forms.Button).Text;
            UserProductCreate userproductcreate = new UserProductCreate();
            panel2.Controls.Clear();
            panel2.Controls.Add(userproductcreate);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("即將清空現有資料!確定匯入...", "商品匯入", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                DataTable dt = new DataTable();
                SqlConnection conn1 = new SqlConnection(YScon);
                SqlCommand cmd1 = new SqlCommand("SELECT TRIM([MB001])[MB001],[MB002],[MB003],[MB004],[MB064],[MB051] FROM [INVMB]  where len(MB001)=6", conn1);
                cmd1.Connection.Open();
                SqlDataReader dr1 = cmd1.ExecuteReader();
                if (dr1.HasRows)
                {
                    dt.Load(dr1);
                    //cmd1.Connection.Close();             
                    conn1 = new SqlConnection(OutPoscon);
                    cmd1 = new SqlCommand("truncate table Products;truncate table [PDList];", conn1);
                    cmd1.Connection.Open();
                    cmd1.ExecuteNonQuery();
                    foreach (DataRow dr in dt.Rows)
                    {
                        cmd1.CommandText = "INSERT INTO Products(MB001, MB002, MB003, MB004, MB064, MB051) values('" + dr[0].ToString() +"','" + dr[1].ToString() + "','"+ dr[2].ToString() +"','"+ dr[3].ToString() +"',"+ dr[4].ToString() +","+ dr[5].ToString() +");";
                        cmd1.CommandText += "INSERT INTO [PDList]([userid],[MB001],[Quty]) values('Init','" + dr[0].ToString() + "'," + dr[4].ToString() +");";
                        cmd1.ExecuteNonQuery();
                    }
                }
                button2.PerformClick();
            }
        }

        public class exportdataM
        {
            public string Sno { get; set; }
            public double Price { get; set; }
            public string Cdate { get; set; }
            public string Ctime { get; set; }
            public string Scode { get; set; }
        }

        public class exportdataT
        {
            public string MB001 { get; set; }
            public Int32 Quty { get; set; }
            public double Price { get; set; }
            public double Tprice { get; set; }
        }

        public class AllTranscate
        {
            public exportdataM Master { get; set; }
            public List<exportdataT> Titail { get; set; }
        }
        private List<AllTranscate> GetAllTranscate()
        {
            //主檔
            List<AllTranscate> rows = new List<AllTranscate>();
            var sqlstring = "SELECT [Sno],[Price],FORMAT(Cdate, 'yyyyMMdd')[Cdate],FORMAT(Cdate, 'HH:mm:ss')[Ctime],[Scode] FROM [MSales] WHERE [Isok] = '1' order by Cdate";
	        DataTable dt = new DataTable();
            SqlConnection conn1 = new SqlConnection(OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
            cmd1.Connection.Open();
            SqlDataReader dr1 = cmd1.ExecuteReader();
            if (dr1.HasRows)
            {
                dt.Load(dr1);         
                SqlConnection conn = new SqlConnection(OutPoscon);
                SqlCommand cmd = new SqlCommand("", conn);
                cmd.Connection.Open();

                foreach (DataRow dr in dt.Rows)
                {
                    exportdataM row1 = new exportdataM();
                    List<exportdataT> row2 = new List<exportdataT>();
                    row1.Sno = dr[0].ToString();
                    row1.Price = Convert.ToInt32(dr[1]);
                    row1.Cdate = dr[2].ToString();
                    row1.Ctime = dr[3].ToString();
                    row1.Scode = dr[4].ToString();
                    cmd.CommandText = "SELECT [MB001], [Quty], [Price], [Tprice] FROM [TSales] WHERE [Sno]='"+ row1.Sno + "'";
                    SqlDataReader drt = cmd.ExecuteReader();
                    while (drt.Read())
                    {
                        exportdataT row22 = new exportdataT();
                        row22.MB001 = drt[0].ToString() ;
                        row22.Quty = Convert.ToInt32(drt[1]);
                        row22.Price=Convert.ToInt32(drt[2]);
                        row22.Tprice = Convert.ToInt32(drt[3]);
                        row2.Add(row22);
                    }
                    drt.Close();
                    AllTranscate tmp =new AllTranscate();
                    tmp.Titail = row2;
                    tmp.Master = row1;
                    rows.Add(tmp);
                }
            }            
            return rows;
	    }

        private int GetSaleNumber(string dateno) {

            Int16 code = 0;

            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand("SELECT MAX([TG002]) FROM [COPTG] where [TG001]='236' and [TG002] like '" + dateno + "%'", conn);
            cmd.Connection.Open();
            string pcode = cmd.ExecuteScalar().ToString();

	        if (pcode.Length == 11) {
                code= Convert.ToInt16(pcode.Substring(8, 4));
            }
            return code;
        }

        //扣除批號數量
        private DataTable subkindnu()
        {
            DataTable dt = new DataTable();
            string cl = "SELECT MF002,sum(MF010*MF008) as MF0108 FROM [INVMF] where MF001 = 'Z1-0005'  group by  MF002 having sum(MF010 * MF008) > 0 order by MF002";

            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand(cl, conn);
            cmd.Connection.Open();
            SqlDataReader sdr= cmd.ExecuteReader();
            if (sdr.HasRows)
            {
                dt.Load(sdr);
            }
            return dt;
        }
        private string exportToerp(AllTranscate row)
        {
            string msg = "";
            int TG012 = 1, TG022 = 0, TG025 = 0,TG032 = 0, TG033 = 0, TG041 = 0;
            float TG044 = 0.05F;
            string TG015 = ""; //統一編號
            string COMPANY = "YS", CREATOR = "WIS062", USR_GROUP = "DSC", CREATE_AP = "WISDOM22", CREATE_PRID = "COPI08";
            string TG001 = "236", TG004 = "0000", TG007="路人甲",TG010 = "001";
            string TG011 = "NTD", TG016 = "1", TG017 = "1", TG023 = "N", TG024 = "N", TG030 = "N", TG031 = "1", TG034 = "Y", TG036 = "N", TG037 = "N", TG047 = "N";
            string TG055 = "0", TG056 = "1", TG057 = "0", TG058 = "0", TG063 = "0", TG064 = "0", TG068 = "1", TG070 = "0", TG071 = "0", TG072 = "N", TG091 = "0";
            string TG092 = "0", TG093 = "0", TG094 = "0";          
            string  TH020 = "Y", TH021 = "N", TH026 = "Y", TH057 = "0", TH031 = "2"; //類型
            int TH024 = 0, TH025 = 1, TH043 = 0, TH099 = 1, TH079 = 0;
            //取得最後單號
            var mun = GetSaleNumber(row.Master.Cdate);
            string CREATE_DATE = row.Master.Cdate;
            string CREATE_TIME = row.Master.Ctime;
            string TG002 = row.Master.Cdate +( mun + 1).ToString("000");
            string TG003= row.Master.Cdate;
            double TG013 = row.Master.Price;
            string TG014 = row.Master.Sno;
            string TG038= row.Master.Cdate.Substring(0,6);
            string TG042= row.Master.Cdate;
            double TG045= row.Master.Price;
            double TG046= row.Master.Price * 0.05;

            //公司統編
            if (row.Master.Scode.Length == 10) {
                TG016 = "2";
                TG017 = "2";
            }
            else //零售
            {
                TG016 = "1";
                TG017 = "1";
            }
            string TH005 = "", TH006 = "", TH007 = "", TH009 = "", TH010 = "", TH011 = "", TH042 = "", TH044 = "";
            int j = 0;
            SqlConnection conn = new SqlConnection(YScon);
            SqlCommand cmd = new SqlCommand("", conn);
            cmd.Connection.Open();
            SqlConnection conn1 = new SqlConnection(YScon);
            SqlCommand cmd1 = new SqlCommand("", conn1);
            cmd1.Connection.Open();
            SqlTransaction sqlTransaction = null;
            sqlTransaction = conn.BeginTransaction();
            cmd.Transaction = sqlTransaction;
            try
            {
                foreach (exportdataT item in row.Titail)
                {
                    //銷貨單身
                    //新增批號
                    DataTable tmp = subkindnu();
                    cmd1.CommandText = "SELECT [MB002] TH005,[MB003] TH006,[MB004] TH009,[MB017] TH007,[MB064] TH010,[MB072] TH011,[MB090] TH044,[MB092] TH042 FROM [INVMB] where MB001 ='" + item.MB001 + "'";
                    SqlDataReader sdr = cmd1.ExecuteReader();
                    if (sdr.Read())
                    {
                        TH005 = sdr[0].ToString();
                        TH006 = sdr[1].ToString();
                        TH009 = sdr[2].ToString();
                        TH007 = sdr[3].ToString();
                        TH010 = sdr[4].ToString();
                        TH011 = sdr[5].ToString();
                        TH044 = sdr[6].ToString();
                        TH042 = sdr[7].ToString();
                    }
                    sdr.Close();
                    string TH003 = (j + 1).ToString("0000");
                    string TH004 = item.MB001;
                    int TH008 = item.Quty;
                    double TH012 = item.Price;
                    double TH013 = item.Tprice;
                    string TH017 = "";                               //批號
                    double TH035 = item.Tprice - (item.Tprice * 0.05);//原幣未稅金額
                    double TH036 = item.Tprice * 0.05;               //原幣稅額
                    double TH037 = item.Tprice - (item.Tprice * 0.05); //本幣未稅金額
                    double TH038 = item.Tprice * 0.05;               //本幣稅額
                    string TH045 = row.Master.Sno;
                    string sqlstring2 = "";
                    sqlstring2 = "insert into [COPTH] ([COMPANY], [CREATOR], [USR_GROUP], [CREATE_DATE],[MODIFIER],[MODI_DATE], [FLAG], [CREATE_TIME], [CREATE_AP], [CREATE_PRID]";
                    sqlstring2 += ",[MODI_TIME],[MODI_AP],[MODI_PRID], [TH001], [TH002], [TH003], [TH004], [TH005], [TH006], [TH007] ";
                    sqlstring2 += ", [TH008], [TH009], [TH010], [TH012] , [TH013], [TH017], [TH020], [TH021], [TH024] , [TH025]";
                    sqlstring2 += ", [TH026], [TH031], [TH035], [TH036] , [TH037], [TH038], [TH042], [TH043], [TH045], [TH057] ";
                    sqlstring2 += ", [TH099], [TH079]";
                    sqlstring2 += $" ) values('{COMPANY}', '{CREATOR}', '{USR_GROUP}', '{CREATE_DATE}','{CREATOR}', '{CREATE_DATE}', '2', '{CREATE_TIME}','{CREATE_AP}','{CREATE_PRID}'";
                    sqlstring2 += $", '{CREATE_TIME}','{CREATE_AP}','{CREATE_PRID}', '{TG001}','{TG002}','{TH003}','{TH004}','{TH005}','{TH006}','{TH007}'";
                    sqlstring2 += $",{TH008},'{TH009}','{TH010}',{TH012},{TH013},'{TH017}','{TH020}','{TH021}',{TH024},{TH025}";
                    sqlstring2 += $",'{ TH026}','{ TH031}','{ TH035}','{ TH036}','{ TH037}','{TH038}','{ TH042}',{ TH043},'{ TH045}','{ TH057}',{ TH099},{ TH079})";
                    cmd.CommandText = sqlstring2;
                    cmd.ExecuteNonQuery();
                    //變更產品總庫存
                    sqlstring2 = $"update [INVMB] set MB064=MB064-{TH008},[MODIFIER]='{CREATOR}',[MODI_DATE]='{CREATE_DATE}',[MODI_TIME]='{CREATE_TIME}',[MODI_AP]='{CREATE_AP}',[MODI_PRID]='{CREATE_PRID}' where MB001='{TH004}';";                  
                    //變更產品庫別總庫存
                    sqlstring2 += $"update [INVMC] set MC007=MC007-{TH008},[MODIFIER]='{CREATOR}',[MODI_DATE]='{CREATE_DATE}',[MODI_TIME]='{CREATE_TIME}',[MODI_AP]='{CREATE_AP}',[MODI_PRID]='{CREATE_PRID}' where MC001='{TH004}';";
                    cmd.CommandText = sqlstring2;
                    cmd.ExecuteNonQuery();
                   
                    int tmpquty=TH008 ;
                    foreach (DataRow dr in tmp.Rows)
                    {
                        int quty = Convert.ToInt32(dr[1]);
                        TH017 = dr[0].ToString();
                        if (quty >= tmpquty)
                        {
                            sqlstring2 = "INSERT INTO [INVMF] ([COMPANY], [CREATOR], [USR_GROUP], [CREATE_DATE], [FLAG]";
                            sqlstring2 += ",[CREATE_TIME], [CREATE_AP], [CREATE_PRID] ";
                            sqlstring2 += ", [MF001], [MF002], [MF003], [MF004], [MF005], [MF006], [MF007], [MF008], [MF009], [MF010], [MF014])";
                            sqlstring2 += $" values('{COMPANY}', '{CREATOR}', '{USR_GROUP}', '{CREATE_DATE}', '1', '{CREATE_TIME}','{CREATE_AP}','{CREATE_PRID}','{TH004}','批號','{CREATE_DATE}','{TG001}','{TG002}','{TH003}','Z',-1,2,{TH008},0)";
                            cmd.CommandText = sqlstring2;
                            cmd.ExecuteNonQuery();
                            break;
                        }
                        else
                        {
                            tmpquty -= quty;
                            sqlstring2 = "INSERT INTO [INVMF] ([COMPANY], [CREATOR], [USR_GROUP], [CREATE_DATE], [FLAG]";
                            sqlstring2 += ",[CREATE_TIME], [CREATE_AP], [CREATE_PRID] ";
                            sqlstring2 += ", [MF001], [MF002], [MF003], [MF004], [MF005], [MF006], [MF007], [MF008], [MF009], [MF010], [MF014])";
                            sqlstring2 += $" values('{COMPANY}', '{CREATOR}', '{USR_GROUP}', '{CREATE_DATE}', '1', '{CREATE_TIME}','{CREATE_AP}','{CREATE_PRID}','{TH004}','批號','{CREATE_DATE}','{TG001}','{TG002}','{TH003}','Z',-1,2,{quty},0)";
                            cmd.CommandText = sqlstring2;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                //銷貨單頭檔      
                string sqlstring1 = "insert into [COPTG] ([COMPANY], [CREATOR], [USR_GROUP], [CREATE_DATE],[MODIFIER],[MODI_DATE], [FLAG]";
                sqlstring1 += ", [CREATE_TIME], [CREATE_AP], [CREATE_PRID],[MODI_TIME],[MODI_AP],[MODI_PRID]";
                sqlstring1 += ", [TG001], [TG002], [TG003], [TG004],[TG007], [TG010], [TG011], [TG012], [TG013], [TG014]";
                sqlstring1 += ", [TG015], [TG016], [TG017], [TG022], [TG023], [TG024], [TG025], [TG030], [TG031], [TG032]";
                sqlstring1 += ", [TG033], [TG034], [TG036], [TG037], [TG038], [TG041], [TG042] ,[TG043], [TG044], [TG045]";
                sqlstring1 += ", [TG046], [TG047],[TG049], [TG055], [TG056], [TG057], [TG058], [TG063], [TG064], [TG068]";
                sqlstring1 += ", [TG070], [TG071], [TG072], [TG091], [TG092], [TG093], [TG094])";
                sqlstring1 += $" values('{COMPANY}','{CREATOR}','{USR_GROUP}','{CREATE_DATE}','{CREATOR}','{CREATE_DATE}','2'";
                sqlstring1 += $",'{CREATE_TIME}','{CREATE_AP}',{CREATE_PRID}','{CREATE_TIME}','{CREATE_AP}','{CREATE_PRID}'";
                sqlstring1 += $",'{TG001}','{TG002}','{TG003}','{TG004}','{TG007}','{TG010}','{TG011}','{TG012}','{TG013}','{TG014}'";
                sqlstring1 += $",'{TG015}','{TG016}','{TG017}','{TG022}','{TG023}','{TG024}','{TG025}','{TG030}','{TG031}','{TG032}'";
                sqlstring1 += $",'{TG033}','{TG034}','{TG036}','{TG037}','{TG038}','{TG041}','{TG042}','{CREATOR}','{TG044}','{TG045}'";
                sqlstring1 += $",'{TG046}','{TG047}','{TG007}','{TG055}','{TG056}','{TG057}','{TG058}','{TG063}','{TG064}','{TG068}'";
                sqlstring1 += $",'{ TG070}','{ TG071}','{ TG072}','{ TG091}','{ TG092}','{ TG093}','{ TG094}'";
                cmd.CommandText = sqlstring1;
                cmd.ExecuteNonQuery();
                sqlTransaction.Commit();
            }
            catch
            {
                if (sqlTransaction != null)
                {
                    sqlTransaction.Rollback();
                    msg = row.Master.Sno;
                    MessageBox.Show($"{msg}匯入失敗!!!!");
                }
            }
            return msg;
        }
       
        private void button10_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as System.Windows.Forms.Button).Text;
            UserAssember userassember = new UserAssember();
            panel2.Controls.Clear();
            panel2.Controls.Add(userassember);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            dialogExcelImportPt dialogExcelImportPt = new dialogExcelImportPt();
            dialogExcelImportPt.ShowDialog();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as System.Windows.Forms.Button).Text;
            UserDiscount userDiscount = new UserDiscount();
            panel2.Controls.Clear();
            panel2.Controls.Add(userDiscount);
        }

        private void 新增採購ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text; 
            UserOrder userOrder = new UserOrder();
            panel2.Controls.Clear();
            panel2.Controls.Add(userOrder);
        }

        private void 廠商管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuyOrder.UseSupportInfo supportInfo = new BuyOrder.UseSupportInfo();
            panel2.Controls.Clear();
            panel2.Controls.Add(supportInfo);
        }

        private void 錢櫃準備金ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            SqlConnection conn1 = new SqlConnection(OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [F1] FROM [OtherConfigs] where [FSno]=2", conn1);
            cmd1.Connection.Open();

            dialogform dl = new dialogform("錢櫃準備金");
            dl.Text = "錢櫃準備金";
            dl.label1.Text = "金額";
            try { dl.numericUpDown1.Value = Convert.ToInt16(cmd1.ExecuteScalar()); }
            catch { }
            
            DialogResult dr = dl.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string sql = $"IF(SELECT COUNT(*) FROM [OtherConfigs] where [FSno]=2)=0 INSERT INTO [OtherConfigs]([FSno],[FName],[F1]) VALUES(2,'錢櫃準備金','{dl.GetMsg()}')";
                sql += $" ELSE UPDATE [OtherConfigs] SET [F1]='{dl.GetMsg()}' where [FSno]=2";
                cmd1.CommandText = sql;
                if(cmd1.ExecuteNonQuery()<1) MessageBox.Show("錢櫃準備金設定失敗!!!請重設...");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as System.Windows.Forms.Button).Text;
            FOverAcount fa=new FOverAcount();
            fa.ShowDialog();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            label1.Text = (sender as System.Windows.Forms.Button).Text;
            UserOtherIO userotherio = new UserOtherIO();
            panel2.Controls.Clear();
            panel2.Controls.Add(userotherio);
        }

        private void 發票設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as System.Windows.Forms.Button).Text;
            UserInvConfig userinvconfig = new UserInvConfig();
            panel2.Controls.Clear();
            panel2.Controls.Add(userinvconfig);
        }

        private void 倉庫建立ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserStockLocal userstocklocal = new UserStockLocal();
            panel2.Controls.Clear();
            panel2.Controls.Add(userstocklocal);
        }

        private void 商品入庫ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
            button4.PerformClick();
        }

        private void 公司設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserCompant usercompant=new UserCompant();
            panel2.Controls.Clear();
            panel2.Controls.Add(usercompant);
        }

        private void 庫存統計ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
            UserSearchstock usersearchstock = new UserSearchstock();
            panel2.Controls.Clear();
            panel2.Controls.Add(usersearchstock);
        }

        private void 商品轉庫ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
            UserAdjust userAdjust = new UserAdjust();
            panel2.Controls.Clear();
            panel2.Controls.Add(userAdjust);
        }

        private void 品項分類ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
            UserProductClass useclass = new UserProductClass();
            panel2.Controls.Clear();
            panel2.Controls.Add(useclass);
        }

        private void 盤點作業ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text; 
            UserPtCheck useptcheck= new UserPtCheck();
            panel2.Controls.Clear();
            panel2.Controls.Add(useptcheck);
        }

        private void 廠商管理ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
            BuyOrder.UseSupportInfo useSupportInfo = new BuyOrder.UseSupportInfo();
            panel2.Controls.Clear();
            panel2.Controls.Add(useSupportInfo);
        }

        private void 進貨作業ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text; 
            UserOrderList userorderlist = new UserOrderList();
            panel2.Controls.Clear();
            panel2.Controls.Add(userorderlist);
        }

        private void 退貨作業ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 客戶基本資料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 商品定價ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 報價作業ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 訂單管理ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 銷退管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 銷貨管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 庫存調整ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 驗收入庫ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as ToolStripMenuItem).Text;
        }

        private void 採購入庫ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void button9_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as Button).Text;
            UserOrderList userorderlist = new UserOrderList();
            panel2.Controls.Clear();
            panel2.Controls.Add(userorderlist);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as Button).Text;
            UserGetStock usergetstock = new UserGetStock();
            panel2.Controls.Clear();
            panel2.Controls.Add(usergetstock);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as Button).Text;
            UserOtherConfigs userotherconfigs = new UserOtherConfigs();
            panel2.Controls.Clear();
            panel2.Controls.Add(userotherconfigs);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as Button).Text;
            SaleOrder.UserNetSale usernetsale = new SaleOrder.UserNetSale();
            panel2.Controls.Clear();
            panel2.Controls.Add(usernetsale);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            label1.Text = (sender as Button).Text;
            UserSqlAdapter usersqladapter = new UserSqlAdapter();
            panel2.Controls.Clear();
            panel2.Controls.Add(usersqladapter);
        }
    }
    public class ProLocalM
    {
        public int upid {  get; set; }
        public int id {  get; set; }
        public string name { get; set; }
    }
    public class ProLocal
    {
        public List<ProLocalM> master {  get; set; }
        public List<ProLocalM> detail { get; set; }
    }
}
