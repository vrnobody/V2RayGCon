using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace VgcApis.Libs.Infr
{
    public static class ZipExtensions
    {
        #region public
        public static void SerializeObjectAsCompressedUnicodeBase64StringToFile(
            string path,
            object value
        )
        {
            using (var file = File.Open(path, FileMode.Append))
            {
                using (
                    CryptoStream base64Stream = new CryptoStream(
                        file,
                        new ToBase64Transform(),
                        CryptoStreamMode.Write
                    )
                )
                using (var gZipStream = new GZipStream(base64Stream, CompressionMode.Compress))
                using (StreamWriter writer = new StreamWriter(gZipStream, Encoding.Unicode))
                using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                {
                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, value);
                }
            }
        }

        public static string SerializeObjectToCompressedUnicodeBase64(object value)
        {
            var dest = new MemoryStream();
            using (
                var b64 = new CryptoStream(dest, new ToBase64Transform(), CryptoStreamMode.Write)
            )
            using (var gZipStream = new GZipStream(b64, CompressionMode.Compress))
            using (var writer = new StreamWriter(gZipStream, Encoding.Unicode))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, value);
            }
            var s = Encoding.ASCII.GetString(dest.ToArray());
            dest.Dispose();
            return s;
        }

        public static T DeserializeObjectFromCompressedUnicodeBase64<T>(string b64Str)
        {
            using (var src = new MemoryStream())
            using (var w = new StreamWriter(src, Encoding.ASCII))
            using (
                var b64 = new CryptoStream(src, new FromBase64Transform(), CryptoStreamMode.Read)
            )
            using (var gzip = new GZipStream(b64, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzip, Encoding.Unicode))
            using (var jsonReader = new JsonTextReader(reader))
            {
                w.Write(b64Str);
                w.Flush();
                src.Position = 0;

                var ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }

#pragma warning disable CA1802 // Use literals where appropriate
        static readonly string marker = @"H4sIAAAAAAA";
#pragma warning restore CA1802 // Use literals where appropriate

        public static bool IsCompressedBase64(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= marker.Length)
            {
                return false;
            }
            return str.StartsWith(marker);
        }

        public static string CompressToBase64(string data)
        {
            var dest = new MemoryStream();
            using (
                var b64 = new CryptoStream(dest, new ToBase64Transform(), CryptoStreamMode.Write)
            )
            using (var gzip = new GZipStream(b64, CompressionMode.Compress))
            using (var w = new StreamWriter(gzip, Encoding.Unicode))
            {
                w.Write(data);
            }
            var s = Encoding.ASCII.GetString(dest.ToArray());
            dest.Dispose();
            return s;
        }

        public static string DecompressFromBase64(string data)
        {
            using (var src = new MemoryStream())
            using (var w = new StreamWriter(src, Encoding.ASCII))
            using (
                var b64 = new CryptoStream(src, new FromBase64Transform(), CryptoStreamMode.Read)
            )
            using (var gzip = new GZipStream(b64, CompressionMode.Decompress))
            using (var r = new StreamReader(gzip, Encoding.Unicode))
            {
                w.Write(data);
                w.Flush();
                src.Position = 0;

                var s = r.ReadToEnd();
                return s;
            }
        }
        #endregion
    }
}
