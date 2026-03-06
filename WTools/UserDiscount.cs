using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools
{
    public partial class UserDiscount : UserControl
    {
        public DataTable dt;
        public UserDiscount()
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
            button2.Enabled = true;
            button1.Enabled = true;
            button6.Enabled = true;
            switch (index)
            {
                case 1://新增狀態
                    button2.Enabled = false;
                    button6.Enabled = false;
                    button1.Enabled = false;
                    break;
            }
        }

        private void UserDiscount_Load(object sender, EventArgs e)
        {
            Change(1);
        }     

        private void button1_Click(object sender, EventArgs e)
        {
            decimal Quty=0, Price=0, SubMoney=0, SubDiscount=0;
            if (radioButton3.Checked)
            {
                Quty=numericUpDown1.Value;
            }
            else if (radioButton4.Checked)
            {
                Price = numericUpDown1.Value;
            }

            if (radioButton1.Checked)
            {
                SubMoney = numericUpDown2.Value;
            }
            else if (radioButton2.Checked)
            {
                SubDiscount = numericUpDown2.Value;
            }

            if (numericUpDown1.Value > 0 && numericUpDown2.Value>0 && textBox1.Text != "" && textBox3.Text != "")
            {
                string sql = $"UPDATE [DiscountRule] SET [Quty] = {Quty},[Price] = {Price},[SubMoney] = {SubMoney},[SubDiscount] = {SubDiscount},[StDate] = '{dateTimePicker1.Value.ToString("yyyy-MM-dd")}',[EdDate] = '{dateTimePicker2.Value.ToString("yyyy-MM-dd")}',[GpName] = '{textBox3.Text}' WHERE [GpSno]='{textBox1.Text}'";
                if (tabPage2.Text == "編輯")
                {

                } else if (tabPage2.Text == "新增")
                {
                    sql = "INSERT INTO [DiscountRule]([GpSno],[Quty],[Price],[SubMoney],[SubDiscount],[StDate],[EdDate],[GpName]) VALUES";
                    sql += $"('{textBox1.Text}',{Quty},{Price},{SubMoney},{SubDiscount},'{dateTimePicker1.Value.ToString("yyyy-MM-dd")}','{dateTimePicker2.Value.ToString("yyyy-MM-dd")}','{textBox3.Text}')";
                }                                
                SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                SqlCommand cmd1 = new SqlCommand(sql, conn1);
                cmd1.Connection.Open();
                if (cmd1.ExecuteNonQuery() > 0)
                {
                    button3.PerformClick();
                    UserDiscount_Load(null,null);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            Change(1);
            dt = new DataTable();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [GpSno],[Quty],[Price],[SubMoney],[SubDiscount],[StDate],[EdDate],[GpName] FROM [DiscountRule]", conn1);
            cmd1.Connection.Open();
            SqlDataReader dr1 = cmd1.ExecuteReader();
            dt.Load(dr1);
            dr1.Close();
            dataGridView1.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var tmp= dataGridView1.SelectedRows[0].Index;
                tabPage2.Text = "編輯";
                Change(2);
                ButtonChange(1);
                DataRow dr=dt.Rows[tmp];
               // [GpSno],[Quty],[Price],[SubMoney],[SubDiscount],[StDate],[EdDate],[GpName]
                if (Convert.ToInt16(dr["Quty"]) > 0 && Convert.ToDecimal(dr["Price"]) == 0)
                {
                    radioButton3.Checked = true;
                    numericUpDown1.Value = Convert.ToInt16(dr["Quty"]);
                }
                else if (Convert.ToInt16(dr["Quty"]) == 0 && Convert.ToDecimal(dr["Price"]) > 0)
                {
                    radioButton4.Checked = true;
                    numericUpDown1.Value = Convert.ToDecimal(dr["Price"]);
                }
                else
                {
                    radioButton3.Checked = false;
                    radioButton4.Checked = false;
                    numericUpDown1.Value = 0;
                }

                if (Convert.ToInt16(dr["SubMoney"]) > 0 && Convert.ToInt16(dr["SubDiscount"]) == 0)
                {
                    radioButton1.Checked = true;
                    numericUpDown2.Value = Convert.ToInt16(dr["SubMoney"]);
                }
                else if (Convert.ToInt16(dr["SubMoney"]) == 0 && Convert.ToInt16(dr["SubDiscount"]) > 0)
                {
                    radioButton2.Checked = true;
                    numericUpDown2.Value = Convert.ToInt16(dr["SubDiscount"]);
                }
                else
                {
                    radioButton3.Checked = false;
                    radioButton4.Checked = false;
                    numericUpDown2.Value = 0;
                }

                dateTimePicker1.Value = Convert.ToDateTime(dr["StDate"]);
                dateTimePicker2.Value = Convert.ToDateTime(dr["EdDate"]);
                textBox3.Text = dr["GpName"].ToString();
                textBox1.Text = dr["GpSno"].ToString();
                textBox1.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            radioButton3.Checked = true;
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            textBox1.Enabled = true;
            textBox3.Text = "";
            tabPage2.Text = "新增";
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;
            ButtonChange(1);
            Change(2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ButtonChange(0);
            Change(1);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var tmp = dataGridView1.SelectedRows[0].Index;
                DataRow dr = dt.Rows[tmp];
                if (DialogResult.Yes == MessageBox.Show($"折扣代碼{dr["GpSno"]}確認刪除???", "刪除資料", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    // [GpSno],[Quty],[Price],[SubMoney],[SubDiscount],[StDate],[EdDate],[GpName]
                    SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
                    SqlCommand cmd1 = new SqlCommand($"DELETE FROM [DiscountRule] WHERE [GpSno]='{dr["GpSno"]}'", conn1);
                    cmd1.Connection.Open();
                    if (cmd1.ExecuteNonQuery() > 0)
                    {
                        dt.Rows.Remove(dr);
                        MessageBox.Show("刪除成功....");
                    }
                    else MessageBox.Show("刪除失敗!!!請重試....");
                }
            }
        }
    }
}
