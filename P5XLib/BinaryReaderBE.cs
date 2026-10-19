using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{
    public class BinaryReaderBE(Stream stream) : BinaryReader(stream)
    {
        private readonly byte[] _buffer = new byte[8];
        public override short ReadInt16()
        {
            Read(_buffer, 0, 2);
            return BinaryPrimitives.ReadInt16BigEndian(_buffer);
        }
        public override int ReadInt32()
        {
            Read(_buffer, 0, 4);
            return BinaryPrimitives.ReadInt32BigEndian(_buffer);
        }
        public override long ReadInt64()
        {
            Read(_buffer, 0, 8);
            return BinaryPrimitives.ReadInt64BigEndian(_buffer);
        }
        public override ushort ReadUInt16()
        {
            Read(_buffer, 0, 2);
            return BinaryPrimitives.ReadUInt16BigEndian(_buffer);
        }
        public override uint ReadUInt32()
        {
            Read(_buffer, 0, 4);
            return BinaryPrimitives.ReadUInt32BigEndian(_buffer);
        }
        public override ulong ReadUInt64()
        {
            Read(_buffer, 0, 8);
            return BinaryPrimitives.ReadUInt64BigEndian(_buffer);
        }
    }
}
