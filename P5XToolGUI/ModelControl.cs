using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using P5XTool;

namespace P5XToolGUI
{
    public partial class ModelControl : UserControl
    {
        bool initModelsList = false;
        bool initResources = false;
        static int preferredTab = 0;
        string modelSn = "";
        ScenePreviewControl? preview;
        public ModelControl()
        {
            InitializeComponent();
        }
        public ModelControl(string modelSn)
        {
            this.modelSn = modelSn;
            InitializeComponent();
            tabControl1.SelectedIndex = preferredTab;
            OnTabChange();
        }
        private void InitModelsList()
        {
            if (initModelsList) return;
            var charModel = DB.GetDataDict("CharacterModel", "ConfCharacterModel", modelSn);
            if (charModel == null)
            {
                return;
            }
            prefabCheckList.BeginUpdate();
            prefabCheckList.Items.Add(I18NAssetMap.GetAssetName((string)charModel["modelSn"]!), true);
            var fashionPartModels = GetDefaultPartModels(modelSn);
            foreach (var partModel in fashionPartModels)
            {
                prefabCheckList.Items.Add(I18NAssetMap.GetAssetName(partModel.Prefab), true);
            }
            prefabCheckList.EndUpdate();
            initModelsList = true;
        }
        private void InitPreview()
        {
            if (preview != null) return;
            var charModel = DB.GetDataDict("CharacterModel", "ConfCharacterModel", modelSn);
            if (charModel == null)
            {
                return;
            }

            preview = new ScenePreviewControl();
            preview.Dock = DockStyle.Fill;
            preview.BringToFront();
            previewTab.Controls.Add(preview);
            preview.OnLoad(() =>
            {
                preview.AddRootPart(I18NAssetMap.GetAssetName((string)charModel["modelSn"]!));
                foreach (var partModel in GetDefaultPartModels(modelSn))
                {
                    preview.AddFashionPart(I18NAssetMap.GetAssetName(partModel.Prefab), partModel.HangPoint);
                }
                preview.ResetCamera();
            });
        }
        private void InitResources()
        {
            if (initResources) return;
            resourceControl1.SuspendResources();
            try
            {
                var fashionData = DB.QueryDict("Thief", $"SELECT * FROM ConfThiefModelChange WHERE modelSn = \"{modelSn}\"").FirstOrDefault();
                if (fashionData != null)
                {
                    foreach (var kvp in fashionData)
                    {
                        if (kvp.Key == "modelSn") continue;
                        string? kvpstr = kvp.Value as string;
                        if (kvpstr != null)
                        {
                            resourceControl1.AddResource(kvp.Key, kvpstr);
                        }
                    }
                }
            }
            catch { }
            var modelData = DB.GetDataDict("CharacterModel", "ConfCharacterModel", modelSn);
            if (modelData != null)
            {
                foreach (var kvp in modelData)
                {
                    if (kvp.Key == "sn") continue;
                    if (kvp.Value == null) continue;
                    string? kvpstr = kvp.Value as string;
                    if (kvpstr != null)
                    {
                        resourceControl1.AddResource(kvp.Key, kvpstr);
                    }
                }
            }
            var charRes = DB.GetDataDict("CharacterRes", "ConfCharacterRes", modelSn);
            if (charRes != null)
            {
                foreach (var kvp in charRes)
                {
                    if (kvp.Key == "sn") continue;
                    if (kvp.Value == null) continue;
                    string? kvpstr = kvp.Value as string;
                    if (kvpstr != null)
                    {
                        resourceControl1.AddResource(kvp.Key, kvpstr);
                    }
                }
            }
            resourceControl1.ResumeResources();
            initResources = true;
        }
        private List<FashionPartModel> GetDefaultPartModels(string modelSn)
        {
            List<FashionPartModel> partModels = [];
            var fashionDefault = DB.GetDataDict("Fashion", "ConfFashionDefault", modelSn);
            if (fashionDefault != null)
            {
                foreach (var kv in fashionDefault)
                {
                    if (kv.Key != "sn" && kv.Key != "dialogHead2DSn")
                    {
                        if (kv.Value is long partSn)
                        {
                            var partModel = GetPartModels(partSn);
                            if (partModel != null)
                            {
                                partModels.AddRange(partModel);
                            }
                        }
                    }
                }
            }
            return partModels;
        }
        private List<FashionPartModel>? GetPartModels(long partSn)
        {
            var fashion = DB.GetDataDict("Fashion", "ConfFashion", partSn);
            if (fashion != null)
            {
                string fashionMdlStr = (fashion["partModel"] as string)!;
                if (GameMgr.IsCbt)
                {
                    byte[] bytes = Convert.FromBase64String(fashionMdlStr);
                    fashionMdlStr = Encoding.UTF8.GetString(bytes);
                }
                //fashionMdlId = Convert.ToInt64(fashionMdlStr);
                //return DB.GetData<FashionPartModel>(fashionMdlId);
                //return fashionMdlStr.Split(',').Select(s => DB.GetData<FashionPartModel>(Convert.ToInt64(s))).ToList();
                List<FashionPartModel> mdls = [];
                foreach (var str in fashionMdlStr.Split(','))
                {
                    var fashionMdl = DB.GetData<FashionPartModel>(Convert.ToInt64(str));
                    if (fashionMdl != null) mdls.Add(fashionMdl);
                }
                return mdls;
            }
            return null;
        }
        //private void addPart(int partSn)
        //{
        //    if (partSn == 0)
        //    {
        //        return;
        //    }
        //    Console.WriteLine("adding part " + partSn);
        //    var fashionModel = GetPartModel(partSn);
        //    if (fashionModel != null)
        //    {
        //        prefabCheckList.Items.Add(I18NAssetMap.GetAssetName(fashionModel.Prefab), true);
        //        var fashionPrefab = fashionModel.Prefab;
        //        Console.WriteLine(fashionModel.Prefab);
        //        var prefabBundle = AssetBundleUtils.GetBundleByAsset(fashionPrefab);
        //        // heuristic to maybe find some meshes
        //        int pos = fashionPrefab.IndexOf("_wholeexport_rig");
        //        if (pos != -1)
        //        {
        //            var asset = $"{fashionPrefab.AsSpan(0, fashionPrefab.Length - 7)}_{fashionPrefab.AsSpan(0, pos)}.asset";
        //            var bundleName = AssetBundleUtils.GetBundleByAsset(asset);
        //            if (bundleName != null && prefabBundle != bundleName)
        //            {
        //                prefabCheckList.Items.Add(asset, true);
        //            }
        //        }
        //    }
        //}
        //private void addPartToPreview(int partSn)
        //{
        //    if (partSn == 0)
        //    {
        //        return;
        //    }
        //    var fashionModel = GetPartModel(partSn);
        //    if (fashionModel != null)
        //    {
        //        preview.AddFashionPart(fashionModel.Prefab, fashionModel.HangPoint);
        //    }
        //}

        private void saveBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveFileDialog = new();
            var result = saveFileDialog.ShowDialog(this);
            if (result != DialogResult.OK) return;
            var filePath = saveFileDialog.SelectedPath;
            HashSet<string> bundleList = [];
            foreach (var item in prefabCheckList.CheckedItems)
            {
                string prefabName = item as string ?? "";
                string? bundleName = AssetBundleUtils.GetBundleByAsset(prefabName);
                if (bundleName != null)
                {
                    bundleList.Add(bundleName);
                    AssetBundleUtils.GetAllDependencies(bundleName, bundleList);
                }
            }
            Utils.SaveBundles(filePath, bundleList);
        }
        private void OnTabChange()
        {
            if (tabControl1.SelectedIndex == 0)
            {
                InitModelsList();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                InitPreview();
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                InitResources();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            preferredTab = tabControl1.SelectedIndex;
            OnTabChange();
        }
    }
}
