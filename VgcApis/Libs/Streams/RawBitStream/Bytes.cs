namespace VgcApis.Libs.Streams.RawBitStream
{
    public class Bytes :
        Models.BaseClasses.ComponentOf<RawBitStream>
    {
        const int BitPerByte = Models.Consts.BitStream.BitsPerByte;
        const int MaxStringLenInBits = Models.Consts.BitStream.MaxStringLenInBits;

        Numbers numbers;
        public Bytes() { }

        public void Run(Numbers numbers)
        {
            this.numbers = numbers;
        }

        #region public methods
        public void Write(byte[] bytes, int len)
        {
            for (int i = 0; i < len; i++)
            {
                numbers.Write(bytes[i], BitPerByte);
            }
        }

        public void Write(byte[] bytes)
        {
            var cache = Utils.CutBytes(bytes);
            var len = cache.Length;
            numbers.Write(len, MaxStringLenInBits);
            Write(cache, len);
        }

        public byte[] Read(int len)
        {
            var cache = new byte[len];
            for (int i = 0; i < len; i++)
            {
                cache[i] = (byte)numbers.Read(BitPerByte);
            }
            return cache;
        }

        public byte[] Read()
        {
            var len = numbers.Read(MaxStringLenInBits);
            return Read(len);
        }
        #endregion

        #region private methods

        #endregion
    }
}
