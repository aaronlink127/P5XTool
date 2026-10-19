using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using UnityAssetLib;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using P5XTool;

namespace P5XToolGUI
{
    public partial class ScenePreviewControl : UserControl
    {
        bool init = false;
        readonly ScenePreview preview = new();
        readonly GLControl glControl;
        SceneObject? activeObject = null;
        readonly HashSet<int> expandedObjs = [];
        readonly List<Action> loadCallbacks = [];
        private class FashionPart
        {
            public long sn;
            public string prefab = "";
            public string attachPoint = "";
            public string materialColor = "";
            public override string ToString()
            {
                return prefab;
            }
        }
        public ScenePreviewControl()
        {
            InitializeComponent();
            GLControlSettings gLControlSettings = new();
            gLControlSettings.NumberOfSamples = 4;
            glControl = new GLControl(gLControlSettings);
            splitContainer1.Panel1.Controls.Add(glControl);
            glControl.Dock = DockStyle.Fill;
            glControl.Load += glControl_Load;
            glControl.Paint += glControl_Paint;
            glControl.MouseDown += glControl_MouseDown;
            glControl.MouseUp += glControl_MouseUp;
            glControl.MouseMove += glControl_MouseMove;
            glControl.MouseWheel += glControl_MouseWheel;
            glControl.KeyDown += glControl_KeyDown;
            glControl.KeyUp += glControl_KeyUp;
            glControl.Disposed += glControl_Disposed;
            var fashions = DB.GetTableDict("Fashion", "ConfFashionPartModel");
            foreach (var fashion in fashions)
            {
                var boxItem = new FashionPart() { sn = (long)fashion["sn"]!, prefab = (string)fashion["prefab"]!, attachPoint = (string)fashion["hangPoint"]!, materialColor = (string)fashion.GetValueOrDefault("materialColor")! };
                partSel.Items.Add(boxItem);
            }
        }

        public void ResetCamera()
        {
            preview.ResetCamera();
        }
        private void glControl_Disposed(object? sender, EventArgs e)
        {
            glControl.MakeCurrent();
            preview.Dispose();
        }

        private void glControl_Load(object? sender, EventArgs e)
        {
            glControl.Disposed += glControl_Disposed;
            glControl.Resize += glControl_Resize;
            preview.Init();
            init = true;
            foreach (var callback in loadCallbacks)
            {
                callback();
            }
            loadCallbacks.Clear();
        }

        private TreeNode createNode(SceneObject obj)
        {
            var node = new TreeNode(obj.Name);
            foreach (var tf in obj.Transform.Children)
            {
                node.Nodes.Add(createNode(tf.Object));
            }
            node.Tag = obj;
            node.Checked = obj.Active;
            if (obj == activeObject)
            {
                treeView1.SelectedNode = node;
            }
            if (expandedObjs.Contains(obj.Id))
            {
                node.Expand();
            }
            if (obj.Components.Count != 0)
            {
                node.BackColor = System.Drawing.Color.Green;
                node.ForeColor = System.Drawing.Color.White;
            }
            return node;
        }
        public void updateTree()
        {
            var scrollPos = treeView1.GetScrollPos(ScrollBarDirection.Vertical);
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            foreach (var obj in preview.scene!.GetRootObjects())
            {
                treeView1.Nodes.Add(createNode(obj));
            }
            if (scrollPos != 0)
            {
                treeView1.SetScrollPos(ScrollBarDirection.Vertical, scrollPos, false);
            }
            treeView1.EndUpdate();
        }

        private void printTree(SceneObject obj, int depth = 0)
        {
            Console.WriteLine($"{new string(' ', depth)}{obj.Name}");
            foreach (var tf in obj.Transform.Children)
            {
                printTree(tf.Object, depth + 1);
            }
        }


        private void glControl_Resize(object? sender, EventArgs e)
        {
            glControl.MakeCurrent();

            preview.UpdateSize(glControl.ClientSize.Width, glControl.ClientSize.Height);
        }

