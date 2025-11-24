using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VgcApis.Models.Datas
{
    public class MobItem
    {
        #region properties
        public string ver = "1";
        public List<string> server = new List<string>();
        public List<string> protocol = new List<string>();
        public List<string> stream = new List<string>();
        public List<string> enc = new List<string>();
        #endregion

        public MobItem() { }

        public MobItem(SharelinkMetaData meta)
        {
            server.Add(meta.name);
            server.Add(meta.host);
            server.Add($"{meta.port}");
            protocol.Add(meta.proto);
            protocol.Add(meta.auth1);
            protocol.Add(meta.auth2);
            protocol.Add(meta.auth3);
            stream.Add(meta.streamType);
            stream.Add(meta.streamParam1);
            stream.Add(meta.streamParam2);
            stream.Add(meta.streamParam3);
            enc.Add(meta.tlsType);
            enc.Add(meta.tlsServName);
            enc.Add(meta.tlsFingerPrint);
            enc.Add(meta.tlsAlpn);
            enc.Add(meta.tlsParam1);
            enc.Add(meta.tlsParam2);
            enc.Add(meta.tlsParam3);
            enc.Add(meta.tlsParam4);

            TrimArrays(protocol, stream, enc);
        }

        #region private methods
        static Type metaTy = typeof(SharelinkMetaData);
        static List<string> metaProtoKeys = new List<string>()
        {
            nameof(SharelinkMetaData.proto),
            nameof(SharelinkMetaData.auth1),
            nameof(SharelinkMetaData.auth2),
            nameof(SharelinkMetaData.auth3),
        };
        static List<string> metaStreamKeys = new List<string>()
        {
            nameof(SharelinkMetaData.streamType),
            nameof(SharelinkMetaData.streamParam1),
            nameof(SharelinkMetaData.streamParam2),
            nameof(SharelinkMetaData.streamParam3),
        };
        static List<string> metaTlsKeys = new List<string>()
        {
            // tlsType is a property, metaTlsFields[0] will be null
            nameof(SharelinkMetaData.tlsType),
            nameof(SharelinkMetaData.tlsServName),
            nameof(SharelinkMetaData.tlsFingerPrint),
            nameof(SharelinkMetaData.tlsAlpn),
            nameof(SharelinkMetaData.tlsParam1),
            nameof(SharelinkMetaData.tlsParam2),
            nameof(SharelinkMetaData.tlsParam3),
            nameof(SharelinkMetaData.tlsParam4),
        };

        static List<FieldInfo> metaProtoFields = ToFieldInfos(metaProtoKeys);
        static List<FieldInfo> metaStreamFields = ToFieldInfos(metaStreamKeys);
        static List<FieldInfo> metaTlsFields = ToFieldInfos(metaTlsKeys);

        static List<FieldInfo> ToFieldInfos(List<string> keys)
        {
            var fields = new List<FieldInfo>();
            foreach (var key in keys)
            {
                var field = metaTy.GetField(key, BindingFlags.Instance | BindingFlags.Public);
                fields.Add(field);
            }
            return fields;
        }

        void SetFieldsValue(SharelinkMetaData meta, List<FieldInfo> fields, List<string> values)
        {
            var last = Math.Min(fields.Count, values.Count) - 1;
            for (int i = last; i >= 0; i--)
            {
                var field = fields[i];
                field?.SetValue(meta, values[i]);
            }
        }

        void TrimArrays(params List<string>[] arrs)
        {
            foreach (var arr in arrs)
            {
                for (int i = arr.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(arr[i]))
                    {
                        arr.RemoveAt(i);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        #region public methods

        public string ToShareLink()
        {
            var body = JsonConvert.SerializeObject(this);
            var b64 = Misc.Utils.Base64EncodeString(body);
            return $"mob://{b64}";
        }

        // This function could throw exception!
        public SharelinkMetaData ToShareLinkMetaData()
        {
            var meta = new SharelinkMetaData
            {
                name = server[0],
                host = server[1],
                port = Misc.Utils.Str2Int(server[2]),
            };

            SetFieldsValue(meta, metaProtoFields, protocol);
            SetFieldsValue(meta, metaStreamFields, stream);
            SetFieldsValue(meta, metaTlsFields, enc);
            if (enc.Count > 0)
            {
                meta.tlsType = enc[0];
            }
            return meta;
        }
        #endregion
    }
}
