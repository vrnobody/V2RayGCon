using System;

namespace VgcApis.Libs.Streams.RawBitStream
{
    public sealed class Numbers :
        Models.BaseClasses.ComponentOf<RawBitStream>
    {

        const int BitsPerPort = Models.Consts.BitStream.BitsPerInt;
        public Numbers() { }

        #region public methods
        public void WritePortNum(int val) =>
            Write(val, BitsPerPort);

        public int ReadPortNum() =>
            Read(BitsPerPort);

        public void Write(int val, int len)
        {
            if (val < 0 || val > 65535)
            {
                throw new ArgumentOutOfRangeException("val must between 0 and 65535");
            }

            CheckLen(len);
            var cache = Utils.Int2BoolList(val, len);
            GetContainer().Write(cache);
        }

        public int Read(int len)
        {
            CheckLen(len);
            var cache = GetContainer().Read(len);
            if (cache.Count != len)
            {
                throw new ArgumentNullException("Read overflow!");
            }
            var result = Utils.BoolList2Int(cache);
            return result;
        }
        #endregion

        #region private methods
        void CheckLen(int len)
        {
            if (len < 1 || len > 16)
            {
                throw new ArgumentOutOfRangeException("len must between 1 and 16");
            }
        }
        #endregion
    }
}
