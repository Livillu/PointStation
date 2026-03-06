using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WTools
{
    public partial class EditProduct : Form
    {
        DataRow DR;
        DataTable dt;
        public EditProduct(DataRow dr)
        {
            DR=dr;
            InitializeComponent();
        }

        public DataRow ResultProduct()
        {
            return DR;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string SupId = "";
            if (combbox1.SelectedIndex > -1) SupId = dt.Rows[combbox1.SelectedIndex][0].ToString();
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand($"UPDATE [Products] SET [MB051] ={textBox5.Text} ,[MB002]='{textBox2.Text}',[MB003]='{textBox3.Text}',[MB004]='{textBox4.Text}',[SupId]='{SupId}',[CostPrice]={textBox7.Text},[GpSno]='{textBox8.Text}' WHERE [MB001]='{textBox1.Text}'", conn1);
            cmd1.Connection.Open();
            //新增
            if (DR[0].ToString() == "")
            {
                cmd1.CommandText = $"INSERT INTO Products([MB001],[MB002],[MB003],[MB004],[MB051],[MB064],[SupId],[CostPrice],[GpSno]) VALUES('{textBox1.Text}','{textBox2.Text}','{textBox3.Text}','{textBox4.Text}',{textBox5.Text},0,'{SupId}',{textBox7.Text},'{textBox8.Text}')";            

            }//修改
            if (cmd1.ExecuteNonQuery() > 0)
            {
                DR[0] = textBox1.Text;//MB001
                DR[1] = textBox2.Text;//MB002
                DR[2] = textBox3.Text; //MB003   
                DR[3] = textBox4.Text;//MB004
                DR[4] = textBox5.Value;//MB051
                DR[6] = textBox8.Text.Trim();//GpSno
                DR[8] = SupId;//SupId
                DR[9] = textBox7.Value;//CostPrice
            }
        }

        private void EditProduct_Load(object sender, EventArgs e)
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [SupId],[SupName] FROM [Support]", conn1);
            cmd1.Connection.Open();
            SqlDataReader sdr= cmd1.ExecuteReader();
            dt = new DataTable();
            dt.Load(sdr);
            combbox1.Items.Clear();
            combbox1.DataSource = dt;
            combbox1.DisplayMember = "SupName";
            if (DR[0].ToString() !="")
            {
                textBox1.Text= DR[0].ToString();
                textBox1.ReadOnly= true;
                textBox2.Text= DR[1].ToString();
                textBox3.Text= DR[2].ToString();
                textBox4.Text= DR[3].ToString();
                textBox5.Text= DR[4].ToString();
                textBox7.Text = DR[9].ToString();
                textBox8.Text = DR[6].ToString();
                combbox1.Text = DR[8].ToString();
                combbox1.Enabled = false;
            }
            else
            {
                textBox1.ReadOnly = false;
                textBox2.ReadOnly = false;
                combbox1.Enabled = true;
            }
        }
    }
}
