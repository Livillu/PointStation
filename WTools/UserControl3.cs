using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools
{
    public partial class UserControl3 : UserControl
    {
        DataTable DT;
       
        public UserControl3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sqlparam = "";
            if (textBox4.Text != "") sqlparam += " AND a.Sno='" + textBox4.Text+"'";
            else
            {
                if (dateTimePicker1.Text != "" && dateTimePicker2.Text != "") sqlparam += " AND Cdate BETWEEN '" + dateTimePicker1.Text + "' AND '" + dateTimePicker2.Text + "  23:59:59'";
                if (textBox2.Text != "") sqlparam += " AND a.MB001 LIKE '%" + textBox2.Text + "%' OR  MB002 LIKE '%" + textBox2.Text + "%'";
            }
            //dataGridView1.DataSource = null;
            DT = new DataTable();

            string sqlstring = "SELECT a.[Sno] 發票編號,a.[MB001] 品號,[MB002] 品名,[MB003] 規格,[MB004] 單位,[Quty] 數量,a.[Price] 單價,a.[Discount] 折扣,[Tprice]-a.[Discount] 金額,FORMAT([Cdate], 'yyyy-MM-dd HH:mm:ss') 日期 ";
            sqlstring += "FROM [TSales] a inner join Products b on a.MB001 = b.MB001 inner join [MSales] c on a.Sno = c.Sno WHERE [Isok]='1'" + sqlparam;

            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sqlstring, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DT.Load(sdr);
            Int64 total = 0;
            Int32 rows = 0;
            if (DT != null && DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    total += Convert.ToInt64(dr[8]);
                    rows++;
                }
            }
            textBox1.Text = rows.ToString();
            textBox3.Text = total.ToString();
            dataGridView1.DataSource = DT;
        }
    }
}
