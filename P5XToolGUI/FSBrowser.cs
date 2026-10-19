using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using P5XTool;

namespace P5XToolGUI
{
    public partial class FSBrowser : Form
    {
        BundleView? bundleView;
        string? currentBundle;
        readonly List<string> bundleList;
        readonly List<int> bundleIndices = [];
        public int SortColumn = 0;
        public SortOrder SortOrder = SortOrder.Ascending;
        public FSBrowser()
        {
            InitializeComponent();
            bundleList = AssetBundleUtils.GetBundles();
            bundleList.Add("Windows");
            bundleList.Sort();
            UpdateTree();
        }
        private void UpdateTree()
        {
            string filterText = filterTextBox.Text?.Trim() ?? string.Empty;
            string? curBundle = listView1.SelectedIndices.Count == 1 ? bundleList[bundleIndices[listView1.SelectedIndices[0]]] : null;
            int newIdx = -1;
            listView1.BeginUpdate();
            listView1.SelectedIndices.Clear();
            bundleIndices.Clear();
            var idx = 0;
            var filters = FilterParser.Parse(filterText);
            HashSet<string> filteredBundles = [..bundleList];
            HashSet<string> GetPassed(HashSet<string> objects, string? key, object? value)
            {
                if (key == null)
                {
                    if (value is (string innerKey, object innerValue))
                    {
                        return GetPassed(objects, innerKey, innerValue);
                    }
                    else if (value is string str)
                    {
                        return [.. objects.Where(e => e.Contains(str, StringComparison.OrdinalIgnoreCase))];
                    }
                }
                else
                {
                    switch (key)
                    {
                        case "rdep":
                            {
                                var o = GetPassed(objects, null, value);
                                if (o.Count == 1)
                                {
                                    var bundle2 = o.FirstOrDefault();
                                    if (bundle2 != null)
                                    {
                                        var deps = AssetBundleUtils.GetDependants(bundle2);
                                        return [.. objects.Where(o => o == bundle2 || deps.Contains(o))];
                                    }
                                }
                                return [];
                            }
                        case "dep":
                            {
                                var o = GetPassed(objects, null, value);
                                if (o.Count == 1)
                                {
                                    var bundle2 = o.FirstOrDefault();
                                    if (bundle2 != null)
                                    {
                                        var deps = AssetBundleUtils.GetAllDependencies(bundle2);
                                        return [.. objects.Where(o => o == bundle2 || deps.Contains(o))];
                                    }
                                }
                                return [];
                            }
                        case "ddep":
                            {
                                var o = GetPassed(objects, null, value);
                                if (o.Count == 1)
                                {
                                    var bundle2 = o.FirstOrDefault();
                                    if (bundle2 != null)
                                    {
                                        var deps = AssetBundleUtils.GetDirectDependencies(bundle2);
                                        return [.. objects.Where(o => o == bundle2 || deps.Contains(o))];
                                    }
                                }
                                return [];
                            }
                        case "asset":
                            var bundle = AssetBundleUtils.GetBundleByAsset(value?.ToString() ?? "");
                            if (bundle != null && objects.Contains(bundle)) return [ bundle ];
                            return [];
                    }
                }
                return [];
            }
            {
                foreach (var filter in filters)
                {
                    filteredBundles = GetPassed(filteredBundles, filter.Key, filter.Value);
                }
            }
            foreach (var bundle in bundleList)
            {
                //if (filters.All(f =>
                //{
                //    if (f.Key == null)
                //    {
                //        if (bundle.IndexOf(f.Value, StringComparison.OrdinalIgnoreCase) < 0) return false;
                //    }
                //    else
                //    {
                //        switch (f.Key)
                //        {
                //            case "dep":
                //                if (!string.Equals(f.Value, bundle, StringComparison.OrdinalIgnoreCase) && !AssetBundleUtils.GetAllDependencies(f.Value).Contains(bundle, StringComparer.OrdinalIgnoreCase)) return false;
                //                break;
                //            case "asset":
                //                var bundle2 = AssetBundleUtils.GetBundleByAsset(f.Value);
                //                if (bundle2 == null || !string.Equals(bundle2, bundle, StringComparison.OrdinalIgnoreCase)) return false;
                //                break;
                //            default:
                //                break;

                //        }
                //    }
                //    return true;
                //}))
                if (filteredBundles.Contains(bundle))
                {
                    if (curBundle != null && bundle.Equals(curBundle, StringComparison.OrdinalIgnoreCase))
                    {
                        newIdx = bundleIndices.Count;
                    }
                    bundleIndices.Add(idx);
                }
                idx++;
            }
            listView1.VirtualListSize = bundleIndices.Count;
            if (newIdx != -1) listView1.SelectedIndices.Add(newIdx);
            listView1.EndUpdate();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            swapTimer.Stop();
            swapTimer.Start();
        }

        private readonly Dictionary<string, ListViewItem> itemCache = [];

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var bundleName = bundleList[bundleIndices[e.ItemIndex]];
            if (!itemCache.TryGetValue(bundleName, out var item))
            {
                var bundlePath = AssetBundleUtils.GetRelativeBundlePath(bundleName);
                item = new(bundleName);
                itemCache.Add(bundleName, item);
            }
            e.Item = item;
        }

        private void listView1_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {
            for (int i = e.StartIndex; i < bundleIndices.Count; i++)
            {
                var bundleName = bundleList[bundleIndices[i]];
                if (bundleName.StartsWith(e.Text!, StringComparison.OrdinalIgnoreCase))
                {
                    e.Index = i;
                    break;
                }
            }
        }

        private void swapTimer_Tick(object sender, EventArgs e)
        {
            string? bundleName = null;
            if (listView1.SelectedIndices.Count == 1)
            {
                bundleName = bundleList[bundleIndices[listView1.SelectedIndices[0]]];
            }
            if (currentBundle == bundleName) return;
            currentBundle = bundleName;
            bundleView?.Dispose();
            bundleView = null;
            if (bundleName != null)
            {
                bundleView = new(bundleName)
                {
                    Dock = DockStyle.Fill
                };
                splitContainer1.Panel2.Controls.Add(bundleView);
            }
            swapTimer.Stop();
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTree();
        }

        private void FSBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            swapTimer.Stop();
            listView1.VirtualListSize = 0;
            bundleView?.Dispose();
        }
    }
}
