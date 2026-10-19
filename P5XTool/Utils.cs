using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace P5XTool
{
    public static class Utils
    {
        public static void SaveBundles(string filePath, IEnumerable<string> bundleList)
        {
            foreach (string bundleName in bundleList)
            {
                byte[]? data = VFileSystem.ReadAllBytes(AssetBundleUtils.GetRelativeBundlePath(bundleName));
                if (data != null)
                {
                    File.WriteAllBytes(Path.Join(filePath, bundleName), data.AsSpan(AssetUtils.GetBundleObfuscateOffset(bundleName)));
                }
            }
        }
        [SupportedOSPlatform("windows6.1")]
        public static void CopyPngToClipboard(Image img, string name = "image.png")
        {
            using var clipboard = RawClipboard.Open();
            clipboard.Empty();
            using var pngMs = new MemoryStream();
            img.Save(pngMs, ImageFormat.Png);
            using var bmpMs = new MemoryStream();
            img.Save(bmpMs, ImageFormat.Bmp);
            clipboard.SetBytes("PNG", pngMs.ToArray());
            string html = $@"Version:0.9
StartHTML:0000000105
EndHTML:0000000198
StartFragment:0000000141
EndFragment:0000000162
<html>
<body>
<!--StartFragment--><img src=""{name}""><!--EndFragment-->
</body>
</html>";

            clipboard.SetBytes("HTML Format", Encoding.ASCII.GetBytes(html + '\0'));
            clipboard.SetBytes(RawClipboard.CF_DIB, bmpMs.ToArray());
        }

        public static string FixBetaString(string str)
        {
            if (!GameMgr.IsCbt) return str;
            var b = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(b);
        }
    }
}
