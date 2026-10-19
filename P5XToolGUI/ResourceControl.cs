using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZstdSharp.Unsafe;
using P5XTool;

namespace P5XToolGUI
{
    public partial class ResourceControl : UserControl
    {
        const int imageW = 64;
        const int imageH = 64;
        readonly ImageList imgList = new()
        {
            ImageSize = new Size(imageW, imageH)
        };
        public ResourceControl()
        {
            InitializeComponent();
            listView1.LargeImageList = imgList;
        }

        public void ClearResources()
        {
            listView1.Clear();
            imgList.Images.Clear();
        }

        public void SuspendResources()
        {
            listView1.SuspendLayout();
        }
        public void ResumeResources()
        {
            listView1.ResumeLayout();
        }

        public void AddResource(string key, string name)
        {
            if (string.IsNullOrEmpty(name) || name == "AAAAAA==") return;
            foreach (var assetName in name.Split(','))
            {
                if (assetName.EndsWith(".png"))
                {
                    foreach (var subAsset in I18NAssetMap.GetAllAssetNames(assetName))
                    {
                        if (!imgList.Images.ContainsKey(assetName))
                        {
                            var bmp = TexUtils.GetBitmapForImage(subAsset);
                            if (bmp != null)
                            {
                                var resizedBmp = GUtils.GetPreviewImg(bmp, imageW, imageH, Color.DarkGray);
                                imgList.Images.Add(assetName, resizedBmp);
                            }
                            var listItem = new ListViewItem(assetName)
                            {
                                Tag = assetName,
                                ImageKey = assetName
                            };
                            listView1.Items.Add(listItem);
                        }
                    }
                }
            }
            //var groupBox = new GroupBox() { Text = key, Width = 216, Height = 48 };
            //groupBox.Controls.Add(new TextBox() { Text = $"{name}", Width = 200, Location = new Point(8, 16), ReadOnly = true });
            //var offset = 46;
            //foreach (var assetName in name.Split(','))
            //{
            //    if (assetName.EndsWith(".png"))
            //    {
            //        foreach (var subAsset in I18NAssetMap.GetAllAssetNames(assetName))
            //        {
            //            var bmp = TexUtils.GetBitmapForImage(subAsset);
            //            if (bmp != null)
            //            {
            //                var pictureBox1 = new PictureBox();
            //                pictureBox1.Image = bmp;
            //                pictureBox1.BackColor = Color.Magenta;
            //                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //                if (bmp.Width > bmp.Height)
            //                {
            //                    pictureBox1.Height = (int)(128 * ((float)bmp.Height / bmp.Width));
            //                    pictureBox1.Width = 128;
            //                }
            //                else
            //                {
            //                    pictureBox1.Height = 128;
            //                    pictureBox1.Width = (int)(128 * ((float)bmp.Width / bmp.Height));
            //                }
            //                pictureBox1.Tag = assetName;
            //                pictureBox1.MouseClick += Utils.PictureBox_OnRightClick;
            //                groupBox.Controls.Add(pictureBox1);
            //                pictureBox1.Location = new Point(8, offset);
            //                offset += pictureBox1.Height + 7;
            //                groupBox.Height += pictureBox1.Height + 7;
            //            }
            //        }
            //    }
            //}
            //if (offset != 46)
            //{
            //    resFlow.Controls.Add(groupBox);
            //}
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var listItem = listView1.SelectedItems[0];
            string? assetName = listItem.Tag as string;
            if (string.IsNullOrEmpty(assetName)) return;
            var bmp = TexUtils.GetBitmapForImage(assetName);
            if (bmp == null) return;
            Utils.CopyPngToClipboard(bmp, assetName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    var listItem = listView1.SelectedItems[0];
                    string? assetName = listItem.Tag as string;
                    if (string.IsNullOrEmpty(assetName)) return;
                    var bmp = TexUtils.GetBitmapForImage(assetName);
                    if (bmp == null) return;
                    SaveFileDialog saveFileDialog = new()
                    {
                        FileName = assetName,
                        Filter = "PNG (*.png)|*.png"
                    };
                    var result = saveFileDialog.ShowDialog(this);
                    if (result != DialogResult.OK) return;
                    bmp.Save(saveFileDialog.FileName);
                }
                else
                {
                    FolderBrowserDialog dialog = new();
                    var result = dialog.ShowDialog(this);
                    if (result != DialogResult.OK) return;
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        string? assetName = item.Tag as string;
                        if (!string.IsNullOrEmpty(assetName))
                        {
                            var bmp = TexUtils.GetBitmapForImage(assetName);
                            bmp?.Save(Path.Join(dialog.SelectedPath, assetName));
                        }
                    }
                }
            }
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            copyToolStripMenuItem.Enabled = listView1.SelectedItems.Count == 1;
            saveToolStripMenuItem.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var listItem = listView1.SelectedItems[0];
                string? assetName = listItem.Tag as string;
                if (!string.IsNullOrEmpty(assetName)) Clipboard.SetText(assetName);
            }
        }
    }
}
