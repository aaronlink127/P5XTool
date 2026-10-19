using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Zeus.Core.FileSystem;
using Google.FlatBuffers;

namespace P5XLib
{
    public interface IFSFile
    {
        public byte[] ReadAllBytes();
        public Stream Open();
        public long GetSize();
        public string GetRealPath();
    }
    internal class VFile(string filePath, VFSMount srcMount, string contentFile, uint fileSize, uint fileOffset) : IFSFile
    {
        private string getVFilePath()
        {
            return Path.Join(srcMount.root, contentFile);
        }
        public byte[] ReadAllBytes()
        {
            using FileStream fb = new(getVFilePath(), FileMode.Open, FileAccess.Read, FileShare.Read);
            fb.Position = fileOffset;
            byte[] bytes = new byte[fileSize];
            fb.ReadExactly(bytes, 0, (int)fileSize);
            return bytes;
        }
        public Stream Open()
        {
            return new PartialStream(new FileStream(getVFilePath(), FileMode.Open, FileAccess.Read, FileShare.Read), fileOffset, fileSize);
            //return new MemoryStream(ReadAllBytes());
        }
        public long GetSize()
        {
            return fileSize;
        }
        public string GetRealPath() {
            var tmpFilePath = VFileSystem.GetTempPath(filePath);
            byte[] bytes = ReadAllBytes();
            File.WriteAllBytes(tmpFilePath, bytes);
            return tmpFilePath;
        }
    }
    internal class RealFile(string path) : IFSFile
    {
        public byte[] ReadAllBytes()
        {
            return File.ReadAllBytes(path);
        }
        public Stream Open()
        {
            //return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;
            return ms;
        }
        public long GetSize()
        {
            FileInfo fileInfo = new(path);

            return fileInfo.Length;
        }
        public string GetRealPath() {
            return path;
        }
    }
    internal interface IFSMount
    {
        public IFSFile? GetFile(string path);
        public bool Exists(string path);
        public void GetDirectFilesInPath(string path, HashSet<string> fileSet);
        public void GetFiles(HashSet<string> fileSet);
    }
    internal class VFSMount : IFSMount
    {
        public readonly string root;
        Dictionary<string, IFSFile> pathFiles = [];
        public VFSMount(string path)
        {
            root = path;
            var data = File.ReadAllBytes(Path.Join(path, "_vfileIndexV2.fb"));
            var bb = new ByteBuffer(data);
            var vfileIndex = VFileContentFB.GetRootAsVFileContentFB(bb);
            for (int i = 0; i < vfileIndex.EntrysLength; i++)
            {
                var entry = vfileIndex.Entrys(i)!.Value;
                var contentFile = entry.ContentFile;
                for (int j = 0; j < entry.EntrysLength; j++)
                {
                    var fileEntry = entry.Entrys(j)!.Value;
                    string filePath = fileEntry.Path!;
                    ulong offset = fileEntry.Offset;
                    uint length = fileEntry.Length;
                    var vfile = new VFile(filePath, this, contentFile, length, (uint)offset);
                    pathFiles.Add(filePath, vfile);
                }
            }
        }
        public bool Exists(string path)
        {
            return pathFiles.ContainsKey(path);
        }
        public IFSFile? GetFile(string path)
        {
            if (pathFiles.TryGetValue(path, out IFSFile? value))
            {
                return value;
            }
            return null;
        }
        public void GetDirectFilesInPath(string path, HashSet<string> fileSet)
        {
            string parent = path.TrimEnd('/');
            foreach (var kv in pathFiles)
            {
                var childDir = Path.GetDirectoryName(kv.Key)!.Replace('\\', '/');
                if (childDir == parent)
                {
                    fileSet.Add(kv.Key);
                }
            }
        }
        public void GetFiles(HashSet<string> fileSet)
        {
            foreach (var kv in pathFiles)
            {
                fileSet.Add(kv.Key);
            }
        }
    }
    internal class RealFSMount(string root) : IFSMount
    {
        public IFSFile? GetFile(string path)
        {
            string realPath = Path.Join(root, path);
            if (File.Exists(realPath))
            {
                return new RealFile(realPath);
            }
            return null;
        }
        public bool Exists(string path)
        {
            string realPath = Path.Join(root, path);
            return File.Exists(realPath);
        }
        private string AbsoluteToRelative(string absolutePath)
        {
            return absolutePath[(root.Length+1)..].Replace("\\", "/");
        }


