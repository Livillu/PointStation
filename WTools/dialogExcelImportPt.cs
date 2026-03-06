using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTools
{
    public partial class dialogExcelImportPt : Form
    {
        DataTable dt, dt1,dt2;
        public dialogExcelImportPt()
        {
            InitializeComponent();
        }

        private void dialogExcelImportPt_Load(object sender, EventArgs e)
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("", conn1);
            cmd1.Connection.Open();
            cmd1.CommandText = "SELECT [sno],[Name],[Upitem] FROM [Stockhouse] where [Upitem] > 0 order by [sno]";
            SqlDataReader sdr = cmd1.ExecuteReader();
            dt1 = new DataTable();
            dt1.Load(sdr);
            sdr.Close();
            
            cmd1.CommandText = "SELECT [sno],[Name] FROM [Stockhouse] where [Upitem]=0 order by [sno]";
            dt = new DataTable();
            SqlDataReader sdr1 = cmd1.ExecuteReader();
            dt.Load(sdr1);
            sdr1.Close();
            comboBox1.DataSource=dt;
            comboBox1.DisplayMember= "Name";
            comboBox1.ValueMember= "sno";         
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (dt1 !=null && dt1.Rows.Count > 0)
            {
                /*DataRow[] rslt = dt1.Select($"Upitem = {tmp}");
                foreach (DataRow row in rslt)
                {
                    Console.WriteLine("{0}, {1}", row[0], row[1]);
                }*/

                DataView dataView1 = dt1.DefaultView;                
                dataView1.RowFilter = $"Upitem = {((DataRowView)comboBox1.SelectedValue).Row["sno"]}";
                dt2=new DataTable();
                dt2 = dataView1.ToTable();
                comboBox2.DataSource = dt2;
                comboBox2.DisplayMember = "Name"; 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1 && comboBox2.SelectedIndex > -1)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // 特別注意，頁籤和儲存格等操作 是由 1 開始而非 0
                    // 打開存在的 Excel 檔案
                    ExcelPackage.License.SetNonCommercialOrganization("My Excel Test."); // 關閉新許可模式通知
                    string path = openFileDialog1.FileName;
                    var excelFile = new FileInfo(path);
                    var excel = new ExcelPackage(excelFile);
                    // 指定頁籤
                    //ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1]; // 這邊用是 1 在 Core 用是 0 = =
                    ExcelWorksheet sheet1 = excel.Workbook.Worksheets[0]; // 可以使用頁籤名稱
                    if (sheet1.Rows.Count() > 0)
                    {
                        SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                        SqlCommand cmd1 = new SqlCommand("truncate table Products;truncate table [PDList];truncate table [PtLocation];", conn1);
                        cmd1.Connection.Open();
                        cmd1.ExecuteNonQuery();
                        //i=1資料標題欄
                        for (int i = 0; i < sheet1.Rows.Count(); i++)
                        {
                            string GpSno = "", MB003="";
                            try { GpSno = sheet1.Cells[i + 2, 4].Value.ToString(); }
                            catch { }
                            try { MB003 = sheet1.Cells[i + 2, 7].Value.ToString(); }
                            catch { }
                            string MB001 = sheet1.Cells[i + 2, 5].Value.ToString();
                            string MB002 = sheet1.Cells[i + 2, 6].Value.ToString();
                            string MB064 = sheet1.Cells[i + 2, 8].Value.ToString();
                            string MB051 = sheet1.Cells[i + 2, 9].Value.ToString();
                            cmd1.CommandText = "INSERT INTO Products(MB001, MB002, MB003, MB004, MB064, MB051,GpSno) values('" + MB001 + "','" + MB002 + "','"+ MB003 +"',''," + MB064 + "," + MB051 + ",'" + GpSno + "');";
                            cmd1.CommandText += "INSERT INTO [PDList]([userid],[MB001],[Quty]) values('Init','" + MB001 + "'," + MB051 + ");";
                            cmd1.CommandText += "INSERT INTO [PtLocation]([Upid],[Itemid],[MB001],[Quty],[DateNumber]) ";
                            cmd1.CommandText += $"VALUES({dt.Rows[comboBox1.SelectedIndex][0].ToString()},{dt2.Rows[comboBox2.SelectedIndex][0].ToString()},'{MB001}',{MB064},'{DateTime.Today.ToString("yyyyMMdd001")}')";
                            cmd1.ExecuteNonQuery();
                        }
                    }
                }

            }
        }
    }
}
