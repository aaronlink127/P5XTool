using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class PPtr : Object
    {
        public int m_FileID;
        public long m_PathID;

        public SerializedFile File { get
            {
                if (m_FileID == 0) return srcFile;
                var depName = srcFile.m_Externals[m_FileID - 1].pathName;
                return UnityAssetManager.GetLoadedFile(depName)!;
            }
        }
        public int GetObjectHashCode()
        {
            var pathName = File.path;
            return HashCode.Combine(pathName, m_PathID);
        }
        public override void LoadDataFrom(Dictionary<string, object?> data, SerializedFile sourceSerializedFile)
        {
            base.LoadDataFrom(data, sourceSerializedFile);
        }
        public bool IsNull() { return m_PathID == 0; }
        public bool TryGet([NotNullWhen(true)] out Object? result)
        {
            if (IsNull())
            {
                result = null;
                return false;
            }
            result = File.GetObject(m_PathID);
            return true;
        }
    }
    public class PPtr<T> : PPtr where T : Object
    {
        public bool TryGet([NotNullWhen(true)] out T? result)
        {
            result = null;
            if (!TryGet(out Object? obj)) return false;
            if (obj is T objT)
            {
                result = objT;
                return true;
            }
            return false;
        }
    }
}
