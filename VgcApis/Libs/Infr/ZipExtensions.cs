using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace VgcApis.Libs.Infr
{
    public static class ZipExtensions
    {
        #region ZSTD serialize API
        public static string SerializeObjectToZstdBase64(object value)
        {
            if (value == null)
            {
                return "";
            }
            using (var dest = new Streams.ArrayPoolMemoryStream(Encoding.ASCII))
            {
                using (
                    var b64 = new CryptoStream(
                        dest,
                        new ToBase64Transform(),
                        CryptoStreamMode.Write
                    )
                )
                using (var zstd = new ZstdNet.CompressionStream(b64))
                using (var writer = new StreamWriter(zstd, Encoding.Unicode))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, value);
                }
                var s = dest.GetString();
                return s;
            }
        }

        public static T DeserializeObjectFromZstdBase64<T>(string b64Str)
            where T : class
        {
            if (string.IsNullOrEmpty(b64Str))
            {
                return default;
            }

            using (var src = new Streams.ReadonlyStringStream(b64Str, Encoding.ASCII))
            using (
                var b64 = new CryptoStream(src, new FromBase64Transform(), CryptoStreamMode.Read)
            )
            using (var zstd = new ZstdNet.DecompressionStream(b64))
            using (var reader = new StreamReader(zstd, Encoding.Unicode))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }

        public static void SerializeObjectAsZstdBase64ToStream(Stream s, object o)
        {
            if (o == null)
            {
                return;
            }
            using (var wrapper = new Streams.KeepStreamOpenWrapper(s))
            using (
                var b64 = new CryptoStream(wrapper, new ToBase64Transform(), CryptoStreamMode.Write)
            )
            using (var zstd = new ZstdNet.CompressionStream(b64))
            using (var w = new StreamWriter(zstd, Encoding.Unicode))
            using (var jw = new JsonTextWriter(w))
            {
                var ser = new JsonSerializer();
                ser.Serialize(jw, o);
            }
        }
        #endregion

        #region GZip serialize API
        public static void SerializeObjectAsCompressedUnicodeBase64ToStream(Stream s, object o)
        {
            if (o == null)
            {
                return;
            }
            using (var wrapper = new Streams.KeepStreamOpenWrapper(s))
            using (
                var b64 = new CryptoStream(wrapper, new ToBase64Transform(), CryptoStreamMode.Write)
            )
            using (var gzip = new GZipStream(b64, CompressionMode.Compress))
            using (var w = new StreamWriter(gzip, Encoding.Unicode))
            using (var jw = new JsonTextWriter(w))
            {
                var ser = new JsonSerializer();
                ser.Serialize(jw, o);
            }
        }

        public static string SerializeObjectToCompressedUnicodeBase64(object value)
        {
            if (value == null)
            {
                return "";
            }
            using (var dest = new Streams.ArrayPoolMemoryStream(Encoding.ASCII))
            {
                using (
                    var b64 = new CryptoStream(
                        dest,
                        new ToBase64Transform(),
                        CryptoStreamMode.Write
                    )
                )
                using (var gZipStream = new GZipStream(b64, CompressionMode.Compress))
                using (var writer = new StreamWriter(gZipStream, Encoding.Unicode))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, value);
                }
                var s = dest.GetString();
                return s;
            }
        }

        public static T DeserializeObjectFromCompressedUnicodeBase64<T>(string b64Str)
            where T : class
        {
            if (string.IsNullOrEmpty(b64Str))
            {
                return default;
            }

            using (var src = new Streams.ReadonlyStringStream(b64Str, Encoding.ASCII))
            using (
                var b64 = new CryptoStream(src, new FromBase64Transform(), CryptoStreamMode.Read)
            )
            using (var gzip = new GZipStream(b64, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzip, Encoding.Unicode))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }

        #endregion

        #region GZip string API
        static readonly string gZipMagicBytes = @"H4sIAAAAAAA";

        public static bool IsCompressedBase64(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= gZipMagicBytes.Length)
            {
                return false;
            }
            return str.StartsWith(gZipMagicBytes);
        }

        public static string CompressToBase64(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }
            using (var dest = new Streams.ArrayPoolMemoryStream(Encoding.ASCII))
            {
                using (
                    var b64 = new CryptoStream(
                        dest,
                        new ToBase64Transform(),
                        CryptoStreamMode.Write
                    )
                )
                using (var gzip = new GZipStream(b64, CompressionMode.Compress))
                using (var w = new StreamWriter(gzip, Encoding.Unicode))
                {
                    w.Write(data);
                }
                return dest.GetString();
            }
        }

        public static string DecompressFromBase64(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }

            using (var src = new Streams.ReadonlyStringStream(data, Encoding.ASCII))
            using (
                var b64 = new CryptoStream(src, new FromBase64Transform(), CryptoStreamMode.Read)
            )
            using (var gzip = new GZipStream(b64, CompressionMode.Decompress))
            using (var r = new StreamReader(gzip, Encoding.Unicode))
            {
                var s = r.ReadToEnd();
                return s;
            }
        }

        #endregion

        #region ZSTD string API
        static readonly string zstdMagicBytes = @"KLUv/Q";

        public static bool IsZstdBase64(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= zstdMagicBytes.Length)
            {
                return false;
            }
            return str.StartsWith(zstdMagicBytes);
        }

        public static string ZstdToBase64(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }
            using (var dest = new Streams.ArrayPoolMemoryStream(Encoding.ASCII))
            {
                using (
                    var b64 = new CryptoStream(
                        dest,
                        new ToBase64Transform(),
                        CryptoStreamMode.Write
                    )
                )
                using (var zstd = new ZstdNet.CompressionStream(b64))
                using (var w = new StreamWriter(zstd, Encoding.Unicode))
                {
                    w.Write(data);
                }
                return dest.GetString();
            }
        }

        public static string ZstdFromBase64(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }

            using (var src = new Streams.ReadonlyStringStream(data, Encoding.ASCII))
            using (
                var b64 = new CryptoStream(src, new FromBase64Transform(), CryptoStreamMode.Read)
            )
            using (var zstd = new ZstdNet.DecompressionStream(b64))
            using (var r = new StreamReader(zstd, Encoding.Unicode))
            {
                var s = r.ReadToEnd();
                return s;
            }
        }
        #endregion
    }
}
