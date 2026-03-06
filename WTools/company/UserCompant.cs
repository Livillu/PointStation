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

namespace WTools
{
    public partial class UserCompant : UserControl
    {
        public UserCompant()
        {
            InitializeComponent();
        }

        private void UserCompant_Load(object sender, EventArgs e)
        {
            string sql = "SELECT TOP (1) [SupId],[SupName],[SupCname],[SupTel],[SupAddr],[SupSno],[Boss],[CTel],[SupEmail],[SupWeb],[SupFax] FROM [Company]";
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand(sql, conn1);
            cmd1.Connection.Open();
            SqlDataReader reader = cmd1.ExecuteReader();
            if (reader.Read())
            {
                tbSupId.Text = reader["SupId"].ToString();
                tbBoss.Text = reader["Boss"].ToString();
                tbCTel.Text = reader["CTel"].ToString();
                tbSupAddr.Text = reader["SupAddr"].ToString();
                tbSupCname.Text = reader["SupCname"].ToString();
                tbSupEmail.Text = reader["SupEmail"].ToString();
                tbSupFax.Text = reader["SupFax"].ToString();
                tbSupName.Text = reader["SupName"].ToString();
                tbSupSno.Text = reader["SupSno"].ToString();
                tbSupTel.Text = reader["SupTel"].ToString();
                tbSupWeb.Text = reader["SupWeb"].ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = $"if(SELECT count(*) FROM [Company] where SupId='{tbSupId.Text}')>0 ";
            sql+=$"UPDATE [Company] SET [SupName] = ,[SupCname] = ,[SupTel] = ,[SupAddr] = ,[SupSno] = ,[Boss] = ,[CTel] = ,[SupEmail] = ,[SupWeb] = ,[CachMoney] = ,[SupFax] = WHERE [SupId] =";
            sql += "else INSERT INTO [Company] ([SupId] ,[SupName],[SupCname],[SupTel],[SupAddr],[SupSno],[Boss],[CTel],[SupEmail],[SupWeb],[CachMoney],[SupFax]) VALUES(";
            sql += ")";
        }
    }
}
