using Newtonsoft.Json;
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

        public static string SerializeObjectToCompressedBase64(object value)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    using (StreamWriter writer = new StreamWriter(gZipStream))
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        JsonSerializer ser = new JsonSerializer();
                        ser.Serialize(jsonWriter, value);
                    }
                }
                var o = outputStream.ToArray();
                return Convert.ToBase64String(o);
            }
        }

        public static T DeserializeObjectFromCompressedBase64<T>(string b64Str)
        {
            var b64Bytes = Convert.FromBase64String(b64Str);
            using (var sourceStream = new MemoryStream(b64Bytes))
            using (var gZipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            using (StreamReader reader = new StreamReader(gZipStream))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }

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
