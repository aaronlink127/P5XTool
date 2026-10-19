using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UnityAssetLib;

namespace P5XToolGUI
{
    public partial class SpriteView : UserControl
    {
        public SpriteView()
        {
            InitializeComponent();
            metaLabel.Text = "";
        }
        public void SetImg(Bitmap bmp, string? name)
        {
            metaLabel.Text += $"Size: {bmp.Width}x{bmp.Height}\n";
            centerImgView1.SetImg(bmp, name);
        }
        public void ClearImage()
        {
            metaLabel.Text = "";
            centerImgView1.ClearImage();
        }
        public void SetSprite(string name)
        {
            ClearImage();
            var tex = AssetManager.LoadAsset(name);
            if (tex != null)
            {
                SetSprite(tex.Asset);
            }
        }
        public void SetSprite(UnityAssetLib.Object obj)
        {
            ClearImage();
            var bmp = TexUtils.GetBitmapForImage(obj);
            if (bmp != null)
            {
                string? name = null;
                Texture2D? tex = null;
                if (obj is Texture2D ntex)
                {
                    tex = ntex;
                    name = ntex.m_Name + ".png";
                }
                else if (obj is Sprite spr)
                {
                    tex = spr.GetTexture();
                    name = spr.m_Name + ".png";
                }
                if (tex != null)
                {
                    metaLabel.Text += $"Format: {tex.m_TextureFormat}\n";
                }
                SetImg(bmp, name);
            }
        }
    }
}
