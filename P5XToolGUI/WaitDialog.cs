using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P5XToolGUI
{
    public partial class WaitDialog : Form
    {
        public WaitDialog()
        {
            InitializeComponent();
        }
        public void SetStatus(int prog, string text = "")
        {
            statusLabel.Text = text;
            progressBar.Value = prog;
        }
    }
}
