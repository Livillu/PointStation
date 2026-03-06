using OfficeOpenXml.Export.HtmlExport.StyleCollectors.StyleContracts;
using ScottPlot.TickGenerators.TimeUnits;
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
    public partial class FOverAcount : Form
    {
        public FOverAcount()
        {
            InitializeComponent();
        }

        private void FOverAcount_Load(object sender, EventArgs e)
        {
            label16.Text = DateTime.Today.ToString("yyyy-MM-dd");
            label17.Text = DateTime.Now.ToString("HH:mm:ss");
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [F1] FROM [OtherConfigs] where [FSno]=2", conn1);
            cmd1.Connection.Open();
            label11.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            cmd1.CommandText = "SELECT ISNULL(sum([Price]),0) FROM [MSales] where DATEDIFF(day, [Cdate], GETDATE())= 0";
            label12.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            cmd1.CommandText = "SELECT ISNULL(sum([Price]),0) FROM [MSales] where Isok = '0' and DATEDIFF(day, [Cdate], GETDATE())= 0";
            label18.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            cmd1.CommandText = "SELECT ISNULL(sum([Price]*[InOut]),0) FROM [OtherCost] where [InOut] = 1 and DATEDIFF(day, [Cdate], GETDATE())= 0";
            label9.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            cmd1.CommandText = "SELECT ISNULL(sum([Price]*[InOut]*-1),0) FROM [OtherCost] where [InOut] = -1 and DATEDIFF(day, [Cdate], GETDATE())= 0";
            label10.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            label15.Text = (Convert.ToInt32(label12.Text) - Convert.ToInt32(label18.Text) + Convert.ToInt32(label11.Text) + Convert.ToInt32(label9.Text) - Convert.ToInt32(label10.Text)).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection conn1 = new SqlConnection(MainForm.OutPoscon);
            SqlCommand cmd1 = new SqlCommand("SELECT [F1] FROM [OtherConfigs] where [FSno]=2", conn1);
            cmd1.Connection.Open();
            label11.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            cmd1.CommandText = $"SELECT ISNULL(sum([Price]),0) FROM [MSales] where FORMAT(Cdate, 'yyyy/MM/dd') between '{dateTimePicker1.Text}' and '{dateTimePicker2.Text}'";
            label12.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            cmd1.CommandText = $"SELECT ISNULL(sum([Price]),0) FROM [MSales] where Isok = '0' and FORMAT(Cdate, 'yyyy/MM/dd') between '{dateTimePicker1.Text}' and '{dateTimePicker2.Text}'";
            label18.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            cmd1.CommandText = $"SELECT ISNULL(sum([Quty]*[Price]*[InOut]),0) FROM [OtherCost] where[InOut] = 1 and FORMAT(Cdate, 'yyyy/MM/dd') between '{dateTimePicker1.Text}' and '{dateTimePicker2.Text}'";
            label9.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            cmd1.CommandText = $"SELECT ISNULL(sum([Quty]*[Price]*[InOut]*-1),0) FROM [OtherCost] where[InOut] = -1 and FORMAT(Cdate, 'yyyy/MM/dd') between '{dateTimePicker1.Text}' and '{dateTimePicker2.Text}'";
            label10.Text = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            label15.Text = (Convert.ToInt32(label12.Text) - Convert.ToInt32(label18.Text) + Convert.ToInt32(label11.Text) + Convert.ToInt32(label9.Text) - Convert.ToInt32(label10.Text)).ToString();

        }
    }
}
