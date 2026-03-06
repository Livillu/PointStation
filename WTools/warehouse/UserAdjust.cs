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
    public partial class UserAdjust : UserControl
    {
        DataTable Tdt;//來源庫別商品清單
        DataTable Sdt;//目標，來源庫別
        DataTable Sdt1;//目標全部儲位
        DataTable Sdt2;//目標選擇儲位
        public UserAdjust()
        {
            InitializeComponent();
        }
        public class ptstock
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        private void SetTrees()
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [sno],[Name] FROM [Stockhouse] where [Upitem]=0 order by[sno]", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            Sdt = new DataTable();
            Sdt.Load(sdr);
            sdr.Close();
            foreach (DataRow dr in Sdt.Rows)
            {
                comboBox2.Items.Add(dr["Name"].ToString());
                comboBox3.Items.Add(dr["Name"].ToString());
            }
            cmd1 = new SqlCommand("SELECT [sno],[Name],[Upitem] FROM [Stockhouse] where [Upitem]>0 order by[sno]", conn1);
            SqlDataReader sdr1 = cmd1.ExecuteReader();
            Sdt1 = new DataTable();
            Sdt1.Load(sdr1);
            sdr1.Close();
        }

        private void UserAdjust_Load(object sender, EventArgs e)
        {
            SetTrees();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count>0)
            {
                string value = Sdt.Rows[comboBox2.SelectedIndex]["sno"].ToString();
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.CommandText = "SELECT c.[Name] 儲位,a.[MB001] 品號,[MB002] 品名,sum([Quty]) 數量,[DateNumber] 批號,c.[sno] FROM [PtLocation] a";
                cmd1.CommandText += " inner join [Products] b on a.MB001=b.MB001 inner join [Stockhouse] c on a.Upid=c.Upitem and a.Itemid=c.sno";
                cmd1.CommandText += $" WHERE [Upid]={value} group by a.[MB001],[MB002],[DateNumber],c.[Name],c.[sno]";
                cmd1.Connection.Open();
                SqlDataReader sdr = cmd1.ExecuteReader();
                Tdt = new DataTable();
                Tdt.Load(sdr);
                sdr.Close();
                dataGridView1.DataSource = Tdt;               
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex > -1)
            {
                comboBox4.Items.Clear();
                comboBox4.Text="";
                Sdt2 =new DataTable();
                Sdt2= Sdt1.Clone();
                foreach (DataRow dataRow in Sdt1.Rows)
                {
                    string s1 = dataRow["Upitem"].ToString();
                    string s2= Sdt.Rows[comboBox3.SelectedIndex]["sno"].ToString();
                    if (s1==s2)
                    {
                        Sdt2.ImportRow(dataRow);
                        comboBox4.Items.Add(dataRow["Name"].ToString());
                    }
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || comboBox3.SelectedIndex < 0 || comboBox4.SelectedIndex < 0) { return; }
            if (comboBox2.Text != comboBox3.Text || Tdt.Rows[e.RowIndex][0].ToString() != Sdt.Rows[comboBox4.SelectedIndex][1].ToString())
            {
                string srcDateNumber = Tdt.Rows[e.RowIndex][4].ToString();
                string srcUpid = Sdt.Rows[comboBox2.SelectedIndex]["sno"].ToString();
                string srcSno = Tdt.Rows[e.RowIndex][5].ToString();
                string srcName = Tdt.Rows[e.RowIndex][2].ToString();
                string srcQuty = Tdt.Rows[e.RowIndex][3].ToString();
                string MB001 = Tdt.Rows[e.RowIndex][1].ToString();
                dialogform dl = new dialogform($"{srcName} ({srcQuty})");
                dl.Text = "移轉量";
                dl.numericUpDown1.Maximum = Convert.ToInt32(srcQuty);
                DialogResult dr = dl.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string desUpid = Sdt.Rows[comboBox3.SelectedIndex]["sno"].ToString();
                    string desSno = Sdt2.Rows[comboBox4.SelectedIndex]["sno"].ToString();
                    int desQuty = dl.GetMsg();
                    if (desQuty > 0)
                    {
                        //update [PtLocation]                       
                        SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                        SqlCommand cmd1 = new SqlCommand($"select count(*) from [PtLocation] where [Itemid]={desSno} and Upid={desUpid} and DateNumber='{srcDateNumber}' and [MB001]='{MB001}'", conn1);
                        cmd1.Connection.Open();
                        SqlTransaction sqlTransaction = null;
                        sqlTransaction = conn1.BeginTransaction();
                        cmd1.Transaction = sqlTransaction;
                       
                        //更新目標檔
                        if (Convert.ToInt16(cmd1.ExecuteScalar()) > 0)
                        {
                            cmd1.CommandText = $"UPDATE [PtLocation] SET [Quty] =[Quty]+ {desQuty} WHERE Itemid={desSno} and Upid={desUpid} and DateNumber='{srcDateNumber}' and [MB001]='{MB001}'";
                        }
                        else
                        {
                            cmd1.CommandText = $"INSERT INTO [PtLocation]([Upid],[Itemid],[MB001],[Quty],[DateNumber]) Values({desUpid},{desSno},'{MB001}',{desQuty},'{srcDateNumber}')";
                        }                       
                        cmd1.ExecuteNonQuery();

                        //更新來源檔
                        cmd1.CommandText = $"UPDATE [PtLocation] SET [Quty] =[Quty]- {desQuty} WHERE Itemid={srcSno} and Upid={srcUpid} and DateNumber='{srcDateNumber}' and [MB001]='{MB001}'";
                        cmd1.ExecuteNonQuery();

                        //update PtMoveList
                        cmd1.CommandText = $"INSERT INTO [PtMoveList]([SrcUpid],[SrcItemid],[MB001],[Quty],[DateNumber],[DesUpid],[DesItemid]) VALUES({srcUpid},{srcSno},'{MB001}',{desQuty},'{srcDateNumber}',{desUpid},{desSno})";
                        cmd1.ExecuteNonQuery();
                        try
                        {
                            cmd1.Transaction.Commit();
                            MessageBox.Show("儲位變更完成...");
                        }
                        catch
                        {
                            cmd1.Transaction.Rollback();
                            MessageBox.Show("儲位變更失敗!!!請重試...");
                        }
                    }
                }
            }

        }
    }
}
