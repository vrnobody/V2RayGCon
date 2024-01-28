using System;
using System.Buffers;
using System.IO;

namespace VgcApis.Libs.Streams
{
    public class AsciiStringStream : Stream
    {
        readonly string str;

        readonly ArrayPool<char> pool = ArrayPool<char>.Shared;

        readonly int len;
        int pos = 0;
        bool canRead = true;

        public AsciiStringStream(String source)
        {
            this.str = source ?? "";
            len = this.str.Length;
        }

        public override bool CanRead => canRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => len;

        public override long Position
        {
            get => pos;
            set
            {
                if (value < 0 || value >= len)
                {
                    throw new ArgumentOutOfRangeException();
                }
                pos = (int)value;
            }
        }

        public override void Flush() { }

        public override int Read(byte[] dest, int offset, int count)
        {
            count = Math.Min(count, len - pos);
            if (!canRead || count < 1)
            {
                return 0;
            }
            var src = pool.Rent(count);
            str.CopyTo(pos, src, 0, count);
            for (int i = 0; i < count; i++)
            {
                dest[offset + i] = Buffer.GetByte(src, i * 2);
            }
            pos += count;
            pool.Return(src);
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
