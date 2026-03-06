using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTools
{
    public partial class dialogLogin : Form
    {
        public dialogLogin()
        {
            InitializeComponent();
        }

        private void dialogLogin_Load(object sender, EventArgs e)
        {
            dialogLogin1 dl=new dialogLogin1();
            DialogResult result = dl.ShowDialog();
            if (result == DialogResult.OK)
            {
                var tmp = dl.Getmsg();
                if (tmp==null || tmp == "")
                {
                    Application.Exit();
                }
                else
                {
                    MainForm.UserId = dl.Getmsg();
                    MainForm.UserPrivat = dl.Getprivate();
                    Close();
                }
            }
        }
    }
}
