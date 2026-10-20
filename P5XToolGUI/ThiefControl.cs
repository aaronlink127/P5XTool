using AssetRipper.TextureDecoder;
using AssetRipper.TextureDecoder.Bc;
using AssetRipper.TextureDecoder.Dxt;
using AssetRipper.TextureDecoder.Rgb.Formats;
using P5XLib;
using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using P5XTool;

namespace P5XToolGUI
{
    public partial class ThiefControl : UserControl
    {
        static int preferredTabIndex = 0;
        long thiefSn;
        bool modelInit = false;
        bool resourceInit = false;
        public ThiefControl()
        {
            InitializeComponent();
        }
        public ThiefControl(long _thiefSn)
        {
            InitializeComponent();
            thiefSn = _thiefSn;
            Dictionary<string, object?>? thiefData;
            if (thiefSn < 1000)
            {
                thiefData = DB.GetDataDict("ZhuJue", "ConfZhuJue", thiefSn)!;
                thiefData["model"] = GUtils.FixBetaString((thiefData["model"] as string)!);
            }
            else
            {
                thiefData = DB.GetDataDict("Thief", "ConfThief", thiefSn)!;
            }
            var thiefModel = (thiefData["model"] as string)!;
            var thiefModelUI = (thiefData["modelUI"] as string)!;
            foreach (var model in thiefModel.Split(','))
            {
                if (!comboBox1.Items.Contains(model))
                {
                    comboBox1.Items.Add(model);
                }
            }
            if (!comboBox1.Items.Contains(thiefModelUI))
            {
                comboBox1.Items.Add(thiefModelUI);
            }
            try
            {
                var fashions = DB.Query<ThiefModelChange>($"SELECT * FROM ConfThiefModelChange WHERE thiefSn = {thiefSn}");
                foreach (var fashion in fashions)
                {
                    if (!comboBox1.Items.Contains(fashion.ModelSn))
                    {
                        comboBox1.Items.Add(fashion.ModelSn);
                    }
                }
            }
            catch { }
            if (comboBox1.Items.Count <= 1)
            {
                comboBox1.Enabled = false;
            }
            comboBox1.SelectedIndex = 0;
            tabControl.SelectedIndex = preferredTabIndex;
        }
        private void InitData()
        {
            modelInit = false;
            onChangeTab();
        }
        private void onChangeTab()
        {
            if (tabControl.SelectedIndex == 0)
            {
                if (!modelInit) InitModelControls();
            }
            if (tabControl.SelectedIndex == 1) {
                if (!resourceInit) InitResourceControls();
            }
        }
        private void InitModelControls()
        {
            modelTab.Controls.Clear();
            string? selectedModel = comboBox1.SelectedItem as string;
            if (selectedModel == null) return;
            var modelControl = new ModelControl(selectedModel)
            {
                Dock = DockStyle.Fill
            };
            modelTab.Controls.Add(modelControl);
            modelInit = true;
            //var fashionData = DB.Query<ThiefModelChange>($"SELECT * FROM ConfThiefModelChange WHERE modelSn = \"{selectedModel}\"").FirstOrDefault();
        }
        private void InitResourceControls()
        {
            resourceControl1.SuspendResources();
            string? selectedModel = comboBox1.SelectedItem as string;
            if (selectedModel == null) return;
            Dictionary<string, object?> thiefData;
            if (thiefSn < 1000)
            {
                thiefData = DB.GetDataDict("ZhuJue", "ConfZhuJue", thiefSn)!;
            }
            else
            {
                thiefData = DB.GetDataDict("Thief", "ConfThief", thiefSn)!;
            }
            try
            {
                var thiefRating = DB.GetDataDict("ThiefRating", "ConfThiefRating", thiefSn);
                if (thiefRating != null)
                {
                    foreach (var kv in thiefRating)
                    {
                        if (kv.Value is string s)
                        {
                            resourceControl1.AddResource(kv.Key, s);
                        }
                    }
                }
            }
            catch { }
            foreach (var kv in thiefData)
            {
                if (kv.Value is string s)
                {
                    resourceControl1.AddResource(kv.Key, s);
                }
            }
            resourceControl1.ResumeResources();
            resourceInit = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitData();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            preferredTabIndex = tabControl.SelectedIndex;
            onChangeTab();
        }
    }
}