        public void GetDirectFilesInPath(string path, HashSet<string> fileSet)
        {
            string realPath = Path.Join(root, path);
            if (Directory.Exists(realPath))
            {
                foreach (string file in Directory.GetFiles(realPath))
                {
                    fileSet.Add(AbsoluteToRelative(file));
                }
            }
        }
        public void GetFiles(HashSet<string> fileSet)
        {
            foreach (string file in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            {
                fileSet.Add(AbsoluteToRelative(file));
            }
        }
    }
    public class VFileSystem
    {
        static public bool IsInit { get; private set; }
        static readonly string tmpRoot = Path.Join(Path.GetTempPath(), "P5XLib");
        static readonly string tmpPath = Path.Join(tmpRoot, Environment.ProcessId.ToString());
        static readonly List<IFSMount> mounts = [];
        public static bool TryInit(string clientDir)
        {
            if (IsInit)
            {
                Reset();
            }
            ResetTemp();
            string binDir = Path.Join(clientDir, "bin");
            // Paths should be mounted highest->lowest priority
            mounts.Add(new RealFSMount(Path.Join(clientDir, "OuterPackage")));
            if (File.Exists(Path.Join(binDir, "_vfileIndexV2.fb")))
            {
                mounts.Add(new VFSMount(binDir));
            }
            mounts.Add(new RealFSMount(Path.Join(clientDir, "bin")));
            IsInit = true;
            return true;
        }
        static IFSFile? GetFile(string path)
        {
            foreach (IFSMount mount in mounts)
            {
                IFSFile? file = mount.GetFile(path);
                if (file != null)
                {
                    return file;
                }
            }
            return null;
        }
        public static bool Exists(string path)
        {
            foreach (IFSMount mount in mounts)
            {
                if (mount.Exists(path))
                {
                    return true;
                }
            }
            return false;
        }
        public static byte[] ReadAllBytes(string path)
        {
            IFSFile file = GetFile(path) ?? throw new FileNotFoundException($"VFS File not found: '{path}'.");
            return file.ReadAllBytes();
        }
        public static long GetFileSize(string path)
        {
            IFSFile file = GetFile(path) ?? throw new FileNotFoundException($"VFS File not found: '{path}'.");
            return file.GetSize();
        }
        public static Stream Open(string path)
        {
            IFSFile file = GetFile(path) ?? throw new FileNotFoundException($"VFS File not found: '{path}'.");
            return file.Open();
        }
        public static string GetTempPath(string path)
        {
            Directory.CreateDirectory(tmpPath);
            return Path.Join(tmpPath, path.GetHashCode().ToString("X8"));
        }
        public static string? GetRealPath(string path)
        {
            var file = GetFile(path);
            if (file != null) {
                return file.GetRealPath();
            }
            return null;
        }
        public static void ResetTemp()
        {
            if (Directory.Exists(tmpPath))
            {
                Directory.Delete(tmpPath, true);
            }
            if (Directory.Exists(tmpRoot))
            {
                try
                {
                    Directory.Delete(tmpRoot);
                }
                catch { }
            }
        }
        public static void Reset()
        {
            IsInit = false;
            mounts.Clear();
            ResetTemp();
        }
        public static List<string> GetDirectFilesInPath(string path)
        {
            HashSet<string> fileSet = [];
            foreach (IFSMount mount in mounts)
            {
                mount.GetDirectFilesInPath(path, fileSet);
            }
            return [.. fileSet];
        }
        public static List<string> GetFiles()
        {
            HashSet<string> fileSet = [];
            foreach (IFSMount mount in mounts)
            {
                mount.GetFiles(fileSet);
            }
            return [.. fileSet];
        }
    }
}
