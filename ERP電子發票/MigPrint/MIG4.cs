
using ScottPlot.Colormaps;
using ScottPlot.Statistics;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Schema;
using System.Xml.Serialization;
using AllowanceNumberType = System.String;
using AllowanceTypeEnum = System.String;
using BAN = System.String;
using BondedAreaEnum = System.String;
using BuyerRemarkEnum = System.String;
using CarrierTypeEnum = System.String;
using CurrencyCodeEnum = System.String;
using CustomsClearanceMarkEnum = System.String;
using DateType = System.String;
using DonateMarkEnum = System.String;
using InvoiceNumberType = System.String;
using InvoiceTypeEnum = System.String;
using MessageTypeEnum = System.String;
using TaxRateEnum = System.Decimal;
using TaxTypeEnum = System.String;
using TimeType = System.String;
using ZeroTaxRateReasonEnum = System.String;
namespace WTools.MigPrint
{
    public class MIG4
    {
        public class F0401
        {
            public class MsgResult
                {
                    public string msg {  get; set; }
                    public Invoice data { get; set; }
                }
            public class Invoice
            {
                [XmlAttribute("schemaLocation", Namespace = XmlSchema.InstanceNamespace)]
                public string xsiSchemaLocation = "urn:GEINV:eInvoiceMessage:F0401:4.1 F0401.xsd";
                public Main Main { set; get; }
                public List<ProductItem> Details { set; get; }
                public Amount Amount { set; get; }
            }
            public class Main
            {
                public InvoiceNumberType InvoiceNumber { set; get; }
                public DateType InvoiceDate { set; get; }
                public TimeType InvoiceTime { set; get; }
                public RoleDescriptionType Seller { set; get; }
                public RoleDescriptionType Buyer { set; get; }
                public InvoiceTypeEnum InvoiceType { set; get; }
                public DonateMarkEnum DonateMark { set; get; }
                public string PrintMark { set; get; }
            }
            /*public class Details
            {
                public List<ProductItem> ProductItem;//`xml:"ProductItem"`
            }*/
            public class ProductItem
            {
                public string Description { set; get; }//   `xml:"Description"`
                public decimal Quantity { set; get; }// float64 `xml:"Quantity"`
                public decimal UnitPrice { set; get; }// float64 `xml:"UnitPrice"`
                public TaxTypeEnum TaxType { set; get; }// `xml:"TaxType"`
                public decimal Amount { set; get; }// float64 `xml:"Amount"`
                public string SequenceNumber { set; get; }// string  `xml:"SequenceNumber"`
                public string Remark { set; get; }//   `xml:"Remark,omitempty"`
            }
            public class Amount
            {
                public decimal SalesAmount { set; get; }// float64            `xml:"SalesAmount"`
                public decimal FreeTaxSalesAmount { set; get; }
                public decimal ZeroTaxSalesAmount { set; get; }
                public TaxTypeEnum TaxType { set; get; }// `xml:"Taxpublic class"`
                public TaxRateEnum TaxRate { set; get; }//            `xml:"TaxRate"`
                public decimal TaxAmount { set; get; }// float64            `xml:"TaxAmount"`
                public decimal TotalAmount { set; get; }// float64            `xml:"TotalAmount"`
            }
            private Main GetMain(string invoicenumber)
            {
                Main main = new Main();
                SqlConnection conn1 = new SqlConnection(MainForm.WP01);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                string sqlstring = "SELECT TG014,TG021,'07' InvoiceType,'0' as DonateMark ,TG004";
                sqlstring += ",CASE TG017 WHEN '2' THEN '1' WHEN '3' THEN '2' WHEN '4' THEN '3' ELSE TG017 END TG017";
                sqlstring += $" FROM COPTG WHERE TG014='{invoicenumber}'";
                cmd1.CommandText = sqlstring;
                SqlDataReader sdr= cmd1.ExecuteReader();
                string InvoiceTime = DateTime.Now.ToString("HH:mm:ss");
                if (sdr.Read()) {
                    var Seller = MigCommon.MyBan();
                    var Buyer = MigCommon.Buyer(sdr["TG004"].ToString());
                    main.InvoiceNumber = sdr["TG014"].ToString(); 
                    main.InvoiceDate = sdr["TG021"].ToString();
                    main.InvoiceTime = InvoiceTime;
                    main.Seller = Seller;
                    main.Buyer = Buyer;
                    main.PrintMark = "Y";
                    main.InvoiceType = sdr["InvoiceType"].ToString();
                    main.DonateMark = sdr["DonateMark"].ToString();
                }
                return main;
            }
            private List<ProductItem> GetDetails(string invoiceid)
            {
                List<ProductItem> details = new List<ProductItem>();
                string sqlstring = "SELECT TH005,TH008,TH012,TH013,CAST(TH003 AS INT) TH003,TH018 FROM [COPTH] ";
                sqlstring += $"INNER JOIN [COPTG] ON TH001=TG001 AND TH002=TG002 WHERE TG014='{invoiceid}'";
                SqlConnection conn1 = new SqlConnection(MainForm.WP01);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                cmd1.CommandText = sqlstring;
                SqlDataReader sdr = cmd1.ExecuteReader();
                while (sdr.Read())
                { 
                    ProductItem tmp = new ProductItem();
                    if (sdr["TH018"].ToString().Length > 40) {
                        tmp.Remark = sdr["TH018"].ToString().Substring(0, 40);               
                    }
                    tmp.Description = sdr["TH005"].ToString();
                    tmp.Quantity=Convert.ToDecimal(sdr["TH008"]);
                    tmp.UnitPrice= Convert.ToDecimal(sdr["TH012"]);
                    tmp.TaxType = "1";
                    tmp.Amount= Convert.ToDecimal(sdr["TH013"]);
                    tmp.SequenceNumber= sdr["TH003"].ToString();
                    details.Add(tmp);
                }
                return details;
            }
            private Amount GetAmount(string invoicenumber)
            {
                Amount amount = new Amount();
                SqlConnection conn1 = new SqlConnection(MainForm.WP01);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                string sqlstring = $"SELECT TG031,TG045,TG044,TG046,TG045+TG046 TG047 FROM COPTG WHERE TG014='{invoicenumber}'";
                cmd1.CommandText = sqlstring;
                SqlDataReader sdr = cmd1.ExecuteReader();
                if (sdr.Read())
                {
                    amount.SalesAmount = Convert.ToDecimal(sdr["TG047"]);
                    amount.FreeTaxSalesAmount=0;
                    amount.ZeroTaxSalesAmount=0;
                    amount.TaxType = "1";
                    amount.TaxRate = Convert.ToDecimal(sdr["TG044"]);
                    amount.TaxAmount = Convert.ToDecimal(sdr["TG046"]);
                    amount.TotalAmount = Convert.ToDecimal(sdr["TG047"]);
                }
                return amount;
            }
            public MsgResult View(string invoiceid)
            {
                MsgResult result = new MsgResult();
                result.data = new Invoice();
                result.msg = "";
                result.data.Main = GetMain(invoiceid);
                result.data.Amount = GetAmount(invoiceid);
                result.data.Details = GetDetails(invoiceid);
                result.msg += Main_Checked(result.data.Main);
                result.msg += Amount_Checked(result.data.Amount);
                result.msg += Details_Checked(invoiceid, result.data.Details);
                if (result.msg != "")
                {
                    result.msg = "發票號碼:" + invoiceid + "[" + result.msg + "]\n";
                }
                return result;
            }
            public string Main_Checked(Main c)
            {
                string errmsg = "";
                string ivnoerr = MigCommon.InvoiceNumberType_Checked(c.InvoiceNumber);
                if (ivnoerr != "")
                {
                    errmsg += ivnoerr;
                }
                else
                {
                    /*
                    //檢查當月發票
                    ivnoerr = MigCommon.CurrentInvoce(c.InvoiceNumber, c.InvoiceDate.Substring(0, 6));
                    if (ivnoerr != "")
                    {
                        errmsg += ivnoerr;
                    }
                    else
                    {
                        //ivnoerr = turnkey.InvoiceRepeat("F0401", string(c.InvoiceNumber))
                        if (ivnoerr != "")
                        {
                            errmsg += ivnoerr;
                        }
                    }
                    */
                }
                errmsg += MigCommon.DateType_Checked(c.InvoiceDate);
                errmsg += MigCommon.TimeType_Checked(c.InvoiceTime);
                errmsg += MigCommon.RoleDescriptionType_Checked(c.Seller);
                errmsg += MigCommon.RoleDescriptionType_Checked(c.Buyer);
                errmsg += MigCommon.InvoiceTypeEnum_Checked(c.InvoiceType);
                //errmsg += MigCommon.CustomsClearanceMarkEnum_Checked(c.CustomsClearanceMark);
                errmsg += MigCommon.DonateMarkEnum_Checked(c.DonateMark);
                if (errmsg != "")
                {
                    errmsg = errmsg + "\n";
                }
                return errmsg;
            }
            public string Details_Checked(string invoicenumber, List<ProductItem> productItems)
            {
                string errmsg = "";
                foreach (ProductItem item in productItems)
                {
                    errmsg += ProductItem_Checked(invoicenumber,item);
                }
                return errmsg;
            }
            public string Amount_Checked(Amount c)
            {
                string errmsg = "";
                if (c.SalesAmount.GetType() != typeof(decimal))
                {
                    errmsg += "銷售額合計欄位錯誤!!!\t";
                }
                errmsg += MigCommon.TaxTypeEnum_Checked(c.TaxType);
                errmsg += MigCommon.TaxRateEnum_Checked(c.TaxRate);
                if (c.TaxAmount.GetType() != typeof(decimal))
                {
                    errmsg += "營業稅額欄位錯誤!!!\t";
                }
                if (c.TotalAmount.GetType() != typeof(decimal))
                {
                    errmsg += "總計欄位錯誤!!!\t";
                }
                if (errmsg != "")
                {
                    errmsg = "[銷售額合計]:" + errmsg + "\n";
                }
                return errmsg;
            }
            public string ProductItem_Checked(string invoicenumber, ProductItem c)
            {
                string errmsg = "";
                if (c.Description == "" || c.Description.Length > 500)
                {
                    errmsg += "品名欄位錯誤!!!\t";
                }
                if (c.Quantity.GetType() != typeof(decimal))
                {
                    errmsg += "數量欄位錯誤!!!\t";
                }
                if (c.UnitPrice.GetType() != typeof(decimal))
                {

                    errmsg += "單價欄位錯誤!!!\t";
                }
                if (c.Amount.GetType() != typeof(decimal))
                {
                    errmsg += "金額欄位錯誤!!!\t";
                }
                errmsg += MigCommon.TaxTypeEnum_Checked(c.TaxType);

                errmsg += MigCommon.SequenceChecked(c.SequenceNumber);

                if (errmsg != "")
                {
                    errmsg = "[" + invoicenumber + "]明細序號(" + c.SequenceNumber + "):" + errmsg + "\n";
                }
                return errmsg;
            }
        }
        public class F0501
        {          
                public class MsgResult
                {
                    public string msg { get; set; }
                    public CancelInvoice data { get; set; }
                }
                public class CancelInvoice
                {
                    [XmlAttribute("schemaLocation", Namespace = XmlSchema.InstanceNamespace)]
                    public string xsiSchemaLocation = "urn:GEINV:eInvoiceMessage:F0501:4.1 F0501.xsd";
                    public InvoiceNumberType CancelInvoiceNumber { get; set; }// common. `xml:"CancelInvoiceNumber"`
                    public DateType InvoiceDate { get; set; }// common.          `xml:"InvoiceDate"`