        private void updTimer_Tick(object? sender, EventArgs e)
        {
            Vector3 input = Vector3.Zero;
            if (keyW) input.Z += 1;
            if (keyS) input.Z -= 1;
            if (keyA) input.X -= 1;
            if (keyD) input.X += 1;
            if (keyQ) input.Y -= 1;
            if (keyE) input.Y += 1;
            if (input.Equals(Vector3.Zero) == false)
            {
                var cameraRot = preview.cameraRot;
                input = Vector3.Normalize(input);
                float cosPitch = MathF.Cos(cameraRot.X);
                float sinPitch = MathF.Sin(cameraRot.X);
                float cosYaw = MathF.Cos(cameraRot.Y);
                float sinYaw = MathF.Sin(cameraRot.Y);
                Vector3 cameraDir = new(
                    sinYaw * cosPitch,
                    sinPitch,
                    cosYaw * cosPitch
                );
                Vector3 worldUp = Vector3.UnitY;
                Vector3 cameraRight = Vector3.Cross(worldUp, cameraDir);
                //if (cameraRight.LengthSquared < 1e-6f)
                //{
                //    cameraRight = Vector3.UnitX;
                //}
                //else
                //{
                    cameraRight = Vector3.Normalize(cameraRight);
                //}
                Vector3 cameraUp = Vector3.Cross(cameraDir, cameraRight);
                //if (cameraUp.LengthSquared < 1e-6f)
                //{
                //    cameraUp = Vector3.UnitY;
                //}
                //else
                //{
                    cameraUp = Vector3.Normalize(cameraUp);
                //}
                float moveSpeed = 0.02f;
                if (keyShift)
                {
                    moveSpeed *= 5;
                }
                preview.cameraPos += cameraRight * input.X * moveSpeed;
                preview.cameraPos += cameraUp * input.Y * moveSpeed;
                preview.cameraPos += cameraDir * input.Z * moveSpeed;
            }
            glControl.Invalidate();
        }

        private int mdx = 0;
        private int mdy = 0;

        bool leftMouse = false;
        bool rightMouse = false;

        private static float EuclidModulo(float dividend, float divisor)
        {
            var remaind = dividend % divisor;
            if (remaind < 0)
            {
                remaind += divisor;
            }
            return remaind;
        }

        private void glControl_MouseMove(object? sender, MouseEventArgs e)
        {
            int dx = mdx - e.X;
            int dy = mdy - e.Y;
            mdx = e.X;
            mdy = e.Y;

            var ctrl = (Control.ModifierKeys & Keys.Control) == Keys.Control;

            if (leftMouse)
            {
                if (ctrl)
                {
                    preview.cameraDistance = Math.Max(0.001f, preview.cameraDistance * (1 - dy * 0.001f));
                    preview.cameraFov = Math.Clamp(preview.cameraFov * (1 - dx * 0.001f), 0.001f, MathF.PI);
                    preview.UpdateProjection();
                }
                else
                {
                    preview.cameraRot.Y = EuclidModulo(preview.cameraRot.Y + dx * -0.01f, MathF.PI * 2);
                    preview.cameraRot.X = (float)Math.Clamp(preview.cameraRot.X + dy * 0.01f, -Math.PI / 2 + 0.01, Math.PI / 2 - 0.01);
                }
            }

            if (rightMouse)
            {
                float cosPitch = MathF.Cos(preview.cameraRot.X);
                float sinPitch = MathF.Sin(preview.cameraRot.X);
                float cosYaw = MathF.Cos(preview.cameraRot.Y);
                float sinYaw = MathF.Sin(preview.cameraRot.Y);

                Vector3 cameraDir = new(
                    sinYaw * cosPitch,
                    sinPitch,
                    cosYaw * cosPitch
                );

                Vector3 worldUp = Vector3.UnitY;
                Vector3 cameraRight = Vector3.Cross(worldUp, cameraDir);

                if (cameraRight.LengthSquared < 1e-6f)
                {
                    cameraRight = Vector3.UnitX;
                }
                else
                {
                    cameraRight = Vector3.Normalize(cameraRight);
                }

                Vector3 cameraUp = Vector3.Cross(cameraDir, cameraRight);
                if (cameraUp.LengthSquared < 1e-6f)
                {
                    cameraUp = Vector3.UnitY;
                }
                else
                {
                    cameraUp = Vector3.Normalize(cameraUp);
                }

                const float moveSpeed = 0.01f;

                preview.cameraPos += cameraRight * dx * moveSpeed * preview.cameraDistance;
                preview.cameraPos += cameraUp * dy * -moveSpeed * preview.cameraDistance;
            }
            if (leftMouse || rightMouse)
            {
                glControl.Invalidate();
            }
        }

