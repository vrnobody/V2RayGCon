using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VgcApis.Interfaces;
using ZstdNet;

namespace VgcApis.Libs.Infr
{
    public static class ZipExtensions
    {
        #region ZSTD with dictionary
        static readonly int ZSTD_DICT_COMPRESS_LEVEL = Models.Consts.Libs.DefCompressConfigLevel;

        public static readonly string ZSTD_DICT_TAG_CORE_INFO_V1 = "v1";
        public static readonly string ZSTD_DICT_TAG_V2CFG_V3 = "v2cfg_v3";

        static readonly Dictionary<string, byte[]> zstd_dicts = new Dictionary<string, byte[]>()
        {
            { ZSTD_DICT_TAG_CORE_INFO_V1, Properties.Resources.zstd_dict_ver1 },
            { ZSTD_DICT_TAG_V2CFG_V3, Properties.Resources.zstd_dict_v2cfg_ver3 },
        };

        public static byte[] ZstdDictGet(string tag) => zstd_dicts[tag];

        public static string ZstdToBase64WithDictFile(string dictFile, string data)
        {
            var dict = File.ReadAllBytes(dictFile);
            return ZstdDictToBase64Core(dict, data);
        }

        public static string ZstdFromBase64WithDictFile(string dictFile, string b64)
        {
            var dict = File.ReadAllBytes(dictFile);
            return ZstdDictFromBase64Core(dict, b64);
        }

        public static int ZstdBuildDictFile(
            IEnumerable<byte[]> samples,
            string outputFile,
            int dictCapacity
        )
        {
            byte[] bytes;
            if (dictCapacity > 0)
            {
                bytes = DictBuilder.TrainFromBuffer(samples, dictCapacity);
            }
            else
            {
                bytes = DictBuilder.TrainFromBuffer(samples);
            }
            File.WriteAllBytes(outputFile, bytes);
            return bytes.Length;
        }

        public static string ZstdToBase64WithDictTag(string tag, string data)
        {
            var dict = ZstdDictGet(tag);
            return ZstdDictToBase64Core(dict, data);
        }

        public static string ZstdDictToBase64Core(byte[] dict, string data)
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
                using (var option = new CompressionOptions(dict, ZSTD_DICT_COMPRESS_LEVEL))
                using (var zstd = new CompressionStream(b64, option, 4 * 1024))
                using (var writer = new StreamWriter(zstd, Encoding.Unicode))
                {
                    writer.Write(data);
                }
                var s = dest.GetString();
                return s;
            }
        }

        public static byte[] ZstdDictToBytes(string tag, string data)
        {
            var dict = ZstdDictGet(tag);
            return ZstdDictToBytes(dict, data);
        }

        public static byte[] ZstdDictToBytes(byte[] dict, string data)
        {
            var empty = new byte[0];
            if (string.IsNullOrEmpty(data))
            {
                return empty;
            }

            using (var dest = new Streams.ArrayPoolMemoryStream(Encoding.ASCII))
            {
                using (var option = new CompressionOptions(dict, ZSTD_DICT_COMPRESS_LEVEL))
                using (var zstd = new CompressionStream(dest, option, 4 * 1024))
                using (var writer = new StreamWriter(zstd, Encoding.Unicode))
                {
                    writer.Write(data);
                }
                var r = dest.ToArray();
                return r;
            }
        }

        public static string ZstdFromBase64WithDictTag(string tag, string data)
        {
            var dict = ZstdDictGet(tag);
            return ZstdDictFromBase64Core(dict, data);
        }

        public static string ZstdDictFromBase64Core(byte[] dict, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }

            using (var src = new Streams.ReadonlyStringStream(data, Encoding.ASCII))
            using (
                var b64 = new CryptoStream(src, new FromBase64Transform(), CryptoStreamMode.Read)
            )
            using (var option = new DecompressionOptions(dict))
            using (var zstd = new DecompressionStream(b64, option, bufferSize: 4 * 1024))
            using (var r = new StreamReader(zstd, Encoding.Unicode))
            {
                var s = r.ReadToEnd();
                return s;
            }
        }

        public static string ZstdDictFromBytes(string tag, byte[] data)
        {
            var dict = ZstdDictGet(tag);
            return ZstdDictFromBytes(dict, data);
        }

        public static string ZstdDictFromBytes(byte[] dict, byte[] data)
        {
            if (data.Length < 1)
            {
                return "";
            }

            using (var src = new MemoryStream(data))
            using (var option = new DecompressionOptions(dict))
            using (var zstd = new DecompressionStream(src, option, bufferSize: 4 * 1024))
            using (var r = new StreamReader(zstd, Encoding.Unicode))
            {
                var s = r.ReadToEnd();
                return s;
            }
        }

        #endregion

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
                using (var option = new CompressionOptions(6))
                using (var zstd = new CompressionStream(b64, option, 4 * 1024))
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
            using (var zstd = new DecompressionStream(b64))
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
            using (var option = new CompressionOptions(6))
            using (var zstd = new CompressionStream(b64, option, 4 * 1024))
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
                using (var zstd = new CompressionStream(b64))
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
            using (var zstd = new DecompressionStream(b64))
            using (var r = new StreamReader(zstd, Encoding.Unicode))
            {
                var s = r.ReadToEnd();
                return s;
            }
        }
        #endregion
    }
}