                    public BAN BuyerID { get; set; }// common.               `xml:"BuyerId"`

                    public BAN SellerID { get; set; }// common.BAN               `xml:"SellerId"`

                    public DateType CancelDate { get; set; }// common.          `xml:"CancelDate"`

                    public TimeType CancelTime { get; set; }// common.          `xml:"CancelTime"`

                    public string CancelReason { get; set; }//                           `xml:"CancelReason"`
                }
                public CancelInvoice CancelInvoice_View(string invoiceid) 
                {
                    string sqlstring = $"SELECT TG014,TG021,MODI_DATE,TG027 FROM [dbo].[COPTG] WHERE TG014='{invoiceid}'";
                    SqlConnection conn1 = new SqlConnection(MainForm.WP01);
                    SqlCommand cmd1 = new SqlCommand("", conn1);
                    cmd1.Connection.Open();
                    cmd1.CommandText = sqlstring;
                    SqlDataReader sdr = cmd1.ExecuteReader();
                    CancelInvoice data = new CancelInvoice();
                    if (sdr.Read())
                    {
                        data.CancelInvoiceNumber = sdr["TG014"].ToString();
                        data.InvoiceDate = sdr["TG021"].ToString();
                        data.BuyerID = "0000000000";
                        data.SellerID = MigCommon.SellerId;
                        data.CancelDate = sdr["MODI_DATE"].ToString();
                        data.CancelTime = DateTime.Now.ToString("HH:mm:ss");
                        data.CancelReason = sdr["TG027"].ToString();
                    }
                    return data;
                }
                public string CancelInvoice_Checked(CancelInvoice c)  {
                                    string errmsg = "";
                    errmsg += MigCommon.InvoiceNumberType_Checked(c.CancelInvoiceNumber);
                    errmsg += MigCommon.DateType_Checked(c.InvoiceDate);
                    errmsg += MigCommon.BAN_Checked(c.BuyerID);
                    errmsg += MigCommon.BAN_Checked(c.SellerID);
                    errmsg += MigCommon.DateType_Checked(c.CancelDate);
                    errmsg += MigCommon.TimeType_Checked(c.CancelTime);
                    int counts = c.CancelReason.Length;

                    if (counts < 1 || counts > 20) {
                        errmsg += "作廢原因欄位錯誤,長度不為空需小於等於20!!!\t";

                    }
                    return errmsg;
                }
        }
    }
}
