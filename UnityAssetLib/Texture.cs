using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnityAssetLib
{
    public enum TextureColorSpace
    {
        Linear,
        sRGB
    }
    public partial class Texture : Object
    {
        public string m_Name = "";
        public int m_ForcedFallbackFormat;
        public bool m_DownscaleFallback;
        public TextureColorSpace m_ColorSpace;
        public int m_TextureDimension;
    }
}
