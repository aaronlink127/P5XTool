using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using P5XLib;
using P5XTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using UnityAssetLib;

namespace P5XToolGUI
{
    public partial class DialogHeadForm : Form
    {
        static readonly List<string> layerNames = [
            "AVGImageShadow",
            "BlackBorderImage",
            "Head",
            "HeadMask",
            "AVGImage",
            "Mouth",
            "Eye",
            "Expression",
            "Adornment1",
            "Adornment2",
            "Adornment3",
            "Adornment4",
            "Adornment5",
            "Adornment6"
        ];
        readonly List<DialogHead2D> dialogHeads;
        DialogHead2D? curHead;
        List<string> eyeFrames = [];
        List<string> mouthFrames = [];
        List<string> expressionFrames = [];
        private readonly DialogHeadRenderer renderer = new();
        readonly bool[] enabledLayers = new bool[14];
        readonly CheckBox[] checkboxes = new CheckBox[14];
        public DialogHeadForm()
        {
            InitializeComponent();
            dialogHeads = DB.GetTable<DialogHead2D>();
            foreach (DialogHead2D d in dialogHeads)
            {
                dialogHeadSelect.Items.Add(d.Sn);
            }
            var nativeSettings = new NativeWindowSettings()
            {
                ClientSize = new OpenTK.Mathematics.Vector2i(1, 1),
                Title = "Dummy",
                Flags = ContextFlags.ForwardCompatible,
                StartVisible = false
            };

            int posOffset = simToggle.Location.Y;
            for (int i = 0; i < enabledLayers.Length; i++)
            {
                posOffset += simToggle.Height;
                enabledLayers[i] = true;
                var checkbox = new CheckBox()
                {
                    Text = layerNames[i],
                    Checked = true,
                    AutoSize = true,
                    Location = new Point(3, posOffset)
                };
                int thisIdx = i;
                checkbox.CheckStateChanged += (s, e) =>
                {
                    enabledLayers[thisIdx] = checkbox.Checked;
                    updateImage();
                };
                checkboxes[i] = checkbox;
                splitContainer1.Panel2.Controls.Add(checkbox);
            }
        }
        private void updateImage()
        {
            var newPic = GetNewPic();
            toolTip1.SetToolTip(eyeAnimFrame, eyeAnimFrame.SelectedIndex == -1 ? null : eyeFrames[eyeAnimFrame.SelectedIndex]);
            toolTip1.SetToolTip(mouthAnimFrame, mouthAnimFrame.SelectedIndex == -1 ? null : mouthFrames[mouthAnimFrame.SelectedIndex]);
            toolTip1.SetToolTip(expressionAnimFrame, expressionAnimFrame.SelectedIndex == -1 ? null : expressionFrames[expressionAnimFrame.SelectedIndex]);

            if (newPic != null)
            {
                string imgName = $"{curHead?.Sn}_{eyeAnimFrame.SelectedIndex}_{mouthAnimFrame.SelectedIndex}_{expressionAnimFrame.SelectedIndex}.png";
                centerImgView1.SetImg(newPic, imgName);
            }
            else
            {
                centerImgView1.ClearImage();
            }
        }
        private Bitmap? GetNewPic()
        {
            if (curHead == null) return null;
            //var materialParam = DB.GetData<DialogMatParam>(curHead.MaterialParamSn);
            var materialParam = DB.GetDataDict("Dialog", "ConfDialogMatParam", curHead.MaterialParamSn);
            int layerIdx = 0;

            ImageLayer rootLayer = new ImageLayer(null, null);
            var headRoot = rootLayer;
            bool noHead = false;
            rootLayer.visible = false;
            renderer.root_layer = rootLayer;

            rootLayer.pivot = new Vector2(0.5f, 1.0f);

            ImageLayer addLayer(string? imgName, string? matParam = null, bool head = true)
            {
                if (imgName == "AAAAAA==") imgName = null;
                checkboxes[layerIdx].ForeColor = imgName == null ? SystemColors.GrayText : SystemColors.ControlText;
                var enabled = enabledLayers[layerIdx++];
                var layer = new ImageLayer(imgName, simToggle.Checked ? matParam : null)
                {
                    visible = enabled
                };
                layer.pivot = new Vector2(0.5f, 1.0f);
                if (head && !noHead)
                {
                    headRoot.AddChild(layer);
                }
                else
                {
                    rootLayer.AddChild(layer);
                    layer.SetSizeFromTexture();
                }
                return layer;
            }


            var avgImageShadow = addLayer(curHead.AVGImageShadow, null, false);
            if (avgImageShadow.HasImage())
            {
                avgImageShadow.pivot = new Vector2(0.5f, 1.0f);
                avgImageShadow.position = new Vector2(0, 1074);
            }

            if (curHead.HeadImage != "AAAAAA==")
            {
                headRoot = new ImageLayer(curHead.HeadImage, null);
                headRoot.SetSizeFromTexture();
                headRoot.visible = false;
                headRoot.pivot = new Vector2(0.5f, 1.0f);
                rootLayer.AddChild(headRoot);
            }
            else
            {
                noHead = true;
            }

            addLayer(curHead.BlackBorderImage);
            addLayer(curHead.HeadImage, materialParam?["head"] as string);
            addLayer(curHead.HeadMaskImage);
            var avgImage = addLayer(curHead.AVGImage, null, false);
            avgImage.pivot = new Vector2(0.5f, 1.0f);
            if (avgImage.HasImage())
            {
                avgImage.position = new Vector2(0, 1074);
            }
            string? eyeMouth = materialParam?.GetValueOrDefault("eyeMouth") as string;
            string? eye = eyeMouth ?? materialParam?.GetValueOrDefault("eye") as string;
            string? mouth = eyeMouth ?? materialParam?.GetValueOrDefault("mouth") as string;
            addLayer(mouthAnimFrame.SelectedIndex == -1 ? null : (mouthFrames[mouthAnimFrame.SelectedIndex] + ".png"), mouth);
            addLayer(eyeAnimFrame.SelectedIndex == -1 ? null : (eyeFrames[eyeAnimFrame.SelectedIndex] + ".png"), eye);
            addLayer(expressionAnimFrame.SelectedIndex == -1 ? null : (expressionFrames[expressionAnimFrame.SelectedIndex] + ".png"), materialParam?.GetValueOrDefault("expression") as string);
            addLayer(curHead.Adornment1, materialParam?.GetValueOrDefault("peishi1") as string);
            addLayer(curHead.Adornment2, materialParam?.GetValueOrDefault("peishi2") as string);
            addLayer(curHead.Adornment3, materialParam?.GetValueOrDefault("peishi3") as string);
            addLayer(curHead.Adornment4, materialParam?.GetValueOrDefault("peishi4") as string);
            var adornment5 = addLayer(curHead.Adornment5, materialParam?.GetValueOrDefault("peishi5") as string);
            adornment5.color = new(1, 1, 1, 0.95f);
            addLayer(curHead.Adornment6, materialParam?.GetValueOrDefault("peishi6") as string);

            Bitmap? img = renderer.Draw();
            renderer.Clear();
            return img;
        }