        private void glControl_Paint(object? sender, PaintEventArgs e)
        {
            if (!init)
                return;
            glControl.MakeCurrent();
            preview.Render();
            glControl.SwapBuffers();
        }

        private void glControl_MouseDown(object? sender, MouseEventArgs e)
        {
            glControl.Focus();
            if (e.Button == MouseButtons.Left) { leftMouse = true; }
            if (e.Button == MouseButtons.Right) { rightMouse = true; }
        }

        private void glControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            float zoomFactor = MathF.Exp(-e.Delta * 0.001f);
            preview.cameraDistance = Math.Max(0.01f, preview.cameraDistance * zoomFactor);
            glControl.Invalidate();
        }

        private void glControl_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { leftMouse = false; }
            if (e.Button == MouseButtons.Right) { rightMouse = false; }
        }
        public void AddRootPart(string prefabName)
        {
            var obj = preview.CreatePrefab(prefabName);
            activeObject = obj;
            updateTree();
        }
        public void AddFashionPart(string prefabName, string attachPoint)
        {
            var obj = preview.CreatePrefab(prefabName);
            if (obj != null && activeObject != null)
            {
                if (attachPoint != "AAAAAA==")
                {
                    var parent = activeObject.Transform.GetChildByName(attachPoint);
                    obj.Transform.CopyTransform = parent;
                }
                obj.Transform.Parent = activeObject.Transform;
            }
            updateTree();
        }
        public SceneObject AddObject(GameObject obj)
        {
            var rootObj = preview.AddObject(obj);
            updateTree();
            return rootObj;
        }

        public SceneObject AddMesh(Mesh umesh)
        {
            var obj = preview.AddMesh(umesh);
            updateTree();
            return obj;
        }

        public SceneObject? CreatePrefab(string prefabName)
        {
            var prefab = preview.CreatePrefab(prefabName);
            updateTree();
            return prefab;
        }

        private void treeView1_AfterCheck(object? sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.Tag is SceneObject obj)
            {
                obj.Active = e.Node.Checked;
                bool shiftHeld = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

                if (shiftHeld)
                {
                    foreach (TreeNode child in e.Node.Nodes)
                    {
                        child.Checked = e.Node.Checked;
                    }
                    // Apply checked state to this node and all children, updating SceneObject.Active for each.
                    //suppressTreeAfterCheck = true;
                    //try
                    //{
                    //    void ApplyRecursive(TreeNode node, bool state)
                    //    {
                    //        if (node.Tag is SceneObject sobj)
                    //        {
                    //            sobj.Active = state;
                    //        }

                    //        // Setting Checked will normally re-trigger AfterCheck, but suppressTreeAfterCheck prevents handling.
                    //        node.Checked = state;

                    //        foreach (TreeNode child in node.Nodes)
                    //        {
                    //            ApplyRecursive(child, state);
                    //        }
                    //    }

                    //    ApplyRecursive(e.Node, e.Node.Checked);
                    //}
                    //finally
                    //{
                    //    suppressTreeAfterCheck = false;
                    //}
                }

                glControl.Invalidate();
            }
        }

        bool keyW = false;
        bool keyA = false;
        bool keyS = false;
        bool keyD = false;
        bool keyQ = false;
        bool keyE = false;
        bool keyShift = false;
        int numUpdaters = 0;

        void startTimer()
        {
            if (numUpdaters++ == 0)
            {
                updTimer.Start();
            }
        }
        void stopTimer()
        {
            if (--numUpdaters == 0)
            {
                updTimer.Stop();
            }
        }
        void resetTimer()
        {
            numUpdaters = 0;
            updTimer.Stop();
        }

        private void glControl_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (keyW) return;
                    keyW = true;
                    break;
                case Keys.A:
                    if (keyA) return;
                    keyA = true;
                    break;
                case Keys.S:
                    if (keyS) return;
                    keyS = true;
                    break;
                case Keys.D:
                    if (keyD) return;
                    keyD = true;
                    break;
                case Keys.Q:
                    if (keyQ) return;
                    keyQ = true;
                    break;
                case Keys.E:
                    if (keyE) return;
                    keyE = true;
                    break;
                case Keys.ShiftKey:
                    keyShift = true;
                    return;
                default:
                    return;
            }
            startTimer();
        }

        private void glControl_KeyUp(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (!keyW) return;
                    keyW = false;
                    break;
                case Keys.A:
                    if (!keyA) return;
                    keyA = false;
                    break;
                case Keys.S:
                    if (!keyS) return;
                    keyS = false;
                    break;
                case Keys.D:
                    if (!keyD) return;
                    keyD = false;
                    break;
                case Keys.Q:
                    if (!keyQ) return;
                    keyQ = false;
                    break;
                case Keys.E:
                    if (!keyE) return;
                    keyE = false;
                    break;
                case Keys.ShiftKey:
                    keyShift = false;
                    return;
                default:
                    return;
            }
            stopTimer();
        }

        private void ScenePreview_Leave(object? sender, EventArgs e)
        {
            keyW = false;
            keyA = false;
            keyS = false;
            keyD = false;
            keyQ = false;
            keyE = false;
            keyShift = false;
            leftMouse = false;
            rightMouse = false;
            resetTimer();
        }

        private void addPartBtn_Click(object? sender, EventArgs e)
        {
            AddFashionPart(prefabName.Text, boneName.Text);
        }

        public void OnLoad(Action callback)
        {
            if (init)
            {
                callback();

            }
            else
            {
                loadCallbacks.Add(callback);
            }
        }

        private void partSel_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedPart = partSel.SelectedItem as FashionPart;
            prefabName.Text = selectedPart != null ? selectedPart.prefab : "";
            boneName.Text = selectedPart != null && selectedPart.attachPoint != "AAAAAA==" ? selectedPart.attachPoint : "";
        }

        private void prefabName_TextChanged(object? sender, EventArgs e)
        {
            addPartBtn.Enabled = !string.IsNullOrEmpty(prefabName.Text) && AssetBundleUtils.GetBundleByAsset(prefabName.Text) != null;
        }

        private void treeView1_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Delete", null, (s, ea) =>
                {
                    if (e.Node.Tag is SceneObject obj)
                    {
                        obj.Transform.Parent = null;
                        var children = obj.Transform.GetChildrenRecursively();
                        foreach (var child in children)
                        {
                            preview.scene!.RemoveObject(child.Object);
                        }
                        preview.scene!.RemoveObject(obj);
                        updateTree();
                        glControl.Invalidate();
                    }
                });
                contextMenu.Items.Add("Center", null, (s, ea) =>
                {
                    if (e.Node.Tag is SceneObject obj)
                    {
                        preview.cameraPos = obj.Transform.GetWorldPosition();
                        preview.cameraDistance = 1f;
                        glControl.Invalidate();
                    }
                });

                var item = contextMenu.Items.Add("Active", null, (s, ea) =>
                {
                    if (e.Node.Tag is SceneObject obj)
                    {
                        if (activeObject == obj)
                        {
                            activeObject = null;
                        }
                        else
                        {
                        activeObject = obj;
                        }
                    }
                }) as ToolStripMenuItem;
                item?.Checked = e.Node.Tag == activeObject;
                contextMenu.Show(treeView1, e.Location);
            }
        }

        public void LoadScene(string unityFile)
        {
            preview.LoadScene(unityFile);
            updateTree();
        }

        public void PopOut()
        {
            Form form = new();
            form.Icon = MainForm.MainIcon;
            form.Controls.Add(this);
            popOutBtn.Dispose();
            Dock = DockStyle.Fill;
            form.Show();
        }

        private void popOutBtn_Click(object? sender, EventArgs e)
        {
            PopOut();
        }

        private void treeView1_AfterExpand(object? sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.Tag is SceneObject obj)
            {
                expandedObjs.Add(obj.Id);
            }
        }

        private void treeView1_AfterCollapse(object? sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.Tag is SceneObject obj)
            {
                expandedObjs.Remove(obj.Id);
            }
        }
    }

}
