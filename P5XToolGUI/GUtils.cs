using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using P5XTool;

namespace P5XToolGUI
{
    public static class GUtils
    {
        public async static void LongTask(Form? owner, string what, Func<Action<int, string>, object?> work, Action<object?>? done = null)
        {
            var progressDialog = new WaitDialog();
            progressDialog.Show(owner);
            if (owner != null)
            {
                owner.Enabled = false;
                progressDialog.Location = new Point(
                    owner.Location.X + (owner.Width - progressDialog.Width) / 2,
                    owner.Location.Y + (owner.Height - progressDialog.Height) / 2);
                progressDialog.Icon = owner.Icon;
            }
            progressDialog.Text = what;
            IProgress<(int, string)> progress = new Progress<(int, string)>(t =>
            {
                progressDialog.SetStatus(t.Item1, t.Item2);
            });
            var action = new Action<int, string>((p, s) => progress.Report((p, s)));
            var r = await Task.Run(() => work(action));
            owner?.Enabled = true;
            progressDialog.Close();
            done?.Invoke(r);
        }

        public static void PictureBox_OnRightClick(object? sender, MouseEventArgs e)
        {
            PictureBox? pictureBox = sender as PictureBox;
            if (pictureBox == null || pictureBox.Image == null) return;
            string assetName = pictureBox.Tag as string ?? "image.png";
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Copy", null, (s, ea) =>
                {
                    var image = pictureBox.Image;
                    if (image != null)
                    {
                        Utils.CopyPngToClipboard(image, assetName);
                    }
                });
                contextMenu.Items.Add("Save As...", null, (s, ea) =>
                {
                    SaveFileDialog saveFileDialog = new();
                    saveFileDialog.FileName = assetName;
                    saveFileDialog.Filter = "PNG (*.png)|*.png";
                    var result = saveFileDialog.ShowDialog(pictureBox);
                    if (result != DialogResult.OK) return;
                    pictureBox.Image.Save(saveFileDialog.FileName);
                });
                contextMenu.Show(pictureBox, e.Location);
            }
        }
        public static string FixBetaString(string str)
        {
            if (!GameMgr.IsCbt) return str;
            var b = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(b);
        }

        public static Bitmap ResizeImage(Bitmap bmp, int width, int height)
        {
            var newBmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(newBmp))
            using (var attributes = new ImageAttributes())
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.CompositingMode = CompositingMode.SourceCopy;

                attributes.SetWrapMode(WrapMode.TileFlipXY);

                g.DrawImage(
                    bmp,
                    new Rectangle(0, 0, width, height),
                    0, 0, bmp.Width, bmp.Height,
                    GraphicsUnit.Pixel,
                    attributes
                );
            }

            return newBmp;
        }

        public static Bitmap GetPreviewImg(Bitmap full, int previewWidth, int previewHeight, Color bgColor)
        {
            float imgAspect = (float)full.Width / full.Height;

            int drawWidth = Math.Min((int)(previewWidth * imgAspect), previewWidth);
            int drawHeight = (int)(drawWidth / imgAspect);

            int xPos = (previewWidth - drawWidth) / 2;
            int yPos = (previewHeight - drawHeight) / 2;

            Bitmap result = new(previewWidth, previewHeight);

            using (Graphics g = Graphics.FromImage(result))
            using (var attributes = new ImageAttributes())
            using (SolidBrush bgBrush = new(bgColor))
            {
                g.FillRectangle(bgBrush, xPos, yPos, drawWidth, drawHeight);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighQuality;

                attributes.SetWrapMode(WrapMode.TileFlipXY);

                g.DrawImage(
                    full,
                    new Rectangle(xPos, yPos, drawWidth, drawHeight),
                    0, 0, full.Width, full.Height,
                    GraphicsUnit.Pixel,
                    attributes
                );
            }

            return result;
        }
        public static Form NewFormForControl<T>(T control, string title) where T : Control {
            var form = new Form
            {
                Icon = MainForm.MainIcon,
                Text = title
            };
            form.Controls.Add(control);
            form.FormClosing += (s, e) =>
            {
                control.Dispose();
            };
            return form;
        }
    }
}
