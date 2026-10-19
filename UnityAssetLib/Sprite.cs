using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace UnityAssetLib
{
    public partial class Sprite : Object
    {
        public string m_Name = "";
        public Rectf m_Rect = new();
        public SpriteRenderData m_RD = new();
        public PPtr<SpriteAtlas> m_SpriteAtlas = new();
        public KeyValuePair<Guid, long> m_RenderDataKey;

        public Texture2D? GetTexture()
        {
            if (!m_SpriteAtlas.IsNull() && m_SpriteAtlas.TryGet(out var atlas))
            {
                var renderData = atlas.m_RenderDataMap[m_RenderDataKey];
                if (renderData.texture.TryGet(out var tex)) return tex;
            }
            else if (!m_RD.texture.IsNull())
            {
                if (m_RD.texture.TryGet(out var tex)) return tex;
            }
            return null;
        }
    }
}
