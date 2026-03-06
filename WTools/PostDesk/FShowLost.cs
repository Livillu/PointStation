using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTools.PostDesk
{
    public partial class FShowLost : Form
    {
        DataTable dt;
        public bool isok = true;
        public FShowLost(DataTable dt)
        {
            InitializeComponent();
            this.dt = dt;
        }

        private void FShowLost_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = dt;
        }
        public bool GetResult() {  
            return isok; 
        }
        private void button2_Click(object sender, EventArgs e)
        {
            isok = false;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isok = true;
            Close();
        }
    }
}
