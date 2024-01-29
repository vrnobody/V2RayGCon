using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VgcApis.Libs.Streams
{
    public class ReadonlyStringStream : Stream
    {
        readonly string str;

        readonly ArrayPool<char> pool = ArrayPool<char>.Shared;

        // int ReadUnicode(byte[] dest, int offset, int count)
        readonly bool isAscii;

        readonly int len;
        int pos = 0;
        bool canRead = true;
        char[] buff;

        public ReadonlyStringStream(string source, Encoding encoding)
        {
            this.str = source ?? "";
            this.buff = pool.Rent(4 * 1024);

            if (encoding == Encoding.ASCII)
            {
                len = this.str.Length;
                isAscii = true;
            }
            else if (encoding == Encoding.Unicode)
            {
                // stream length is measured by bytes
                len = this.str.Length * 2;
            }
            else
            {
                throw new ArgumentException("not supported encoding");
            }
        }

        #region properties
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
        #endregion

        #region public methods

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] dest, int offset, int count)
        {
            return isAscii ? ReadAscii(dest, offset, count) : ReadUnicode(dest, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region private methods
        int ReadAscii(byte[] dest, int offset, int count)
        {
            count = Math.Min(count, len - pos);
            if (!canRead || count < 1)
            {
                return 0;
            }

            if (buff.Length < count)
            {
                pool.Return(this.buff);
                this.buff = pool.Rent(count);
            }

            str.CopyTo(pos, buff, 0, count);
            for (int i = 0; i < count; i++)
            {
                dest[offset + i] = Buffer.GetByte(buff, i * 2);
            }
            pos += count;
            return count;
        }

        int ReadUnicode(byte[] dest, int offset, int count)
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
            var minBuffSize = count / 2 + 1;
            if (this.buff.Length < minBuffSize)
            {
                pool.Return(this.buff);
                this.buff = pool.Rent(minBuffSize);
            }
            str.CopyTo(cstart, buff, 0, cnum);
            Buffer.BlockCopy(buff, skip, dest, offset, bnum);
            pos += bnum;
            return bnum;
        }
        #endregion


        #region protected methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            pool.Return(this.buff);
        }
        #endregion
    }
}
