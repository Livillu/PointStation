using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OfficeOpenXml.ExcelErrorValue;

namespace WTools
{
    public partial class UserStockLocal : UserControl
    {
        public UserStockLocal()
        {
            InitializeComponent();
        }

        private void 增加倉庫ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogtext dl = new dialogtext();
            dl.Text = "新增倉庫";
            dl.label1.Text = "倉庫名稱";
            DialogResult dr = dl.ShowDialog();
            if (dr == DialogResult.OK && dl.textBox1.Text != "")
            {
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand($"INSERT INTO [Stockhouse]([Name],[Upitem]) VALUES('{dl.textBox1.Text.Trim()}',0)", conn1);
                cmd1.Connection.Open();
                if (cmd1.ExecuteNonQuery() > 0)
                {
                    SetTrees();
                }
                else
                {
                    MessageBox.Show("新增倉庫失敗!!!!");
                }
                
            }
        }

        private void 刪除節點ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string sql = $"Delete FROM [Stockhouse] where [sno] = {treeView1.SelectedNode.Tag.ToString()}";
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand($"SELECT count(*) FROM [PtLocation] where [Upid] = {treeView1.SelectedNode.Tag.ToString()}", conn1);
                cmd1.Connection.Open();
                TreeNode parentNode = treeView1.SelectedNode.Parent;
                if (parentNode != null)
                {
                    sql= $"Delete FROM [Stockhouse] where [Upitem] = {parentNode.Tag.ToString()} and [sno]={treeView1.SelectedNode.Tag.ToString()}";
                    cmd1.CommandText = $"SELECT count(*) FROM [PtLocation] where [Upid] = {parentNode.Tag.ToString()} and [Itemid]={treeView1.SelectedNode.Tag.ToString()}";
                }
                    
                if (cmd1.ExecuteScalar().ToString() == "0")
                {
                    if (DialogResult.OK == MessageBox.Show($"確定刪除???{treeView1.SelectedNode.Text}", "刪除節點", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                    {
                        cmd1.CommandText = sql;
                        if (cmd1.ExecuteNonQuery() > 0)
                        {
                            treeView1.Nodes.Remove(treeView1.SelectedNode);
                            SetTrees();
                        }
                        else
                        {
                            MessageBox.Show("刪除失敗!!!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("已有入庫資料!!!不可刪除...");
                }
                           
            }
            
        }

        private void 增加儲位ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode !=null)
            {
                // 假设 selectedNode 是你想要获取父节点的 TreeNode 对象
                TreeNode parentNode = treeView1.SelectedNode.Parent;

                // 检查父节点是否存在
                if (parentNode == null)
                {
                    dialogtext dl = new dialogtext();
                    dl.Text = "新增儲位";
                    dl.label1.Text = "儲位名稱";
                    DialogResult dr = dl.ShowDialog();
                    if (dr == DialogResult.OK && dl.textBox1.Text !="")
                    {
                        SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                        SqlCommand cmd1 = new SqlCommand($"INSERT INTO [Stockhouse] ([Upitem],[Name]) VALUES('{treeView1.SelectedNode.Tag.ToString()}','{dl.textBox1.Text.Trim()}')", conn1);
                        cmd1.Connection.Open();
                        if (cmd1.ExecuteNonQuery() > 0)
                        {
                            SetTrees();
                        }
                        else
                        {
                            MessageBox.Show("新增儲位失敗!!!!");
                        }
                        
                    }                  
                }
                
            }
        }
        private void SetTrees()
        {
            treeView1.Nodes.Clear();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [sno],[Name] FROM [Stockhouse] where [Upitem]=0 order by[sno]", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr = cmd1.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            sdr.Close();
            cmd1.CommandText = "SELECT [sno],[Name],[Upitem] FROM [Stockhouse] where [Upitem] <> 0 order by[sno]";
            DataTable dt1 = new DataTable();
            SqlDataReader sdr1 = cmd1.ExecuteReader();
            dt1.Load(sdr1);
            sdr1.Close();
            foreach (DataRow dr in dt.Rows)
            {
                TreeNode tmp = new TreeNode();
                tmp.Text = dr[1].ToString();
                tmp.Tag = dr[0].ToString();
                foreach (DataRow dr2 in dt1.Rows)
                {
                    if (dr[0].ToString() == dr2[2].ToString())
                    {
                        TreeNode tmp1 = new TreeNode();
                        tmp1.Text = dr2[1].ToString();
                        tmp1.Tag = dr2[0].ToString();
                        tmp.Nodes.Add(tmp1);
                    }
                }
                treeView1.Nodes.Add(tmp);
            }
            treeView1.ExpandAll();
        }
        private void UserStockLocal_Load(object sender, EventArgs e)
        {
            SetTrees();
        }

        private void 查看庫存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand("", conn1);
                cmd1.Connection.Open();
                TreeNode parentNode = treeView1.SelectedNode.Parent;
                cmd1.CommandText = "SELECT a.[MB001] 品號,[MB002] 品名,sum([Quty]) 數量,[DateNumber] 批號 FROM [PtLocation] a";
                cmd1.CommandText += " inner join [Products] b on a.MB001=b.MB001 inner join [Stockhouse] c on a.Upid=c.Upitem and a.Itemid=c.sno";

                // 检查父节点是否存在
                if (parentNode == null)
                {
                    cmd1.CommandText += $" where a.Upid={treeView1.SelectedNode.Tag.ToString()} group by a.[MB001],[MB002],[DateNumber]";
                }
                else
                {
                    cmd1.CommandText += $" where a.Upid={parentNode.Tag.ToString()} and [Itemid]={treeView1.SelectedNode.Tag.ToString()} group by a.[MB001],[MB002],[DateNumber]";
                }
                SqlDataReader sdr = cmd1.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(sdr);
                sdr.Close();
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            }
        }
    }
}
