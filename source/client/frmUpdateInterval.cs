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
    public partial class frmUpdateInterval : Form
    {

        private Client form;

        public frmUpdateInterval()
        {
            InitializeComponent();
        }

        public frmUpdateInterval(Client form)
        {
            InitializeComponent();
            this.form = form;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            form.ImageDelayChange(trackBar1.Value);
            imageDelayLabel.Text = ((float)trackBar1.Value / 1000).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
