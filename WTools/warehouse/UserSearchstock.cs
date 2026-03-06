using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTools.warehouse
{
    public partial class UserSearchstock : UserControl
    {
        DataTable dtM, dtT;
        public UserSearchstock()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sqlwhere = "";
            if (comboBox1.SelectedIndex > 0)
            {
                sqlwhere += $" AND [Upid]={dtM.Rows[comboBox1.SelectedIndex - 1][0]}";
                if (comboBox2.SelectedIndex >0)
                {
                    sqlwhere += $" AND [Itemid]={dtT.Rows[comboBox2.SelectedIndex - 1][0]}";
                }
            }
            if (textBox2.Text.Length > 0)
            {
                sqlwhere += $" AND (a.[MB001] like '%{textBox2.Text}%' OR [MB002] like '%{textBox2.Text}%')";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT a.[MB001] 品號,[MB002] 品名,[MB003] 規格,[MB004] 單位,sum([Quty]) 數量 FROM [PtLocation] a");
            sb.AppendLine("inner join Products b on a.MB001 = b.MB001");
            if(sqlwhere !="") sb.Append(" WHERE 1=1"+sqlwhere);
            sb.AppendLine("group by a.[MB001],[MB002],[MB003],[MB004] order by a.[MB001]");
            DataTable dt = new DataTable();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sb.ToString(), conn1);
            cmd1.Connection.Open();
            SqlDataReader dr1 = cmd1.ExecuteReader();
            dt.Load(dr1);       
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT a.[MB001] 品號,d.Name 倉庫,c.Name 儲位,sum([Quty]) 數量,[DateNumber] 批號 FROM [PtLocation] a");
            sb.AppendLine("inner join Products b on a.MB001 = b.MB001");
            sb.AppendLine("inner join[Stockhouse] c on a.[Itemid] = c.sno");
            sb.AppendLine($"inner join[Stockhouse] d on a.[Upid] = d.sno where a.[MB001]='{dataGridView1.Rows[e.RowIndex].Cells[0].Value}'");
            sb.AppendLine("group by a.[MB001],c.Name,d.Name,[DateNumber]");
  
            DataTable dt2 = new DataTable();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sb.ToString(), conn1);
            cmd1.Connection.Open();
            SqlDataReader dr1 = cmd1.ExecuteReader();
            dt2.Load(dr1);
            dataGridView2.DataSource = dt2;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex > 0)
            {
                comboBox2.Items.Clear();
                comboBox2.Items.Add("全部");
                foreach (DataRow dr in dtT.Rows)
                {
                    comboBox2.Items.Add(dr["Name"].ToString());
                }
                comboBox2.SelectedIndex = 0;
            }
        }

        private void UserSearchstock_Load(object sender, EventArgs e)
        {
      
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [sno],[Name] FROM [Stockhouse] where [Upitem]=0 order by[sno]", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            dtM = new DataTable();
            dtM.Load(sdr);
            sdr.Close();
            foreach (DataRow dr in dtM.Rows)
            {
                comboBox1.Items.Add(dr["Name"].ToString());
            }
            comboBox1.SelectedIndex = 0;

            cmd1.CommandText = "SELECT [sno],[Name],[Upitem] FROM [Stockhouse] where [Upitem] <> 0 order by[sno]";
            dtT = new DataTable();
            SqlDataReader sdr1 = cmd1.ExecuteReader();
            dtT.Load(sdr1);
            sdr1.Close();
        }
    }
}
