using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityAssetLib;
using P5XTool;

namespace P5XToolGUI
{
    public partial class SpriteBrowser : Form
    {
        const int imageW = 256;
        const int imageH = 256;
        const int imageLargeW = 128;
        const int imageLargeH = 128;
        const int imageSmallW = 64;
        const int imageSmallH = 64;
        ImageList imgList = new()
        {
            ImageSize = new(imageLargeW, imageLargeH),
        };
        List<string> imageList = [];
        List<int> filteredIndices = [];
        HashSet<string> imageQueue = [];
        List<string> workQueue = [];
        Bitmap defaultImage;
        private int processedImages = 0;
        public SpriteBrowser()
        {
            InitializeComponent();
            // Enable double buffering on the ListView to reduce flicker
            try
            {
                var prop = typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                prop?.SetValue(listView1, true);
            }
            catch { }
            defaultImage = new(imageW, imageH);
            //imageIndexCache.ValueRemovedCallback = Uncache;
            var textBytes = VFileSystem.ReadAllBytes(AssetBundleUtils.GetRelativeBundlePath("ZeusCustomAssetDep.json"));
            var text = Encoding.UTF8.GetString(textBytes);
            var obj = JsonSerializer.Deserialize<CustomAssetDep>(text)!;
            foreach (var dep in obj.depList)
            {
                if (dep.assetPath.EndsWith(".png"))
                {
                    imageList.Add(dep.assetPath);
                }
            }
            listView1.LargeImageList = imgList;
            UpdateFilter();
            timer1.Start();
        }

        private void UpdateFilter()
        {
            filteredIndices.Clear();
            int idx = 0;
            foreach (var dep in imageList)
            {
                if (dep.Contains(textBox1.Text, StringComparison.OrdinalIgnoreCase)) filteredIndices.Add(idx);
                idx++;
            }
            listView1.VirtualListSize = filteredIndices.Count;
            backgroundWorker1.CancelAsync();
        }

        Dictionary<string, ListViewItem> itemCache = [];
        Dictionary<string, int> imageIndices = [];
        HashSet<string> loadedImages = [];
        private int GetImageIndex(string assetName)
        {
            if (imageIndices.TryGetValue(assetName, out var index)) return index;
            index = imgList.Images.Count;
            imgList.Images.Add(defaultImage);
            imageIndices.Add(assetName, index);
            return index;
        }
        private int CheckImage(string assetName)
        {
            var idx = GetImageIndex(assetName);
            var img = imgList.Images[idx];
            if (!loadedImages.Contains(assetName))
            {
                imageQueue.Add(assetName);
            }
            return idx;
        }
        //private int RegisterImg(Bitmap img)
        //{
        //    int idx = imgList.Images.Count;
        //    var largeBmp = Utils.ResizeImage(img, imageLargeW, imageLargeH);
        //    imgList.Images.Add(largeBmp);
        //    return idx;
        //}

        //DictionaryLRU<string, int> imageIndexCache = new(50);

        //private void Uncache(KeyValuePair<string, int> kvp)
        //{
        //    imgList.Images.RemoveByKey(kvp.Key);
        //}
        //private int GetImageIndex(string assetName)
        //{
        //    if (imageIndexCache.TryGetValue(assetName, out int idx)) return idx;
        //    idx = -1;
        //    var bmp = TexUtils.GetBitmapForImage(assetName);
        //    if (bmp != null)
        //    {
        //        var resizedBmp = Utils.GetPreviewImg(bmp, imageW, imageH, System.Drawing.Color.DarkGray);
        //        idx = RegisterImg(resizedBmp);
        //    }
        //    imageIndexCache[assetName] = idx;
        //    return idx;
        //}

