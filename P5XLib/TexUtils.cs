using AssetRipper.Conversions.UnityCrunch;
using AssetRipper.TextureDecoder.Bc;
using AssetRipper.TextureDecoder.Dxt;
using AssetRipper.TextureDecoder.Rgb;
using AssetRipper.TextureDecoder.Rgb.Formats;
using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace P5XLib
{
    public class TexUtils
    {
        public static byte[] DeCrunch(byte[] data, TextureFormat texFmt)
        {
            if (texFmt is TextureFormat.DXT1Crunched or TextureFormat.DXT5Crunched or TextureFormat.ETC_RGB4Crunched or TextureFormat.ETC2_RGBA8Crunched)
            {
                if (UnityCrunch.TryDecompress(data, out var decData))
                    return decData;
            }
            return data;
        }
        public static TextureFormat GetUncrunched(TextureFormat texFmt)
        {
            return texFmt switch
            {
                TextureFormat.DXT1Crunched => TextureFormat.DXT1,
                TextureFormat.DXT5Crunched => TextureFormat.DXT5,
                TextureFormat.ETC_RGB4Crunched => TextureFormat.ETC_RGB4,
                TextureFormat.ETC2_RGBA8Crunched => TextureFormat.ETC2_RGBA8,
                _ => texFmt
            };
        }
        public static byte[]? GetPixelData(Texture2D tex)
        {
            var data = DeCrunch(tex.GetImageData(), tex.m_TextureFormat);
            var fmt = GetUncrunched(tex.m_TextureFormat);
            switch (fmt)
            {
                case TextureFormat.DXT1:
                    {
                        DxtDecoder.DecompressDXT1<ColorBGRA32, byte>(data, tex.m_Width, tex.m_Height, out var colData);
                        return colData;
                    }
                case TextureFormat.BC7:
                    {
                        Bc7.Decompress<ColorBGRA32, byte>(data, tex.m_Width, tex.m_Height, out var colData);
                        return colData;
                    }
                case TextureFormat.DXT5:
                    {
                        DxtDecoder.DecompressDXT5<ColorBGRA32, byte>(data, tex.m_Width, tex.m_Height, out var colData);
                        return colData;
                    }
                case TextureFormat.RGBA32:
                    {
                        RgbConverter.RGBA32ToBGRA32(data, tex.m_Width, tex.m_Height, out var colData);
                        return colData;
                    }
                case TextureFormat.RGB24:
                    {
                        RgbConverter.RGB24ToBGRA32(data, tex.m_Width, tex.m_Height, out var colData);
                        return colData;
                    }
                case TextureFormat.RGBAHalf:
                    {
                        RgbConverter.RGBAHalfToBGRA32(data, tex.m_Width, tex.m_Height, out var colData);
                        return colData;
                    }
                case TextureFormat.BC6H:
                    {
                        Bc6h.Decompress<ColorBGRA32, byte>(data, tex.m_Width, tex.m_Height, false, out var colData);
                        return colData;
                    }
                case TextureFormat.Alpha8:
                    {
                        RgbConverter.A8ToBGRA32(data, tex.m_Width, tex.m_Height, out var colData);
                        return colData;
                    }
            }
            Console.WriteLine($"Unsupported Fmt: {tex.m_TextureFormat}");
            return null;
        }
        [SupportedOSPlatform("windows6.1")]
        public static Bitmap? GetBitmapForImage(UnityAssetLib.Object asset, bool usePadding = true)
        {
            Texture2D? tex = null;
            RectangleF texArea = new();
            Vector2 sprOffset = new(0, 0);
            int width = 0;
            int height = 0;
            if (asset is Texture2D d)
            {
                tex = d;
            }
            else if (asset is Sprite spr)
            {
                width = (int)Math.Ceiling(spr.m_Rect.width);
                height = (int)Math.Ceiling(spr.m_Rect.height);
                if (!spr.m_SpriteAtlas.IsNull() && spr.m_SpriteAtlas.TryGet(out var atlas))
                {
                    var renderData = atlas.m_RenderDataMap[spr.m_RenderDataKey];
                    renderData.texture.TryGet(out tex);
                    texArea = renderData.textureRect.ToRectangleF();
                    if (usePadding)
                    {
                        sprOffset = renderData.textureRectOffset;
                    }
                    else
                    {
                        width = (int)Math.Ceiling(texArea.Width);
                        height = (int)Math.Ceiling(texArea.Height);
                    }
                }
                else if (!spr.m_RD.texture.IsNull())
                {
                    spr.m_RD.texture.TryGet(out tex);
                    texArea = spr.m_Rect.ToRectangleF();
                }
            }
            if (tex != null)
            {
                byte[]? rgbPixelData = GetPixelData(tex);

                if (rgbPixelData == null || rgbPixelData.Length == 0)
                    return null;

                Bitmap bitmap = new(tex.m_Width, tex.m_Height, PixelFormat.Format32bppArgb);
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, tex.m_Width, tex.m_Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                Marshal.Copy(rgbPixelData, 0, bmpData.Scan0, rgbPixelData.Length);
                bitmap.UnlockBits(bmpData);
                Bitmap outBmp = bitmap;
                if (width != 0)
                {
                    outBmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    using Graphics g = Graphics.FromImage(outBmp);
                    g.DrawImage(bitmap, sprOffset.X, sprOffset.Y, texArea, GraphicsUnit.Pixel);
                }
                outBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                return outBmp;
            }
            return null;
        }
        [SupportedOSPlatform("windows6.1")]
        public static Bitmap? GetBitmapForImage(string assetName, bool usePadding = true)
        {
            var assetR = AssetManager.LoadAsset(assetName);
            if (assetR == null) return null;
            var asset = assetR.Asset;
            var bmp = GetBitmapForImage(asset, usePadding);
            return bmp;
        }
    }
}
