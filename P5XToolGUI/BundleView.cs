using P5XLib;
using P5XTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityAssetLib;

namespace P5XToolGUI
{
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
    using ObjectKey = (int fileId, long pathId);
    public partial class BundleView : UserControl
    {
        const int imageW = 256;
        const int imageH = 256;
        const int imageLargeW = 128;
        const int imageLargeH = 128;
        const int imageSmallW = 64;
        const int imageSmallH = 64;
        ObjectKey? selectedObject;
        List<BundleRef> bundles = [];
        public int SortColumn = 0;
        public SortOrder SortOrder = SortOrder.Ascending;
        private static int preferredView = 0;
        private static float preferredSplitterPos = -1.0f;
        private ImageList imgList = new()
        {
            ImageSize = new Size(imageLargeW, imageLargeH)
        };
        private ImageList imgListSmall = new()
        {
            ImageSize = new Size(imageSmallW, imageSmallH)
        };
        private ToolStripMenuItem allFilterMenuItem;
        private bool suppressFilterEvents = false;
        private List<ObjectKey> objects = [];
        private List<int> filteredIndices = [];
        private List<SerializedFile> serializedFiles = [];
        private Dictionary<ObjectKey, ListViewItem> itemCache = [];
        private List<ToolStripMenuItem> sortItems = [];
        private ToolStripMenuItem ascendingItem;
        private ToolStripMenuItem descendingItem;
        private HashSet<ObjectKey> imageQueue = [];
        Bitmap defaultImage;
        Dictionary<string, ToolStripMenuItem> typeFilterItems = [];
        public BundleView(string bundleName) : this([bundleName]) { }
        public BundleView(List<string> bundleNames)
        {
            InitializeComponent();
            defaultImage = new(imageW, imageH);
            allFilterMenuItem = new ToolStripMenuItem("All") { Checked = true, CheckOnClick = true };
            allFilterMenuItem.Click += (s, e) =>
            {
                if (suppressFilterEvents) return;
                suppressFilterEvents = true;
                if (allFilterMenuItem.Checked)
                {
                    foreach (var kv in typeFilterItems)
                    {
                        kv.Value.Checked = true;
                    }
                }
                else
                {
                    foreach (var kv in typeFilterItems)
                    {
                        kv.Value.Checked = false;
                    }
                }
                suppressFilterEvents = false;
                ApplyFilterSort();
            };
            filterToolStripMenuItem.DropDownItems.Add(allFilterMenuItem);
            int fileIdx = 0;
            ToolStripMenuItem? monoSubMenu = null;
            foreach (var bundleName in bundleNames)
            {
                var bundle = AssetManager.LoadBundle(bundleName, false);
                bundles.Add(bundle);
                foreach (var file in bundle.Bundle.SerializedFiles)
                {
                    serializedFiles.Add(file);
                    foreach (var kv in file.GetObjectInfos())
                    {
                        var objKey = new ObjectKey(fileIdx, kv.Key);
                        objects.Add(objKey);
                        string? tname = file.GetObjectTypeName(kv.Key);
                        if (tname != null && !typeFilterItems.ContainsKey(tname))
                        {
                            if (tname == "MonoBehaviour")
                            {
                                var friendName = file.GetFriendlyObjectTypeName(kv.Key)!;
                                if (monoSubMenu == null)
                                {
                                    monoSubMenu = new ToolStripMenuItem("MonoBehaviour");
                                    monoSubMenu.Click += (s, e) =>
                                    {
                                        bool doCheck = monoSubMenu.CheckState == CheckState.Unchecked;
                                        foreach (var item in monoSubMenu.DropDownItems)
                                        {
                                            if (item is ToolStripMenuItem tsi)
                                            {
                                                tsi.Checked = doCheck;
                                            }
                                        }
                                        ApplyFilterSort();
                                    };
                                    filterToolStripMenuItem.DropDownItems.Add(monoSubMenu);
                                }
                                if (!typeFilterItems.ContainsKey(friendName))
                                {
                                    var item = AddFilterItem(monoSubMenu, friendName);
                                    monoSubMenu.DropDownItems.Add(item);
                                    typeFilterItems.Add(friendName, item);
                                }
                            }
                            else
                            {
                                typeFilterItems.Add(tname, AddFilterItem(filterToolStripMenuItem, tname));
                            }
                        }
                    }
                    fileIdx++;
                }
            }
            int colIdx1 = 0;
            foreach (ColumnHeader column in listView1.Columns)
            {
                int colIdx = colIdx1;
                var item = new ToolStripMenuItem();
                item.Text = column.Text;
                item.Click += (sender, e) =>
                {
                    if (!item.Checked)
                    {
                        SortColumn = colIdx;
                        UpdateSort();
                    }
                };
                sortItems.Add(item);
                sortByToolStripMenuItem.DropDownItems.Add(item);
                colIdx1++;
            }
            sortByToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            ascendingItem = new("Ascending");
            descendingItem = new("Descending");
            ascendingItem.Click += (sender, e) =>
            {
                if (!ascendingItem.Checked)
                {
                    SortOrder = SortOrder.Ascending;
                    UpdateSort();
                }
            };
            descendingItem.Click += (sender, e) =>
            {
                if (!descendingItem.Checked)
                {
                    SortOrder = SortOrder.Descending;
                    UpdateSort();
                }
            };
            sortByToolStripMenuItem.DropDownItems.Add(ascendingItem);
            sortByToolStripMenuItem.DropDownItems.Add(descendingItem);
            UpdateSort();
            viewToolStripMenuItem.DropDown.Closing += (s, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
            };
            filterToolStripMenuItem.DropDown.Closing += (s, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
            };
            sortByToolStripMenuItem.DropDown.Closing += (s, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
            };
            int viewIdx1 = 0;
            foreach (ToolStripMenuItem item in viewToolStripMenuItem.DropDownItems)
            {
                int viewIdx = viewIdx1;
                item.Checked = preferredView == viewIdx;
                item.Click += (sender, e) =>
                {
                    if (!item.Checked)
                    {
                        item.Checked = true;
                        foreach (ToolStripMenuItem item2 in viewToolStripMenuItem.DropDownItems)
                        {
                            if (item != item2)
                            {
                                item2.Checked = false;
                            }
                        }
                        SetView(viewIdx);
                    }
                };
                viewIdx1++;
            }
            SetView(preferredView);
            this.Load += (s, e) =>
            {
                if (preferredSplitterPos != -1.0f && splitContainer1.ClientSize.Width > 0)
                {
                    int target = (int)Math.Round(preferredSplitterPos * splitContainer1.ClientSize.Width);
                    int min = splitContainer1.Panel1MinSize;
                    int max = Math.Max(0, splitContainer1.ClientSize.Width - splitContainer1.Panel2MinSize);
                    target = Math.Max(min, Math.Min(max, target));
                    splitContainer1.SplitterDistance = target;
                }
            };
            timer1.Start();
        }