        private ListViewItem GetItem(string assetName)
        {
            if (!itemCache.TryGetValue(assetName, out var resultItem))
            {
                resultItem = new ListViewItem(assetName)
                {
                    Tag = assetName,
                };
                // Cache the created item so subsequent requests reuse it
                itemCache[assetName] = resultItem;
                resultItem.ImageIndex = CheckImage(assetName);
            }
            return resultItem;
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var imageName = imageList[filteredIndices[e.ItemIndex]];
            ListViewItem item = GetItem(imageName);
            e.Item = item;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        private void SpriteBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            backgroundWorker1.CancelAsync();
            listView1.VirtualListSize = 0;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateFilter();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var idx = listView1.SelectedIndices[0];
            var imageName = imageList[filteredIndices[idx]];
            var bmp = TexUtils.GetBitmapForImage(imageName);
            if (bmp != null) Utils.CopyPngToClipboard(bmp, imageName);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var queue = e.Argument as List<string>;
            if (queue == null) return;
            //var results = new List<(string assetName, Bitmap bmp)>();
            var idx = 0;
            foreach (var assetName in queue)
            {
                var bmp = TexUtils.GetBitmapForImage(assetName);
                if (bmp != null)
                {
                    var resizedBmp = GUtils.GetPreviewImg(bmp, imageLargeW, imageLargeH, System.Drawing.Color.DarkGray);
                    loadedImages.Add(assetName);
                    var imgIdx = GetImageIndex(assetName);
                    imgList.Images[imgIdx] = resizedBmp;
                    backgroundWorker1.ReportProgress(idx++);
                }
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
            //e.Result = results;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy) return;
            if (imageQueue.Count > 0)
            {
                processedImages = 0;
                workQueue = imageQueue.ToList();
                imageQueue.Clear();
                backgroundWorker1.RunWorkerAsync(workQueue);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Cancelled) return;
            //if (e.Error != null) return;
            //if (e.Result is not List<(string assetName, Bitmap bmp)> results) return;
            //listView1.BeginUpdate();
            //try
            //{
            //    foreach (var (assetName, bmp) in results)
            //    {
            //    }
            //}
            //catch { }
            //finally
            //{
            //    listView1.EndUpdate();
            //}
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() != DialogResult.OK) return;
            string dir = folderBrowser.SelectedPath;
            foreach (int itemIdx in listView1.SelectedIndices)
            {
                var imageName = imageList[filteredIndices[itemIdx]];
                var bmp = TexUtils.GetBitmapForImage(imageName);
                if (bmp != null) bmp.Save(Path.Join(dir, imageName));
            }
        }

        private int GetIndexOfAsset(string assetName)
        {
            return filteredIndices.FirstOrDefault(i => imageList[i] == assetName, -1);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progress = e.ProgressPercentage;
            for (int i = processedImages; i <= progress; i++)
            {
                string assetName = workQueue[i];
                int index = GetIndexOfAsset(assetName);
                if (index != -1 && index < listView1.VirtualListSize)
                {
                    Console.WriteLine($"Redraw {index}");
                    listView1.RedrawItems(index, index, false);
                }
            }
            processedImages = progress + 1;
        }
    }

    internal class Dependency
    {
        public string assetPath { get; set; } = "";
        public List<string> depBundles { get; set; } = [];
        public List<string> unityExtraDepBundles { get; set; } = [];
    }
    internal class CustomAssetDep
    {
        public List<Dependency> depList { get; set; } = [];
        public string extraInfo { get; set; } = "";
    }

    //internal class DictionaryLRU<TKey, TValue> where TKey : notnull
    //{
    //    private readonly Dictionary<TKey, TValue> dict = [];
    //    private readonly List<TKey> list = [];
    //    public int _maxElements;
    //    public int MaxElements {
    //        get => _maxElements;
    //        set {
    //            _maxElements = value;
    //            Clean();
    //        }
    //    }
    //    public Action<KeyValuePair<TKey, TValue>>? ValueRemovedCallback;
    //    public DictionaryLRU(int maxElements)
    //    {
    //        _maxElements = maxElements;
    //    }
    //    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    //    {
    //        if (dict.TryGetValue(key, out value))
    //        {
    //            PushToTop(key);
    //            return true;
    //        }
    //        value = default;
    //        return false;
    //    }
    //    private void PushToTop(TKey key)
    //    {
    //        int idx = list.IndexOf(key);
    //        if (idx != list.Count - 1)
    //        {
    //            list.RemoveAt(idx);
    //            list.Add(key);
    //        }
    //    }
    //    public void Add(TKey key, TValue value)
    //    {
    //        if (dict.ContainsKey(key))
    //        {
    //            throw new Exception($"Key ({key}) already in dictionary!");
    //        }
    //        dict.Add(key, value);
    //        list.Add(key);
    //        Clean();
    //    }
    //    private void Clean()
    //    {
    //        if (list.Count > _maxElements)
    //        {
    //            int excess = list.Count - _maxElements;
    //            for (int i = 0; i < excess; i++)
    //            {
    //                var key1 = list[i];
    //                var value1 = dict[key1];
    //                ValueRemovedCallback?.Invoke(new(key1, value1));
    //                dict.Remove(key1);
    //            }
    //            list.RemoveRange(0, excess);
    //        }
    //    }
    //    public TValue this[TKey key]
    //    {
    //        get
    //        {
    //            if (TryGetValue(key, out var value)) { return value; }
    //            throw new InvalidOperationException();
    //        }
    //        set
    //        {
    //            if (dict.ContainsKey(key))
    //            {
    //                PushToTop(key);
    //                dict[key] = value;
    //            }
    //            else
    //            {
    //                Add(key, value);
    //            }
    //        }
    //    }
    //}
}
