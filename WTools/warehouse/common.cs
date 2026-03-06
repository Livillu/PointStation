using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Documents;
using WTools;

using BAN = System.String;
using MessageTypeEnum = System.String;
using InvoiceTypeEnum = System.String;
using AllowanceNumberType = System.String;
using AllowanceTypeEnum = System.String;
using BuyerRemarkEnum = System.String;
using CustomsClearanceMarkEnum = System.String;
using CurrencyCodeEnum =System.String;
using ZeroTaxRateReasonEnum = System.String;
using TaxRateEnum = System.Decimal;
using BondedAreaEnum  = System.String;
using TimeType  = System.String;
using DateType  = System.String;
using CarrierTypeEnum  = System.String;
using TaxTypeEnum  = System.String;
using InvoiceNumberType = System.String;
using DonateMarkEnum  = System.String;

namespace WTools.warehouse
{
    internal class common
    {
        public const string Psconnection = "server=192.168.1.252;user id=sa;password=dsc@53290529;database=WP01;port=1433";

        public const string Nletter    = "0123456789";//數字判斷

        public const string Aletter    = "QWERTYUIOPLKJHGFDSAZXCVBNM";

        public const string tringMigVer     = "4.1";

        public const string SellerId   = "53290529";

        public const string SellerName = "璿智國際寵物科技有限公司";

        public const string SellerAddr = "嘉義市東區後湖里保義路327號";

        public const string V3j0002    = "QWERTYUIOPLKJHGFDSAZXCVBNM1234567890.-+";

        string Errormsg;
        byte[] Msg;
       

        public class RoleDescriptionType {
            public BAN Identifier { get; set; }     //`xml:"Identifier"`
            public string Name { get; set; } //`xml:"Name"`
            public string Address { get; set; }// `xml:"Address,omitempty"`
            public string Checked()
            {
                string errmsg = "";
                if (Identifier == "") errmsg += "識別碼欄位錯誤!!!\t";
                if (Name == "" || Name.Length > 60 ) errmsg += "名稱欄位錯誤!!!\t";
                if (Address != "" && Address.Length > 100)  errmsg += "地址欄位錯誤!!!\t";
                return errmsg;
            }
            //自己公司RoleDescription
            public RoleDescriptionType Myselfe(string banid)
            {
                string sqlstring = $"SELECT TOP 1 ML007,ML003,ML012 FROM CMSML WHERE ML007='{banid}'";
                RoleDescriptionType c = new RoleDescriptionType();
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
                cmd1.Connection.Open();
                SqlDataReader sdr = cmd1.ExecuteReader();
                if (sdr.Read())
                {
                    c.Identifier = sdr[0].ToString();
                    c.Name = sdr[1].ToString();
                    c.Address = sdr[2].ToString();
                }
                return c;
            }

            //供應商RoleDescription
            public RoleDescriptionType Seller(string banid)
            {
                RoleDescriptionType c = new RoleDescriptionType();
                string sqlstring = $"SELECT MA005,MA003,'' FROM PURMA WHERE MA005='{banid}'";
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
                cmd1.Connection.Open();
                SqlDataReader sdr = cmd1.ExecuteReader();
                if (sdr.Read())
                {
                    c.Identifier = sdr[0].ToString();
                    c.Name = sdr[1].ToString();
                    c.Address = sdr[2].ToString();
                }
                return c;
            }
        }
        
        //公司資料預設值
        //var MyBan =new RoleDescriptionType{ SellerId, SellerName, SellerAddr }

        //千位數註記，
        public static string StrDelimit(string str , string sepstr ,int sepcount )  
        {
            int tmp = str.LastIndexOf(".");
            int pos = 0;
            if (tmp > 0) pos = tmp - sepcount;
            else pos = str.Length - sepcount;
            while (pos > 0) {
                str = str.Substring(0, pos) + sepstr + str.Substring(pos);
                pos = pos - sepcount;
            }
            return str;
        }

