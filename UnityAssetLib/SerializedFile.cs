using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnityAssetLib
{
    public class FileIdentifier
    {
        public Guid guid;
        public int type;
        public string pathName = "";
    }
    public class SerializedType
    {
        //public static Dictionary<int, string> classIdNames = new()
        //{
        //    {1, "AssetBundle"},
        //    {20, "MonoBehaviour"},
        //    {21, "MonoScript"},
        //    {114, "ScriptableObject"},
        //};
        public static Dictionary<string, TypeTreeNode> preloadTypeTrees = [];
        public int classID;
        public bool m_IsStrippedType;
        public short m_ScriptTypeIndex = -1;
        public TypeTreeNode? m_RootNode;
        public Guid m_ScriptID;
        public byte[] m_OldTypeHash = [];
        public int[] m_TypeDependencies = [];
        public string m_KlassName = "";
        public string m_NameSpace = "";
        public string m_AsmName = "";

        public string? ClassName => m_RootNode?.m_Type;

        public TypeTreeNode RootNode
        {
            get
            {
                m_RootNode ??= LoadPreTypeTree(GetFileName());
                return m_RootNode;
            }
        }

        public string GetFileName()
        {
            var filename = $"{classID}";
            if (m_ScriptID != Guid.Empty)
            {
                filename += $"_{m_ScriptID}";
            }
            return filename + ".bin";
        }

        public static TypeTreeNode LoadPreTypeTree(string filename)
        {
            if (preloadTypeTrees.ContainsKey(filename)) return preloadTypeTrees[filename];
            var loc = Path.GetDirectoryName(AppContext.BaseDirectory)!;
            var path = Path.Combine(loc, "TypeTrees", filename);
            if (!File.Exists(path)) throw new Exception($"Preload typetree {filename} not found.");
            using var br = new BinaryReader(File.OpenRead(path));
            var node = SerializedFile.ParseNodeTree(br);
            preloadTypeTrees[filename] = node;
            return node;
        }

        public void PassUntil(BinaryReader br, int maxIdx)
        {
            for (int i = 0; i < maxIdx; i++)
            {
                var node = RootNode.m_Nodes[i];
                node.Pass(br);
            }
        }
        public Dictionary<string, object?> DeserializeType(BinaryReader br)
        {
            return (DeserializeNode(br, RootNode) as Dictionary<string, object?>)!;
        }

        public static object DeserializeArray(BinaryReader br, TypeTreeNode elem, int len)
        {
            object[] arr = new object[len];
            for (int i = 0; i < len; i++)
            {
                arr[i] = DeserializeNode(br, elem)!;
            }
            var nodeType = GetNodeType(elem);
            if (nodeType == typeof(object)) return arr;
            Array dest = Array.CreateInstance(GetNodeType(elem), arr.Length);
            if (arr.Length > 0)
                Array.Copy(arr, dest, arr.Length);
            return dest;
        }

        public static Type GetNodeType(TypeTreeNode node)
        {
            return node.m_Type switch
            {
                "Vector2f" => typeof(Vector2),
                "Vector3f" => typeof(Vector3),
                "Vector4f" => typeof(Vector4),
                "float2" => node.m_ByteSize == 8 ? typeof(Vector2) : typeof(Vector2[]),
                "float3" => node.m_ByteSize == 12 ? typeof(Vector3) : typeof(Vector3[]),
                "float4" => node.m_ByteSize == 16 ? typeof(Vector4) : typeof(Vector4[]),
                "Quaternionf" => typeof(Quaternion),
                "Matrix4x4f" => typeof(Matrix4x4),
                "Hash128" => typeof(Hash128),
                "GUID" => typeof(Guid),
                "SInt8" => typeof(sbyte),
                "UInt8" or "char" => typeof(byte),
                "SInt16" or "short" => typeof(short),
                "UInt16" or "unsigned short" => typeof(ushort),
                "SInt32" or "int" => typeof(int),
                "UInt32" or "unsigned int" or "Type*" => typeof(uint),
                "SInt64" or "long long" => typeof(long),
                "UInt64" or "unsigned long long" or "FileSize" => typeof(ulong),
                "float" => typeof(float),
                "double" => typeof(double),
                "bool" => typeof(bool),
                "string" => typeof(byte[]),
                "map" or "vector" => GetNodeType(node.m_Nodes[0]),
                "Array" => GetNodeType(node.m_Nodes[1]).MakeArrayType(),
                "pair" => typeof(KeyValuePair<,>).MakeGenericType(GetNodeType(node.m_Nodes[0]), GetNodeType(node.m_Nodes[1])),
                _ => typeof(object),
            };
        }
        public static object? DeserializeNode(BinaryReader br, TypeTreeNode node)
        {
            object? value;
            switch (node.m_Type)
            {
                case "Vector2f":
                    value = new Vector2(br.ReadSingle(), br.ReadSingle());
                    break;
                case "Vector3f":
                    value = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    break;
                case "Vector4f":
                    value = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    break;
                case "float2":
                    if (node.m_ByteSize != -1)
                    {
                        value = new Vector2(br.ReadSingle(), br.ReadSingle());
                    }
                    else
                    {
                        value = DeserializeNode(br, node.m_Nodes[0]);
                    }
                    break;
                case "float3":
                    if (node.m_ByteSize != -1)
                    {
                        value = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    }
                    else
                    {
                        value = DeserializeNode(br, node.m_Nodes[0]);
                    }
                    break;
                case "float4":
                    if (node.m_ByteSize != -1)
                    {
                        value = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    }
                    else
                    {
                        value = DeserializeNode(br, node.m_Nodes[0]);
                    }
                    break;
                case "Quaternionf":
                    value = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    break;
                case "Matrix4x4f":
                    {
                        value = Matrix4x4.Transpose(new Matrix4x4(
                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                        ));
                        break;
                    }
                case "Hash128":
                    value = new Hash128() { p1 = br.ReadInt64(), p2 = br.ReadInt64() };
                    break;
                case "GUID":
                    value = new Guid(br.ReadBytes(16));
                    break;
                case "SInt8":
                    value = br.ReadSByte();
                    break;
                case "UInt8":
                case "char":
                    value = br.ReadByte();
                    break;
                case "SInt16":
                case "short":
                    value = br.ReadInt16();
                    break;
                case "UInt16":
                case "unsigned short":
                    value = br.ReadUInt16();
                    break;
                case "SInt32":
                case "int":
                    value = br.ReadInt32();
                    break;
                case "UInt32":
                case "unsigned int":
                case "Type*":
                    value = br.ReadUInt32();
                    break;
                case "SInt64":
                case "long long":
                    value = br.ReadInt64();
                    break;
                case "UInt64":
                case "unsigned long long":
                case "FileSize":
                    value = br.ReadUInt64();
                    break;
                case "float":
                    value = br.ReadSingle();
                    break;
                case "double":
                    value = br.ReadDouble();
                    break;
                case "bool":
                    value = br.ReadBoolean();
                    break;
                case "string":
                case "vector":
                case "map":
                    value = DeserializeNode(br, node.m_Nodes[0]);
                    break;
                case "TypelessData":
                    {
                        var size = br.ReadInt32();
                        value = br.ReadBytes(size);
                        break;
                    }
                case "Array":
                    {
                        var size = br.ReadInt32();
                        TypeTreeNode data = node.m_Nodes[1];
                        value = DeserializeArray(br, data, size);
                        break;
                    }
                case "pair":
                    {
                        TypeTreeNode node1 = node.m_Nodes[0];
                        TypeTreeNode node2 = node.m_Nodes[1];
                        return Activator.CreateInstance(typeof(KeyValuePair<,>).MakeGenericType(GetNodeType(node1), GetNodeType(node2)), DeserializeNode(br, node1), DeserializeNode(br, node2));
                    }
                default:
                    {
                        if (node.m_Nodes.Count == 1 && node.m_Nodes[0].m_Type == "Array")
                        {
                            value = DeserializeNode(br, node.m_Nodes[0]);
                            break;
                        }
                        Dictionary<string, object?> dict = [];
                        dict.Add("__type", node.m_Type);
                        foreach (var childNode in node.m_Nodes)
                        {
                            dict[childNode.m_Name] = DeserializeNode(br, childNode);
                        }
                        value = dict;
                        break;
                    }
            }
            if ((node.m_MetaFlag & 0x4000) != 0) {
                Utils.AlignStream(br.BaseStream, 4);
            }
            return value;
        }
    }
    public class ObjectInfo
    {
        public long pathId;
        public int typeId;
        public long byteStart;
        public int byteSize;
    }
    public class TypeTreeNode
    {
        public string m_Type = "";
        public string m_Name = "";
        public int m_ByteSize;
        public int m_Index;
        public int m_TypeFlags;
        public int m_Version;
        public int m_MetaFlag;
        public int m_Level;
        public uint m_TypeStrOffset;
        public uint m_NameStrOffset;
        public ulong m_RefTypeHash;
        public List<TypeTreeNode> m_Nodes = [];
        private Dictionary<string, int> nameIndexMap = [];

        public TypeTreeNode() { }
        public int GetNodeIndexByName(string key)
        {
            return nameIndexMap.GetValueOrDefault(key, -1);
        }
        public TypeTreeNode? GetNodeByName(string key)
        {
            return m_Nodes.FirstOrDefault((n) => n.m_Name == key);
        }

        public void AddNode(TypeTreeNode node)
        {
            nameIndexMap[node.m_Name] = m_Nodes.Count;
            m_Nodes.Add(node);
        }

        public void Pass(BinaryReader br)
        {
            if (m_ByteSize != -1)
            {
                br.BaseStream.Position += m_ByteSize;
            }
            else if (m_Type == "Array")
            {
                var arrSize = br.ReadInt32();
                var node = m_Nodes[1];
                for (int i = 0; i < arrSize; i++) node.Pass(br);
            }
            else
            {
                foreach (var node in m_Nodes)
                {
                    node.Pass(br);
                }
            }
            if ((m_MetaFlag & 0x4000) != 0)
            {
                Utils.AlignStream(br.BaseStream, 4);
            }
        }

        public override string ToString()
        {
            static void recurse(TypeTreeNode tree, StringBuilder stringBuilder, int depth = 0)
            {
                stringBuilder.Append('\t', depth);
                stringBuilder.Append($"{tree.m_Type} {tree.m_Name} // {tree.m_ByteSize} {tree.m_MetaFlag:X} {tree.m_TypeFlags:X}");
                stringBuilder.Append('\n');
                foreach (var node in tree.m_Nodes)
                {
                    recurse(node, stringBuilder, depth + 1);
                }
            }
            var s = new StringBuilder();
            recurse(this, s);
            return s.ToString();
        }

    }
    public class SerializedFile
    {
        public bool isLittleEndian = false;
        public BinaryReader? _br;
        public List<SerializedType> m_Types = [];
        public string path = "";
        private Dictionary<long, Dictionary<string, object?>> m_Objects = [];
        private Dictionary<long, ObjectInfo> m_ObjectInfos = [];
        private int objRemain = 0;
        public List<FileIdentifier> m_Externals = [];
        public UnityVersion unityVersion;
        public static readonly Dictionary<uint, string> strBuf = new()
        {
            {0, "AABB"},
            {5, "AnimationClip"},
            {19, "AnimationCurve"},
            {34, "AnimationState"},
            {49, "Array"},
            {55, "Base"},
            {60, "BitField"},
            {69, "bitset"},
            {76, "bool"},
            {81, "char"},
            {86, "ColorRGBA"},
            {96, "Component"},
            {106, "data"},
            {111, "deque"},
            {117, "double"},
            {124, "dynamic_array"},
            {138, "FastPropertyName"},
            {155, "first"},
            {161, "float"},
            {167, "Font"},
            {172, "GameObject"},
            {183, "Generic Mono"},
            {196, "GradientNEW"},
            {208, "GUID"},
            {213, "GUIStyle"},
            {222, "int"},
            {226, "list"},
            {231, "long long"},
            {241, "map"},
            {245, "Matrix4x4f"},
            {256, "MdFour"},
            {263, "MonoBehaviour"},
            {277, "MonoScript"},
            {288, "m_ByteSize"},
            {299, "m_Curve"},
            {307, "m_EditorClassIdentifier"},
            {331, "m_EditorHideFlags"},
            {349, "m_Enabled"},
            {359, "m_ExtensionPtr"},
            {374, "m_GameObject"},
            {387, "m_Index"},
            {395, "m_IsArray"},
            {405, "m_IsStatic"},
            {416, "m_MetaFlag"},
            {427, "m_Name"},
            {434, "m_ObjectHideFlags"},
            {452, "m_PrefabInternal"},
            {469, "m_PrefabParentObject"},
            {490, "m_Script"},
            {499, "m_StaticEditorFlags"},
            {519, "m_Type"},
            {526, "m_Version"},
            {536, "Object"},
            {543, "pair"},
            {548, "PPtr<Component>"},
            {564, "PPtr<GameObject>"},
            {581, "PPtr<Material>"},
            {596, "PPtr<MonoBehaviour>"},
            {616, "PPtr<MonoScript>"},
            {633, "PPtr<Object>"},
            {646, "PPtr<Prefab>"},
            {659, "PPtr<Sprite>"},
            {672, "PPtr<TextAsset>"},
            {688, "PPtr<Texture>"},
            {702, "PPtr<Texture2D>"},
            {718, "PPtr<Transform>"},
            {734, "Prefab"},
            {741, "Quaternionf"},
            {753, "Rectf"},
            {759, "RectInt"},
            {767, "RectOffset"},
            {778, "second"},
            {785, "set"},
            {789, "short"},
            {795, "size"},
            {800, "SInt16"},
            {807, "SInt32"},
            {814, "SInt64"},
            {821, "SInt8"},
            {827, "staticvector"},
            {840, "string"},
            {847, "TextAsset"},
            {857, "TextMesh"},
            {866, "Texture"},
            {874, "Texture2D"},
            {884, "Transform"},
            {894, "TypelessData"},
            {907, "UInt16"},
            {914, "UInt32"},
            {921, "UInt64"},
            {928, "UInt8"},
            {934, "unsigned int"},
            {947, "unsigned long long"},
            {966, "unsigned short"},
            {981, "vector"},
            {988, "Vector2f"},
            {997, "Vector3f"},
            {1006, "Vector4f"},
            {1015, "m_ScriptingClassIdentifier"},
            {1042, "Gradient"},
            {1051, "Type*"},
            {1057, "int2_storage"},
            {1070, "int3_storage"},
            {1083, "BoundsInt"},
            {1093, "m_CorrespondingSourceObject"},
            {1121, "m_PrefabInstance"},
            {1138, "m_PrefabAsset"},
            {1152, "FileSize"},
            {1161, "Hash128"},
        };
        public static SerializedType ReadSerializedType(BinaryReader br, bool enableTypeTree, bool isRefType = false)
        {
            var type = new SerializedType
            {
                classID = br.ReadInt32(),
                m_IsStrippedType = br.ReadBoolean(),
                m_ScriptTypeIndex = br.ReadInt16()
            };
            if (isRefType && type.m_ScriptTypeIndex >= 0 || type.classID == 114)
            {
                var bytes = br.ReadBytes(16);
                type.m_ScriptID = new(bytes);
            }
            type.m_OldTypeHash = br.ReadBytes(16);
            //Console.WriteLine($"Reading type {type.classID} stripped={type.m_IsStrippedType} scriptIndex={type.m_ScriptTypeIndex} scriptId={Convert.ToBase64String(type.m_ScriptID.ToByteArray())}");
            if (enableTypeTree)
            {

                //var startPos = br.BaseStream.Position;
                type.m_RootNode = ParseNodeTree(br);
                //Console.WriteLine($"{type.m_RootNode}");
                //var endPos = br.BaseStream.Position;
                //br.BaseStream.Position = startPos;
                //Directory.CreateDirectory("extypetrees");
                //File.WriteAllBytes($"extypetrees/{type.GetFileName()}", br.ReadBytes((int)(endPos - startPos)));
                var depCount = br.ReadInt32();
                type.m_TypeDependencies = new int[depCount];
                for (int i = 0; i < depCount;i++)
                {
                    if (isRefType)
                    {
                        type.m_KlassName = Utils.ReadNullTerminatedString(br);
                        type.m_NameSpace = Utils.ReadNullTerminatedString(br);
                        type.m_AsmName = Utils.ReadNullTerminatedString(br);
                    }
                    else
                    {
                        type.m_TypeDependencies[i] = br.ReadInt32();
                    }
                }
            }
            return type;
        }
        public static TypeTreeNode ParseNodeTree(BinaryReader br)
        {
            var nodeCount = br.ReadInt32();
            var stringBufferSize = br.ReadInt32();
            int maxDepth = 0;
            List<TypeTreeNode> flatNodes = [];
            for (int i = 0; i < nodeCount; i++)
            {
                var typeTreeNode = new TypeTreeNode()
                {
                    m_Version = br.ReadUInt16(),
                    m_Level = br.ReadByte(),
                    m_TypeFlags = br.ReadByte(),
                    m_TypeStrOffset = br.ReadUInt32(),
                    m_NameStrOffset = br.ReadUInt32(),
                    m_ByteSize = br.ReadInt32(),
                    m_Index = br.ReadInt32(),
                    m_MetaFlag = br.ReadInt32(),
                    m_RefTypeHash = br.ReadUInt64()
                };
                if (typeTreeNode.m_Level > maxDepth) maxDepth = typeTreeNode.m_Level;
                flatNodes.Add(typeTreeNode);
            }
            TypeTreeNode root = flatNodes[0];
            var stringBuffer = br.ReadBytes(stringBufferSize);
            using var buffer = new BinaryReader(new MemoryStream(stringBuffer));
            for (int i = 0; i < nodeCount; i++)
            {
                var node = flatNodes[i];
                node.m_Type = ReadString(node.m_TypeStrOffset);
                node.m_Name = ReadString(node.m_NameStrOffset);
            }

            TypeTreeNode[] nodeStack = new TypeTreeNode[maxDepth + 1];
            int stackSize = 0;
            foreach (var typeTreeNode in flatNodes)
            {
                if (stackSize > typeTreeNode.m_Level)
                {
                    stackSize = typeTreeNode.m_Level;
                }
                if (stackSize > 0)
                {
                    nodeStack[stackSize - 1].AddNode(typeTreeNode);
                }
                nodeStack[stackSize] = typeTreeNode;
                stackSize++;
            }


            return root;

            string ReadString(uint value)
            {
                var isOffset = (value & 0x80000000) == 0;
                if (isOffset)
                {
                    buffer.BaseStream.Position = value;
                    return Utils.ReadNullTerminatedString(buffer);
                }
                var offset = value & 0x7FFFFFFF;
                if (strBuf.TryGetValue(offset, out var str))
                {
                    return str;
                }
                return offset.ToString();
            }
        }
        public Dictionary<string, object?>? InitObject(ObjectInfo objInfo)
        {
            _br!.BaseStream.Position = objInfo.byteStart;
            var serType = m_Types[objInfo.typeId];
            var dict = serType.DeserializeType(_br);
            dict["__pathId"] = objInfo.pathId;
            m_Objects[objInfo.pathId] = dict;
            if (--objRemain == 0)
            {
                _br.Dispose();
                _br = null;
            }
            return dict;
        }
        public void InitAllObjects()
        {
            foreach (var objInfo in m_ObjectInfos.Values)
            {
                if (m_Objects.ContainsKey(objInfo.pathId)) continue;
                InitObject(objInfo);
            }
        }
        public IEnumerable<KeyValuePair<long, ObjectInfo>> GetObjectInfos()
        {
            return m_ObjectInfos;
        }
        public string? GetFriendlyObjectTypeName(long pathId)
        {
            if (m_ObjectInfos.TryGetValue(pathId, out var objInfo))
            {
                var serType = m_Types[objInfo.typeId];
                var className = serType.ClassName;
                if (className == "MonoBehaviour")
                {
                    var obj = GetObject(pathId) as MonoBehaviour;
                    if (obj != null && obj.m_Script.TryGet(out var scriptData))
                    {
                        return scriptData.m_Name + " (MonoBehaviour)";
                    }

                }
                return className;
            }
            return null;
        }
        public string? GetObjectTypeName(long pathId)
        {
            if (m_ObjectInfos.TryGetValue(pathId, out var objInfo))
            {
                var serType = m_Types[objInfo.typeId];
                return serType.ClassName;
            }
            return null;
        }
        private string? GetObjectPropStr2(long pathId, string propName)
        {
            if (m_ObjectInfos.TryGetValue(pathId, out var objInfo))
            {
                var serType = m_Types[objInfo.typeId];
                if (m_Objects.TryGetValue(pathId, out var obj))
                {
                    var mName = (obj.GetValueOrDefault(propName) as byte[]);
                    if (mName != null)
                    {
                        var nameStr = Encoding.UTF8.GetString(mName);
                        if (!string.IsNullOrEmpty(nameStr)) return nameStr;
                    }
                }
                else
                {
                    var nodeIdx = serType.RootNode.GetNodeIndexByName(propName);
                    if (nodeIdx != -1)
                    {
                        _br!.BaseStream.Position = objInfo.byteStart;
                        serType.PassUntil(_br, nodeIdx);
                        var len = _br.ReadInt32();
                        if (len != 0) return Encoding.UTF8.GetString(_br.ReadBytes(len));
                    }
                }
                return serType.ClassName;
            }
            return null;
        }
        readonly Dictionary<KeyValuePair<long, string>, string?> m_CachedProps = [];
        public string? GetObjectPropStr(long pathId, string propName)
        {
            KeyValuePair<long, string> key = new(pathId, propName);
            if (!m_CachedProps.TryGetValue(key, out var objName))
            {
                objName = GetObjectPropStr2(pathId, propName);
                m_CachedProps[key] = objName;
            }
            return objName;
        }
        public string? GetObjectName(long pathId)
        {
            return GetObjectPropStr(pathId, "m_Name");
        }
        public Dictionary<string, object?>? GetObjectDict(long pathId)
        {
            if (m_Objects.TryGetValue(pathId, out var obj)) { return obj; }
            if (m_ObjectInfos.TryGetValue(pathId, out var objInfo))
            {
                return InitObject(objInfo);
            }
            return null;
        }
        public Object GetObject(long pathId)
        {
            var dict = GetObjectDict(pathId);
            return Object.Create(dict ?? throw new Exception($"Object with pathId {pathId} not found"), this);
        }
        public SerializedFile(byte[] _data)
        {
            var ms = new MemoryStream(_data);
            BinaryReader br = new BinaryReaderBE(ms);
            br.ReadUInt32();
            br.ReadUInt32();
            var version = br.ReadUInt32();
            br.ReadUInt32();
            var endianess = br.ReadUInt32();
            var _metaSize = br.ReadUInt32();
            var _fileSize = br.ReadUInt64();
            var dataOffset = br.ReadInt64();
            br.ReadUInt64();
            if (endianess == 0)
            {
                isLittleEndian = true;
                br = new BinaryReader(ms);
            }
            var _versionStr = Utils.ReadNullTerminatedString(br);
            unityVersion = new(_versionStr);
            var _target = br.ReadUInt32();
            var enableTypeTree = br.ReadBoolean();
            var typeCount = br.ReadInt32();
            m_Types = [];
            for (int i = 0; i < typeCount; i++)
            {
                m_Types.Add(ReadSerializedType(br, enableTypeTree));
            }

            int objectCount = br.ReadInt32();
            objRemain = objectCount;
            for (int i = 0; i < objectCount; i++)
            {
                Utils.AlignStream(ms, 4);
                var pathId = br.ReadInt64();
                long byteStart = dataOffset + br.ReadInt64();
                var byteSize = br.ReadInt32();
                var typeId = br.ReadInt32();
                ObjectInfo obj = new()
                {
                    pathId = pathId,
                    typeId = typeId,
                    byteStart = byteStart,
                    byteSize = byteSize
                };
                m_ObjectInfos[pathId] = obj;
            }

            // Basically list of PPtr<MonoScript>
            int scriptCount = br.ReadInt32();
            for (int i = 0; i < scriptCount; i++)
            {
                var _localSerializedFileIndex = br.ReadInt32();
                Utils.AlignStream(ms, 4);
                var _localIdent = br.ReadInt64();
            }
            // External files, used for PPtr resolution
            int externalsCount = br.ReadInt32();
            m_Externals = [];
            for (int i = 0; i < externalsCount; i++)
            {
                var _tempEmpty = Utils.ReadNullTerminatedString(br);
                var _guid = br.ReadBytes(16);
                var _type = br.ReadInt32();
                var _pathName = Utils.ReadNullTerminatedString(br);
                m_Externals.Add(new()
                {
                    guid = new Guid(_guid),
                    type = _type,
                    pathName = _pathName,
                });
            }
            int refTypeCount = br.ReadInt32();
            for (int i = 0; i < refTypeCount; i++)
            {
                m_Types.Add(ReadSerializedType(br, enableTypeTree, true));
            }
            _br = br;
        }
    }
}
