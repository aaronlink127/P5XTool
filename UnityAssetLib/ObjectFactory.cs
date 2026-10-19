using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public static partial class ObjectFactory
    {
        static partial void GenerateObjectMap();
        private static Dictionary<string, Type>? _objMap;
        private static Dictionary<string, Type> objMap
        {
            get
            {
                if (_objMap == null)
                {
                    GenerateObjectMap();
                }
                return _objMap!;
            }
        }
        public static bool HasType(string typeName)
        {
            return objMap.ContainsKey(typeName);
        }
        public static void RegisterMono<T>(string name) where T : MonoBehaviour {
            objMap[name] = typeof(T);
        }
        public static Object Create(string name)
        {
            if (!objMap.TryGetValue(name, out var objType))
            {
                objType = typeof(Object);
            }
            var obj = Activator.CreateInstance(objType);
            return (Object)obj!;
        }
    }
}
