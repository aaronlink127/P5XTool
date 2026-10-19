using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public enum TextureWrapMode
    {
        Repeat = 0,
        Clamp = 1,
        Mirror = 2,
        MirrorOnce = 3
    }
    public enum FilterMode
    {
        Point = 0,
        Bilinear = 1,
        Trilinear = 2
    }
    public partial class GLTextureSettings : Object
    {
        public FilterMode m_FilterMode;
        public int m_Aniso;
        public float m_MipBias;
        public TextureWrapMode m_WrapU;
        public TextureWrapMode m_WrapV;
        public TextureWrapMode m_WrapW;
    }
}
