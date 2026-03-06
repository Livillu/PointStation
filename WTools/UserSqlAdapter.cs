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
    public partial class UserSqlAdapter : UserControl
    {
        public UserSqlAdapter()
        {
            InitializeComponent();
        }
        public SqlDataAdapter sqlDataAdapter;
        public SqlConnection sqlConnection;
        public SqlCommand sqlCommand;
        public DataTable LDT;
        private void UserSqlAdapter_Load(object sender, EventArgs e)
        {
            
            //sqlConnection1.ConnectionString = MainForm.PosErp;
            //sqlCommand1.CommandText = "SELECT [id],[od_id],[od_state],[cus_name],[od_date],[pt_price] FROM [CusOrderM]";
            //sqlDataAdapter1.SelectCommand = sqlCommand1;
            LDT = new DataTable();
            sqlDataAdapter1.Fill(LDT);
            dataGridView1.DataSource = LDT;
            /*
            sqlDataAdapter = new SqlDataAdapter();
            sqlConnection = new SqlConnection();
            sqlCommand = new SqlCommand();
            sqlConnection.ConnectionString = MainForm.PosErp;
            LDT = new DataTable();
            sqlCommand = new SqlCommand($"SELECT [id],[od_id],[od_state],[cus_name],[od_date],[pt_price] FROM [CusOrderM]", sqlConnection);
            sqlDataAdapter.SelectCommand = sqlCommand;
            sqlDataAdapter.Fill(LDT);
            dataGridView1.DataSource = LDT;*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
 
            sqlDataAdapter1.UpdateCommand.CommandText = "UPDATE [CusOrderM] SET [id] = @id,[od_state] = @od_state,[cus_name] = @cus_name,[pt_price] = @pt_price WHERE [od_id]=@od_id";
            // Add parameters
            sqlDataAdapter1.UpdateCommand.Parameters.Add("@id", System.Data.SqlDbType.NVarChar, 15, "id");
            sqlDataAdapter1.UpdateCommand.Parameters.Add("@od_state", System.Data.SqlDbType.NVarChar, 15, "od_state");
            sqlDataAdapter1.UpdateCommand.Parameters.Add("@cus_name", System.Data.SqlDbType.NVarChar, 15, "cus_name");
            sqlDataAdapter1.UpdateCommand.Parameters.Add("@pt_price", System.Data.SqlDbType.Decimal, 15, "pt_price");
            // The @id parameter uses the original value to check for concurrency
            sqlDataAdapter1.UpdateCommand.Parameters.Add("@od_id", System.Data.SqlDbType.NVarChar,20 , "od_id").SourceVersion = System.Data.DataRowVersion.Original;

            sqlDataAdapter1.InsertCommand.CommandText = "INSERT INTO [dbo].[CusOrderM]([id],[od_id],[od_state],[cus_name],[pt_price]) VALUES(@id,@od_id,@od_state,@cus_name,@pt_price)";
            sqlDataAdapter1.InsertCommand.Parameters.Clear();
            sqlDataAdapter1.InsertCommand.Parameters.Add("@id", System.Data.SqlDbType.NVarChar, 15, "id");
            sqlDataAdapter1.InsertCommand.Parameters.Add("@od_id", System.Data.SqlDbType.NVarChar, 20, "od_id");
            sqlDataAdapter1.InsertCommand.Parameters.Add("@od_state", System.Data.SqlDbType.NVarChar, 15, "od_state");
            sqlDataAdapter1.InsertCommand.Parameters.Add("@cus_name", System.Data.SqlDbType.NVarChar, 15, "cus_name");
            sqlDataAdapter1.InsertCommand.Parameters.Add("@pt_price", System.Data.SqlDbType.Decimal, 15, "pt_price");
            sqlDataAdapter1.Update(LDT);
        }
    }
}
