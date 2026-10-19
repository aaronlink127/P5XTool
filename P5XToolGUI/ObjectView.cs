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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace P5XToolGUI
{
    public partial class ObjectView : UserControl
    {
        Control? innerControl;
        public ObjectView()
        {
            InitializeComponent();
        }
        public void ClearObject()
        {
            innerControl?.Dispose();
            innerControl = null;
            textBox1.Text = "";
        }
        public static void ObjectDump(object? data, StringBuilder sb, int depth = 0)
        {
            if (data == null)
            {
                sb.Append("null");
                return;
            }
            Type type = data.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var keyProp = type.GetProperty("Key");
                var valueProp = type.GetProperty("Value");
                object? key = keyProp?.GetValue(data, null);
                object? value = valueProp?.GetValue(data, null);
                ObjectDump(key, sb, depth);
                sb.Append(": ");
                ObjectDump(value, sb, depth);
            }
            else if (data is byte[] strB)
            {
                sb.Append(Encoding.UTF8.GetString(strB));
            }
            else if (data is string str)
            {
                sb.Append(str);
            }
            else if (data is System.Collections.IEnumerable enumerable)
            {
                var sb2 = new StringBuilder();
                int count = 0;
                foreach (var item in enumerable)
                {
                    sb2.Append("\r\n");
                    sb2.Append(' ', (depth + 1) * 4);
                    ObjectDump(item, sb2, depth + 1);
                    count++;
                }
                sb.Append(data.GetType().Name + " (count: " + count + ")");
                sb.Append(sb2.ToString());
            }
            else
            {
                sb.Append(data.ToString());
            }
        }
        public void SetObject(UnityAssetLib.Object obj)
        {
            ClearObject();
            switch (obj)
            {
                case Texture2D:
                case Sprite:
                    SpriteView spriteView = new();
                    spriteView.SetSprite(obj);
                    spriteView.Dock = DockStyle.Fill;
                    previewTab.Controls.Add(spriteView);
                    innerControl = spriteView;
                    break;
                case GameObject:
                case Mesh:
                    var scenePreview = new ScenePreviewControl();
                    scenePreview.OnLoad(() =>
                    {
                        if (obj is Mesh mesh)
                        {
                            scenePreview.AddMesh(mesh);
                        }
                        else if (obj is GameObject go) {
                            scenePreview.AddObject(go);
                        }
                    });
                    scenePreview.Dock = DockStyle.Fill;
                    previewTab.Controls.Add(scenePreview);
                    innerControl = scenePreview;
                    break;
            }
            StringBuilder sb = new();
            ObjectDump(obj.srcData, sb, 0);
            textBox1.Text = sb.ToString();
        }
    }
}
