using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class Texture2D : Texture
    {
        public int m_Width;
        public int m_Height;
        public uint m_CompleteImageSize;
        public TextureFormat m_TextureFormat;
        public int m_MipCount;
        public GLTextureSettings m_TextureSettings = new();
        public int m_ImageCount;
        public byte[] m_PlatformBlob = [];
        [AutoFieldName("image data")]
        public byte[] image_data = [];
        public StreamingInfo m_StreamData = new();
        public byte[] GetImageData()
        {
            if (image_data != null && image_data.Length > 0)
            {
                return image_data;
            }
            return m_StreamData.GetData().ToArray();
        }
    }
}
