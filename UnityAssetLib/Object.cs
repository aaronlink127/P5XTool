using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnityAssetLib
{
    public partial class Object
    {
        public Dictionary<string, object?> srcData = [];
        public SerializedFile srcFile = null!;
        public override int GetHashCode()
        {
            return HashCode.Combine(srcFile.path, GetPathId());
        }
        public virtual void LoadDataFrom_Generated(Dictionary<string, object?> data, SerializedFile _sourceSerializedFile) { }

        [MemberNotNull(nameof(srcFile))]
        public virtual void LoadDataFrom(Dictionary<string, object?> data, SerializedFile _sourceSerializedFile)
        {
            //Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
            srcData = data;
            srcFile = _sourceSerializedFile;
            LoadDataFrom_Generated(data, _sourceSerializedFile);
        }
        public void LoadDataFrom(object? data, SerializedFile _sourceSerializedFile) {
            LoadDataFrom((Dictionary<string, object?>)data!, _sourceSerializedFile);
        }
        public string GetTypeName()
        {
            if (srcData.TryGetValue("__type", out var typeObj) && typeObj is string s)
            {
                return s;
            }
            return "Object";
        }
        public long GetPathId()
        {
            if (srcData.TryGetValue("__pathId", out var pathIdObj) && pathIdObj != null)
            {
                return (long)pathIdObj;
            }
            return 0;
        }

        public string GetName()
        {
            if (srcData.TryGetValue("m_Name", out var nameStr) && nameStr != null)
            {
                return Encoding.UTF8.GetString((byte[])nameStr);
            }
            return GetTypeName();
        }


        public static T GetData<T>(object? data, SerializedFile _sourceSerializedFile)
        {
            object? value;
            if (data is Dictionary<string, object?> dataDict)
            {
                value = Create(dataDict, _sourceSerializedFile);
            }
            else
            {
                return ParseValue<T>(data!, _sourceSerializedFile);
            }
            return (T)value;
        }
        public static T ParseValue<T>(object data, SerializedFile _sourceSerializedFile)
        {
            
            if (data is byte[] && typeof(T) == typeof(string))
            {
                return (T)(object)Encoding.UTF8.GetString((byte[])data);
            }
            if (data is byte && typeof(T) == typeof(bool))
            {
                return (T)(object)((byte)data == 0);
            }
            return (T)data;
        }
        public static KeyValuePair<T, J> GetPair<T, J>(object data, Func<object, object> cb1, Func<object, object> cb2, SerializedFile _sourceSerializedFile)
        {
            var dyn = (dynamic)data;
            var pair = new KeyValuePair<T, J>((T)cb1(dyn.Key), (J)cb2(dyn.Value));
            return pair;
        }
        public static T[] GetList<T>(object data, Func<object, object> cb, SerializedFile _sourceSerializedFile) {
            if (data is T[] v) return v;
            if (data is Array array)
            {
                T[] resultArr = new T[array.Length];
                int idx = 0;
                foreach (object item in array)
                {
                    resultArr[idx++] = (T)cb(item);
                }
                return resultArr;
            }
            throw new KeyNotFoundException($"Loading data failed.");
        }
        public static T Create<T>(Dictionary<string, object?> data, SerializedFile _sourceSerializedFile) where T : Object, new()
        {
            var obj = new T();
            obj.LoadDataFrom(data, _sourceSerializedFile);
            return obj;
        }
        public static T Create<T>(object data, SerializedFile _sourceSerializedFile) where T : Object, new()
        {
            return Create<T>((Dictionary<string, object?>)data, _sourceSerializedFile);
        }
        public static Object Create(Dictionary<string, object?> data, SerializedFile _sourceSerializedFile)
        {
            var typeName = data["__type"] as string ?? "Object";
            if (typeName == "MonoBehaviour")
            {
                PPtr<MonoScript> pPtr = new();
                pPtr.LoadDataFrom(data["m_Script"], _sourceSerializedFile);
                if (pPtr.TryGet(out var result))
                {
                    if (ObjectFactory.HasType(result.FullClassName)) typeName = result.FullClassName;
                }
            }
            var obj = ObjectFactory.Create(typeName);
            obj.LoadDataFrom(data, _sourceSerializedFile);
            return obj;
        }
    }
}