        private static List<string> getFramesFromAnim(string name)
        {
            var anim = DB.GetData<DialogAnimation>(name);
            if (anim != null)
            {
                List<string> frames = [];
                var start = GUtils.FixBetaString(anim.StartSequence);
                if (start != "" && start != "AAAAAA==")
                {
                    frames.AddRange(start.Split(","));
                }
                var loop = GUtils.FixBetaString(anim.LoopSequence);
                if (loop != "" && loop != "AAAAAA==")
                {
                    frames.AddRange(loop.Split(","));
                }
                var end = GUtils.FixBetaString(anim.EndSequence);
                if (end != "" && end != "AAAAAA==")
                {
                    frames.AddRange(end.Split(","));
                }
                return [.. frames.Distinct().Select(x => $"{anim.FrameImage}{x}")];
            }
            return [];
        }

        private void dialogHeadSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            curHead = dialogHeads[dialogHeadSelect.SelectedIndex];
            eyeFrames = getFramesFromAnim(curHead.EyeFrameAnimation);
            eyeAnimFrame.Items.Clear();
            foreach (var frame in eyeFrames)
            {
                eyeAnimFrame.Items.Add(frame);
            }
            mouthFrames = [.. getFramesFromAnim(curHead.MouthFrameAnimation), .. getFramesFromAnim(curHead.MouthFrameNoTalkAnimation)];
            mouthAnimFrame.Items.Clear();
            foreach (var frame in mouthFrames)
            {
                mouthAnimFrame.Items.Add(frame);
            }
            expressionFrames = getFramesFromAnim(curHead.ExpressionFrameAnimation);
            expressionAnimFrame.Items.Clear();
            foreach (var frame in expressionFrames)
            {
                expressionAnimFrame.Items.Add(frame);
            }
            eyeAnimFrame.SelectedIndex = eyeFrames.Count == 0 ? -1 : 0;
            eyeAnimFrame.Enabled = eyeFrames.Count != 0;
            mouthAnimFrame.SelectedIndex = mouthFrames.Count == 0 ? -1 : 0;
            mouthAnimFrame.Enabled = mouthFrames.Count != 0;
            expressionAnimFrame.SelectedIndex = expressionFrames.Count == 0 ? -1 : 0;
            expressionAnimFrame.Enabled = expressionFrames.Count != 0;
            updateImage();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            updateImage();
        }

        private void DialogHeadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            renderer.Dispose();
        }

        //private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        //{
        //    enabledLayers[e.Index] = e.NewValue == CheckState.Checked;
        //    updateImage();
        //    checkedListBox1.SelectedIndex = -1;
        //}

        private void expressionAnimFrame_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updateImage();
        }

        private void mouthAnimFrame_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updateImage();
        }

        private void eyeAnimFrame_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updateImage();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
