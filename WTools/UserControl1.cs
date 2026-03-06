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
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WTools
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }
        DataSet dataSet = new DataSet();
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        private void button1_Click(object sender, EventArgs e)
        {
            if (dt !=null && dt.Rows.Count > 0) dt.Rows.Clear();
            string sqlparam = "";

            if (textBox2.Text.Trim().Length > 0) sqlparam = " AND　[Sno]='" + textBox2.Text.Trim() + "'";
            else
            {
                if (dateTimePicker2.Text != "" && dateTimePicker1.Text != "") sqlparam += " AND Cdate BETWEEN '" + dateTimePicker1.Text + "' AND '" + dateTimePicker2.Text + " 23:59:59'";
                if (checkBox1.Checked && !checkBox2.Checked) sqlparam += " AND Isok='1'";
                if (checkBox2.Checked && !checkBox1.Checked) sqlparam += " AND Isok='0'";
                if (checkMoney.Checked && !checkLine.Checked) sqlparam += " AND TrType=0";
                if (checkLine.Checked && !checkMoney.Checked) sqlparam += " AND TrType=1";
            }
            if (sqlparam == "") sqlparam = " WHERE 1<>1";
            else sqlparam = " WHERE 1=1"+ sqlparam;

            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand("SELECT [Sno] 發票編號,[Price] 總金額,[Discount] 折扣金額,[Price] -[Discount] 實收金額,FORMAT([Cdate], 'yyyy-MM-dd HH:mm:ss') 日期,case Isok when '1' then '銷' else '退' end 狀態 ,case [TrType] when '1' then 'Line Pay' else '現金' end 付款類型 FROM [MSales]" + sqlparam+ " order by [Sno]", conn);
            cmd.Connection.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            dt.Load(rdr);
            cmd.Connection.Close();
            Int64 total = 0;
            Int32 rows = 0;
            foreach (DataRow dr in dt.Rows)
            {
                total += Convert.ToInt64(dr[3]);
                rows++;
            }
            dataGridView1.DataSource = dt;
            textBox1.Text = rows.ToString();
            textBox3.Text = total.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dt !=null && dt.Rows.Count > 0)
            {
                saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel Files (*.xlsx)|";
                saveFileDialog1.DefaultExt = "xlsx";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");//指明非商业应用
                    using (var excel = new ExcelPackage())
                    {
                        // 建立分頁
                        var ws = excel.Workbook.Worksheets.Add("MySheet");
                        // 寫入資料試試
                        ws.Cells["A1"].Value = "發票編號";
                        ws.Cells["B1"].Value = "金額";
                        ws.Cells["C1"].Value = "日期";
                        ws.Cells["D1"].Value = "狀態";
                        ws.Cells["E1"].Value = "付款類型";
                        ws.Cells["A2"].LoadFromDataTable(dt);
                        // 儲存 Excel
                        var file = new FileInfo(saveFileDialog1.FileName); // 檔案路徑
                        excel.SaveAs(file);
                        MessageBox.Show("轉檔完成.....");
                    }
                }
            }
        }

        private void Getdate(string sqlparam)
        {
            if (dt1 != null && dt1.Rows.Count > 0) dt1.Rows.Clear();
            string sqlstring = "SELECT a.[MB001] 品號,[MB002] 品名,[MB003] 規格,[MB004] 單位,[Quty] 數量,a.[Price] 單價,a.[Discount] 折扣,[Tprice]-a.[Discount] 金額, FORMAT([Cdate], 'yyyy-MM-dd HH:mm:ss') 日期 ";
            sqlstring += $"FROM [TSales] a inner join Products b on a.MB001 = b.MB001 inner join[MSales] c on a.Sno = c.Sno WHERE a.[Sno]='{sqlparam}'";
            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sqlstring, conn);
            cmd.Connection.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            dt1.Load(rdr);
            dataGridView2.DataSource = dt1;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            Getdate(dt.Rows[e.RowIndex][0].ToString());
        }
    }
}
