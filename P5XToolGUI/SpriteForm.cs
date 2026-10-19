using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace P5XToolGUI
{
    public partial class SpriteForm : Form
    {
        public SpriteForm()
        {
            InitializeComponent();
        }

        public void SetSprite(string img)
        {
            textBox1.Text = img;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            spriteView1.SetSprite(textBox1.Text);
        }
    }
}
