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
using P5XTool;

namespace P5XToolGUI
{
    public partial class BundleForm : Form
    {
        HashSet<string> bundleList = [];
        public BundleForm()
        {
            InitializeComponent();
        }

        private void addBundleDeps(string bundleName, TreeNode parent)
        {
            var dependencies = AssetBundleUtils.GetDirectDependencies(bundleName);
            foreach (var dep in dependencies)
            {
                if (bundleList.Contains(dep)) continue;
                TreeNode childNode = new(dep);
                parent.Nodes.Add(childNode);
                bundleList.Add(dep);
                addBundleDeps(dep, childNode);
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            string bundleName = addName.Text;

            if (!bundleName.EndsWith(".bundle"))
            {
                var bundleName2 = AssetBundleUtils.GetBundleByAsset(bundleName);
                if (bundleName2 == null)
                {
                    return;
                }
                bundleName = bundleName2;
            }
            if (string.IsNullOrEmpty(bundleName) || bundleList.Contains(bundleName)) return;
            bundleList.Add(bundleName);
            var treeNode = new TreeNode(bundleName);
            treeView1.Nodes.Add(treeNode);
            if (depCheck.Checked)
            {
                addBundleDeps(bundleName, treeNode);
            }
        }

        private void addName_TextChanged(object sender, EventArgs e)
        {
            addBtn.Enabled = false;
            string bundleName = addName.Text;
            if (string.IsNullOrEmpty(bundleName))
            {
                return;
            }
            if (!bundleName.EndsWith(".bundle"))
            {
                var bundleName2 = AssetBundleUtils.GetBundleByAsset(bundleName);
                if (bundleName2 == null)
                {
                    return;
                }
                bundleName = bundleName2;
            }
            if (!AssetBundleUtils.IsBundleInManifest(bundleName))
            {
                return;
            }
            addBtn.Enabled = true;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
                contextMenuStrip1.Show(treeView1, e.Location);
                contextMenuStrip1.Tag = e.Node;
            }
        }

        private void removeBundle(TreeNode node)
        {
            bundleList.Remove(node.Text);
            List<TreeNode> childNodes = node.Nodes.Cast<TreeNode>().ToList();
            foreach (TreeNode child in childNodes)
            {
                removeBundle(child);
            }
            node.Remove();
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var treenode = contextMenuStrip1.Tag as TreeNode;
            if (treenode != null)
            {
                removeBundle(treenode);
                contextMenuStrip1.Tag = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveFileDialog = new();
            var result = saveFileDialog.ShowDialog(this);
            if (result != DialogResult.OK) return;
            var filePath = saveFileDialog.SelectedPath;
            Utils.SaveBundles(filePath, bundleList);
        }

        private static void AddTreeNodeLine(TreeNode node, StringBuilder sb, string indent, bool isLast)
        {
            sb.Append(indent);
            if (isLast)
            {
                sb.Append("└─ ");
                indent += "   ";
            }
            else
            {
                sb.Append("├─ ");
                indent += "│  ";
            }
            sb.AppendLine(node.Text);
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                AddTreeNodeLine(node.Nodes[i], sb, indent, i == node.Nodes.Count - 1);
            }
        }

        private void listCopyBtn_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new();
            foreach (TreeNode node in treeView1.Nodes)
            {
                AddTreeNodeLine(node, sb, "", true);
            }
            Clipboard.SetText(sb.ToString());
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var treenode = contextMenuStrip1.Tag as TreeNode;
            if (treenode != null)
            {
                var bundleName = treenode.Text;
                var form = BundleView.CreateWindow(bundleName);
                form.Show();
            }
        }
    }
}
