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
    public partial class FCheckProduct : Form
    {
        private DataTable DT;
        public FCheckProduct(DataTable dt)
        {
            InitializeComponent();
            this.DT = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FCheckProduct_Load(object sender, EventArgs e)
        {
            foreach (DataRow item in DT.Rows) {
                listBox1.Items.Add(item["MB001"].ToString()+"("+ item["MB002"].ToString() + ") 無庫存....");
            }
        }
    }
}
