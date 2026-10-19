using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{
    public static class LuaFileUtils
    {
        public class FileMergeInfo
        {
            public long OriginalName;
            public string MergedFileName = "";
            public long StartPosition;
            public long Length;
        }
        public const string luafile_ext = ".bytes";
        public static Dictionary<long, FileMergeInfo> combineFiles = [];
        public static void Init()
        {
            if (!VFileSystem.Exists("Lua/Windows/Lua_ifiles.bytes")) return;
            byte[] ifiles = VFileSystem.ReadAllBytes("Lua/Windows/Lua_ifiles.bytes");
            using var ms = new MemoryStream(ifiles);
            using var br = new BinaryReader(ms);
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                byte[] _param1 = br.ReadBytes(br.ReadInt32());
                long originalName = br.ReadInt64();
                string name = Encoding.UTF8.GetString(br.ReadBytes(br.ReadInt32()));
                long startPosition = br.ReadInt64();
                int _param2 = br.ReadInt32();
                long length = br.ReadInt64();
                var mergeInfo = new FileMergeInfo { OriginalName = originalName, MergedFileName = name, StartPosition = startPosition, Length = length };
                combineFiles.Add(originalName, mergeInfo);
            }
        }
        public static void Reset()
        {
            combineFiles = [];
        }
        public static long GetHashLong(string fileName)
        {
            byte[] data = SHA256.HashData(Encoding.UTF8.GetBytes(fileName));
            return BitConverter.ToInt64(data, 0);
        }
        public static bool IsFileExist(string fileName)
        {
            if (VFileSystem.Exists("Lua/Windows/" + fileName + luafile_ext))
            {
                return true;
            }
            var hash = GetHashLong(fileName + luafile_ext);
            return combineFiles.ContainsKey(hash);
        }
        public static byte[] ReadLuaFromCombine(FileMergeInfo mergeInfo)
        {
            using var f = VFileSystem.Open("Lua/Windows/" + mergeInfo.MergedFileName + luafile_ext);
            using var br = new BinaryReader(f);
            f.Seek(mergeInfo.StartPosition, SeekOrigin.Begin);
            int len = br.ReadInt32();
            return br.ReadBytes(len);
        }
        public static byte[] ReadFile(string fileName)
        {
            if (GameMgr.IsCbt)
            {
                return AssetManager.LoadBytes(fileName + luafile_ext)!;
            }
            var hash = GetHashLong(fileName + luafile_ext);
            if (combineFiles.TryGetValue(hash, out var fileMergeInfo))
            {
                return ReadLuaFromCombine(fileMergeInfo);
            }
            var filePath = "Lua/Windows/" + fileName + luafile_ext;
            return VFileSystem.ReadAllBytes(filePath);
        }
    }
}