        private void SetView(int idx)
        {
            preferredView = idx;
            switch (idx)
            {
                case 0:
                    listView1.View = View.Details;
                    break;
                case 1:
                    listView1.LargeImageList = imgListSmall;
                    listView1.View = View.LargeIcon;
                    break;
                case 2:
                    listView1.LargeImageList = imgList;
                    listView1.View = View.LargeIcon;
                    break;
            }
            //listView1.Refresh();
        }
        private void InitIcons()
        {
            //if (isIconInit) return;
            //foreach (ListViewItem item in allItems)
            //{
            //    var objKey = (ObjectKey)item.Tag;
            //    int idx = GetImageIndex(objKey);
            //    if (idx != -1)
            //    {
            //        item.ImageIndex = idx;
            //    }
            //}
            //isIconInit = true;
        }


        Dictionary<ObjectKey, int> imageIndices = [];
        HashSet<ObjectKey> loadedImages = [];
        private int GetImageIndex(ObjectKey objKey)
        {
            if (imageIndices.TryGetValue(objKey, out var index)) return index;
            index = imgList.Images.Count;
            imgList.Images.Add(defaultImage);
            imgListSmall.Images.Add(defaultImage);
            imageIndices.Add(objKey, index);
            return index;
        }
        private int CheckImage(ObjectKey objKey)
        {
            var idx = GetImageIndex(objKey);
            var img = imgList.Images[idx];
            if (!loadedImages.Contains(objKey))
            {
                imageQueue.Add(objKey);
            }
            return idx;
        }
        //private i

