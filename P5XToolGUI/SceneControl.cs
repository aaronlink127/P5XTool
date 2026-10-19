using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityAssetLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using P5XTool;

namespace P5XToolGUI
{
    public partial class SceneControl : UserControl
    {
        readonly long sceneSn;
        public SceneControl(long sn)
        {
            InitializeComponent();
            sceneSn = sn;
            //Controls.Clear();
            //ScenePreviewControl preview = new();
            //preview.Dock = DockStyle.Fill;
            //Controls.Add(preview);
            //preview.OnLoad(() =>
            //{
            //    var sceneData = DB.GetDataDict("Scene", "ConfScene", sceneSn);
            //    var unityFile = (sceneData["asset"] as string) + ".unity";
            //    preview.LoadScene(unityFile);
            //});

            var scene = DB.GetDataDict("Scene", "ConfScene", sceneSn);
            if (scene != null)
            {
                resourceControl1.SuspendResources();
                var minimapSn = (string)scene["miniMapSn"]!;
                if (minimapSn != "AAAAAA==")
                {
                    var minimap = DB.GetDataDict("Map", "ConfMinimap", minimapSn);
                    if (minimap != null) { 
                        foreach (var kv in minimap)
                        {
                            if (kv.Value is string s)
                            {
                                resourceControl1.AddResource(kv.Key, s);
                            }
                        }
                    }
                }
                resourceControl1.ResumeResources();
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveFileDialog = new();
            var result = saveFileDialog.ShowDialog(this);
            if (result != DialogResult.OK) return;
            var filePath = saveFileDialog.SelectedPath;
            HashSet<string> bundleList = [];
            var scene = DB.GetDataDict("Scene", "ConfScene", sceneSn);
            if (scene == null) return;
            string? asset = scene["asset"] as string;
            if (string.IsNullOrEmpty(asset)) return;
            string? sceneBundle = AssetBundleUtils.GetBundleByAsset(asset + ".unity");
            if (sceneBundle == null)
            {
                MessageBox.Show($"Failed to find bundle for scene asset: {asset}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bundleList.Add(sceneBundle);
            AssetBundleUtils.GetAllDependencies(sceneBundle, bundleList);
            Utils.SaveBundles(filePath, bundleList.ToList());
        }
    }
}
