using System;
using System.Collections.Generic;

namespace V2RayGCon.Model.VeeShareLinks
{
    public static class Utils
    {
        public static Func<string> GenReadStringHelper(
            VgcApis.Libs.Streams.BitStream bitStream,
            List<string> strTable)
        {
            var bs = bitStream;
            var table = strTable;
            return () => ReadString(bs, table);
        }

        public static string ReadString(
            VgcApis.Libs.Streams.BitStream bitStream,
            List<string> strTable)
        {
            var lenInBits = VgcApis.Libs.Utils.GetLenInBitsOfInt(strTable.Count);

            if (bitStream.Read<bool>())
            {
                // using string table
                var index = bitStream.ReadTinyInt(lenInBits);
                return strTable[index];
            }
            else
            {
                return bitStream.Read<string>();
            }
        }

        public static Action<string> GenWriteStringHelper(
            VgcApis.Libs.Streams.BitStream bitStream,
            List<string> strTable)
        {
            var bs = bitStream;
            var table = strTable;
            return str => WriteString(bs, table, str);
        }

        public static void WriteString(
            VgcApis.Libs.Streams.BitStream bitStream,
            List<string> strTable,
            string str)
        {
            var lenInBits = VgcApis.Libs.Utils.GetLenInBitsOfInt(strTable.Count);

            var index = strTable.IndexOf(str);
            if (index == -1)
            {
                bitStream.Write(false);
                bitStream.Write(str);
            }
            else
            {
                bitStream.Write(true);
                bitStream.WriteTinyInt(index, lenInBits);
            }
        }
    }
}
