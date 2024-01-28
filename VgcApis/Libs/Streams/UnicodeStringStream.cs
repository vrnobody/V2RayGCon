using System;
using System.Buffers;
using System.IO;

namespace VgcApis.Libs.Streams
{
    public class UnicodeStringStream : Stream
    {
        readonly string str;

        readonly ArrayPool<char> pool = ArrayPool<char>.Shared;

        readonly int len;
        int pos = 0;
        bool canRead = true;

        public UnicodeStringStream(String source)
        {
            this.str = source ?? "";
            len = this.str.Length * 2;
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
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!canRead)
            {
                return 0;
            }

            // pos = 1, c = 2
            // [0,1,2,3,4,5]
            var skip = pos % 2; // 1
            var cstart = (pos - skip) / 2; // 0
            var bend = Math.Min(pos + count, len); // 3
            var cend = ((bend % 2) + bend) / 2; // 2
            var cnum = cend - cstart; // 2
            var bnum = bend - pos; // 2

            if (cnum < 1 || bnum < 1)
            {
                return 0;
            }

            var src = pool.Rent(count / 2 + 1);
            str.CopyTo(cstart, src, 0, cnum);
            Buffer.BlockCopy(src, skip, dest, offset, bnum);
            pos += bnum;
            pool.Return(src);
            return bnum;
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
