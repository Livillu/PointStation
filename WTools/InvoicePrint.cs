using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static WTools.InvoicePrint;
using System.Drawing; // For Bitmap and ImageFormat
using System.Drawing.Imaging; // For ImageFormat
namespace WTools
{
    public partial class InvoicePrint : Component
    {
        public InvoicePrint()
        {
            InitializeComponent();
        }
        public InvoiceData MastQrcode;//發票明檔
        
        const string KeyIV = "Dt8lyToo17X/XkXaQvihuA==";
        const string AESKey = "2B2DD98C7A1766930E6962E77014C171";/// <param name="AESKey">種子密碼(QRcode)</param>

        #region 產品資訊
        public class Product
        {
            public string Name {  get; set; }
            public int Qutys { get; set; }
            public int Price { get; set; }
        }
        #endregion

        #region 電子發票DATA
        public class InvoiceData
        {
            public string CopanyName { get; set; }
            public string InvoiceTital { get; set; } = "電子發票證明聯";
            public string InvoiceDruin { get; set; }
            public string InvoiceNumber { get; set; }
            public string InvoiceDate { get; set; }
            public string InvoiceTime { get; set; }
            public string RandomNumber { get; set; } = GetRandomCode();
            public decimal SalesAmount { get; set; } = 0;
            public decimal TaxAmount { get; set; } = 0;
            public decimal TotalAmount { get; set; }
            public string SellerIdentifier { get; set; }
            public string BuyerIdentifier { get; set; } = "00000000";
            public string RepresentIdentifier { get; set; } = "00000000";
            public string BusinessIdentifier { get; set; }
            public string BarCode { get; set; }
            public string QRCode1 { get; set; }
            public List<Product> QRCode2 { get; set; }
            public string PrintName;
        }
        #endregion

