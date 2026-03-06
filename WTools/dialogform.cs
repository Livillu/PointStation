using System;
using System.Windows.Forms;

namespace WTools
{
    public partial class dialogform : Form
    {
        int quty;
        public string productName; 
        public dialogform(string ptname)
        {
            InitializeComponent();
            label2.Text = ptname;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            quty=(int)numericUpDown1.Value;
        }
        public int GetMsg()
        {
            return quty;
        }

        private void dialogform_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                button1.PerformClick();
            }
        }

        private void dialogform_KeyPress_1(object sender, KeyPressEventArgs e)
        {

        }
    }
}
