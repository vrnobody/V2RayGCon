using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VgcApis.Libs.Infr
{
    public static class ZipExtensions
    {

        public static string CompressToBase64(string data)
        {
            var b = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(Compress(b));
        }

        public static string DecompressFromBase64(string data)
        {
            var b64 = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(Decompress(b64));
        }

        public static byte[] Compress(byte[] data)
        {
            using (var sourceStream = new MemoryStream(data))
            using (var destinationStream = new MemoryStream())
            {
                CompressTo(sourceStream, destinationStream);
                return destinationStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var sourceStream = new MemoryStream(data))
            using (var destinationStream = new MemoryStream())
            {
                DecompressTo(sourceStream, destinationStream);
                return destinationStream.ToArray();
            }
        }

        public static void CompressTo(Stream inputStream, Stream outputStream)
        {
            using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(gZipStream);
                gZipStream.Flush();
            }
        }

        public static void DecompressTo(Stream inputStream, Stream outputStream)
        {
            using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                gZipStream.CopyTo(outputStream);
            }
        }
    }
}
