namespace VgcApis.Models.Consts
{
    public static class BitStream
    {
        public const int BitsPerByte = 8;
        public const int BitsPerInt = 16;
        public const int BytesPerUuid = 16;
        public const int BitsPerUnicode = 16;

        public const int BytesPerIpv4 = 4;
        public const int BytesPerIpv6 = 16;

        public const int MaxStringLenInBits = 8;
        public const int MaxStringLen = 1 << (MaxStringLenInBits - 1);

        public const int InfoAreaLenInBytes = 3;
        public const int LastByteLenInBits = 3;
        public const int MajorVersionByteIndex = 1;
        public const int SubVersionByteIndex = 2;
        public const int Crc8ByteIndex = 0;
    }
}