        #region 四位數的隨機碼
        public static string GetRandomCode()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000); // 產生 $0$ 到 $9999$ 之間的隨機數
            return randomNumber.ToString("D4"); // 使用 "D4" 格式化為四位數，不足的補零 (例如：56 -> 0056)
        }
        #endregion

        #region 產生QRCODE2
        /// <summary>
        /// 產生發票左邊QR碼
        /// </summary>
        /// <summary>
        public string QRCodeProduct(List<Product> products)
        {
            string msg = "";
            foreach (var item in products)
            {
                msg += item.Name.ToString()+":"+ item.Qutys.ToString() + ":"+ item.Price.ToString() + ":";
            }
            if(msg !="") msg=msg.Remove(0, msg.Length-1);
            return msg;
        }
        #endregion

        #region 加密驗證文字
        /// 將發票資訊文字加密成驗證文字
        /// </summary>
        /// <param name="plainText">發票資訊</param>
        /// <param name="AESKey">種子密碼(QRcode)</param>
        /// <returns>加密後的HEX字串</returns>
        public string AESEncrypt(string plainText, string AESKey)
        {
            byte[] bytes = Encoding.Default.GetBytes(plainText);
            ICryptoTransform transform = new RijndaelManaged
            {
                KeySize = 0x80,
                Key = this.convertHexToByte(AESKey),
                BlockSize = 0x80,
                IV = Convert.FromBase64String(KeyIV)
            }.CreateEncryptor();
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            stream2.Close();
            return Convert.ToBase64String(stream.ToArray());
        }
        /// <summary>
        /// 轉換HEX值為 Binaries
        /// </summary>
        /// <param name="hexString">HEX字串</param>
        /// <returns>Binaries值</returns>
        private byte[] convertHexToByte(string hexString)
        {
            byte[] buffer = new byte[hexString.Length / 2];
            int index = 0;
            for (int i = 0; i < hexString.Length; i += 2)
            {
                int num3 = Convert.ToInt32(hexString.Substring(i, 2), 0x10);
                buffer[index] = BitConverter.GetBytes(num3)[0];
                index++;
            }
            return buffer;
        }
        #endregion
        
        #region 發票BarCode驗證
        /// <summary>
        /// 檢查發票輸入資訊
        /// </summary>
        /// <param name="InvoiceDruin"> 發票年期別(5)：記錄發票字軌所屬民國年份(3碼)及期別之雙數月份(2碼)，例如104年3-4月發票年期別記載為「10404」。
        /// <param name="InvoiceNumber">發票字軌號碼共 10 碼</param>
        /// <param name="RandomNumber">4碼隨機碼</param>

        private static void barcodeValidate(string InvoiceDruin,string InvoiceNumber,string RandomNumber)
        {
            if (string.IsNullOrEmpty(InvoiceNumber) || (InvoiceNumber.Length != 10))
            {
                throw new Exception("Invaild InvoiceNumber: " + InvoiceNumber);
            }
            if (string.IsNullOrEmpty(InvoiceDruin) || (InvoiceDruin.Length != 5))
            {
                throw new Exception("Invaild InvoiceDate: " + InvoiceDruin);
            }
            try
            {
                long num = long.Parse(InvoiceDruin);
                int num2 = int.Parse(InvoiceDruin.Substring(3, 2));
                
                if ((num2 < 1) || (num2 > 12))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception("Invaild InvoiceDruin: " + InvoiceDruin);
            }
            if (string.IsNullOrEmpty(RandomNumber) || (RandomNumber.Length != 4))
            {
                throw new Exception("Invaild RandomNumber: " + RandomNumber);
            }
        }

        #endregion

        #region 產生BarCode
        public string BarCodeINV( string InvoiceDruin, string InvoiceNumber,string RandomNumber)
        {
            try
            {
                barcodeValidate(InvoiceDruin,InvoiceNumber,RandomNumber);
                return (InvoiceDruin + InvoiceNumber + RandomNumber);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion

        #region 發票QRCODE驗證
        /// <summary>
        /// 檢查發票輸入資訊
        /// </summary>
        /// <param name="InvoiceNumber">發票字軌號碼共 10 碼</param>
        /// <param name="InvoiceDate">發票開立年月日(中華民國年份月份日期)共 7 碼</param>
        /// <param name="InvoiceTime">發票開立時間 (24 小時制) 共 6 碼</param>
        /// <param name="RandomNumber">4碼隨機碼</param>
        /// <param name="SalesAmount">以整數方式載入銷售額 (未稅)，若無法分離稅項則記載為0</param>
        /// <param name="TaxAmount">以整數方式載入稅額，若無法分離稅項則記載為0</param>
        /// <param name="TotalAmount">整數方式載入總計金額(含稅)</param>
        /// <param name="BuyerIdentifier">買受人統一編號，若買受人為一般消費者，請填入 00000000 8位字串</param>
        /// <param name="RepresentIdentifier">代表店統一編號，電子發票證明聯二維條碼規格已不使用代表店，請填入00000000 8位字串</param>
        /// <param name="SellerIdentifier">銷售店統一編號</param>
        /// <param name="BusinessIdentifier">總機構統一編號，如無總機構請填入銷售店統一編號</param>
        /// <param name="productArray">單項商品資訊</param>
        /// <param name="AESKey">加解密金鑰(QR種子密碼)</param>
        private void inputValidate(string InvoiceNumber,
            string InvoiceDate,
            string InvoiceTime,
            string RandomNumber,
            decimal SalesAmount,
            decimal TaxAmount,
            decimal TotalAmount,
            string BuyerIdentifier,
            string RepresentIdentifier,
            string SellerIdentifier,
            string BusinessIdentifier,
            string AESKey)
        {
            if (string.IsNullOrEmpty(InvoiceNumber) || (InvoiceNumber.Length != 10))
            {
                throw new Exception("Invaild InvoiceNumber: " + InvoiceNumber);
            }
            if (string.IsNullOrEmpty(InvoiceDate) || (InvoiceDate.Length != 7))
            {
                throw new Exception("Invaild InvoiceDate: " + InvoiceDate);
            }
            try
            {
                long num = long.Parse(InvoiceDate);
                int num2 = int.Parse(InvoiceDate.Substring(3, 2));
                int num3 = int.Parse(InvoiceDate.Substring(5));
                if ((num2 < 1) || (num2 > 12))
                {
                    throw new Exception();
                }
                if ((num3 < 1) || (num3 > 0x1f))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception("Invaild InvoiceDate: " + InvoiceDate);
            }
            if (string.IsNullOrEmpty(InvoiceTime))
            {
                throw new Exception("Invaild InvoiceTime: " + InvoiceTime);
            }
            if (string.IsNullOrEmpty(RandomNumber) || (RandomNumber.Length != 4))
            {
                throw new Exception("Invaild RandomNumber: " + RandomNumber);
            }
            if (SalesAmount < 0M)
            {
                throw new Exception("Invaild SalesAmount: " + SalesAmount);
            }
            if (TotalAmount < 0M)
            {
                throw new Exception("Invaild TotalAmount: " + TotalAmount);
            }
            if (string.IsNullOrEmpty(BuyerIdentifier) || (BuyerIdentifier.Length != 8))
            {
                throw new Exception("Invaild BuyerIdentifier: " + BuyerIdentifier);
            }
            if (string.IsNullOrEmpty(RepresentIdentifier))
            {
                throw new Exception("Invaild RepresentIdentifier: " + RepresentIdentifier);
            }
            if (string.IsNullOrEmpty(SellerIdentifier) || (SellerIdentifier.Length != 8))
            {
                throw new Exception("Invaild SellerIdentifier: " + SellerIdentifier);
            }
            if (string.IsNullOrEmpty(BusinessIdentifier))
            {
                throw new Exception("Invaild BusinessIdentifier: " + BusinessIdentifier);
            }
            if (string.IsNullOrEmpty(AESKey))
            {
                throw new Exception("Invaild AESKey");
            }
        }

        #endregion

        #region 產生QRCODE字串
        /// <summary>
        /// 產生發票左邊QR碼
        /// </summary>
        /// <param name="InvoiceNumber">發票字軌號碼共 10 碼</param>
        /// <param name="InvoiceDate">發票開立年月日(中華民國年份月份日期)共 7 碼</param>
        /// <param name="InvoiceTime">發票開立時間 (24 小時制) 共 6 碼</param>
        /// <param name="RandomNumber">4碼隨機碼</param>
        /// <param name="SalesAmount">以整數方式載入銷售額 (未稅)，若無法分離稅項則記載為0</param>
        /// <param name="TaxAmount">以整數方式載入稅額，若無法分離稅項則記載為0</param>
        /// <param name="TotalAmount">整數方式載入總計金額(含稅)</param>
        /// <param name="BuyerIdentifier">買受人統一編號，若買受人為一般消費者，請填入 00000000 8位字串</param>
        /// <param name="RepresentIdentifier">代表店統一編號，電子發票證明聯二維條碼規格已不使用代表店，請填入00000000 8位字串</param>
        /// <param name="SellerIdentifier">銷售店統一編號</param>
        /// <param name="BusinessIdentifier">總機構統一編號，如無總機構請填入銷售店統一編號</param>
        /// <param name="productArray">單項商品資訊</param>
        /// <param name="AESKey">加解密金鑰(QR種子密碼)</param>

        /// <summary>
        public string QRCodeINV()
        {
            try
            {
                this.inputValidate(MastQrcode.InvoiceNumber,
                    MastQrcode.InvoiceDate,
                    MastQrcode.InvoiceTime,
                    MastQrcode.RandomNumber,
                    MastQrcode.SalesAmount,
                    MastQrcode.TaxAmount,
                    MastQrcode.TotalAmount,
                    MastQrcode.BuyerIdentifier,
                    MastQrcode.RepresentIdentifier,
                    MastQrcode.SellerIdentifier,
                    MastQrcode.BusinessIdentifier,
                    AESKey);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return ((MastQrcode.InvoiceNumber + MastQrcode.InvoiceDate + MastQrcode.RandomNumber +
                Convert.ToInt32(MastQrcode.SalesAmount).ToString("x8") +
                Convert.ToInt32(MastQrcode.TotalAmount).ToString("x8") +
                MastQrcode.BuyerIdentifier + MastQrcode.SellerIdentifier) + AESEncrypt(MastQrcode.InvoiceNumber + MastQrcode.RandomNumber, AESKey).PadRight(0x18));

        }
        #endregion

        #region 字串轉QRCODE
        public Image QRCodeToImage(string textToEncode)
        {
            // Create a QR code generator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // Create QR code data from the input string
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.Q);

            // Create a QR code object
            QRCode qrCode = new QRCode(qrCodeData);

            // Get the QR code graphic as a Bitmap
            // The parameter (e.g., 20) controls the size of each module (pixel) in the QR code.
            // A larger value results in a larger image.
            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {
                return (Image)qrCodeImage;
            }
        }
        #endregion

        #region 啟動列印程序
        public void Print()
        {
            //檢查是否有品項可以列印，如果沒有半個品項就踢出
            if (MastQrcode.QRCode2 == null || MastQrcode.QRCode2.Count == 0) { throw new System.Exception("沒有任何的項次資訊可供列印。"); }
            //檢查是否超過金額欄位最大列印長度（包含正負符號、Comma符號）
            int iPrintLimitLength = 7;
            foreach (Product oItem in MastQrcode.QRCode2)
            {
                if (string.Format("{0:n0}", oItem.Price).Length > iPrintLimitLength)
                { throw new System.Exception(string.Format("品項內所提供之金額，含正負號、千位逗號後，已經超過最大列印極限{0}個字。", iPrintLimitLength.ToString())); }
            }
            if (string.Format("{0:n0}", MastQrcode.TotalAmount).Length > iPrintLimitLength)
            { throw new System.Exception(string.Format("總金額所提供之金額，含正負號、千位逗號後，已經超過最大列印極限{0}個字。", iPrintLimitLength.ToString())); }
            /*//檢查是否有沒設定的參數，並適時的調整為空字串
            if (string.IsNullOrWhiteSpace(cSystemName)) { cSystemName = ""; }
            */
            /**** 列印資料 ****/

            /**** 準備列印 ****/
            System.Drawing.Printing.PrintDocument oDocument = new System.Drawing.Printing.PrintDocument();
            oDocument.DefaultPageSettings.PrinterSettings.PrinterName = MastQrcode.PrintName;
            oDocument.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("3InchPaper", 290, int.MaxValue);
            oDocument.PrintPage += setDocumentContent;  //delegate給setDocumentContent
            oDocument.Print();
        }
        #endregion

        #region 列印發票
        private void setDocumentContent(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (MastQrcode.QRCode2.Count > 0)
            {
                //定義相關變數
                float fPageWidth = 217F;    //紙張最大可列印寬度（以三英吋熱感應紙為基準；290 * 2.54 = 736mm）
                float fPageHeight = 350F;   //紙張最大可列印高度（以國家規定電子發票之高度為基準；280 * 2.54 = 711mm，加上預設退紙高度，大約可以勉強等於法規電子發票的870mm高度）
                float fPrintX = 2F;         //當前列印X軸（被用來作為左邊界位移）
                float fPrintY = 40F;         //當前列印Y軸（必須隨時被記載）
                float fVertical = 20F;      //垂直行距
                float fPrintX_Item = 2F;    //品項（表格用）
                float fPrintX_Price = 120F; //金額（表格用）
                System.Drawing.Font oFontHeader = new System.Drawing.Font("微軟正黑體", 14F);
                System.Drawing.Font oFontBarcode = new System.Drawing.Font("Code 128", 16F);
                System.Drawing.Font oFontContent = new System.Drawing.Font("微軟正黑體", 10F);
                System.Drawing.Font oFontContentSmall = new System.Drawing.Font("微軟正黑體", 8F, System.Drawing.FontStyle.Bold);
                System.Drawing.Brush oBrush = System.Drawing.Brushes.Black;

                //最佳化繪圖輸出
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                //繪製輸出可視邊界（用來調整版面用）
                e.Graphics.DrawRectangle(Pens.Aqua, new Rectangle(1, 1, 217, 350));

                //以離線的方式讀取並列印LOGO圖檔，以免系統將檔案咬死
                /*System.Drawing.Image oLogo;
                using (var oTemp = new System.Drawing.Bitmap(cLogoPath))
                {
                    var oTemp2 = new System.Drawing.Bitmap(oTemp);
                    oTemp2.SetResolution(400F, 400F);
                    oLogo = oTemp2;
                }
                e.Graphics.DrawImage(oLogo, new System.Drawing.PointF(64F, fPrintY));
                fPrintY += fVertical * 2;
                */
                //繪製公司抬頭
                e.Graphics.DrawString(MastQrcode.CopanyName, oFontHeader, oBrush, fPrintX_Item, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製固定格式
                e.Graphics.DrawString(MastQrcode.InvoiceTital, oFontHeader, oBrush, 40F, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製發票中文日期"11412";
                int tmp = Convert.ToUInt16(MastQrcode.InvoiceDruin.Substring(3, 2)) - 1;
                e.Graphics.DrawString($"{MastQrcode.InvoiceDruin.Substring(0, 3)}年{tmp.ToString()}-{MastQrcode.InvoiceDruin.Substring(3, 2)}月", oFontHeader, oBrush, 40F, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製發票號碼
                e.Graphics.DrawString(MastQrcode.InvoiceNumber, oFontHeader, oBrush, 40F, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製發票日期
                e.Graphics.DrawString(DateTime.Today.ToString("yyyy-MM-dd"), oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Item, fPrintY, fPrintX_Price - fPrintX_Item, fVertical));
                e.Graphics.DrawString(MastQrcode.InvoiceTime, oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Price, fPrintY, fPageWidth - fPrintX_Price, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far });
                fPrintY += fVertical * 1.15F;

                //繪製隨機碼,總計
                e.Graphics.DrawString($"隨機碼 {MastQrcode.RandomNumber}", oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Item, fPrintY, fPrintX_Price - fPrintX_Item, fVertical));
                e.Graphics.DrawString($"總計 {MastQrcode.TotalAmount.ToString()}", oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Price, fPrintY, fPageWidth - fPrintX_Price, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far });
                fPrintY += fVertical * 1.15F;

                //繪製買賣方發票
                e.Graphics.DrawString("賣方 " + MastQrcode.SellerIdentifier, oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Item, fPrintY, fPrintX_Price - fPrintX_Item, fVertical));
                if (MastQrcode.BuyerIdentifier != "0000000000")
                { e.Graphics.DrawString("買方 " + MastQrcode.BuyerIdentifier, oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Price, fPrintY, fPageWidth - fPrintX_Price, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far }); }
                fPrintY += fVertical * 1.15F;

                //繪製BARCODE
                e.Graphics.DrawString(MastQrcode.BarCode, oFontBarcode, oBrush, fPrintX_Item, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製QRCODE1
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(MastQrcode.QRCode1, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                Bitmap oTemp = qrCode.GetGraphic(20);
                oTemp.SetResolution(400F, 400F);
                Bitmap oTemp2 = new Bitmap(84, 84);
                Graphics graphics = Graphics.FromImage(oTemp2);
                graphics.DrawImage(oTemp, new Rectangle(0, 0, 160, 160));
                e.Graphics.DrawImage(oTemp2, new PointF(fPrintX_Item, fPrintY));

                if (MastQrcode.QRCode2.Count > 1)
                {
                    string items = "**";
                    for (int i = 1; i < MastQrcode.QRCode2.Count; i++)
                    {
                        items += $"{MastQrcode.QRCode2[i].Name}:{MastQrcode.QRCode2[i].Qutys.ToString()}:{MastQrcode.QRCode2[i].Price.ToString()}:";
                    }
                    qrGenerator = new QRCodeGenerator();
                    qrCodeData = qrGenerator.CreateQrCode(items, QRCodeGenerator.ECCLevel.Q);
                    qrCode = new QRCode(qrCodeData);     
                    oTemp = qrCode.GetGraphic(20);
                    oTemp.SetResolution(400F, 400F);
                    oTemp2 = new Bitmap(84, 84);
                    graphics = Graphics.FromImage(oTemp2);
                    graphics.DrawImage(oTemp, new Rectangle(0, 0, 160, 160));
                    e.Graphics.DrawImage(oTemp2, new PointF(fPrintX_Item + 100, fPrintY));
                }
            }
        }
        #endregion

    }

    class PDS326Plus
    {
        /// <summary>
        /// 主要建構子
        /// </summary>
        public PDS326Plus()
        {

        }
        /// <summary>
        /// Logo圖檔路徑
        /// 建議不要亂更換圖檔，因為這種低接的機器不若一般事務印表機那麼好調校。如果真的要更換，盡量以600 X 160像素、300 DPI為準。
        /// </summary>
        public string cLogoPath { get; set; }
        /// <summary>
        /// 品項名稱以及金額的列舉
        /// </summary>
        public System.Collections.Generic.List<PDS326PlusData> oItemsList { get; set; }
        /// <summary>
        /// 所有品項的加總後之金額
        /// </summary>
        public int iTotalAccount { get; set; }
        /// <summary>
        /// 系統資訊
        /// （是哪個系統來調用這次的列印？）
        /// </summary>
        public string cSystemName { get; set; }
        /// <summary>
        /// 作業資訊
        /// （是從哪一台KIOSK列印出來的？如果是一般電腦，那這邊的資訊可以寫入是由哪個承辦人員操作的。）
        /// </summary>
        public string cOperateInfo { get; set; }
        /// <summary>
        /// 廣告字串
        /// （活動廣告用途，如果給空字串，那麼紙張就不會輸出任何的字語。）
        /// </summary>
        public string cAdWords { get; set; }

        public InvoiceData invoiceData1 { get; set; }

        //字串轉QRCODE
        public byte[] GenerateQRCodeImage(string textToEncode)
        {
            // Create a QR code generator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // Create QR code data from the input string
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.Q);

            // Create a QR code object
            QRCode qrCode = new QRCode(qrCodeData);

            // Get the QR code graphic as a Bitmap
            // The parameter (e.g., 20) controls the size of each module (pixel) in the QR code.
            // A larger value results in a larger image.
            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {
                // Convert the Bitmap to a byte array (e.g., in PNG format)
                using (MemoryStream ms = new MemoryStream())
                {
                    Image img = Image.FromStream(ms);
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        //字串轉QRCODE
        public Image QRCodeToImage(string textToEncode)
        {
            // Create a QR code generator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // Create QR code data from the input string
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.Q);

            // Create a QR code object
            QRCode qrCode = new QRCode(qrCodeData);

            // Get the QR code graphic as a Bitmap
            // The parameter (e.g., 20) controls the size of each module (pixel) in the QR code.
            // A larger value results in a larger image.
            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {
                return (Image)qrCodeImage;
            }
        }

        /// <summary>
        /// 啟動列印程序
        /// </summary>
        public void Print()
        {
            /**** 檢查與調整必要資訊 ****/
            //檢查是否有品項可以列印，如果沒有半個品項就踢出
            if (oItemsList == null || oItemsList.Count == 0) { throw new System.Exception("沒有任何的項次資訊可供列印。"); }
            //檢查是否超過金額欄位最大列印長度（包含正負符號、Comma符號）
            int iPrintLimitLength = 7;
            foreach (PDS326PlusData oItem in oItemsList)
            {
                if (string.Format("{0:n0}", oItem.iPrice).Length > iPrintLimitLength)
                { throw new System.Exception(string.Format("品項內所提供之金額，含正負號、千位逗號後，已經超過最大列印極限{0}個字。", iPrintLimitLength.ToString())); }
            }
            if (string.Format("{0:n0}", iTotalAccount).Length > iPrintLimitLength)
            { throw new System.Exception(string.Format("總金額所提供之金額，含正負號、千位逗號後，已經超過最大列印極限{0}個字。", iPrintLimitLength.ToString())); }
            //檢查是否有沒設定的參數，並適時的調整為空字串
            if (string.IsNullOrWhiteSpace(cSystemName)) { cSystemName = ""; }
            if (string.IsNullOrWhiteSpace(cOperateInfo)) { cOperateInfo = ""; }
            if (string.IsNullOrWhiteSpace(cAdWords)) { cAdWords = ""; }
            /**** 列印資料 ****/
            
            /**** 準備列印 ****/
            System.Drawing.Printing.PrintDocument oDocument = new System.Drawing.Printing.PrintDocument();
            oDocument.DefaultPageSettings.PrinterSettings.PrinterName = "EPSON LQ-690CII ESC/P2";
            oDocument.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("3InchPaper", 290, int.MaxValue);
            oDocument.PrintPage += setDocumentContent;  //delegate給setDocumentContent
            oDocument.Print();
        }

        /// <summary>
        /// 製作文件內容程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setDocumentContent(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Product p = new Product();
            p.Name = "麵包";
            p.Qutys = 1;
            p.Price = 145;
            invoiceData1.QRCode2.Add(p);
            if (invoiceData1.QRCode2.Count > 0)
            {
                //定義相關變數
                float fPageWidth = 217F;    //紙張最大可列印寬度（以三英吋熱感應紙為基準；290 * 2.54 = 736mm）
                float fPageHeight = 350F;   //紙張最大可列印高度（以國家規定電子發票之高度為基準；280 * 2.54 = 711mm，加上預設退紙高度，大約可以勉強等於法規電子發票的870mm高度）
                float fPrintX = 2F;         //當前列印X軸（被用來作為左邊界位移）
                float fPrintY = 40F;         //當前列印Y軸（必須隨時被記載）
                float fVertical = 20F;      //垂直行距
                float fPrintX_Item = 2F;    //品項（表格用）
                float fPrintX_Price = 120F; //金額（表格用）
                System.Drawing.Font oFontHeader = new System.Drawing.Font("微軟正黑體", 14F);
                System.Drawing.Font oFontBarcode = new System.Drawing.Font("Code 128", 16F);
                System.Drawing.Font oFontContent = new System.Drawing.Font("微軟正黑體", 10F);
                System.Drawing.Font oFontContentSmall = new System.Drawing.Font("微軟正黑體", 8F, System.Drawing.FontStyle.Bold);
                System.Drawing.Brush oBrush = System.Drawing.Brushes.Black;

                //最佳化繪圖輸出
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                //繪製輸出可視邊界（用來調整版面用）
                e.Graphics.DrawRectangle(Pens.Aqua, new Rectangle(1, 1, 217, 350));

                //以離線的方式讀取並列印LOGO圖檔，以免系統將檔案咬死
                /*System.Drawing.Image oLogo;
                using (var oTemp = new System.Drawing.Bitmap(cLogoPath))
                {
                    var oTemp2 = new System.Drawing.Bitmap(oTemp);
                    oTemp2.SetResolution(400F, 400F);
                    oLogo = oTemp2;
                }
                e.Graphics.DrawImage(oLogo, new System.Drawing.PointF(64F, fPrintY));
                fPrintY += fVertical * 2;
                */
                //繪製公司抬頭
                e.Graphics.DrawString(invoiceData1.CopanyName, oFontHeader, oBrush, 59F, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製固定格式
                e.Graphics.DrawString(invoiceData1.InvoiceTital, oFontHeader, oBrush, 40F, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製發票中文日期"11412";
                int tmp = Convert.ToUInt16(invoiceData1.InvoiceDruin.Substring(3, 2)) - 1;
                e.Graphics.DrawString($"{invoiceData1.InvoiceDruin.Substring(0, 3)}年{tmp.ToString()}-{invoiceData1.InvoiceDruin.Substring(3, 2)}月", oFontHeader, oBrush, 40F, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製發票號碼
                e.Graphics.DrawString(invoiceData1.InvoiceNumber, oFontHeader, oBrush, 40F, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製發票日期
                e.Graphics.DrawString(invoiceData1.InvoiceDate, oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Item, fPrintY, fPrintX_Price - fPrintX_Item, fVertical));
                e.Graphics.DrawString(invoiceData1.InvoiceTime, oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Price, fPrintY, fPageWidth - fPrintX_Price, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far });
                fPrintY += fVertical * 1.15F;

                //繪製隨機碼,總計
                e.Graphics.DrawString($"隨機碼 {invoiceData1.RandomNumber}", oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Item, fPrintY, fPrintX_Price - fPrintX_Item, fVertical));
                e.Graphics.DrawString($"總計 {invoiceData1.TotalAmount.ToString()}", oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Price, fPrintY, fPageWidth - fPrintX_Price, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far });
                fPrintY += fVertical * 1.15F;

                //繪製買賣方發票
                e.Graphics.DrawString("賣方 " + invoiceData1.SellerIdentifier, oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Item, fPrintY, fPrintX_Price - fPrintX_Item, fVertical));
                if (invoiceData1.BuyerIdentifier != "")
                { e.Graphics.DrawString("買方 " + invoiceData1.BuyerIdentifier, oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Price, fPrintY, fPageWidth - fPrintX_Price, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far }); }
                fPrintY += fVertical * 1.15F;

                //繪製BARCODE
                e.Graphics.DrawString(invoiceData1.BarCode, oFontBarcode, oBrush, fPrintX_Item, fPrintY);
                fPrintY += fVertical * 1.4F;

                //繪製QRCODE
                InvoicePrint invoicePrint = new InvoicePrint();

                // Create a QR code generator
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(invoiceData1.QRCode1, QRCodeGenerator.ECCLevel.Q);
                // Create a QR code object
                QRCode qrCode = new QRCode(qrCodeData);

                // Get the QR code graphic as a Bitmap
                // The parameter (e.g., 20) controls the size of each module (pixel) in the QR code.
                // A larger value results in a larger image.

                Bitmap oTemp = qrCode.GetGraphic(20);
                oTemp.SetResolution(400F, 400F);
                Bitmap oTemp2 = new Bitmap(80, 80);
                Graphics graphics = Graphics.FromImage(oTemp2);
                graphics.DrawImage(oTemp, new Rectangle(0, 0, 160, 160));
                e.Graphics.DrawImage(oTemp2, new PointF(fPrintX_Item, fPrintY));
                if (invoiceData1.QRCode2.Count > 1)
                {
                    string items = "**";
                    for (int i = 1; i < invoiceData1.QRCode2.Count; i++)
                    {
                        items += $"{invoiceData1.QRCode2[i].Name}:{invoiceData1.QRCode2[i].Qutys.ToString()}:{invoiceData1.QRCode2[i].Price.ToString()}:";
                    }
                    // Create a QR code generator
                    qrGenerator = new QRCodeGenerator();
                    qrCodeData = qrGenerator.CreateQrCode(items, QRCodeGenerator.ECCLevel.Q);
                    // Create a QR code object
                    qrCode = new QRCode(qrCodeData);

                    // Get the QR code graphic as a Bitmap
                    // The parameter (e.g., 20) controls the size of each module (pixel) in the QR code.
                    // A larger value results in a larger image.

                    oTemp = qrCode.GetGraphic(20);
                    oTemp.SetResolution(400F, 400F);
                    oTemp2 = new Bitmap(80, 80);
                    graphics = Graphics.FromImage(oTemp2);
                    graphics.DrawImage(oTemp, new Rectangle(0, 0, 160, 160));
                    e.Graphics.DrawImage(oTemp2, new PointF(fPrintX_Item + 100, fPrintY));
                }

                //fPrintY += fVertical * 2;

                //繪製表格（上）分隔線
                /* e.Graphics.DrawLine(new System.Drawing.Pen(oBrush, 1), fPrintX, fPrintY, 280, fPrintY);
                 fPrintY += fVertical * 0.25F;

                 //繪製明細金額
                 foreach (PDS326PlusData oItem in oItemsList)
                 {
                     e.Graphics.DrawString(oItem.cName, oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Item, fPrintY, fPrintX_Price - fPrintX_Item, fVertical));
                     e.Graphics.DrawString(string.Format("{0:n0}", oItem.iPrice), oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Price, fPrintY, fPageWidth - fPrintX_Price, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far });
                     fPrintY += fVertical;
                 }

                 //繪製表格（下）分隔線
                 fPrintY += fVertical * 0.25F;
                 e.Graphics.DrawLine(new System.Drawing.Pen(oBrush, 1), fPrintX, fPrintY, 280, fPrintY);
                 fPrintY += fVertical * 0.25F;

                 //繪製結算金額
                 e.Graphics.DrawString("總收費金額：NT$", oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Item, fPrintY, fPrintX_Price - fPrintX_Item, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far });
                 e.Graphics.DrawString(string.Format("{0:n0}", iTotalAccount), oFontContent, oBrush, new System.Drawing.RectangleF(fPrintX_Price, fPrintY, fPageWidth - fPrintX_Price, fVertical), new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Far });
                 fPrintY += fVertical * 1.2F;

                 //系統日期（單子的列印時間日期）
                 e.Graphics.DrawString(string.Format("列印時間：{0}", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")), oFontContentSmall, oBrush, fPrintX, fPrintY);
                 fPrintY += fVertical;

                 //系統資訊（哪一個系統列印出這張單子）
                 e.Graphics.DrawString(string.Format("系統資訊：{0}", cSystemName), oFontContentSmall, oBrush, fPrintX, fPrintY);
                 fPrintY += fVertical;

                 //輸出機器資訊（例如ＫＩＯＳＫ編號）
                 e.Graphics.DrawString(string.Format("作業資訊：{0}", cOperateInfo), oFontContentSmall, oBrush, fPrintX, fPrintY);
                 fPrintY += fVertical;

                 //廣告區塊
                 e.Graphics.DrawString(cAdWords, oFontContentSmall, oBrush, new System.Drawing.RectangleF(fPrintX, fPrintY, fPageWidth - fPrintX, e.Graphics.MeasureString(cAdWords, oFontContentSmall).Height * 205F));
                */
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //寫入資料
            PDS326Plus oPrinter = new PDS326Plus()
            {
                cLogoPath = @"D:\Slashview\Sample.png",
                oItemsList = new System.Collections.Generic.List<PDS326PlusData> {
                  new PDS326PlusData() { cName = "麵包", iQuty=1,iPrice = 12345 },
                  new PDS326PlusData() { cName = "手續費", iQuty=2, iPrice = 10 },
                  new PDS326PlusData() { cName = "感光紙費", iQuty=3, iPrice = 1 },
                },
                iTotalAccount = 12356,
                cSystemName = "真好吃麵包坊收銀系統",
                cOperateInfo = "天母分店",
                cAdWords = "即日起，凡持有真好吃麵包坊貴賓卡之消費者，來電均可贈送真難吃麵包乙顆。"
            };
            //送出列印
            oPrinter.Print();
        }

    }

    /// <summary>
    /// 傳入給PD-S326 Plus發票收據列印機的資料定義檔案
    /// Object Rrelational Mapping（ORM）
    /// </summary>
    class PDS326PlusData
    {
        /// <summary>
        /// 品項名稱
        /// </summary>
        public string cName { get; set; }
        /// <summary>
        /// 品項之金額
        /// </summary>
        public int iQuty { get; set; }
        /// <summary>
        /// 品項之金額
        /// </summary>
        public int iPrice { get; set; }
    }    
}
