using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    internal class Utils
    {
        public static string ReadNullTerminatedString(BinaryReader br, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            List<byte> bytes = [];
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                bytes.Add(b);
            }
            return encoding.GetString(bytes.ToArray());
        }
        public static void AlignStream(Stream stream, int alignment)
        {
            long pos = stream.Position;
            long alignedPos = (long)Math.Ceiling((double)pos / alignment) * alignment;
            stream.Seek(alignedPos, SeekOrigin.Begin);
        }
    }
}
