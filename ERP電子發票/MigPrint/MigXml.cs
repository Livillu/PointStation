using System.Xml;
using System.Xml.Serialization;
using static WTools.MigPrint.MIG4;
using static WTools.MigPrint.MIG4.F0401;
using static WTools.MigPrint.MIG4.F0501;

namespace WTools.MigPrint
{
    internal class MigXml
    {
        public static string F0401XML(string invoiceid)
        {
            F0401 invoice = new F0401();
            F0401.MsgResult result = new F0401.MsgResult();
            result = invoice.View(invoiceid);
            if(string.IsNullOrEmpty(result.msg)) result.msg = F0401ToXmls(result.data);
            /*//更新銷貨單狀態
            if errmsg == "" {
                        ChangState(invoiceid, "G")

            }
            */
            return result.msg;
        }
        public static string F0401ToXmls(Invoice data) 
        {
            string errmsg = "";
	        if(data.Main.InvoiceNumber != "") 
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true; // <-- 核心設定
                settings.Indent = true; // 可選：格式化縮排
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance"); // 常用前置
                // 4. 序列化
                using (XmlWriter writer = XmlWriter.Create("F0401.xml", settings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Invoice), "urn:GEINV:eInvoiceMessage:F0401:" + MigCommon.MigVer);
                    serializer.Serialize(writer, data, ns);
                }
            }
            return errmsg;
        }
        public static string F0501ToXmls(string invoiceid, string cancelReason)
        {
            F0501 cancelInvoice = new F0501();
            CancelInvoice data = cancelInvoice.CancelInvoice_View(invoiceid);//包含CHECKED
            data.CancelReason = cancelReason;
            string errmsg = cancelInvoice.CancelInvoice_Checked(data);
            if (data.CancelInvoiceNumber != "" && errmsg == "") {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true; // <-- 核心設定
                settings.Indent = true; // 可選：格式化縮排
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance"); // 常用前置
                // 4. 序列化
                using (XmlWriter writer = XmlWriter.Create("F0501.xml", settings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CancelInvoice), "urn:GEINV:eInvoiceMessage:F0501:" + MigCommon.MigVer);
                    serializer.Serialize(writer, data, ns);
                }
            }
            return errmsg;
        }
    }
}