        //金額轉中文大寫
        public string ChMunber(string codes)  {
            string[] m = new string[] { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖" };
            string result = "";
            int index = 0;
            int counts= codes.Length;
            for (int i = 0; i < counts; i++) {
                string v = codes.Substring(i,1);
                index = counts - i;
                if (!v.Contains(Nletter)) break;                
                else
                {
                    result += m[Convert.ToUInt16(v)];
                    switch (index) 
                    {
                        case 12:
                            result += "仟";
                            break;

                        case 11:
                            result += "佰";
                            break;
                        case 10:
                            result += "拾";
                            break;
                        case 9:
                            result += "億";
                            break;
                        case 8:
                            result += "仟";
                            break;
                        case 7:
                            result += "佰";
                            break;
                        case 6:
                            result += "拾";
                            break;
                        case 5:
                            result += "萬";
                            break;
                        case 4:
                            result += "仟";
                            break;
                        case 3:
                            result += "佰";
                            break;
                        case 2:
                            result += "拾";
                            break;
                        case 1:
                            result += "元整";
                            break;
                    }
                }
            }
            return result;
        }

        //發票期別驗證
        public string InvoiceMonth(string invoiceMonth) 
        {
            string errmsg = "發票[" + invoiceMonth + "]期別格式錯誤!!!!\t";
            string[] sno = new string[] { "02", "04", "06", "08", "10", "12" };
	        if (invoiceMonth.Length == 5) 
            {
                string yo= invoiceMonth.Substring(0, 3);
                string no = invoiceMonth.Substring(3, 5);
                UInt16 i=0;
                bool isok = UInt16.TryParse(yo,out i);

                if (isok) {
                    foreach(var tmp in sno) 
                    {
                        if (tmp == no) {
                            errmsg = "";
                            break;
                        }
                    }
                }
            }
            return errmsg;
        }

        //發票字軌驗證
        public string InvoiceTrack(string invoiceTrack) {
            string errmsg = "發票[" + invoiceTrack + "]字軌格式錯誤!!!!t";
            string no1 = invoiceTrack.Substring(0, 1);
            string no2= invoiceTrack.Substring(1, 2);
            if (invoiceTrack.Length == 2 && no1.Contains(Aletter) && no2.Contains(Aletter)) errmsg = "";
            return errmsg;
        }

        //發票號碼驗證
        public string InvoiceNo(string invoiceNo ) {
            string errmsg = "發票[" + invoiceNo + "]號碼格式錯誤!!!!t";

            if (invoiceNo.Length == 8) {
                UInt16 itmp = 0;
                bool isok = UInt16.TryParse(invoiceNo, out itmp);
                if (isok) errmsg = "";
            }
            return errmsg;
        }

        //發票申請組數驗證
        public string InvoiceBooks(int invoiceBooks )  {
            string errmsg = "發票[" + invoiceBooks.ToString() + "]組數格式錯誤!!!!t";
            if (invoiceBooks > 0) errmsg = "";
            return errmsg;
        }

        //銷貨資料查詢參數
        public  struct InvoiceSearch {
            public  string Tdate;  //日期20170812
            public  string Tstuts;  //狀態 Y(OK),V(CNACEL)
            public  string Tb2w; //1(b2c),2(b2b)
        }

        //銷貨資料查詢
         string InvoiceLists(InvoiceSearch c)  {
            string tmps = "";

            string sqlstring = $"SELECT TG014 FROM COPTG WHERE TG021 like '{c.Tdate}%' AND TG023='{c.Tstuts}' AND TG017='{c.Tb2w}' AND TG014<>''";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr= cmd1.ExecuteReader();
            while(sdr.Read()) {
                string InvoiceNumber;
                InvoiceNumber = sdr[0].ToString();
                if (tmps != "") tmps += ",";
                tmps += InvoiceNumber;
            }
            return tmps;
        }

        public struct FMain  {
            public string InvoiceNumber;
            public string InvoiceDate;
            public string Identifier;
            public string Name;
            public string SalesAmount;
            public string TaxRate;
            public string TaxAmount;
            public string TotalAmount;
            public string InvoiceState;
        }

        //Clinet銷貨資料查詢參數
         public struct InvoSearch {
            public string InvoiceNumber;
            public string InvoiceDate1;
            public string InvoiceDate2;
            public string Identifier;
        }

        //Clinet銷貨資料
        public List<FMain> InvoDisplay(InvoSearch c)  {
            List<FMain> tmps = new List<FMain>();
            string sqlstring = "SELECT TG014,TG021,TG015,TG007,CAST(TG045 as NUMERIC( 10, 2)) TG045,";

            sqlstring += "CAST(TG044 as NUMERIC( 10, 2)) TG044,";

            sqlstring += "CAST(TG046 as NUMERIC( 10, 2)) TG046,";

            sqlstring += "CAST((TG045+TG046 ) as NUMERIC( 10, 2)) tolamount,";

            sqlstring += "'1' UDF_state FROM COPTG WHERE TG023='Y' AND TG014<>''";

            if (c.Identifier != "") sqlstring += " AND TG015='" + c.Identifier + "'";
            if (c.InvoiceDate1 != "") {
                if (c.InvoiceDate2 == "") sqlstring += " AND TG021='" + c.InvoiceDate1 + "'";
                else sqlstring += " AND (TG021 BETWEEN '" + c.InvoiceDate1 + "' AND '" + c.InvoiceDate2 + "')";
            }
            if (c.InvoiceNumber != "") sqlstring += " AND TG014='" + c.InvoiceNumber + "'";
            sqlstring += " ORDER BY TG021,TG014";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            while (sdr.Read())
            {
                FMain row = new FMain();
                row.InvoiceNumber = sdr[0].ToString();
                row.InvoiceDate = sdr[1].ToString();
                row.Identifier = sdr[2].ToString();
                row.Name = sdr[3].ToString();
                row.SalesAmount = sdr[4].ToString();
                row.TaxRate = sdr[5].ToString();
                row.TaxAmount = sdr[6].ToString();
                row.TotalAmount = sdr[7].ToString();
                row.InvoiceState = sdr[8].ToString();
                tmps.Add(row);
            }
            return tmps;
        }

        //銷貨退回(折讓)
        public string AllowanceLists(InvoiceSearch invoiceSearch )  {
            string tmps = "";
            string sqlstring = $"SELECT TI001+'-'+TI002 as TI001 FROM [COPTI] INNER JOIN [COPTJ] ON COPTJ.TJ001=COPTI.TI001 AND COPTJ.TJ002=COPTI.TI002 WHERE TI003='{invoiceSearch.Tdate}' AND TI019='{invoiceSearch.Tstuts}' AND TI014<>'' AND TI013='{invoiceSearch.Tb2w}' AND (TJ030='2' OR (TJ030='1' AND LEN(TI068)=10))";
            //銷貨退回取消
            if (invoiceSearch.Tstuts == "V") sqlstring = $"SELECT TI001+'-'+TI002 as TI001 FROM [dbo].[COPTI] INNER JOIN [dbo].[COPTJ] ON COPTJ.TJ001=COPTI.TI001 AND COPTJ.TJ002=COPTI.TI002 WHERE [COPTI].MODI_DATE='{invoiceSearch.Tdate}' AND TI019='{invoiceSearch.Tstuts}' AND TI014<>'' AND TI013='{invoiceSearch.Tb2w}' AND (TJ030='2' OR (TJ030='1' AND LEN(TI068)=10))";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            while (sdr.Read())
            {
                string allowanceNumber;
                allowanceNumber = sdr[0].ToString();
                if (tmps != "") tmps += ",";
                tmps += allowanceNumber;
            }
            return tmps;
        }

        //採購退回(折讓)
        public string BuyAllowanceLists(InvoiceSearch c)  {
            string tmps = "";
            string sqlstring = $"SELECT TI001+'-'+TI002 TI001 FROM PURTI WHERE TI014='{c.Tdate}' AND TI013='{c.Tstuts}' AND TI018<>'' AND TI009='{c.Tb2w}'";
            if (c.Tstuts == "V") sqlstring =$"SELECT TI001+'-'+TI002 TI001 FROM PURTI WHERE MODI_DATE='{c.Tdate}' AND TI013='{c.Tstuts}' AND TI018<>'' AND TI009'{c.Tb2w}'";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            while (sdr.Read())
            {
                string allowanceNumber;
                allowanceNumber = sdr[0].ToString();
                if (tmps != "") tmps += ",";
                tmps += allowanceNumber;
            }
            return tmps;
        }

        public string SequenceChecked(string c ){
            string errmsg = "明細排列序號欄位錯誤!!!\t";
            UInt32 s = 0;
            bool isok= UInt32.TryParse(c, out s);
            if (isok && s < 10000) errmsg = "";
            return errmsg;
        }

        //客戶RoleDescription
        public RoleDescriptionType Buyer(string banid) {
            string sqlstring = $"SELECT (CASE WHEN MA010='' THEN '0000000000' ELSE MA010 END) MA010,MA003,'' FROM dbo.COPMA WHERE MA001='{banid}'";
            RoleDescriptionType c = new RoleDescriptionType();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            if (sdr.Read())
            {
                c.Identifier = sdr[0].ToString();
                c.Name = sdr[1].ToString();
                c.Address= sdr[2].ToString();
            }
            return c;
        }

        public string MessageTypeEnum_Checked(MessageTypeEnum c)
        {
            string errmsg = "MIG 表單[" + c + "]格式錯誤!!!!\t";
            string[] msgtype = new string[] { "A0101", "A0102", "A0201", "A0202", "A0301", "A0302", "B0101", "B0102", "B0201", "B0202", "E0401", "E0402", "E0501", "E0502", "E0503", "E0504", "F0401", "F0501", "F0701", "G0401", "G0501" };
	        foreach(string val in msgtype) {
            if (c == val) {
                    errmsg = "";
                    break;
                }
            }
            return errmsg;
        }


        /*
        07： 一般稅額計算之電子發
        票
        08：特種稅額計算之電子發
        票
        */
        public string InvoiceTypeEnum_Checked(InvoiceTypeEnum c)  {

            string errmsg = "發票類別[" + c + "]格式錯誤!!!!\t";
            if (c.Contains("07,08")) errmsg = "";
            return errmsg;
        }


        /*
        折讓單號規定跨年亦不得重複，否則系統將予以剔退
        */
        public string AllowanceNumberType_Checked(AllowanceNumberType n){
            string errmsg = "折讓單[" + n + "]格式錯誤!!!!\t";
            if (n.Length < 17) errmsg = "";
            return errmsg;
        }

        public string AllowanceTypeEnum_Checked(AllowanceTypeEnum n) {
            string errmsg = "折讓證明單類別[" + n + "]格式錯誤!!!!\t";
            if (n.Length == 1 && n.Contains("12")) errmsg = "";
            return errmsg;
        }



        /*
        B2B 交易填入買方/賣方-營業人統一編號
        B2C 交易賣方填入統一編號/買方則填入 10 個 "0"*/
        public string BAN_Checked(BAN n)  {
            string errmsg = "統一編號[" + n + "]格式錯誤!!!!\t";
            UInt16 s;
            bool isok= UInt16.TryParse(n, out s);
            if (isok && (n.Length == 8 || n.Length == 10)) errmsg = "";
            return errmsg;
        }


        /*
        1：得抵扣之進貨及費用
        2：得抵扣之固定資產
        3：不得抵扣之進貨及費用
        4：不得抵扣之固定資產*/
        public string BuyerRemarkEnum_Checked(BuyerRemarkEnum n) {
            string errmsg = "買受人註記[" + n + "]格式錯誤!!!!\t";
            if (n == "") errmsg = "";
            else if(n.Length == 1 && n.Contains("1234")) errmsg = "";
            return errmsg;
        }


        /*
        1：非經海關出口
        2：經海關出口
        (若為零稅率發票，此
        為必填欄位)
        */
        public string CustomsClearanceMarkEnum_Checked(CustomsClearanceMarkEnum n){
            string errmsg = "通關方式[" + n + "]格式錯誤!!!!\t";
            if (n == "") errmsg = "";
            else if (n.Length == 1 && n.Contains("12")) errmsg = "";
            return errmsg;
        }

        /*
        含字軌
        範例：
        QQ12345678
        */
        public string InvoiceNumberType_Checked(InvoiceNumberType n)  {
            string errmsg = "發票編號[" + n + "]格式錯誤!!!!\t";

            if (n.Length == 10 && n.Substring(0, 1).Contains(Aletter) && n.Substring(1, 2).Contains(Aletter)) {
                UInt16 s = 0;
                bool isok= UInt16.TryParse(n.Substring(2,8), out s);
                if (isok) errmsg = "";
            }
            return errmsg;
        }

        /*
        0：非捐贈發票
        1：捐贈發票境外電商營業人可選擇提供買受人於交易前或交易時以捐贈碼索取雲端發票，參閱註。
        */
        public string DonateMarkEnum_Checked(DonateMarkEnum n) {
            string errmsg = "捐贈註記[" + n + "]格式錯誤!!!!\t";
            if (n.Length == 1 && n.Contains("01")) errmsg = "";
            return errmsg;
        }


        /*
        1：應稅
        2：零稅率
        3：免稅
        4：應稅(特種稅率)
        9：混合應稅與免稅或零稅率(限訊
        息 C0401 使用)
        */
        public string TaxTypeEnum_Checked(TaxTypeEnum n) {
            string errmsg = "稅率註記[" + n + "]格式錯誤!!!!\t";
            if (n.Length == 1 && n.Contains("12349")) errmsg = "";
            return errmsg;
        }

        /*
        載具類別號碼為6碼， 共通性載
        具類別號碼如下：
        1. 手機條碼為3J0002
        2. 自然人憑證條碼為CQ0001
        境外電商營業人：• 依照統一發票使用辦法第 7-1 條第 2 款規定「本法第 6 條第 4 款所定營業人應開立雲端發票交付買受人」
        • 應提供買受人以財政部核准之跨境電商電子郵件載具(即消費者之電子郵件信箱)索取雲端發票，填入載具類別號碼為： 5G0001
        • 可選擇提供買受人以共通性載具索取雲端發票

        手機條碼
            由Code39組成，總長度為8碼字元
            第一碼必為『/』
            其餘七碼則由數字【0-9】、大寫英文【A-Z】與特殊符號【.】【-】【+】組成
            手機條碼申請
        自然人憑證條碼CQ0001
            總長度為16碼字元
            前兩碼為大寫英文【A-Z】
            後14碼為數字【0-9】
            自然人憑證申請
        */

        public string CarrierTypeEnum_Checked(string n,string CarrierId1, string CarrierId2, string PrintMark, string BuyerId ) {
            string result = "載具類別號碼[" + n + "]格式錯誤!!!!\t";
            if (n == "" && CarrierId1 == "" && CarrierId2 == "") result = "";
            else
            {
                string[] msgtype = new string[] { "3J0002", "CQ0001", "5G0001" };
                 foreach(string val in msgtype) {
                    if (n == val) {
                        result = carriervalue(n, CarrierId1, CarrierId2, PrintMark, BuyerId);
                        break;
                    }
                }
            }
            return result;
        }

        public string carriervalue(string CarrierType, string CarrierId1, string CarrierId2, string PrintMark, string BuyerId)  {
            string errmsg = "";
            switch (CarrierType) {
            case "3J0002":
                errmsg = c3J0002(CarrierId1);
                    if (errmsg == "") errmsg = c3J0002(CarrierId2);
                    break;
            case "CQ0001":
                    errmsg = cCQ0001(CarrierId1);
                    if (errmsg == "") errmsg = cCQ0001(CarrierId2);
                    break;
            case "5G0001":
                    errmsg = c5G0001(CarrierId1);
                    if (errmsg == "") errmsg = c5G0001(CarrierId2);
                    break;
            }
            if (BuyerId == "0000000000") {
                if (PrintMark != "N") errmsg += "PrintMark需等於[N]\t";
            }
            else if (PrintMark != "Y") errmsg += "PrintMark需等於[Y]\t";
            return errmsg;
        }

        public string c3J0002(string CarrierId ){
            string errmsg = "";
            if (CarrierId.Length != 8) errmsg = "CarrierId:長度錯誤!!!\t";
            else if (CarrierId.Substring(0, 1) != "/") errmsg = "CarrierId:編碼錯誤!!!\t";
            else
            {
                for (int v = 1; v < 8; v++) {
                    if (!CarrierId.Substring(v, v + 1).Contains(V3j0002)) {
                        errmsg = "CarrierId:編碼錯誤!!!\t";
                        break;
                    }
                }
            }
            return errmsg;
        }
        public string cCQ0001(string CarrierId)  {
            string errmsg = "";
            if (CarrierId.Length != 16) errmsg = "CarrierId:長度錯誤!!!\t";
            else
            {
                for (int i = 0; i < 16; i++ ){
                    string v = CarrierId.Substring(i, 1);
                    if (i < 2) {
                        if (!v.Contains(Aletter)) {
                            errmsg = "CarrierId:編碼錯誤!!!\t";
                        break;
                        }
                    }
                    else if (!v.Contains(Nletter)) {
                        errmsg = "CarrierId:編碼錯誤!!!\t";
                        break;
                    }
                }
            }
            return errmsg;
        }
        public string c5G0001(string NpoBan ) {
            string errmsg = "";
            int counts = NpoBan.Length;
            if (counts < 3 || counts > 7)  errmsg = "愛心編碼錯誤長度錯誤!!!\t";
            else
            {
                for (int i = 0; i < counts; i++) {
                    string v = NpoBan.Substring(i, 1);
                    if (!v.Contains(Nletter)) {
                        errmsg = "愛心編碼錯誤!!!\t";
                        break;
                    }
                }
            }
            return errmsg;
        }

        /*捐贈碼5G0001
          總長度為3至7碼字元
          全部由數字【0-9】組成
          捐贈碼查詢
        */
        public string NpoBanCheck(string NpoBan, string DonateMark, string PrintMark ) {
            string errmsg = "";
            if (DonateMark == "1") {
                errmsg = c5G0001(NpoBan);
                if (PrintMark != "N") errmsg += "PrintMark需等於[N]\t";
            }
            return errmsg;
        }

        /*
        /台灣時區僅支援西元年方式
        例:20170101*/
        public string DateType_Checked(DateType n)  
        {
            string errmsg = "日期[" + n + "]格式錯誤!!!!\t";
            if (n.Length == 8)
            {
                string tmpdate=n.Substring(0,4)+"/"+ n.Substring(4, 2) + "/"+ n.Substring(6, 2);
                DateTime dt;
                if(DateTime.TryParse(tmpdate,out dt))
                {
                    errmsg = "";
                }
            }
            return errmsg;
        }

        public string TimeType_Checked(TimeType n)  {
            string errmsg = "時間格式[" + n + "]格式錯誤!!!!\t";
            DateTime dt;
            if (DateTime.TryParse(n, out dt))
            {
                errmsg = "";
            }
            return errmsg;
        }

        /*
        1： 符合加值型及非加值型營業稅法第 7 條第 4 款規定(買受人為保稅區營業人)
        2： 符合加值型及非加值型營業稅法第 7 條第 7 款規定(買受人為遠洋漁業營業人)
        3:符合自由貿易港區設置管理條例第28條第1項第1款及第 4 款規定(買受人為自由貿易港區營業人)
        4.其他
        */
        public string BondedAreaEnum_Checked(BondedAreaEnum n)  {
                    string errmsg = "買受人簽署適用零稅率註記[" + n + "]格式錯誤!!!!\t";
                    if (n == "") errmsg = "";
                    else if (n.Length == 1 && n.Contains("1234")) errmsg = "";
                    return errmsg;
        }

        /*value="[0,0.01,0.02,0.05,0.15,0.2 5]"*/
        public string TaxRateEnum_Checked(TaxRateEnum n)  {
            string errmsg = "稅率欄位[" + n.ToString() + "]格式錯誤!!!!\t";
            if (n > -1 && n <= Convert.ToDecimal(0.25)) errmsg = "";
            return errmsg;
        }


        public string ZeroTaxRateReasonEnum_Checked(ZeroTaxRateReasonEnum n) {
            string errmsg = "零稅率原因[" + n + "]格式錯誤!!!!\t";
            if (n == "") errmsg = "";
            else if (n.Length == 2 && n.Contains("71,72,73,74,75,76,77,78,79")) errmsg = "";
            return errmsg;
        }

        public string CurrencyCodeEnum_Checked(CurrencyCodeEnum n)  {
            string errmsg = "幣別類型[" + n + "]格式錯誤!!!!\t";
            if (n == "") errmsg = "";
            else if(n.Length == 3) {
                n = n.ToUpper();
                Dictionary<string, string> currency = new Dictionary<string, string>();
                currency.Add("AED", "United Arab Emirates,Dirhams");
                currency.Add("AFN", "Afghanistan, Afghanis");
                currency.Add("ALL", "Albania, Leke");
                currency.Add("AMD", "Armenia, Drams");
                currency.Add("ANG", "Netherlands Antilles,Guilders (also called Florins)");
                currency.Add("AOA", "Angola, Kwanza");
                currency.Add("ARS", "Argentina, Pesos");
                currency.Add("AUD", "Australia, Dollars");
                currency.Add("AWG", "Aruba, Guilders (alsocalled Florins)");
                currency.Add("AZN", "Azerbaijan, New Manats");
                currency.Add("BAM", "Bosnia and Herzegovina,Convertible Marka");
                currency.Add("BBD", "Barbados, Dollars");
                currency.Add("BDT", "Bangladesh, Taka");
                currency.Add("BGN", "Bulgaria, Leva");
                currency.Add("BHD", "Bahrain, Dinars");
                currency.Add("BIF", "Burundi, Francs");
                currency.Add("BMD", "Bermuda, Dollars");
                currency.Add("BND", "Brunei Darussalam,Dollars");
                currency.Add("BOB", "Bolivia, Bolivianos");
                currency.Add("BRL", "Brazil, Brazil Real");
                currency.Add("BSD", "Bahamas, Dollars");
                currency.Add("BTN", "Bhutan, Ngultrum");
                currency.Add("BWP", "Botswana, Pulas");
                currency.Add("BYR", "Belarus, Rubles");
                currency.Add("BZD", "Belize, Dollars");
                currency.Add("CAD", "Canada, Dollars");
                currency.Add("CDF", "Congo/Kinshasa,Congolese Francs");
                currency.Add("CHF", "Switzerland, Francs");
                currency.Add("CLP", "Chile, Pesos");
                currency.Add("CNY", "China, Yuan Renminbi");
                currency.Add("COP", "Colombia, Pesos");
                currency.Add("CRC", "Costa Rica, Colones");
                currency.Add("CUP", "Cuba, Pesos");
                currency.Add("CVE", "Cape Verde, Escudos");
                currency.Add("CYP", "Cyprus, Pounds (expires 2008-Jan-31)");
                currency.Add("CZK", "Czech Republic, Koruny");
                currency.Add("DJF", "Djibouti, Francs");
                currency.Add("DKK", "Denmark, Kroner");
                currency.Add("DOP", "Dominican Republic, Pesos");
                currency.Add("DZD", "Algeria, Algeria Dinars");
                currency.Add("EGP", "Egypt, Pounds");
                currency.Add("ERN", "Eritrea, Nakfa");
                currency.Add("ETB", "Ethiopia, Birr");
                currency.Add("EUR", "Euro Member Countries,Euro");
                currency.Add("FJD", "Fiji, Dollars");
                currency.Add("FKP", "Falkland Islands (Malvinas), Pounds");
                currency.Add("GBP", "United Kingdom, Pounds");
                currency.Add("GEL", "Georgia, Lari");
                currency.Add("GGP", "Guernsey, Pounds");
                currency.Add("GHS", "Ghana, Cedis");
                currency.Add("GIP", "Gibraltar, Pounds");
                currency.Add("GMD", "Gambia, Dalasi");
                currency.Add("GNF", "Guinea, Francs");
                currency.Add("GTQ", "Guatemala, Quetzales");
                currency.Add("GYD", "Guyana, Dollars");
                currency.Add("HKD", "Hong Kong, Dollars");
                currency.Add("HNL", "Honduras, Lempiras");
                currency.Add("HRK", "Croatia, Kuna");
                currency.Add("HTG", "Haiti, Gourdes");
                currency.Add("HUF", "Hungary, Forint");
                currency.Add("IDR", "Indonesia, Rupiahs");
                currency.Add("ILS", "Israel, New Shekels");
                currency.Add("IMP", "Isle of Man, Pounds");
                currency.Add("INR", "India, Rupees");
                currency.Add("IQD", "Iraq, Dinars");
                currency.Add("IRR", "Iran, Rials");
                currency.Add("ISK", "Iceland, Kronur");
                currency.Add("JEP", "Jersey, Pounds");
                currency.Add("JMD", "Jamaica, Dollars");
                currency.Add("JOD", "Jordan, Dinars");
                currency.Add("JPY", "Japan, Yen");
                currency.Add("KES", "Kenya, Shillings");
                currency.Add("KGS", "Kyrgyzstan, Soms");
                currency.Add("KHR", "Cambodia, Riels");
                currency.Add("KMF", "Comoros, Francs");
                currency.Add("KPW", "Korea (North), Won");
                currency.Add("KRW", "Korea (South), Won");
                currency.Add("KWD", "Kuwait, Dinars");
                currency.Add("KYD", "Cayman Islands, Dollars");
                currency.Add("KZT", "Kazakhstan, Tenge");
                currency.Add("LAK", "Laos, Kips");
                currency.Add("LBP", "Lebanon, Pounds");
                currency.Add("LKR", "Sri Lanka, Rupees");
                currency.Add("LRD", "Liberia, Dollars");
                currency.Add("LSL", "Lesotho, Maloti");
                currency.Add("LTL", "Lithuania, Litai");
                currency.Add("LVL", "Latvia, Lati");
                currency.Add("LYD", "Libya, Dinars");
                currency.Add("MAD", "Morocco, Dirhams");
                currency.Add("MDL", "Moldova, Lei");
                currency.Add("MGA", "Madagascar, Ariary");
                currency.Add("MKD", "Macedonia, Denars");
                currency.Add("MMK", "Myanmar (Burma), Kyats");
                currency.Add("MNT", "Mongolia, Tugriks");
                currency.Add("MOP", "Macau, Patacas");
                currency.Add("MRO", "Mauritania, Ouguiyas");
                currency.Add("MTL", "Malta, Liri (expires 2008-Jan-31)");
                currency.Add("MUR", "Mauritius, Rupees");
                currency.Add("MVR", "Maldives (Maldive Islands), Rufiyaa");
                currency.Add("MWK", "Malawi, Kwachas");
                currency.Add("MXN", "Mexico, Pesos");
                currency.Add("MYR", "Malaysia, Ringgits");
                currency.Add("MZN", "Mozambique, Meticais");
                currency.Add("NAD", "Namibia, Dollars");
                currency.Add("NGN", "Nigeria, Nairas");
                currency.Add("NIO", "Nicaragua, Cordobas");
                currency.Add("NOK", "Norway, Krone");
                currency.Add("NPR", "Nepal, Nepal Rupees");
                currency.Add("NZD", "New Zealand, Dollars");
                currency.Add("OMR", "Oman, Rials");
                currency.Add("PAB", "Panama, Balboa");
                currency.Add("PEN", "Peru, Nuevos Soles");
                currency.Add("PGK", "Papua New Guinea, Kina");
                currency.Add("PHP", "Philippines, Pesos");
                currency.Add("PKR", "Pakistan, Rupees");
                currency.Add("PLN", "Poland, Zlotych");
                currency.Add("PYG", "Paraguay, Guarani");
                currency.Add("QAR", "Qatar, Rials");
                currency.Add("RON", "Romania, New Lei");
                currency.Add("RSD", "Serbia, Dinars");
                currency.Add("RUB", "Russia, Rubles");
                currency.Add("RWF", "Rwanda, Rwanda Francs");
                currency.Add("SAR", "Saudi Arabia, Riyals");
                currency.Add("SBD", "Solomon Islands, Dollars");
                currency.Add("SCR", "Seychelles, Rupees");
                currency.Add("SDG", "Sudan, Pounds");
                currency.Add("SEK", "Sweden, Kronor");
                currency.Add("SGD", "Singapore, Dollars");
                currency.Add("SHP", "Saint Helena, Pounds");
                currency.Add("SLL", "Sierra Leone, Leones");
                currency.Add("SOS", "Somalia, Shillings");
                currency.Add("SPL", "Seborga, Luigini");
                currency.Add("SRD", "Suriname, Dollars");
                currency.Add("STD", "Sao Tome and Principe,Dobras");
                currency.Add("SVC", "El Salvador, Colones");
                currency.Add("SYP", "Syria, Pounds");
                currency.Add("SZL", "Swaziland, Emalangeni");
                currency.Add("THB", "Thailand, Baht");
                currency.Add("TJS", "Tajikistan, Somoni");
                currency.Add("TMM", "Turkmenistan, Manats");
                currency.Add("TND", "Tunisia, Dinars");
                currency.Add("TOP", "Tonga, Pa'anga");
                currency.Add("TRY", "Turkey, New Lira");
                currency.Add("TTD", "Trinidad and Tobago,Dollars");
                currency.Add("TVD", "Tuvalu, Tuvalu Dollars");
                currency.Add("TWD", "Taiwan, New Dollars");
                currency.Add("TZS", "Tanzania, Shillings");
                currency.Add("UAH", "Ukraine, Hryvnia");
                currency.Add("UGX", "Uganda, Shillings");
                currency.Add("USD", "United States of America,Dollars");
                currency.Add("UYU", "Uruguay, Pesos");
                currency.Add("UZS", "Uzbekistan, Sums");
                currency.Add("VEB", "Venezuela, Bolivares(expires 2008-Jun-30)");
                currency.Add("VEF", "Venezuela, Bolivares Fuertes");
                currency.Add("VND", "Viet Nam, Dong");
                currency.Add("VUV", "Vanuatu, Vatu");
                currency.Add("WST", "Samoa, Tala");
                currency.Add("XAF", "Communaute Financiere Africaine BEAC, Francs");
                currency.Add("XAG", "Silver, Ounces");
                currency.Add("XAU", "Gold, Ounces");
                currency.Add("XCD", "East Caribbean Dollars");
                currency.Add("XDR", "International Monetary Fund (IMF) Special Drawing Rights");
                currency.Add("XOF", "Communaute Financiere Africaine BCEAO, Francs");
                currency.Add("XPD", "Palladium Ounces");
                currency.Add("XPF", "Comptoirs Francais du Pacifique Francs");
                currency.Add("XPT", "Platinum, Ounces");
                currency.Add("YER", "Yemen, Rials");
                currency.Add("ZAR", "South Africa, Rand");
                currency.Add("ZMK", "Zambia, Kwacha");
                currency.Add("ZWD", "Zimbabwe, Zimbabwe Dollars");
                if (currency.ContainsKey(n)) errmsg = "";
		    }
            return errmsg;
        }



        struct MonthInvoice  {
            public Int64 IstarNo;
            public Int64 IsendNo;
            public Int64 IcurrentNo;
        }

        public string CurrentInvoce(string invoice, string date)  {
            string errmsg = "發票:" + invoice + "非當月使用中電子發票!!!!";
            UInt32 ii = 0;
            if(UInt32.TryParse(invoice.Substring(2,8), out ii))
            {
                string sqlstring = $"SELECT SUBSTRING(MB006,3,8) MB006, SUBSTRING(MB007,3,8) MB007,SUBSTRING(MB008,3,8) MB008 FROM [TAXMB] WHERE SUBSTRING(MB006, 1, 2)='{invoice.Substring(0,2)}' AND (MB003='{date}' OR MB003='{date}') AND SUBSTRING(MB008, 3, 8)>='{invoice.Substring(2, 8)}'";
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand(sqlstring, conn1);
                cmd1.Connection.Open();
                SqlDataReader sdr = cmd1.ExecuteReader();
                while (sdr.Read())
                {
                    MonthInvoice tmp = new MonthInvoice();
                    tmp.IstarNo = Convert.ToInt64(sdr[0]);
                    tmp.IsendNo = Convert.ToInt64(sdr[1]);
                    tmp.IcurrentNo = Convert.ToInt64(sdr[2]);
                    if (inno >= tmp.IstarNo && inno <= tmp.IsendNo) {
                        errmsg = "";
                        break;
                    }
                }
            }

            return errmsg;
        }

public string Uppack(string mig)  {
	//mig = `Z:\MP4500SCAN\`
	switch (mig) {
	        case "A0101":
                    mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\A0101\SRC\";
                    break;
	        case "A0102":
		        mig =  @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\A0102\SRC\";
                    break;
                case "A0201":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\A0201\SRC\";
                    break;
                case "A0202":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\A0202\SRC\";
                    break;
                case "A0301":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\A0301\SRC\";
                    break;
                case "A0302":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\A0303\SRC\";
                    break;
                case "B0101":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\B0101\SRC\";
                    break;
                case "B0102":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\B0102\SRC\";
                    break;
                case "B0201":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\B0201\SRC\";
                    break;
                case "B0202":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BEXCHANGE\B0202\SRC\";
                    break;
                case "A0401":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BSTORAGE\A0401\SRC\";
                    break;
                case "A0501":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BSTORAGE\A0501\SRC\";
                    break;
                case "B0501":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BSTORAGE\B0501\SRC\";
                    break;
                case "B0401":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BSTORAGE\B0401\SRC\";
                    break;
                case "A0601":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2BSTORAGE\A0601\SRC\";
                    break;
                case "C0401":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2CSTORAGE\C0401\SRC\";
                    break;
                case "C0501":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2CSTORAGE\C0501\SRC\";
                    break;
                case "C0701":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2CSTORAGE\C0701\SRC\";
                    break;
                case "D0401":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2CSTORAGE\D0401\SRC\";
                    break;
                case "D0501":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2CSTORAGE\D0501\SRC\";
                    break;
                case "E0401":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2PMESSAGE\E0401\SRC\";
                    break;
                case "E0402":
		        mig = @"C:\Program Files\EINVTurnkey\UpCast\B2PMESSAGE\E0402\SRC\";
                    break;
                case "E0501":
                    mig = @"C:\Program Files\EINVTurnkey\UpCast\B2PMESSAGE\E0501\SRC\";
                    break;
            }
	        return mig;
        }

    }
}
