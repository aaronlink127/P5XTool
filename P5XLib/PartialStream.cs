using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{
    class PartialStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _begin;
        private readonly long _length;
        public PartialStream(Stream s)
        {
            _baseStream = s;
            _begin = s.Position;
            _length = s.Length - s.Position;
        }
        public PartialStream(Stream s, long begin, long len)
        {
            _baseStream = s;
            _begin = begin;
            _length = len;
            _baseStream.Position = begin;
        }
        public override long Position
        {
            get => _baseStream.Position - _begin;
            set
            {
                _baseStream.Position = _begin + value;
            }
        }
        public override long Length
        {
            get => _length;
        }
        public override bool CanWrite => false;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanRead { get
            {
                var bytesLeft = _length - Position;
                if (bytesLeft < 0) return false;
                return _baseStream.CanRead;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return Position = offset;
                case SeekOrigin.Current:
                    return Position += offset;
                case SeekOrigin.End:
                    return Position = _length + offset;
            }
            return Position;
        }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesLeft = _length - Position;
            if (bytesLeft < 0)
            {
                throw new Exception("Attempted to read beyond the end of the stream.");
            }
            if (bytesLeft < count)
            {
                count = (int)bytesLeft;
            }
            return _baseStream.Read(buffer, offset, count);
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _baseStream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
