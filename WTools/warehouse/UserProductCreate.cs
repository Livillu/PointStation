using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools.warehouse
{
    public partial class UserProductCreate : UserControl
    {
        DataTable DT,dt;
        public UserProductCreate()
        {
            InitializeComponent();
        }
        private void Change(int index)
        {
            tabControl1.TabPages.Clear();
            if (index == 2)
            {
                tabControl1.TabPages.Add(tabPage2);
            }
            else
            {
                tabControl1.TabPages.Add(tabPage1);
            }
        }
        private void ButtonChange(int index)
        {
            tabControl1.TabPages.Clear();
            button2.Enabled = true;
            button6.Enabled = true;
            switch (index)
            {
                case 1://新增狀態
                    button2.Enabled = false;
                    button6.Enabled = false;
                    tabControl1.TabPages.Add(tabPage2);
                    break;
                default:
                    tabControl1.TabPages.Add(tabPage1);
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            string sqlparam = "";
            if (textBox2.Text != "") sqlparam += " WHERE MB001 LIKE '%" + textBox2.Text + "%' OR  MB002 LIKE '%" + textBox2.Text + "%'";
            string sqlstring = "SELECT [MB001],[MB002],[MB003],[MB051],[MB064],[MB004],[GpSno],[CostPrice] FROM [Products]" + sqlparam;

            SqlConnection conn = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd = new SqlCommand(sqlstring, conn);
            cmd.Connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DT=new DataTable();
            DT.Load(sdr);
            dataGridView1.DataSource = DT;
            //label3.Text = DT.Rows.Count.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string GpSno="";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("", conn1);
            cmd1.Connection.Open();
            if (textBox9.Text.ToString() != "")
            {
                int loc = textBox8.Text.IndexOf(":");
                if (loc > 0)
                {
                    GpSno = textBox8.Text.Substring(0, loc);
                }
                if (tabPage2.Text == "編輯")
                {
                    cmd1.CommandText = $"UPDATE [Products] SET [MB051] ={textBox5.Text} ,[MB002]='{textBox6.Text}',[MB003]='{textBox3.Text}',[MB004]='{textBox4.Text}',[CostPrice]={textBox7.Text},[GpSno]='{GpSno}' WHERE [MB001]='{textBox9.Text}'";
                    if (cmd1.ExecuteNonQuery() > 0)
                    {
                        ButtonChange(1);
                        MessageBox.Show("存檔完成....");
                    }
                    else
                    {
                        MessageBox.Show("存檔失敗!!!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (tabPage2.Text == "新增")
                {
                    cmd1.CommandText = $"INSERT INTO Products([MB001],[MB002],[MB003],[MB004],[MB051],[MB064],[CostPrice],[GpSno]) VALUES('{textBox9.Text}','{textBox6.Text}','{textBox3.Text}','{textBox4.Text}',{textBox5.Text},0,{textBox7.Text},'{GpSno}')";
                    if (cmd1.ExecuteNonQuery() > 0)
                    {
                        textBox5.Value = 0;
                        textBox7.Value = 0;
                        textBox9.Text = "";
                        textBox6.Text = "";
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox8.Text = "";
                        textBox9.Enabled = true;
                        ButtonChange(1);
                        MessageBox.Show("存檔完成....");
                    }
                    else
                    {
                        MessageBox.Show("存檔失敗!!!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }               
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox5.Value = 0;
            textBox7.Value = 0;
            textBox9.Text = "";
            textBox6.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox8.Text = "";
            textBox9.Enabled = true;
            tabPage2.Text = "新增";
            ButtonChange(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow dgr = dataGridView1.SelectedRows[0];
                if (dgr.Cells["GpSno"].Value.ToString().Trim() != "")
                {
                    for (int i = 0; i < textBox8.Items.Count; i++)
                    {
                        string tmp1 = textBox8.Items[i].ToString();
                        int loc = tmp1.IndexOf(":");
                        if (loc > 0)
                        {
                            tmp1 = textBox8.Items[i].ToString().Substring(0, loc);
                            if (tmp1 == dgr.Cells["GpSno"].Value.ToString().Trim())
                            {
                                textBox8.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                textBox5.Value = Convert.ToDecimal(dgr.Cells["MB051"].Value);
                textBox7.Value = Convert.ToDecimal(dgr.Cells["CostPrice"].Value);
                textBox9.Text = dgr.Cells["MB001"].Value.ToString().Trim();
                textBox6.Text = dgr.Cells["MB002"].Value.ToString().Trim();
                textBox3.Text = dgr.Cells["MB003"].Value.ToString().Trim();
                textBox4.Text = dgr.Cells["MB004"].Value.ToString().Trim();
                textBox9.Enabled = false;
                tabPage2.Text = "編輯";
                ButtonChange(1);
            }
        }

        private void UserProductCreate_Load(object sender, EventArgs e)
        {
            textBox7.Visible= MainForm.UserPrivat == "9" ? true : false;
            //if (MainForm.UserPrivat=="9")
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 =new SqlCommand("SELECT [GpSno],[GpName] FROM [DiscountRule]", conn1);
            cmd1.Connection.Open();
            SqlDataReader dr1 = cmd1.ExecuteReader();
            textBox8.Items.Clear();
            textBox8.Items.Add("");
            while (dr1.Read())
            {
                textBox8.Items.Add($"{dr1["GpSno"].ToString()}:{dr1["GpName"].ToString()}");
            }
            dr1.Close();
            Change(1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
            ButtonChange(0);
            Change(1);
        }
    }
}
