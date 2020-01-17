namespace VgcApis.Libs.Streams
{

    // http://sanity-free.org/146/crc8_implementation_in_csharp.html

    public static class Crc8
    {
        static byte[] table = new byte[256];
        // x8 + x7 + x6 + x4 + x2 + 1
        const byte poly = 0xd5;

        public static byte ComputeChecksum(byte[] bytes) =>
            ComputeChecksum(bytes, 0);

        public static byte ComputeChecksum(byte[] bytes, int start)
        {
            byte crc = 0;
            var len = bytes.Length;
            if (bytes != null && start < len && start >= 0)
            {
                for (int i = start; i < len; i++)
                {
                    var b = bytes[i];
                    crc = table[crc ^ b];
                }
            }
            return crc;
        }

        static Crc8()
        {
            for (int i = 0; i < 256; ++i)
            {
                int temp = i;
                for (int j = 0; j < 8; ++j)
                {
                    if ((temp & 0x80) != 0)
                    {
                        temp = (temp << 1) ^ poly;
                    }
                    else
                    {
                        temp <<= 1;
                    }
                }
                table[i] = (byte)temp;
            }
        }
    }
}
