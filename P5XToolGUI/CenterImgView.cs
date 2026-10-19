using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace P5XToolGUI
{
    public partial class CenterImgView : UserControl
    {
        public CenterImgView()
        {
            InitializeComponent();
        }
        public void SetImg(Bitmap img, string? name)
        {
            ClearImage();
            pictureBox1.Image = img;
            pictureBox1.Tag = name;
            updatePicPos();
        }
        private void updatePicPos()
        {
            if (pictureBox1.Image == null) return;
            float imgAspect = (float)pictureBox1.Image.Width / pictureBox1.Image.Height;
            int width = Math.Min((int)(Height * imgAspect), Width);
            int height = (int)(width / imgAspect);
            pictureBox1.Height = height;
            pictureBox1.Width = width;
            pictureBox1.Location = new Point((Width - width) / 2, (Height - height) / 2);
        }
        public void ClearImage()
        {
            pictureBox1.Image = null;
            pictureBox1.Tag = null;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            GUtils.PictureBox_OnRightClick(sender, e);
        }

        private void CenterImgView_Resize(object sender, EventArgs e)
        {
            updatePicPos();
        }
    }
}
