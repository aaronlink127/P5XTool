using OpenTK;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using P5XLib;
using P5XTool;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace P5XToolGUI
{
    public partial class MainForm : Form
    {
        const string toolTitle = "P5X Tool";
        readonly string windowTitle = $"{toolTitle} {GetVersionString()}";
        string? gamePath;
        UserControl? rightPanel;
        public static Icon MainIcon = null!;
        public MainForm()
        {
            InitializeComponent();
            MainIcon = Icon!;
            Text = windowTitle;
            InitRecents();
            SetLoadEnable(true);
            var autoPath = Config.GetAutoLoadPath();
            if (autoPath != null) InitGame(autoPath);
        }

        private void SetLoadEnable(bool enable)
        {
            pickGameFolderToolStripMenuItem.Enabled = enable;
            recentFoldersToolStripMenuItem.Enabled = recentFoldersToolStripMenuItem.DropDownItems.Count != 0 && enable;
            closeGameToolStripMenuItem.Enabled = !enable;
            toolsToolStripMenuItem.Enabled = !enable;
        }
        private void InitRecents()
        {
            recentFoldersToolStripMenuItem.DropDownItems.Clear();
            foreach (string path in Config.GetRecents())
            {
                ToolStripMenuItem item = new(path);
                item.Click += (sender, e) =>
                {
                    InitGame(path);
                };
                recentFoldersToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        // inaccurate currently
        // private DateTime getBuildDate()
        // {
        //     var resDate = DB.GetData<Param>("CLINET_RES_VERSION")?.Value; // YYYYMMDDHH
        //     if (resDate == null) return DateTime.MinValue;
        //     if (resDate.Length == 8) resDate = "20" + resDate;
        //     int year = int.Parse(resDate[0..4]);
        //     int month = int.Parse(resDate[4..6]);
        //     int day = int.Parse(resDate[6..8]);
        //     int hour = int.Parse(resDate[8..10]);
        //     return new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Utc);
        // }

        class ItemTag
        {
            public ItemTag(string type, object sn)
            {
                this.type = type;
                this.sn = sn;
            }
            public string type;
            public object sn;
        }
        private void InitGame(string path)
        {
            gamePath = path;

            GUtils.LongTask(this, "Loading Game", (report) =>
            {
                int progInt = 0;
                void onProg(string prog)
                {
                    report(progInt++ * 100 / 8, prog);
                }
                return GameMgr.TryInit(gamePath, onProg);
            }, (r) =>
            {
                if ((bool)r!)
                {
                    OnGameInit();
                }
                else
                {
                    MessageBox.Show("Failed to initialize game folder. Please make sure the path is correct.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
        private static string ThiefToString(Dictionary<string, object?> thief)
        {
            string model = (string)thief["model"]!;
            string modelUI = (string)thief["modelUI"]!;
            string charaName = modelUI.Length < model.Length ? modelUI : model;
            return $"{thief["sn"]}: {charaName}";
        }
        private static string GetVersionString()
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Environment.ProcessPath!);
            string? prodVer = fileVersionInfo.ProductVersion;
            if (string.IsNullOrEmpty(prodVer)) return "Unknown";
            int plusIndex = prodVer.IndexOf('+');
            if (plusIndex >= 0)
            {
                string commitHash = prodVer.Substring(plusIndex + 1, 7);
                prodVer = prodVer[..plusIndex];
                prodVer = $"{prodVer} (commit " + commitHash + ")";
            }
            return "v" + prodVer;
        }
        private bool OnGameInit()
        {
            Config.AddRecentPath(gamePath!);
            InitRecents();
            SetLoadEnable(false);
            if (DB.IsInit)
            {
                string? serverName = null;
                if (GameMgr.IsSea) serverName = "SEA";
                else if (GameMgr.IsGlobal) serverName = "Global";
                else if (GameMgr.IsCbt) serverName = "Closed Beta Test";
                Text = $"{windowTitle} - Game Ver. {DB.GetData<Param>("CLINET_GAME_VERSION")?.Value ?? "Unknown"}{(serverName != null ? $" ({serverName})" : "")}";
                var thieves = DB.GetTableDict("Thief", "ConfThief");
                var protag = DB.GetDataDict("ZhuJue", "ConfZhuJue", 101)!;
                protag["model"] = GUtils.FixBetaString((protag["model"] as string)!);
                thieves.Insert(0, protag);
                TreeNode thievesNode = new("Thieves");
                treeView1.Nodes.Add(thievesNode);
                foreach (var thief in thieves)
                {
                    TreeNode childNode1 = new(ThiefToString(thief)) { Tag = new ItemTag("Thief", (long)thief["sn"]!) };
                    thievesNode.Nodes.Add(childNode1);
                }
                var personas = DB.GetTableDict("Persona", "ConfPersona");
                TreeNode personaNode = new("Personas");
                treeView1.Nodes.Add(personaNode);
                foreach (var persona in personas)
                {
                    TreeNode childNode1 = new($"{persona["sn"]}: {persona["model"]}") { Tag = new ItemTag("Persona", (long)persona["sn"]!) };
                    personaNode.Nodes.Add(childNode1);
                }
                var characterModels = DB.GetTableDict("CharacterModel", "ConfCharacterModel");
                TreeNode modelNode = new("Models");
                treeView1.Nodes.Add(modelNode);
                foreach (var characterModel in characterModels)
                {
                    TreeNode childNode1 = new((string)characterModel["sn"]!) { Tag = new ItemTag("CharacterModel", (string)characterModel["sn"]!) };
                    modelNode.Nodes.Add(childNode1);
                }

                var scenes = DB.GetTableDict("Scene", "ConfScene");
                TreeNode sceneNode = new("Scenes");
                treeView1.Nodes.Add(sceneNode);
                foreach (var scene in scenes)
                {
                    TreeNode childNode1 = new($"{scene["sn"]}: {scene["asset"]}") { Tag = new ItemTag("Scene", (long)scene["sn"]!) };
                    sceneNode.Nodes.Add(childNode1);
                }
                //var scenes = DB.GetTable<ConfScene>();
                //TreeNode sceneNode = new("Scenes");
                //treeView1.Nodes.Add(sceneNode);
                //foreach (var scene in scenes)
                //{
                //    TreeNode childNode1 = new(scene.Asset) { Tag = scene };
                //    sceneNode.Nodes.Add(childNode1);
                //}
            }
            else
            {
                Text = $"{windowTitle} - Unknown Game Ver.";
                MessageBox.Show("Failed to initialize database. Some features may be broken or unavailable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            setGameLanguageToolStripMenuItem.DropDownItems.Clear();
            if (I18NAssetMap.MainLanguage != null)
            {
                ToolStripMenuItem offItem = new("Off");
                offItem.ToolTipText = "Disable localization and use original assets.";
                offItem.Click += (sender, e) =>
                {
                    I18NAssetMap.SetCurrentLocale(null);
                    I18NAssetMap.Enabled = false;
                    foreach (ToolStripMenuItem subItem in setGameLanguageToolStripMenuItem.DropDownItems)
                    {
                        subItem.Checked = subItem == offItem;
                    }
                    UpdatePanel();
                };
                setGameLanguageToolStripMenuItem.DropDownItems.Add(offItem);
                ToolStripMenuItem noneItem = new("Default");
                noneItem.ToolTipText = "Use the game's default language localization.";
                noneItem.Click += (sender, e) =>
                {
                    I18NAssetMap.SetCurrentLocale(null);
                    I18NAssetMap.Enabled = true;
                    foreach (ToolStripMenuItem subItem in setGameLanguageToolStripMenuItem.DropDownItems)
                    {
                        subItem.Checked = subItem == noneItem;
                    }
                    UpdatePanel();
                };
                setGameLanguageToolStripMenuItem.DropDownItems.Add(noneItem);
                foreach (var lang in I18NAssetMap.LangList)
                {
                    ToolStripMenuItem item = new(lang);
                    item.Checked = lang == I18NAssetMap.CurrentLocale;
                    item.ToolTipText = $"Set game asset localization to {lang}.";
                    item.Click += (sender, e) =>
                    {
                        I18NAssetMap.SetCurrentLocale(lang);
                        I18NAssetMap.Enabled = true;
                        foreach (ToolStripMenuItem subItem in setGameLanguageToolStripMenuItem.DropDownItems)
                        {
                            subItem.Checked = subItem == item;
                        }
                        UpdatePanel();
                    };
                    setGameLanguageToolStripMenuItem.DropDownItems.Add(item);
                }
                setGameLanguageToolStripMenuItem.Enabled = true;
            }
            else
            {
                setGameLanguageToolStripMenuItem.Enabled = false;
            }
            spriteBrowserToolStripMenuItem.Enabled = VFileSystem.Exists(AssetBundleUtils.GetRelativeBundlePath("ZeusCustomAssetDep.json"));

            return true;
        }

        private void pickGameFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameFolderBrowser.ShowDialog(this) != DialogResult.OK) return;
            InitGame(gameFolderBrowser.SelectedPath);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdatePanel();
        }
        private void ClearPanel()
        {
            if (rightPanel != null)
            {
                rightPanel.Dispose();
                rightPanel = null;
            }
        }
        private void UpdatePanel()
        {
            ClearPanel();
            var node = treeView1.SelectedNode;
            if (node != null && node.Tag != null)
            {
                if (node.Tag is ItemTag itemTag)
                {
                    switch (itemTag.type)
                    {
                        case "Thief":
                            {
                                rightPanel = new ThiefControl((long)itemTag.sn);
                                break;
                            }
                        case "CharacterModel":
                            {
                                rightPanel = new ModelControl((string)itemTag.sn);
                                break;
                            }
                        case "Persona":
                            {
                                rightPanel = new PersonaControl((long)itemTag.sn);
                                break;
                            }
                        case "Scene":
                            {
                                rightPanel = new SceneControl((long)itemTag.sn);
                                break;
                            }
                    }
                }
            }
            if (rightPanel != null)
            {
                rightPanel.Dock = DockStyle.Fill;
                splitContainer1.Panel2.Controls.Add(rightPanel);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
                contextMenuStrip1.Tag = e.Node;
                contextMenuStrip1.Show(treeView1, e.Location);
            }
        }

        private void CloseOtherWindows()
        {
            List<Form> formsToClose = [];
            foreach (Form form in Application.OpenForms)
            {
                if (form != this) formsToClose.Add(form);
            }
            foreach (Form form in formsToClose)
            {
                form.Close();
            }
        }

        private void ResetGame()
        {
            CloseOtherWindows();
            GameMgr.Reset();
            treeView1.Nodes.Clear();
            ClearPanel();
            setGameLanguageToolStripMenuItem.Enabled = false;
            SetLoadEnable(true);
            Text = windowTitle;
        }

        private void ShowSubWindow(Form form)
        {
            form.Icon = Icon;
            form.Show();
        }
        private void closeGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void configDataExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var exportForm = new ConfForm();
            ShowSubWindow(exportForm);
        }

        private void assetBundleExtractorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var exportForm = new BundleForm();
            ShowSubWindow(exportForm);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseOtherWindows();
        }


        private void genConfigCClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var genForm = new ConfGenForm();
            ShowSubWindow(genForm);
        }

        private void dialogSpritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialogForm = new DialogHeadForm();
            ShowSubWindow(dialogForm);
        }

        private void spriteExtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var spriteForm = new SpriteForm();
            ShowSubWindow(spriteForm);
        }

        private ulong readULEB128(BinaryReader reader)
        {
            ulong result = 0;
            int shift = 0;
            while (true)
            {
                byte b = reader.ReadByte();
                result |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0) break;
                shift += 7;
            }
            return result;
        }

        private Dictionary<long, string> getLuaHashMap()
        {
            Dictionary<long, string> dict = [];

            void TryInsertString(string str)
            {
                if (str.EndsWith("Mod"))
                {
                    TryInsertString(str + "Config");
                }
                if (str.EndsWith("Mgr"))
                {
                    TryInsertString(string.Concat(str.AsSpan(0, str.Length - 3), "Mod"));
                }
                if (LuaFileUtils.IsFileExist(str))
                {
                    var hash = LuaFileUtils.GetHashLong(str + LuaFileUtils.luafile_ext);
                    dict.TryAdd(hash, str);
                }
            }
            if (LuaFileUtils.combineFiles.Count == 0) return dict;
            foreach (var file in VFileSystem.GetDirectFilesInPath("Lua/Windows"))
            {
                var fileName = Path.GetFileName(file);
                if (fileName.Length == 21 && fileName.EndsWith(".bytes"))
                {
                    var f = VFileSystem.Open(file);
                    if (f == null) continue;
                    using MemoryStream memoryStream = new();
                    using BinaryReader reader = new(f);
                    int numFiles = reader.ReadInt32();
                    for (int i = 0; i < numFiles; i++)
                    {
                        int dataLen = reader.ReadInt32();
                        byte[] dataBytes = reader.ReadBytes(dataLen);
                        using MemoryStream dataStream = new(dataBytes);
                        //bool inString = false;
                        string extractedString = "";
                        while (true)
                        {
                            while (dataStream.Position < dataStream.Length)
                            {
                                var b = dataStream.ReadByte();
                                if (!(
                                    b == 95 ||
                                    (b >= 48 && b <= 57) ||
                                    (b >= 65 && b <= 90) ||
                                    (b >= 97 && b <= 122)
                                    )) break;
                                extractedString += (char)b;
                            }
                            if (extractedString.Length >= 3)
                            {
                                TryInsertString(extractedString);
                            }
                            extractedString = "";
                            if (dataStream.Position >= dataStream.Length) break;
                        }
                    }
                }
            }
            return dict;
        }

        private void extractLuaScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dirSelector = new FolderBrowserDialog();
            if (dirSelector.ShowDialog(this) != DialogResult.OK) return;
            GUtils.LongTask(this, "Extracting Scripts", (report) =>
            {
                report(0, "");
                int progInt = 0;
                var directFiles = VFileSystem.GetDirectFilesInPath("Lua/Windows");
                foreach (var file in directFiles)
                {
                    report(progInt * 100 / (directFiles.Count), $"Extracting {Path.GetFileName(file)} ({progInt} / {directFiles.Count})");
                    var fileName = Path.GetFileName(file);
                    File.WriteAllBytes(Path.Combine(dirSelector.SelectedPath, fileName), VFileSystem.ReadAllBytes(file));
                    progInt++;
                }
                progInt = 0;
                report(0, "Generating Hash Map...");
                var luaMap = getLuaHashMap();
                Directory.CreateDirectory(Path.Combine(dirSelector.SelectedPath, "combined"));
                foreach (var kv in LuaFileUtils.combineFiles)
                {
                    report(progInt * 100 / (LuaFileUtils.combineFiles.Count), $"Extracting {kv.Key} ({progInt} / {LuaFileUtils.combineFiles.Count})");
                    string? realName = luaMap.GetValueOrDefault(kv.Key);
                    if (realName == null)
                    {
                        realName = $"unknown_{kv.Key:X16}";
                    }
                    realName += LuaFileUtils.luafile_ext;
                    var bytes = LuaFileUtils.ReadLuaFromCombine(kv.Value);
                    File.WriteAllBytes(Path.Combine(dirSelector.SelectedPath, "combined", realName), bytes);
                    progInt++;
                }
                return null;
            });
        }

        private void BackgroundWorker_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowSubWindow(new OptionForm());
        }

        private void fSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSubWindow(new FSBrowser());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = $"Loaded AssetBundles: {UnityAssetLib.UnityAssetManager.LoadedFileCount}, Loaded Resources: {UnityAssetLib.UnityAssetManager.LoadedBinCount}";
        }

        private void spriteBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSubWindow(new SpriteBrowser());
        }
    };
}