        private void UpdateSort()
        {
            int colIdx = 0;
            foreach (ToolStripMenuItem item in sortItems)
            {
                item.Checked = colIdx == SortColumn;
                colIdx++;
            }
            ascendingItem.Checked = SortOrder == SortOrder.Ascending;
            descendingItem.Checked = SortOrder == SortOrder.Descending;
            ApplyFilterSort();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortOrder = SortColumn == e.Column && SortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            SortColumn = e.Column;
            UpdateSort();
        }

        private UnityAssetLib.Object GetObject(ObjectKey objId)
        {
            var (fileIdx, pathId) = objId;
            var sf = serializedFiles[fileIdx];
            var obj = sf.GetObject(pathId);
            return obj;
        }

        private string GetObjectName(ObjectKey objId)
        {
            var (fileIdx, pathId) = objId;
            var sf = serializedFiles[fileIdx];
            var objName = sf.GetObjectName(pathId);
            return objName ?? "Unknown";
        }

        private string GetObjectTypeName(ObjectKey objId)
        {
            var (fileIdx, pathId) = objId;
            var sf = serializedFiles[fileIdx];
            var objName = sf.GetObjectTypeName(pathId);
            return objName ?? "Unknown";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 1)
            {
                var curObj = GetObjectAtIndex(listView1.SelectedIndices[0]);
                if (selectedObject != curObj)
                {
                    selectedObject = curObj;
                    objectView1.SetObject(GetObject(curObj));
                }
            }
            else if (selectedObject != null)
            {
                objectView1.ClearObject();
                selectedObject = null;
            }
        }
        private static void FixCheckState(ToolStripMenuItem parent)
        {
            bool foundChecked = false;
            parent.Invalidate();
            foreach (var item in parent.DropDownItems)
            {
                if (item is ToolStripMenuItem tsi)
                {
                    if (tsi.DropDownItems.Count != 0)
                    {
                        FixCheckState(tsi);
                    }
                    switch (tsi.CheckState)
                    {
                        case CheckState.Checked:
                            foundChecked = true;
                            break;
                        case CheckState.Unchecked:
                            if (foundChecked)
                            {
                                parent.CheckState = CheckState.Indeterminate;
                                return;
                            }
                            break;
                        case CheckState.Indeterminate:
                            parent.CheckState = CheckState.Indeterminate;
                            return;
                    }
                }
            }
            parent.CheckState = foundChecked ? CheckState.Checked : CheckState.Unchecked;
        }
        private void FixTypeMenuChecks()
        {
            foreach (var item in filterToolStripMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem tsi && tsi.DropDownItems.Count != 0)
                {
                    FixCheckState(tsi);
                }
            }
        }
        private ToolStripMenuItem AddFilterItem(ToolStripMenuItem parent, string typeName)
        {
            var item = new ToolStripMenuItem(typeName)
            {
                Checked = true,
                CheckOnClick = true
            };
            item.Click += (s, e) =>
            {
                if (suppressFilterEvents)
                {
                    ApplyFilterSort();
                    return;
                }
                suppressFilterEvents = true;
                if (allFilterMenuItem.Checked && !item.Checked)
                {
                    allFilterMenuItem.Checked = false;
                }
                else
                {
                    bool every = true;
                    foreach (var kv in typeFilterItems)
                    {
                        if (!kv.Value.Checked)
                        {
                            every = false;
                            break;
                        }
                    }
                    allFilterMenuItem.Checked = every;
                }
                suppressFilterEvents = false;
                ApplyFilterSort();
            };
            parent.DropDownItems.Add(item);
            return item;
        }

        private void ApplyFilterSort()
        {
            FixTypeMenuChecks();
            listView1.BeginUpdate();
            HashSet<ObjectKey> selectedObjects = [];
            foreach (int itemIdx in listView1.SelectedIndices)
            {
                selectedObjects.Add(GetObjectAtIndex(itemIdx));
            }
            filteredIndices.Clear();
            listView1.VirtualListSize = 0;
            listView1.SelectedIndices.Clear();
            var allowed = new HashSet<string>();
            foreach (ToolStripItem tsi in filterToolStripMenuItem.DropDownItems)
            {
                if (tsi is ToolStripMenuItem tsm && tsm.Checked)
                {
                    allowed.Add(tsm.Text!);
                }
            }
            var idx = 0;
            foreach (var objKey in objects)
            {
                var objTypeName = GetObjectTypeName(objKey);
                var objName = GetObjectName(objKey);
                if (allowed.Contains(objTypeName) && (textBox1.Text == "" || objName.Contains(textBox1.Text, StringComparison.OrdinalIgnoreCase)))
                {
                    filteredIndices.Add(idx);
                }
                idx++;
            }
            filteredIndices.Sort((a, b) =>
            {
                var obj1 = objects[a]; var obj2 = objects[b];
                if (SortOrder == SortOrder.Descending)
                {
                    (obj1, obj2) = (obj2, obj1);
                }
                IComparable[] obj1Values = [GetObjectName(obj1), GetObjectTypeName(obj1), obj1];
                IComparable[] obj2Values = [GetObjectName(obj2), GetObjectTypeName(obj2), obj2];

                int cmpBase = obj1Values[SortColumn].CompareTo(obj2Values[SortColumn]);
                if (cmpBase != 0) return cmpBase;
                for (int i = 0; i < obj1Values.Length; i++)
                {
                    int cmp = obj1Values[i].CompareTo(obj2Values[i]);
                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }
                return 0;
            });
            listView1.VirtualListSize = filteredIndices.Count;
            foreach (var filteredIdx in filteredIndices)
            {
                if (selectedObjects.Contains(objects[filteredIdx]))
                {
                    listView1.SelectedIndices.Add(filteredIdx);
                }
            }
            listView1.EndUpdate();
        }

        private void SaveObject(string dir, UnityAssetLib.Object obj)
        {
            if (obj is Texture2D or Sprite)
            {
                var bmp = TexUtils.GetBitmapForImage(obj);
                bmp?.Save(Path.Join(dir, obj.GetName() + ".png"));
            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() != DialogResult.OK) return;
            string dir = folderBrowser.SelectedPath;
            foreach (int itemIdx in listView1.SelectedIndices)
            {
                var objKey = GetObjectAtIndex(itemIdx);
                var obj = GetObject(objKey);
                SaveObject(dir, obj);
            }
        }

        private void assetListContext_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (splitContainer1.ClientSize.Width > 0)
            {
                preferredSplitterPos = (float)splitContainer1.SplitterDistance / splitContainer1.ClientSize.Width;
            }
        }

        private void assetListContext_Opening(object sender, CancelEventArgs e)
        {
            bool hasSelect = listView1.SelectedIndices.Count > 0;
            extractToolStripMenuItem.Enabled = hasSelect;
            copyNameToolStripMenuItem.Enabled = hasSelect;
            copyToolStripMenuItem.Enabled = hasSelect;
        }

        private void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 1)
            {
                var objKey = GetObjectAtIndex(listView1.SelectedIndices[0]);
                Clipboard.SetText(GetObjectName(objKey));
            }
        }

        private ObjectKey GetObjectAtIndex(int index)
        {
            return objects[filteredIndices[index]];
        }

        private ListViewItem GetItem(ObjectKey objKey)
        {
            if (!itemCache.TryGetValue(objKey, out var resultItem))
            {
                resultItem = new(GetObjectName(objKey))
                {
                    Tag = objKey,
                };
                resultItem.SubItems.Add(GetObjectTypeName(objKey));
                resultItem.SubItems.Add($"{objKey.fileId} {objKey.pathId}");
                resultItem.ImageIndex = CheckImage(objKey);
                itemCache[objKey] = resultItem;
            }
            return resultItem;
        }
        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var objKey = GetObjectAtIndex(e.ItemIndex);
            var resultItem = GetItem(objKey);
            e.Item = resultItem;
        }

        private void listView1_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            //for (int i = e.StartIndex; i < e.EndIndex; i++)
            //{
            //    var objKey = GetObjectAtIndex(i);
            //    var resultItem = GetItem(objKey);
            //}
        }

        private void listView1_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {
            for (int i = e.StartIndex; i < filteredIndices.Count; i++)
            {
                var objectKey = objects[filteredIndices[i]];
                var objectName = GetObjectName(objectKey);
                if (objectName.StartsWith(e.Text!, StringComparison.OrdinalIgnoreCase))
                {
                    e.Index = i;
                    break;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy) return;
            if (preferredView == 0) return;
            if (imageQueue.Count != 0)
            {
                backgroundWorker1.RunWorkerAsync(imageQueue);
                imageQueue = [];
            }
        }

        protected override void Dispose(bool disposing)
        {
            listView1.VirtualListSize = 0;
            backgroundWorker1.CancelAsync();
            imgList.Dispose();
            imgListSmall.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private Dictionary<string, Bitmap> typeIconMap = [];
        private Bitmap GetTextImage(string text)
        {
            if (typeIconMap.TryGetValue(text, out var bmp)) return bmp;
            bmp = new(imageW, imageH);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Font drawFont = new("Arial", 16);
                var size = g.MeasureString(text, drawFont);
                drawFont.Dispose();
                drawFont = new("Arial", 16.0f * (imageW / size.Width));
                size = g.MeasureString(text, drawFont);
                SolidBrush drawBrush = new(System.Drawing.Color.Black);
                g.DrawString(text, drawFont, drawBrush, new Point(0, (int)(imageH / 2.0f - size.Height / 2.0f)));
            }
            typeIconMap[text] = bmp;
            return bmp;
        }
        private Bitmap GetImage(ObjectKey objectKey)
        {
            var obj = GetObject(objectKey);
            if (obj is Texture2D or Sprite)
            {
                var bmp = TexUtils.GetBitmapForImage(obj);
                if (bmp != null)
                {
                    var resizedBmp = GUtils.GetPreviewImg(bmp, imageW, imageH, System.Drawing.Color.DarkGray);
                    return resizedBmp;
                }
            }
            return GetTextImage(GetObjectTypeName(objectKey));
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var queue = (e.Argument as HashSet<ObjectKey>)!;
            //var results = new List<(ObjectKey objKey, Bitmap bmp, Bitmap bmp2)>();
            foreach (var objKey in queue)
            {
                if (!loadedImages.Contains(objKey))
                {
                    var bmp = GetImage(objKey);
                    var largeBmp = GUtils.ResizeImage(bmp, imageLargeW, imageLargeH);
                    var smallBmp = GUtils.ResizeImage(bmp, imageSmallW, imageSmallH);
                    loadedImages.Add(objKey);
                    var idx = GetImageIndex(objKey);
                    listView1.BeginInvoke(new Action(() =>
                    {
                        imgList.Images[idx] = largeBmp;
                        imgListSmall.Images[idx] = smallBmp;
                        for (int vi = 0; vi < filteredIndices.Count; vi++)
                        {
                            var originalIdx = filteredIndices[vi];
                            if (objects[originalIdx] == objKey)
                            {
                                listView1.RedrawItems(vi, vi, false);
                                break;
                            }
                        }
                    }));
                    //results.Add((objKey, largeBmp, smallBmp));
                }
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
            //e.Result = results;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Cancelled) return;
            //if (e.Error != null) return;
            //if (e.Result is not List<(ObjectKey objKey, Bitmap bmp, Bitmap bmp2)> results) return;
            //listView1.BeginUpdate();
            //try
            //{
            //    List<int> redrawIndices = [];
            //    foreach (var (objKey, bmp, bmp2) in results)
            //    {
            //        loadedImages.Add(objKey);
            //        var idx = GetImageIndex(objKey);
            //        imgList.Images[idx] = bmp;
            //        imgListSmall.Images[idx] = bmp2;

            //        for (int vi = 0; vi < filteredIndices.Count; vi++)
            //        {
            //            var originalIdx = filteredIndices[vi];
            //            if (objects[originalIdx] == objKey)
            //            {
            //                redrawIndices.Add(vi);
            //                break;
            //            }
            //        }
            //    }
            //}
            //catch { }
            //finally
            //{
            //    listView1.EndUpdate();
            //}
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var objKey = GetObjectAtIndex(listView1.SelectedIndices[0]);
            var obj = GetObject(objKey);
            if (obj is Texture2D or Sprite)
            {
                var bmp = TexUtils.GetBitmapForImage(obj);
                if (bmp != null)
                {
                    Utils.CopyPngToClipboard(bmp, obj.GetName() + ".png");
                }
            }
        }
        public static Form CreateWindow(string bundleName)
        {
            var view = new BundleView(bundleName)
            {
                Dock = DockStyle.Fill
            };
            return GUtils.NewFormForControl(view, $"{bundleName} - Bundle View");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilterSort();
        }

        private void objectView1_Load(object sender, EventArgs e)
        {

        }
    }
}
