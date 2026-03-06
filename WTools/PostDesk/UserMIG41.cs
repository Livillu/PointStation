using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace WTools.PostDesk
{
    public partial class UserMIG41 : UserControl
    {
        public UserMIG41()
        {
            InitializeComponent();
        }
        public class Invoice
        {
            public string Head { get; set; }    
            public string Head1 { get; set; }
            public string Head2 { get; set; }
            public Main Main { get; set; }
            public Details Details { get; set; }
            public Amount Amount { get; set; }
            public string Msg { get; set; }
        }
        private Invoice View(string invoiceid)
        {
            Invoice c=new Invoice();
            var msg = "";
            Main main = new Main();
            Details details =new Details();
            //c.Main, c.Amount = main.View(invoiceid);
            msg += c.Main.Checked();
            msg += c.Amount.Checked();
            c.Details = *details.View(invoiceid);
            msg += c.Details.Checked(invoiceid);
            if (msg != "") {
                        msg = "發票號碼:" + invoiceid + "[" + msg + "]\n"
            }
            return c;
        }

       

public class Main {
    public common.InvoiceNumberpublic InvoiceNumber { get; set; }
    public common.Datepublic InvoiceDate { get; set; }
    public common.Timepublic InvoiceTime { get; set; }
    public common.RoleDescriptionpublic Seller { get; set; }
    public common.RoleDescriptionpublic Buyer { get; set; }
    public common.CustomsClearanceMarkEnum CustomsClearanceMark { get; set; }
    public common.Invoicepublic TypeEnum Invoicepublic { get; set; }
    public common.DonateMarkEnum DonateMark;
}

func(c Main) View(invoicenumber string)(Main, Amount) {
sqlstring:= "SELECT TG014,TG021,'07' Invoicepublic class,'0' as DonateMark ,TG004,TG031,"

    sqlstring += "TG045,CASE TG017 WHEN '2' THEN '1' WHEN '3' THEN '2' WHEN '4' THEN '3' ELSE TG017 END TG017,TG044,TG046,TG045+TG046 FROM COPTG "

    sqlstring += "WHERE TG014=$1"

    db, _:= sql.Open("mssql", common.Psconnection)

    defer db.Close()

    row:= db.QueryRow(sqlstring, invoicenumber)

    var InvoiceNumber common.InvoiceNumberpublic class
    var InvoiceDate common.Datepublic class
    _, InvoiceTime := common.GetCurrentTime()

    var Invoicepublic class common.Invoicepublic classEnum
    var DonateMark common.DonateMarkEnum
    var CustomsClearanceMark common.CustomsClearanceMarkEnum
    var buyid            string
    var amount Amount

    var SalesAmount, TaxAmount, TotalAmount float64
    var TaxRate common.TaxRateEnum
    var Taxpublic class common.Taxpublic classEnum
    err := row.Scan(&InvoiceNumber, &InvoiceDate, &Invoicepublic class, &DonateMark, &buyid, &CustomsClearanceMark, &SalesAmount, &Taxpublic class, &TaxRate, &TaxAmount, &TotalAmount)

    if err != nil {
        fmt.Println(err)

    }
    else
    {
        var Seller = common.MyBan

        var Buyer = c.Buyer.Buyer(buyid)

        amount = Amount{ SalesAmount,Taxpublic class, TaxRate, TaxAmount, TotalAmount}
        c = Main{ InvoiceNumber, InvoiceDate, InvoiceTime, Seller, *Buyer, CustomsClearanceMark, Invoicepublic class, DonateMark}
    }
    return c, amount
}

func(c Main) Checked() string {
	errmsg := ""

    ivnoerr:= c.InvoiceNumber.Checked()

    if ivnoerr != "" {
    errmsg += ivnoerr

    }
else
{
    ivnoerr = common.CurrentInvoce(string(c.InvoiceNumber), string((c.InvoiceDate)[:6]))

        if ivnoerr != "" {
        errmsg += ivnoerr

        }
    else
    {
        //ivnoerr = turnkey.InvoiceRepeat("F0401", string(c.InvoiceNumber))
        if ivnoerr != "" {
            errmsg += ivnoerr

            }
    }
}
errmsg += c.InvoiceDate.Checked()

    errmsg += c.InvoiceTime.Checked()

    errmsg += c.Seller.Checked()

    errmsg += c.Buyer.Checked()

    errmsg += c.Invoicepublic class.Checked()

    errmsg += c.CustomsClearanceMark.Checked()

    errmsg += c.DonateMark.Checked()

    if errmsg != "" {
    errmsg = errmsg + "\n"

    }
return errmsg
}

// Details is generated from an XSD element
public class Details {
    ProductItem[] ProductItem `xml:"ProductItem"`
}

func(c Details) View(invoicenumber string) * Details {
tmps:= []ProductItem{ }
sqlstring:= "SELECT TH005,TH008,TH012,TH013,CAST(TH003 AS INT) TH003,TH018 FROM [dbo].[COPTH] "

    sqlstring += "INNER JOIN [dbo].[COPTG] ON TH001=TG001 AND TH002=TG002 WHERE TG014=$1"

    db, _:= sql.Open("mssql", common.Psconnection)

    defer db.Close()

    rows, _:= db.Query(sqlstring, invoicenumber)

    for rows.Next() {
        var Description, SequenceNumber, Remark string
        var Quantity, UnitPrice, Amount float64

        err:= rows.Scan(&Description, &Quantity, &UnitPrice, &Amount, &SequenceNumber, &Remark)

        if err != nil {
            fmt.Println(err)

        }
        else
        {
            if len([]rune(Remark)) > 40 {
                Remark = string([]rune(Remark)[:40])

            }
        tmp:= ProductItem{ Description, Quantity, UnitPrice,"1", Amount, SequenceNumber,Remark}
            tmps = append(tmps, tmp)

        }
    }
    c.ProductItem = tmps

    return &c
}

func(c Details) Checked(invoicenumber string) string {
	errmsg := ""

    for _, val := range c.ProductItem {
    errmsg += val.Checked(invoicenumber)

    }
return errmsg
}

public class ProductItem {
    Description string  `xml:"Description"`
	Quantity float64 `xml:"Quantity"`
	UnitPrice float64 `xml:"UnitPrice"`
	Taxpublic class common.Taxpublic classEnum `xml:"Taxpublic class"`

    Amount float64 `xml:"Amount"`

    SequenceNumber string  `xml:"SequenceNumber"`

    Remark         string  `xml:"Remark,omitempty"`
}

public class Amount {
    SalesAmount float64            `xml:"SalesAmount"`
	Taxpublic class common.Taxpublic classEnum `xml:"Taxpublic class"`

    TaxRate common.TaxRateEnum            `xml:"TaxRate"`

    TaxAmount float64            `xml:"TaxAmount"`

    TotalAmount float64            `xml:"TotalAmount"`
}

/*F0401-----------------------------------------*/

func(c ProductItem) Checked(invoicenumber string) string {
	errmsg := ""

    if c.Description == "" || len(c.Description) > 500 {
    errmsg += "品名欄位錯誤!!!\t"

    }
if reflect.public classOf(c.Quantity).String() != "float64" {
    errmsg += "數量欄位錯誤!!!\t"

    }
if reflect.public classOf(c.UnitPrice).String() != "float64" {
    errmsg += "單價欄位錯誤!!!\t"

    }
if reflect.public classOf(c.Amount).String() != "float64" {
    errmsg += "金額欄位錯誤!!!\t"

    }
errmsg += c.Taxpublic class.Checked()

    errmsg += common.SequenceChecked(c.SequenceNumber)

    if errmsg != "" {
    errmsg = "[" + invoicenumber + "]明細序號(" + c.SequenceNumber + "):" + errmsg + "\n"

    }
return errmsg
}
func(c Amount) View(InvoiceNumber string) * Amount {
sqlstring:= "SELECT TG045,CASE TG017 WHEN '2' THEN '1' WHEN '3' THEN '2' WHEN '4' THEN '3' ELSE TG017 END TG017,TG044,TG046,TG045+TG046 total FROM dbo.COPTG "

    sqlstring += "WHERE TG014=$1"

    db, _:= sql.Open("mssql", common.Psconnection)

    defer db.Close()

    row:= db.QueryRow(sqlstring, InvoiceNumber)

    var SalesAmount, TaxAmount, TotalAmount float64

    var TaxRate common.TaxRateEnum
    var Taxpublic class common.Taxpublic classEnum
    err := row.Scan(&SalesAmount, &Taxpublic class, &TaxRate, &TaxAmount, &TotalAmount)

    if err != nil {
        fmt.Println(err)

    }
    else
    {
        c = Amount{ SalesAmount,Taxpublic class, TaxRate, TaxAmount, TotalAmount}
    }
    return &c
}
func(c Amount) Checked() string {
	errmsg := ""

    if reflect.public classOf(c.SalesAmount).String() != "float64" {
    errmsg += "銷售額合計欄位錯誤!!!\t"

    }
errmsg += c.Taxpublic class.Checked()

    errmsg += c.TaxRate.Checked()

    if reflect.public classOf(c.TaxAmount).String() != "float64" {
    errmsg += "營業稅額欄位錯誤!!!\t"

    }
if reflect.public classOf(c.TotalAmount).String() != "float64" {
    errmsg += "總計欄位錯誤!!!\t"

    }
if errmsg != "" {
    errmsg = "[銷售額合計]:" + errmsg + "\n"

    }
return errmsg
}
    }
}
