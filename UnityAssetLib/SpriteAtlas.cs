using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class SpriteAtlasData : Object
    {
        public PPtr<Texture2D> texture = new();
        public Rectf textureRect = new();
        public Vector2 textureRectOffset = new();
        //public override void LoadDataFrom(Dictionary<string, object?> data, AssetBundle _sourceAssetBundle)
        //{
        //    texture = new();
        //    texture.LoadDataFrom(data["texture"], _sourceAssetBundle);
        //    textureRect = new Rectf();
        //    textureRect.LoadDataFrom(data["textureRect"], _sourceAssetBundle);
        //}
    }
    public partial class SpriteAtlas : Object
    {
        public Dictionary<KeyValuePair<Guid, long>, SpriteAtlasData> m_RenderDataMap = [];
    }
}
