using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class frmConnectDialog : Form
    {
        private Client form;

        public frmConnectDialog()
        {
            InitializeComponent();
        }

        public frmConnectDialog(Client form)
        {
            InitializeComponent();
            this.form = form;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            form.ipAdress = txtIP.Text.ToString();
            form.port = int.Parse(txtPort.Text.ToString());
            form.StartConnect();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void txtIP_KeyUp(object sender, KeyEventArgs e)
        {
            //For Debugging Keyboard
            //MessageBox.Show(e.KeyCode.ToString());
        }
    }
}
